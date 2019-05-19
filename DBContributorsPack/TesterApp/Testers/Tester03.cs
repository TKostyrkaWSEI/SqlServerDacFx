using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.IO;
namespace TesterApp.Testers
{
    class Tester03 : ITester
    {
        string file01 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBSource\bin\Debug\")) + 
            @"TestDBSource.dacpac";

        public void Run()
        {
            DacServices services = new DacServices(@"Server=ITK\DEV17;Integrated Security=true;");
            DacPackage package = DacPackage.Load(file01, DacSchemaModelStorageType.Memory, FileAccess.ReadWrite);

            string dbName = @"MojaBazaDAC";
            bool updateExisting = true;

            TSqlModel tm01 = new TSqlModel(file01);
            TSqlModel newModel = new TSqlModel(tm01.Version, tm01.CopyModelOptions());

            //package.UpdateModel(filteredModel, new PackageMetadata())

            DacDeployOptions opts = new DacDeployOptions
            {
                ExcludeObjectTypes = new ObjectType[]
                {
                    ObjectType.Users,
                    ObjectType.RoleMembership
                }
            };

            services.Deploy(package,
                                dbName,
                                updateExisting,
                                opts
                                );
        }
    }
}
