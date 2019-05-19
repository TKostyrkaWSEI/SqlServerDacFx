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
            //  trzeba znaleźć dobry MSBuild, ten, który używany jest przez VS/SSDT
            //  i załączyć również biblioteki: Framework, Tasks.Core i Utilities.Core
            //  C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

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

            Console.WriteLine(buildResult.OverallResult);
            Console.WriteLine(buildResult.Exception);
            Console.WriteLine("------->");
            Console.ReadKey();
        }
    }
}






 

