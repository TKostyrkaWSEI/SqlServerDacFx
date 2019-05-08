using Microsoft.SqlServer.Dac;
using System;

namespace DacFxCApp_02_Contributors
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = @"C:\GitHub\SqlServerDacFx\CreateDatabaseScripts\";
            string file01 = "DACContributor_Test01.dacpac";
            string file02 = "DACContributor_Test02.dacpac";

            var pk1 = DacPackage.Load(folderPath + file01);
            var pk2 = DacPackage.Load(folderPath + file02);

            DacDeployOptions options = new DacDeployOptions
            {
                AdditionalDeploymentContributors = "AgileSqlClub.DeploymentFilterContributor",
                AdditionalDeploymentContributorArguments = "SqlPackageFilterA=IgnoreSchema(dev01)",
                ExcludeObjectTypes = new ObjectType[]
                {
                    ObjectType.StoredProcedures,
                    ObjectType.Views,
                    ObjectType.Users,
                    ObjectType.Logins,
                    ObjectType.RoleMembership
                },
                DropObjectsNotInSource = true
            };

            string s = DacServices.GenerateDeployScript(pk2, pk1, "name", options);
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
