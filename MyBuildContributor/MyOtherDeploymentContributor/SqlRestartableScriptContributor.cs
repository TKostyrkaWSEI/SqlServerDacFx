using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace MyOtherDeploymentContributor
{
    [ExportDeploymentPlanModifier("MyOtherDeploymentContributor.RestartableScriptContributor", "1.0.0.0")]
    public partial class SqlRestartableScriptContributor : DeploymentPlanModifier
    {

        private const string BatchIdColumnName = "BatchId";
        private const string DescriptionColumnName = "Description";

        private const string CompletedBatchesVariableName = "CompletedBatches";
        private const string CompletedBatchesVariable = "$(CompletedBatches)";
        private const string CompletedBatchesSqlCmd = @":setvar " + CompletedBatchesVariableName + " __completedBatches_{0}_{1}";
        private const string TotalBatchCountSqlCmd = @":setvar TotalBatchCount {0}";
        private const string CreateCompletedBatchesTable = @"   if OBJECT_ID(N'tempdb.dbo." + CompletedBatchesVariable + @"', N'U') is null  
                                                                begin  
                                                                use tempdb  
                                                                create table [dbo].[$(CompletedBatches)]  
                                                                (  
                                                                    BatchId int primary key,  
                                                                    Description nvarchar(300)  
                                                                )  
                                                                use [$(DatabaseName)]  
                                                                end  
                                                                ";

        protected override void OnExecute(DeploymentPlanContributorContext context)
        {
            // Obtain the first step in the Plan from the provided context  
            int batchId = 0;
            DeploymentStep nextStep = context.PlanHandle.Head;
            BeginPreDeploymentScriptStep beforePreDeploy = null;

            while (nextStep != null)
            {
                DeploymentStep currentStep = nextStep;
                nextStep = currentStep.Next;

                if (currentStep is BeginPreDeploymentScriptStep)
                {
                    beforePreDeploy = (BeginPreDeploymentScriptStep)currentStep;
                    continue;
                }
                if (currentStep is SqlPrintStep)
                {
                    continue;
                }
                if (currentStep is BeginPostDeploymentScriptStep)
                {
                    break;
                }
                if (beforePreDeploy == null)
                {
                    continue;
                }

                DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;
                if (domStep == null)
                {
                    continue;
                }

                TSqlScript script = domStep.Script as TSqlScript;
                if (script == null)
                {
                    continue;
                }

                // Loop through all the batches in the script for this step.  All the statements  
                // in the batch will be enclosed in an if statement that will check the  
                // table to ensure that the batch has not already been executed  
                TSqlObject sqlObject;
                string stepDescription;
                GetStepInfo(domStep, out stepDescription, out sqlObject);
                int batchCount = script.Batches.Count;

                for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
                {
                    // Create the if statement that will contain the batch's contents  
                    IfStatement ifBatchNotExecutedStatement = CreateIfNotExecutedStatement(batchId);
                    BeginEndBlockStatement statementBlock = new BeginEndBlockStatement();
                    ifBatchNotExecutedStatement.ThenStatement = statementBlock;
                    statementBlock.StatementList = new StatementList();

                    TSqlBatch batch = script.Batches[batchIndex];
                    int statementCount = batch.Statements.Count;

                    // Loop through all statements in the batch, embedding those in an sp_execsql  
                    // statement that must be handled this way (schemas, stored procedures,   
                    // views, functions, and triggers).  
                    for (int statementIndex = 0; statementIndex < statementCount; statementIndex++)
                    {
                        TSqlStatement smnt = batch.Statements[statementIndex];

                        if (IsStatementEscaped(sqlObject))
                        {
                            // "escape" this statement by embedding it in a sp_executesql statement  
                            string statementScript;
                            domStep.ScriptGenerator.GenerateScript(smnt, out statementScript);
                            ExecuteStatement spExecuteSql = CreateExecuteSql(statementScript);
                            smnt = spExecuteSql;
                        }

                        statementBlock.StatementList.Statements.Add(smnt);
                    }

                    // Add an insert statement to track that all the statements in this  
                    // batch were executed.  Turn on nocount to improve performance by  
                    // avoiding row inserted messages from the server  
                    string batchDescription = string.Format(CultureInfo.InvariantCulture,
                        "{0} batch {1}", stepDescription, batchIndex);

                    PredicateSetStatement noCountOff = new PredicateSetStatement();
                    noCountOff.IsOn = false;
                    noCountOff.Options = SetOptions.NoCount;

                    PredicateSetStatement noCountOn = new PredicateSetStatement();
                    noCountOn.IsOn = true;
                    noCountOn.Options = SetOptions.NoCount;
                    InsertStatement batchCompleteInsert = CreateBatchCompleteInsert(batchId, batchDescription);
                    statementBlock.StatementList.Statements.Add(noCountOn);
                    statementBlock.StatementList.Statements.Add(batchCompleteInsert);
                    statementBlock.StatementList.Statements.Add(noCountOff);

                    // Remove all the statements from the batch (they are now in the if block) and add the if statement  
                    // as the sole statement in the batch  
                    batch.Statements.Clear();
                    batch.Statements.Add(ifBatchNotExecutedStatement);

                    // Next batch  
                    batchId++;
                }


            }

            // if we found steps that required processing, set up a temporary table to track the work that you are doing  
            if (beforePreDeploy != null)
            {
                // Declare a SqlCmd variables.  
                //  
                // CompletedBatches variable - defines the name of the table in tempdb that will track  
                // all the completed batches.  The temporary table's name has the target database name and  
                // a guid embedded in it so that:  
                // * Multiple deployment scripts targeting different DBs on the same server  
                // * Failed deployments with old tables do not conflict with more recent deployments  
                //  
                // TotalBatchCount variable - the total number of batches surrounded by if statements.  Using this  
                // variable pre/post deployment scripts can also use the CompletedBatches table to make their  
                // script rerunnable if there is an error during execution  
                StringBuilder sqlcmdVars = new StringBuilder();
                sqlcmdVars.AppendFormat(CultureInfo.InvariantCulture, CompletedBatchesSqlCmd,
                    context.Options.TargetDatabaseName, Guid.NewGuid().ToString("D"));
                sqlcmdVars.AppendLine();
                sqlcmdVars.AppendFormat(CultureInfo.InvariantCulture, TotalBatchCountSqlCmd, batchId);

                DeploymentScriptStep completedBatchesSetVarStep = new DeploymentScriptStep(sqlcmdVars.ToString());
                base.AddBefore(context.PlanHandle, beforePreDeploy, completedBatchesSetVarStep);

                // Create the temporary table we will use to track the work that we are doing  
                DeploymentScriptStep createStatusTableStep = new DeploymentScriptStep(CreateCompletedBatchesTable);
                base.AddBefore(context.PlanHandle, beforePreDeploy, createStatusTableStep);
            }

            //   Cleanup and drop the table
            //   DeploymentScriptStep dropStep = new DeploymentScriptStep(DropCompletedBatchesTable);
            //   base.AddAfter(context.PlanHandle, context.PlanHandle.Tail, dropStep);

        }
    }

}
