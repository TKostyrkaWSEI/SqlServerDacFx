using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DacFxCApp_01.Testers
{
    public class Tester01
    {

        public string CreateDacPac(
            string srv,
            string dbs,
            string filename = null
            )
        {
            filename = filename ?? Path.GetRandomFileName();

            DacServices dacSrv = new DacServices(srv);
            string folderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\DacPacFiles\"));
            string filePath = Path.Combine(folderPath, filename + ".dacpac");

            Directory.CreateDirectory(folderPath);
            dacSrv.Extract(filePath, dbs, "AppName", new Version(1, 0));

            return filePath;
        }

        public TSqlModel CreateModelWithDimsOnly(TSqlModel m0)
        {
            TSqlModel m1 = new TSqlModel(m0.Version, m0.CopyModelOptions());

            foreach (var o in m0.GetObjects(DacQueryScopes.Default)
                        .Where(x => x.ObjectType.Name == "Table")
                        .Where(x => x.Name.Parts[1].StartsWith("Dim"))
                        )
            {
                m1.AddObjects(o.GetScript());
            }

            return m1;
        }
    }
}
