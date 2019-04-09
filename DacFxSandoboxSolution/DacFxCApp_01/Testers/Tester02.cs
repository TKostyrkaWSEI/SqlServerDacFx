using DacFxCApp_01.Testers;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.IO;

namespace DacFxCApp_01.Testers
{
    class Tester02
    {
        public string GenerateCompareScript(
            DacPackage pk1,
            DacPackage pk2
            )
        {

            DacDeployOptions options = new DacDeployOptions
            {
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

            var s = DacServices.GenerateDeployScript(pk2, pk1, "name", options);

            return s;
        }
    }
}
