using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
   
    [DataObject]
    public class AllDepartmentsListODS : List<MainCompanyStructure>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MainCompanyStructure> GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                var departmentLevelThree = db.MainCompanyStructures.Where(x => x.Parent == b.OrgStructureId).DistinctByField(x => x.OrgStructureId);
                                if (departmentLevelThree.Any())
                                {
                                    foreach (var c in departmentLevelThree)
                                    {
                                        var departmentLevelFour = db.MainCompanyStructures.Where(x => x.Parent == c.OrgStructureId).DistinctByField(x => x.OrgStructureId);

                                        foreach(var d in departmentLevelFour)
                                        {
                                            var departmentLevelFive = db.MainCompanyStructures.Where(x => x.Parent == d.OrgStructureId).DistinctByField(x => x.OrgStructureId);
                                            if(departmentLevelFive.Any())
                                            {
                                                foreach (var e in departmentLevelFive)
                                                {
                                                    var departmentLevelsix = db.MainCompanyStructures.Where(x => x.Parent == e.OrgStructureId).DistinctByField(x => x.OrgStructureId);
                                                    if(departmentLevelsix.Any())
                                                    {
                                                        foreach(var f in departmentLevelsix)
                                                        {
                                                            var departmentLevelseven = db.MainCompanyStructures.Where(x => x.Parent == f.OrgStructureId).DistinctByField(x => x.OrgStructureId);
                                                            if(departmentLevelseven.Any())
                                                            {
                                                                foreach(var g in departmentLevelseven)
                                                                {
                                                                    allDepartment.Add(g);
                                                                }
                                                            }
                                                            allDepartment.Add(f);
                                                        }
                                                    }
                                                    allDepartment.Add(e);
                                                }
                                            }
                                            allDepartment.Add(d);
                                        }

                                        allDepartment.Add(c);
                                    }

                                }

                                allDepartment.Add(b);
                            }
                        }

                        allDepartment.Add(a);
                    }
                }
            }

            return allDepartment;

        }
    }
}

