using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TesterApp.Testers
{
    class Tester02 : ITester
    {
        readonly string folderPath01 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestDBSource\bin\Debug\"));
        readonly string file01 = "TestDBSource.dacpac";

        public void Run()
        {
            TSqlModel tm01 = new TSqlModel(folderPath01 + file01);
            TSqlModel newModel = new TSqlModel(tm01.Version, tm01.CopyModelOptions());

            List<TSqlObject> objs = tm01.GetObjects(DacQueryScopes.UserDefined).ToList();
            List<TSqlObject> delt = new List<TSqlObject>();

            foreach (var r in objs.Where(x => x.ObjectType.Name == "Schema"))
            {
                delt.Add(r);
                AddChildren(r, delt);
            }

            objs.RemoveAll(z => delt.Contains(z));

            foreach (TSqlObject o in objs.Where(z => !delt.Contains(z)))
            {
                if (o.TryGetScript(out string s))
                {
                    newModel.AddObjects(s);
                }
            }

            PackageMetadata m = new PackageMetadata()
            {
                Name = "Nazwa",
                Version = "1.0",
                Description = ""
            };

            DacPackageExtensions.BuildPackage
            (
                @"C:\Users\XTOKO\Desktop\NewDacpacSchemaFilter.dacpac"
            , newModel
            , m
            );

            Console.WriteLine("==> koniec");
            Console.ReadLine();
        }

        void AddChildren(TSqlObject obj_root, List<TSqlObject> obj_list)
        {
            List<TSqlObject> tmp_lis = obj_root.GetChildren().ToList();
            if (tmp_lis.Count() > 0)
            {
                obj_list.AddRange(tmp_lis);
                foreach (var t in tmp_lis)
                {
                    AddChildren(t, obj_list);
                }
            }
        }
    }
}
