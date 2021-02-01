using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;


namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeJobObjectDataSource : List<EmployeeJob>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeJob> GetEmployeeJob(string dept, string branchId, string orgStructureId)
        {
            List<EmployeeJob> EmployeeInfoList = new List<EmployeeJob>();
            EmployeeJob EmployeeInfo = new EmployeeJob();
            db = new ExceedDbContext();
           
            var ranks = db.Ranks.ToList();
            var step = db.Steps.ToList();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();
         
            List<Person_Employee> employees;
          
            employees = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId,"").ToList() ;
            var emplist = employees;
            if (emplist.Any())
                {
                    foreach (var e in emplist)
                    {
                        db = new ExceedDbContext();
                      var placements = db.Placements.FirstOrDefault(x => x.isCurrent&& x.EmployeeId == e.EmployeeId );
                        if (placements != null)
                        {
                            EmployeeInfo = new EmployeeJob();
                            EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
                            EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId); ;
                            EmployeeInfo.EmployeeId = e.EmployeeCode;
                            var firstOrDefault = ranks.FirstOrDefault(x => x.RankId == placements.RankId);
                            if (firstOrDefault != null)
                                EmployeeInfo.Rank = firstOrDefault.RankName;
                        var empStep = db.Steps.FirstOrDefault(x=>x.StepId == placements.StepId);
                        if(empStep != null)
                        {
                            EmployeeInfo.Step = empStep.StepName;
                        }
                            var orDefault = lookups.FirstOrDefault(x => x.LookupId == placements.LookupId);
                            if (orDefault != null)
                                EmployeeInfo.JobTitle = orDefault.Description;

                            EmployeeInfo.Salary = placements.Salary;
                            var deptt = db.MainCompanyStructures
                                .FirstOrDefault(x => x.OrgStructureId == placements.OrgStructureId);
                            if (deptt != null) EmployeeInfo.Departement = deptt.Name;
                            EmployeeInfoList.Add(EmployeeInfo);



                        }
                    }
                }
            
            
            return EmployeeInfoList;
        }
    }
}