using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TestContributors
{
    [ExportDeploymentPlanModifier("TestContributors.MyFirstTestContributor", "1.0.0.0")]
    public class MyFirstTestContributor : DeploymentPlanModifier
    {
        protected override void OnExecute(DeploymentPlanContributorContext context)
        {
            // Obtain the first step in the Plan from the provided context  
            DeploymentStep nextStep = context.PlanHandle.Head;

            while (nextStep != null)
            {
                DeploymentStep currentStep = nextStep;
                nextStep = currentStep.Next;

                //Debug.WriteLine(currentStep.GetType());

                if (currentStep is DropElementStep)
                {
                    DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;

                    TSqlScript script = domStep.Script as TSqlScript;
                    TSqlStatement t = script.Batches[0].Statements[0];

                    DropObjectsStatement o = (DropObjectsStatement)t;
                    IList<SchemaObjectName> ol = o.Objects;
                    SchemaObjectName ol1 = ol[0];
                    
                    Sql140ScriptGenerator s = new Sql140ScriptGenerator();
                    s.GenerateScript(t, out string ts);

                    Debug.WriteLine(domStep.IsMessageInFirstBatch);
                    Debug.WriteLine(domStep.Message);
                    Debug.WriteLine(ts);
                    Debug.WriteLine(ol1.SchemaIdentifier.Value);
                    Debug.WriteLine(ol1.BaseIdentifier.Value);

                    continue;
                }
                else
                {
                }

                //if (currentStep is DeploymentScriptDomStep)
                //{
                //    base.Remove(context.PlanHandle, currentStep);
                //    continue;
                //}

                //    if (currentStep is BeginPostDeploymentScriptStep)
                //    {
                //        break;
                //    }
                //    if (beforePreDeploy == null)
                //    {
                //        continue;
                //    }

                //    DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;
                //    if (domStep == null)
                //    {
                //        continue;
                //    }

                //    TSqlScript script = domStep.Script as TSqlScript;
                //    if (script == null)
                //    {
                //        continue;
                //    }

            }
        }
    }

}
