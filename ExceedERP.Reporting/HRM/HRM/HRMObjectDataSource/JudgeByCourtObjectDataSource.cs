using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeByCourtObjectDataSource : List<JudgeByCourt>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JudgeByCourt> GetJudgeByCourt()
        {
            List<JudgeByCourt> JudgeByAgeList = new List<JudgeByCourt>();
            JudgeByCourt JudgeByAge = new JudgeByCourt();
            //  JudgeEduaction JudgeEduaction = new JudgeEduaction();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var branchs = db.Branch;
            var placment = db.Placements.Where(x => x.isCurrent);


            var branchss = from b in db.Branch
                join c in db.Placements on b.BranchId equals c.BranchId
                join m in db.MainCompanyStructures on c.OrgStructureId equals m.OrgStructureId
                join e in db.Person_Employee on c.EmployeeId equals e.EmployeeId
                where c.isCurrent == true
                select new
                {
                    Name = b.Name,
                    CompName = m.Name,
                };

            if (branchss.Any())
            {
                foreach (var b in branchss)
                {
                    JudgeByAge = new JudgeByCourt();

                    JudgeByAge.Name = b.Name;
                    JudgeByAge.CompName = b.CompName;
                    JudgeByAgeList.Add(JudgeByAge);

                }
            }

            return JudgeByAgeList;
        }
    }
}