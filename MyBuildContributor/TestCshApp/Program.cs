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

            string folderPath01 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDB\bin\Debug\"));
            string folderPath02 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBEmpty\bin\Debug\"));

            string file01 = "TestDB.dacpac";
            string file02 = "TestDBEmpty.dacpac";

            DacPackage pk01 = DacPackage.Load(folderPath01 + file01);
            DacPackage pk02 = DacPackage.Load(folderPath02 + file02);

            DacDeployOptions options = new DacDeployOptions
            {
                //AdditionalDeploymentContributors = "MyOtherDeploymentContributor.RestartableScriptContributor",
                AdditionalDeploymentContributors = "TestContributors.MyFirstTestContributor",
                //ExcludeObjectTypes = new ObjectType[]
                //{
                //    ObjectType.StoredProcedures,
                //    ObjectType.Views,
                //    ObjectType.Users,
                //    ObjectType.Logins,
                //    ObjectType.RoleMembership
                //},
                DropObjectsNotInSource = true
            };

            //string s = DacServices.GenerateCreateScript(pk01, "name", options);
            string s = DacServices.GenerateDeployScript(pk01, pk02, "name", options);
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
