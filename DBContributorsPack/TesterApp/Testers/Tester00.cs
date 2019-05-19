using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;

namespace TesterApp.Testers
{
    class Tester00 : ITester
    {
        public void Run()
        {
            string projectFileName = @"C:\GitHub\SqlServerDacFx\DBContributorsPack\TestDBSource\TestDBSource.sqlproj";

            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> GlobalProperty = new Dictionary<string, string>
            {
                { "Configuration", "Debug" },
                { "Platform", "Any CPU" },
                { "OutputPath", @"bin\DebugFromApp"}
            };

            BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Build" }, null);
            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(new BuildParameters(pc), BuidlRequest);

            Console.WriteLine(buildResult.Exception);
            Console.WriteLine(buildResult.OverallResult);
            Console.WriteLine("------->");
            Console.ReadKey();
        }
    }
}






 

