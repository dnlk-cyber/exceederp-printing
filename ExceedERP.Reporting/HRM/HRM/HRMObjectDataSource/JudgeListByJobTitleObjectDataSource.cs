using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeListByJobTitleObjectDataSource : List<JudgeListByJobTitle>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JudgeListByJobTitle> GetJudgeListByJobTitle()
        {
            List<JudgeListByJobTitle> JudgeByAgeList = new List<JudgeListByJobTitle>();
            JudgeListByJobTitle JudgeByAge = new JudgeListByJobTitle();
            //  JudgeEduaction JudgeEduaction = new JudgeEduaction();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var branchs = db.Branch;
            var placment = db.Placements.Where(x => x.isCurrent);


            var branchss = from b in db.MainCompanyStructures
                join c in db.Placements on b.OrgStructureId equals c.OrgStructureId
                join e in db.Person_Employee on c.EmployeeId equals e.EmployeeId

                join l in db.Lookups on c.LookupId equals l.LookupId
                where c.isCurrent == true
                select new
                {
                    Name = b.Name,
                    JobName = l.Description,
                };

            if (branchss.Any())
            {
                foreach (var b in branchss)
                {
                    JudgeByAge = new JudgeListByJobTitle();

                    JudgeByAge.Name = b.Name;
                    JudgeByAge.JobName = b.JobName;
                    JudgeByAgeList.Add(JudgeByAge);

                }
            }

            return JudgeByAgeList;
        }
    }
}