using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace MyOtherDeploymentContributor
{

    public partial class SqlRestartableScriptContributor
    {
        /// <summary>  
        /// The CreateExecuteSql method "wraps" the provided statement script in an "sp_executesql" statement  
        /// Examples of statements that must be so wrapped include: stored procedures, views, and functions  
        /// </summary>  
        private static ExecuteStatement CreateExecuteSql(string statementScript)
        {
            // define a new Exec statement  
            ExecuteStatement executeSp = new ExecuteStatement();
            ExecutableProcedureReference spExecute = new ExecutableProcedureReference();
            executeSp.ExecuteSpecification = new ExecuteSpecification { ExecutableEntity = spExecute };

            // define the name of the procedure that you want to execute, in this case sp_executesql  
            SchemaObjectName procName = new SchemaObjectName();
            procName.Identifiers.Add(CreateIdentifier("sp_executesql", QuoteType.NotQuoted));
            ProcedureReference procRef = new ProcedureReference { Name = procName };

            spExecute.ProcedureReference = new ProcedureReferenceName { ProcedureReference = procRef };

            // add the script parameter, constructed from the provided statement script  
            ExecuteParameter scriptParam = new ExecuteParameter();
            spExecute.Parameters.Add(scriptParam);
            scriptParam.ParameterValue = new StringLiteral { Value = statementScript };
            scriptParam.Variable = new VariableReference { Name = "@stmt" };
            return executeSp;
        }

        /// <summary>  
        /// The CreateIdentifier method returns a Identifier with the specified value and quoting type  
        /// </summary>  
        private static Identifier CreateIdentifier(string value, QuoteType quoteType)
        {
            return new Identifier { Value = value, QuoteType = quoteType };
        }

        /// <summary>  
        /// The CreateCompletedBatchesName method creates the name that will be inserted  
        /// into the temporary table for a batch.  
        /// </summary>  
        private static SchemaObjectName CreateCompletedBatchesName()
        {
            SchemaObjectName name = new SchemaObjectName();
            name.Identifiers.Add(CreateIdentifier("tempdb", QuoteType.SquareBracket));
            name.Identifiers.Add(CreateIdentifier("dbo", QuoteType.SquareBracket));
            name.Identifiers.Add(CreateIdentifier(CompletedBatchesVariable, QuoteType.SquareBracket));
            return name;
        }

        /// <summary>  
        /// Helper method that determins whether the specified statement needs to  
        /// be escaped  
        /// </summary>  
        /// <param name="sqlObject"></param>  
        /// <returns></returns>  
        private static bool IsStatementEscaped(TSqlObject sqlObject)
        {
            HashSet<ModelTypeClass> escapedTypes = new HashSet<ModelTypeClass>
    {
        Schema.TypeClass,
        Procedure.TypeClass,
        View.TypeClass,
        TableValuedFunction.TypeClass,
        ScalarFunction.TypeClass,
        DatabaseDdlTrigger.TypeClass,
        DmlTrigger.TypeClass,
        ServerDdlTrigger.TypeClass
    };
            return escapedTypes.Contains(sqlObject.ObjectType);
        }

        /// <summary>  
        /// Helper method that creates an INSERT statement to track a batch being completed  
        /// </summary>  
        /// <param name="batchId"></param>  
        /// <param name="batchDescription"></param>  
        /// <returns></returns>  
        private static InsertStatement CreateBatchCompleteInsert(int batchId, string batchDescription)
        {
            InsertStatement insert = new InsertStatement();
            NamedTableReference batchesCompleted = new NamedTableReference();
            insert.InsertSpecification = new InsertSpecification();
            insert.InsertSpecification.Target = batchesCompleted;
            batchesCompleted.SchemaObject = CreateCompletedBatchesName();

            // Build the columns inserted into  
            ColumnReferenceExpression batchIdColumn = new ColumnReferenceExpression();
            batchIdColumn.MultiPartIdentifier = new MultiPartIdentifier();
            batchIdColumn.MultiPartIdentifier.Identifiers.Add(CreateIdentifier(BatchIdColumnName, QuoteType.NotQuoted));

            ColumnReferenceExpression descriptionColumn = new ColumnReferenceExpression();
            descriptionColumn.MultiPartIdentifier = new MultiPartIdentifier();
            descriptionColumn.MultiPartIdentifier.Identifiers.Add(CreateIdentifier(DescriptionColumnName, QuoteType.NotQuoted));

            insert.InsertSpecification.Columns.Add(batchIdColumn);
            insert.InsertSpecification.Columns.Add(descriptionColumn);

            // Build the values inserted  
            ValuesInsertSource valueSource = new ValuesInsertSource();
            insert.InsertSpecification.InsertSource = valueSource;

            RowValue values = new RowValue();
            values.ColumnValues.Add(new IntegerLiteral { Value = batchId.ToString() });
            values.ColumnValues.Add(new StringLiteral { Value = batchDescription });
            valueSource.RowValues.Add(values);

            return insert;
        }

        /// <summary>  
        /// This is a helper method that generates an if statement that checks the batches executed  
        /// table to see if the current batch has been executed.  The if statement will look like this  
        ///   
        /// if not exists(select 1 from [tempdb].[dbo].[$(CompletedBatches)]   
        ///                where BatchId = batchId)  
        /// begin  
        /// end  
        /// </summary>  
        /// <param name="batchId"></param>  
        /// <returns></returns>  
        private static IfStatement CreateIfNotExecutedStatement(int batchId)
        {
            // Create the exists/select statement  
            ExistsPredicate existsExp = new ExistsPredicate();
            ScalarSubquery subQuery = new ScalarSubquery();
            existsExp.Subquery = subQuery;

            subQuery.QueryExpression = new QuerySpecification
            {
                SelectElements =
        {
            new SelectScalarExpression  { Expression = new IntegerLiteral { Value ="1" } }
        },
                FromClause = new FromClause
                {
                    TableReferences =
                {
                    new NamedTableReference() { SchemaObject = CreateCompletedBatchesName() }
                }
                },
                WhereClause = new WhereClause
                {
                    SearchCondition = new BooleanComparisonExpression
                    {
                        ComparisonType = BooleanComparisonType.Equals,
                        FirstExpression = new ColumnReferenceExpression
                        {
                            MultiPartIdentifier = new MultiPartIdentifier
                            {
                                Identifiers = { CreateIdentifier(BatchIdColumnName, QuoteType.SquareBracket) }
                            }
                        },
                        SecondExpression = new IntegerLiteral { Value = batchId.ToString() }
                    }
                }
            };

            // Put together the rest of the statement  
            IfStatement ifNotExists = new IfStatement
            {
                Predicate = new BooleanNotExpression
                {
                    Expression = existsExp
                }
            };

            return ifNotExists;
        }

        /// <summary>  
        /// Helper method that generates a useful description of the step.  
        /// </summary>  
        private static void GetStepInfo(
            DeploymentScriptDomStep domStep,
            out string stepDescription,
            out TSqlObject element)
        {
            element = null;

            // figure out what type of step we've got, and retrieve  
            // either the source or target element.  
            if (domStep is CreateElementStep)
            {
                element = ((CreateElementStep)domStep).SourceElement;
            }
            else if (domStep is AlterElementStep)
            {
                element = ((AlterElementStep)domStep).SourceElement;
            }
            else if (domStep is DropElementStep)
            {
                element = ((DropElementStep)domStep).TargetElement;
            }

            // construct the step description by concatenating the type and the fully qualified  
            // name of the associated element.  
            string stepTypeName = domStep.GetType().Name;
            if (element != null)
            {
                string elementName = GetElementName(element);

                stepDescription = string.Format(CultureInfo.InvariantCulture, "{0} {1}",
                    stepTypeName, elementName);
            }
            else
            {
                // if the step has no associated element, just use the step type as the description  
                stepDescription = stepTypeName;
            }
        }

        private static string GetElementName(TSqlObject element)
        {
            StringBuilder name = new StringBuilder();
            if (element.Name.HasExternalParts)
            {
                foreach (string part in element.Name.ExternalParts)
                {
                    if (name.Length > 0)
                    {
                        name.Append('.');
                    }
                    name.AppendFormat("[{0}]", part);
                }
            }

            foreach (string part in element.Name.Parts)
            {
                if (name.Length > 0)
                {
                    name.Append('.');
                }
                name.AppendFormat("[{0}]", part);
            }

            return name.ToString();
        }


    }

}
