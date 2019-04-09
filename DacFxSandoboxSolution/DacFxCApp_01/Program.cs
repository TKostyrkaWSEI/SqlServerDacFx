﻿using DacFxCApp_01.Testers;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.IO;

namespace DacFxCApp_01
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test01();
            Test02();

            Console.WriteLine("-----------");
            Console.ReadKey();
        }

        static void Test01()
        {
            Tester01 t = new Tester01();

            string server = @"Data Source=.;Integrated Security=SSPI;";
            string database = @"ContosoRetailDW";
            string filePath = @"C:\Users\XTOKO\Source\Repos\TKostyrkaWSEI\SqlServerDacFx\DacFxSandoboxSolution\DacFxCApp_01\bin\DacPacFiles\dacpac01.dacpac";

            DacPackage p = DacPackage.Load(filePath);
            TSqlModel m = new TSqlModel(filePath);

            string newDacPackPath = t.CreateDacPac(server, database);
            TSqlModel m1 = t.CreateModelWithDimsOnly(m);
            PackageMetadata pm1 = new PackageMetadata() { Name = "NewApp", Version = "2.0" };

            DacPackage p1 = DacPackage.Load(newDacPackPath, DacSchemaModelStorageType.Memory, FileAccess.ReadWrite);
            p1.UpdateModel(m1, pm1);
        }

        static void Test02()
        {
            Tester02 t = new Tester02();

            string folderPath = @"C:\Users\XTOKO\Source\Repos\TKostyrkaWSEI\SqlServerDacFx\DacFxSandoboxSolution\DacFxCApp_01\bin\DacPacFiles\";
            string file01 = "dacpac00Dims.dacpac";
            string file02 = "dacpac01.dacpac";

            var pk1 = DacPackage.Load(folderPath + file01);
            var pk2 = DacPackage.Load(folderPath + file02);

            string s = t.GenerateCompareScript(pk1, pk2);
            Console.WriteLine(s);
        }
    }
}
