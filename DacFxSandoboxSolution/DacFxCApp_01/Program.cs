using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.IO;
using System.Linq;

namespace DacFxCApp_01
{
    class Program
    {
        static void Main(string[] args)
        {
            Tester t = new Tester();

            string filePath = @"C:\Users\XTOKO\Source\Repos\TKostyrkaWSEI\SqlServerDacFx\DacFxSandoboxSolution\DacFxCApp_01\bin\DacPacFiles\ffgxqye5.4fa.dacpac";
                //t.GenerateDacPac();
            t.LoadDACToModel(filePath);

            Console.WriteLine("-----------");
            Console.ReadKey();
        }
    }

    class Tester
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

        public void LoadDACToModel(string filePath)
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
