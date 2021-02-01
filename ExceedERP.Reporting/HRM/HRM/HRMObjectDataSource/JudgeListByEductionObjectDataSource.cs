using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeListByEductionObjectDataSource : List<LongTermEducation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LongTermEducation> GetJudgeListByEduction()
        {
            List<LongTermEducation> JudgeByAgeList = new List<LongTermEducation>();
            LongTermEducation JudgeByAge = new LongTermEducation();
            //  JudgeEduaction JudgeEduaction = new JudgeEduaction();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var branchs = db.Branch;
            var placment = db.Placements.Where(x => x.isCurrent);


            var branchss = from b in db.MainCompanyStructures
                join c in db.Placements on b.OrgStructureId equals c.OrgStructureId
                join e in db.Person_Employee on c.EmployeeId equals e.EmployeeId
                join ed in db.Education on e.EmployeeId equals ed.EmployeeId
                join l in db.Lookups on ed.EduLevel equals l.LookupId
                where c.isCurrent == true
                select new
                {
                    Name = b.Name,
                    EduName = l.Description,
                };

            if (branchss.Any())
            {
                foreach (var b in branchss)
                {
                    JudgeByAge = new LongTermEducation();

                    JudgeByAge.Name = b.Name;
                    JudgeByAge.EduName = b.EduName;
                    JudgeByAgeList.Add(JudgeByAge);

                }
            }

            return JudgeByAgeList;
        }
    }
}