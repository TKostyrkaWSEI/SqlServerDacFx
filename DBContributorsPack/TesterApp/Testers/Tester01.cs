using Microsoft.SqlServer.Dac;
using System;
using System.IO;

namespace TesterApp.Testers
{
    class Tester01 : ITester
    {
        readonly string folderPath01 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBSource\bin\Debug\"));
        readonly string folderPath02 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBTarget\bin\Debug\"));

        readonly string file01 = "TestDBSource.dacpac";
        readonly string file02 = "TestDBTarget.dacpac";

        public void Run()
        {
            // load dacpacs
            DacPackage pk01 = DacPackage.Load(folderPath01 + file01);
            DacPackage pk02 = DacPackage.Load(folderPath02 + file02);

            // configure (same as .publish xml)
            DacDeployOptions options = new DacDeployOptions
            {
                AdditionalDeploymentContributors = "DBContributorsPack.DropToTxtContributor",

                ExcludeObjectTypes = new ObjectType[]
                {
                    ObjectType.Users,
                    ObjectType.RoleMembership
                },

                DropObjectsNotInSource = true,
                DoNotDropObjectTypes = new ObjectType[]
                {
                    ObjectType.DatabaseRoles
                }
            };

            // compare
            string s = DacServices.GenerateDeployScript(pk01, pk02, "name", options);
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
