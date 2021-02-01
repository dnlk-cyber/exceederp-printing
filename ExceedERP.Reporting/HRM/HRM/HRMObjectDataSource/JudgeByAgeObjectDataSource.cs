using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeByAgeObjectDataSource : List<JudgeByAge>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JudgeByAge> GetJudgeByAge()
        {
            List<JudgeByAge> JudgeByAgeList = new List<JudgeByAge>();
            JudgeByAge JudgeByAge = new JudgeByAge();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var branchs = db.Branch;
            var placment = db.Placements.Where(x => x.isCurrent);
            if (branchs.Any())
            {
                foreach (var b in branchs)
                {
                    JudgeByAge = new JudgeByAge();
                    JudgeByAge.BranchName = b.Name;
                    var assigned = placment.Where(x => x.BranchId == b.BranchId);
                    foreach (var a in assigned)
                    {
                        var getEmp = employees.Where(x => x.EmployeeId == a.EmployeeId).FirstOrDefault();
                        if (getEmp != null)
                        {
                            if (getEmp.Gender == Gender.MALE)
                            {
                                JudgeByAge.NoOfMale += 1;
                            }
                            else
                            {
                                JudgeByAge.NoOfFemale += 1;
                            }
                        }


                    }
                    JudgeByAge.Total = JudgeByAge.NoOfFemale + JudgeByAge.NoOfMale;
                    JudgeByAgeList.Add(JudgeByAge);
                }
            }
            return JudgeByAgeList;
        }
    }
}