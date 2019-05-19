using System;
using System.IO;
using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ContributorsLib
{
    [ExportDeploymentPlanModifier("DBContributorsPack.DropToTxtContributor", "1.0.0.0")]
    class DropToTxtContributor : DeploymentPlanModifier
    {
        string logfilepath = @"C:\Users\tomek\Desktop\logger.txt";

        protected override void OnExecute(DeploymentPlanContributorContext context)
        {
            int i = 0;
            File.WriteAllText(logfilepath, string.Empty);
            using (StreamWriter w = File.AppendText(logfilepath)) { w.WriteLine($"{DateTime.Now.ToLongTimeString()}, {i++:000} - START"); }
            using (StreamWriter w = File.AppendText(logfilepath)) { w.WriteLine($"---------------------"); }

            DeploymentStep nextStep = context.PlanHandle.Head;
            while (nextStep != null)
            {
                DeploymentStep currentStep = nextStep;
                nextStep = currentStep.Next;

                logger(currentStep, i++);
            }
        }

        public void logger(DeploymentStep ds, int i)
        {
            string msg = "";

            if (ds is CreateElementStep || ds is DropElementStep)
            {
                DeploymentScriptDomStep domStep = ds as DeploymentScriptDomStep;

                TSqlScript script = domStep.Script as TSqlScript;
                TSqlStatement t = script.Batches[0].Statements[0];

                msg = $"{DateTime.Now.ToLongTimeString()}, {i:000} - {ds.GetType().Name}: {t.GetType().Name}";
            }
            else
            {
                msg = $"{DateTime.Now.ToLongTimeString()}, {i:000} - {ds.GetType().Name}: - ";
            }

            using (StreamWriter w = File.AppendText(logfilepath)) { w.WriteLine(msg); }
        }
    }
}
