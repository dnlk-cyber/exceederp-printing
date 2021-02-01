using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class DepartmentObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MainCompanyStructure> GetDepartment()
        {
            List<MainCompanyStructure> list = new List<MainCompanyStructure> { };

            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var allworkunits = db.MainCompanyStructures.ToList();
            allDepartment.AddRange(allworkunits);

            return allDepartment;

            //var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            // if (departments != null)
            // {
            //     var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
            //     if (deptlevelOne.Any())
            //     {
            //         foreach (var a in deptlevelOne)
            //         {
            //             var departmentLevelTwo =
            //                 db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
            //             if (departmentLevelTwo.Any())
            //             {
            //                 foreach (var b in departmentLevelTwo)
            //                 {
            //                     allDepartment.Add(b);
            //                 }
            //             }
            //             allDepartment.Add(a);
            //         }
            //     }
            // }
            // if (!allDepartment.Any())
            // {
            //     return list;
            // }


        }
    }
}