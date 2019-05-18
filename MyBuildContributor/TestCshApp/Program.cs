using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;

namespace TestCshApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath01 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBSource\bin\Debug\"));
            string folderPath02 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBTarget\bin\Debug\"));

            string file01 = "TestDBSource.dacpac";
            string file02 = "TestDBTarget.dacpac";
            
            // load dacpacs
            DacPackage pk01 = DacPackage.Load(folderPath01 + file01);
            DacPackage pk02 = DacPackage.Load(folderPath02 + file02);

            // configure (same as .publish xml)
            DacDeployOptions options = new DacDeployOptions
            {
                //AdditionalDeploymentContributors = "TestContributors.MyFirstTestContributor",
                AdditionalDeploymentContributors = "TestContributors.DropToTxtContributor",

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
