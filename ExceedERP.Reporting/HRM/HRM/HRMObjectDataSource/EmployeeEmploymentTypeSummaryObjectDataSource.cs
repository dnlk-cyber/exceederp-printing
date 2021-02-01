using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEmploymentTypeSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeTypeSummary(string branchId, string orgStructureId, string status, string empStatus = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();

            #region Set employemnt Type
            employee = new EmployeeSummary();


            if (empStatus == "CONTRACT")
            {
                employee.SummaryName = "ጊዜያዊ";
            }
            else if (empStatus == "PERMANENT")
            {
                employee.SummaryName = "ቋሚ";
            }
            else if (empStatus == "PROBATION")
            {
                employee.SummaryName = "የሙከራ ጊዜ";
            }
            else if (empStatus == "FREELANCER")
            {
                employee.SummaryName = "ፍሪላንሰር";
            }
            else if (empStatus == "NO_STATUS")
            {
                employee.SummaryName = "አይታወቅም";
            }
            else
            {
                employee.SummaryName = "ሁሉም";
            }
            #endregion


            list = new List<EmployeeSummary>();

            List<Person_Employee> emplists = db.Person_Employee.ToList();
            List<Person_Employee> emplist = new List<Person_Employee>();

            var placements = db.Placements.ToList();

            //foreach (var emp in emplists)
            //{
            //    var place = placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
            //    if (place != null)
            //    {
            //        emplist.Add(emp);
            //    }
            //}


            //if (branchId != null)
            //{
            //    emplist = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId));
            //}
            //if (orgStructureId != null)
            //{
            //    emplist = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(orgStructureId));
            //}

            emplists = HrHelper.AllPersonEmployeesOneBranch(branchId, orgStructureId, "").ToList();

            if(status == "Active")
            {
                emplists = emplists.Where(x=>x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                emplists = emplists.Where(x => !x.IsActive).ToList();
            }

            //foreach (var emp in emplists)
            //{
            //    var place = placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
            //    if (place != null)
            //    {
            //        emplist.Add(emp);
            //    }
            //}
            emplist.AddRange(emplists);
            if (empStatus != null)
            {
                db = new ExceedDbContext();
                emplist = emplist.Where(x => x.Status.ToString() == empStatus).ToList();
                if (emplist.Any())
                {
                    employee.Criteria = "Employement Type";

                    employee.SummaryName = employee.SummaryName;
                    employee.NumberOfFemaleEmployee = emplist.Count(x => x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = emplist.Count(x => x.Gender == Gender.MALE);
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    list.Add(employee);

                }

            }
            else
            {
                var allTypes = Enum.GetValues(typeof(EmployeeStatus))
                            .Cast<EmployeeStatus>()

                            .ToList().Select(s => new { Text = s.ToString(), Value = s.ToString() });
                foreach (var a in allTypes)
                {
                    var emplistByStatus = emplist.Where(x => x.Status.ToString() == a.Value).ToList();
                    employee = new EmployeeSummary();

                    employee.Criteria = "Employement Type";

                    employee.SummaryName = GetStatus(a.Value.ToString());
                    employee.NumberOfFemaleEmployee = emplistByStatus.Count(x => x.Gender == Gender.FEMALE);
                    employee.NumberOfMaleEmployee = emplistByStatus.Count(x => x.Gender == Gender.MALE);
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    list.Add(employee);

                }
            }






            return list.OrderBy(x => x.SummaryName).ToList();
        }

        public string GetStatus(string empStatus)
        {
            switch (empStatus)
            {
                case "PERMANENT":
                    return "ቋሚ";

                case "CONTRACT":
                    return "ጊዜያዊ";

                case "PROBATION":
                    return "የሙከራ ጊዜ";
                case "FREELANCER":
                    return "ፍሪላንሰር";
                case "NO_STATUS":
                    return "የማይታወቅ";


            }
            return empStatus;
        }

    }
}