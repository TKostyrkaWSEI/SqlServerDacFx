using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DacFxCApp_01.Testers
{
    public class Tester00
    {
        public string GenerateDacPac()
        {
            DacServices dacSrv = new DacServices(@"Data Source=.;Integrated Security=SSPI;");
            string folderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\DacPacFiles\"));
            string filePath = Path.Combine(folderPath, Path.GetRandomFileName() + ".dacpac");

            Directory.CreateDirectory(folderPath);
            dacSrv.Extract(filePath, "ContosoRetailDW", "AppName", new Version(1, 0));

            return filePath;
        }

        public void LoadDacPackage(string filePath)
        {
            DacPackage p = DacPackage.Load(filePath);

            Console.WriteLine(p.Name);
            Console.WriteLine(p.PreDeploymentScript);
            Console.WriteLine(p.PostDeploymentScript);
        }

        public void UpdateModel(string filePath)
        {
            TSqlModel m0 = new TSqlModel(filePath);
            TSqlModel m1 = new TSqlModel(m0.Version, m0.CopyModelOptions());

            IEnumerable<TSqlObject> allOb0 = m0.GetObjects(DacQueryScopes.Default);
            IEnumerable<TSqlObject> allOb1 = m1.GetObjects(DacQueryScopes.Default);

            Console.WriteLine(allOb0.Count());
            Console.WriteLine(allOb1.Count());

            //foreach (var o in allOb1.OrderBy(x => x.ObjectType.Name))
            //{ Console.WriteLine($"{o.ObjectType.Name}, {o.Name}"); }

            foreach (var o in allOb0
                        .Where(x => x.ObjectType.Name == "Table")
                        .Where(x => x.Name.Parts[1].StartsWith("Dim"))
                        )
            {
                Console.WriteLine($"{o.ObjectType.Name}, {o.Name.Parts[1]}");
                Console.WriteLine(o.GetScript());

                m1.AddObjects(o.GetScript());
            }
        }


        public void LoadTSqlModel(string filePath)
        {
            TSqlModel model = new TSqlModel(filePath);

            var tables = model.GetObjects(DacQueryScopes.Default, Table.TypeClass).ToList();
            foreach (var t in tables) { Console.WriteLine(t.Name); }
            Console.WriteLine("----------->\n\n");

            var t1 = model.GetObjects(Table.TypeClass,
                                        new ObjectIdentifier("dbo", "DimAccount"),
                                        DacQueryScopes.Default
                                        ).FirstOrDefault();
            foreach (var c in t1.GetReferenced(Table.Columns)) { Console.WriteLine(c.Name); }
            Console.WriteLine("----------->\n\n");

            TSqlObject column = t1.GetReferenced(Table.Columns).First(col => col.Name.Parts[2].Equals("AccountName"));

            int columnLength = column.GetProperty<int>(Column.Length);
            Console.WriteLine("Column c1 has length {0}", columnLength);

            var dataType = column.GetReferenced(Column.DataType).First().Name;
            Console.WriteLine("Column c1 is of type '{0}'", dataType);
            Console.WriteLine("----------->\n\n");
        }
    }
}
