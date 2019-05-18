using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;

namespace TestCshApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            string fol01 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBSource\bin\Debug\"));
            string file01 = fol01 + @"TestDBSource.dacpac";

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

            services.Deploy(    package,
                                dbName,
                                updateExisting,
                                opts
                                );
        }
    }
}
