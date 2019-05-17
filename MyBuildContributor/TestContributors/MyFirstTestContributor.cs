using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
            DeploymentStep nextStep = context.PlanHandle.Head;

            while (nextStep != null)
            {
                DeploymentStep currentStep = nextStep;
                nextStep = currentStep.Next;

                //Debug.WriteLine($"{currentStep.GetType()}");

                if (currentStep is DeploymentScriptStep)
                {
                    DeploymentScriptStep d = currentStep as DeploymentScriptStep;

                    Regex rx = new Regex(@"\[sandbox\]");   //[\n\r]*is being dropped.  Deployment will halt if the table contains data.");
                    if (rx.IsMatch(d.GenerateTSQL()[0]))
                    {
                        base.Remove(context.PlanHandle, currentStep);
                        continue;
                    }
                }

                if (currentStep is CreateElementStep)
                {
                    DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;

                    TSqlScript script = domStep.Script as TSqlScript;
                    TSqlStatement t = script.Batches[0].Statements[0];

                    if (t is CreateTableStatement o)
                    {
                        SchemaObjectName ol = o.SchemaObjectName;
                        string ol1 = ol.SchemaIdentifier.Value;

                        if (ol1 == "sandbox" || ol1 == "unittests")
                        {
                            base.Remove(context.PlanHandle, currentStep);
                            continue;
                        }
                    }

                    //  Sql140ScriptGenerator s = new Sql140ScriptGenerator();
                    //  s.GenerateScript(t, out string ts);
                    //  Debug.WriteLine($"{t.GetType()}: {ts}");
                    //  DropChildObjectsStatement
                    //  DropStatisticsStatement
                }

                if (currentStep is DropElementStep)
                {
                    DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;
                    TSqlScript script = domStep.Script as TSqlScript;
                    TSqlStatement t = script.Batches[0].Statements[0];

                    //Debug.WriteLine($"{currentStep.GetType()}: {t.GetType()}");

                    if (t is DropStatisticsStatement)
                    {
                        base.Remove(context.PlanHandle, currentStep);
                        continue;
                    }

                    if (t is DropObjectsStatement)
                    {
                        DropObjectsStatement o = (DropObjectsStatement)t;
                        IList<SchemaObjectName> ol = o.Objects;
                        string ol1 = ol[0].SchemaIdentifier.Value;

                        if (ol1 == "sandbox" || ol1 == "unittests")
                        {
                            base.Remove(context.PlanHandle, currentStep);
                            continue;
                        }
                    }

                    //  Sql140ScriptGenerator s = new Sql140ScriptGenerator();
                    //  s.GenerateScript(t, out string ts);
                    //  Debug.WriteLine($"{t.GetType()}: {ts}");
                    //  DropChildObjectsStatement
                    //  DropStatisticsStatement
                }

                //if (currentStep is AlterElementStep)
                //{
                //    DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;
                //    TSqlScript script = domStep.Script as TSqlScript;
                //    TSqlStatement t = script.Batches[0].Statements[0];

                //    if (t is AlterProcedureStatement)
                //    {
                //        AlterProcedureStatement o = (AlterProcedureStatement)t;
                //        SchemaObjectName ol = o.Options.sch
                //        string ol1 = ol.SchemaIdentifier.Value;

                //        if (ol1 == "sandbox" || ol1 == "unittests")
                //        {
                //            base.Remove(context.PlanHandle, currentStep);
                //            continue;
                //        }
                //    }
                //}
            }
        }
    }

}
