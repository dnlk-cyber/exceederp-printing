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
    public class EmployeeJobTitleSummaryObjectDataSource : List<EmployeeSummary>
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeJobTitleSummary(string branchId, string orgStructureId, string job, string status)
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            List<Person_Employee> emplist;
            int index = 0;

            #region byJobTitle

            var empPlacement = db.Placements.DistinctByField(x => x.EmployeeId).ToList();
            if (branchId != null && orgStructureId == null && job == null)
            {
                empPlacement = empPlacement.Where(x => x.BranchId.ToString() == branchId).ToList();
            }
            else if (branchId != null && orgStructureId != null && job == null)
            {
                empPlacement = empPlacement.Where(x => x.BranchId.ToString() == branchId &&
                        x.OrgStructureId.ToString() == orgStructureId)
                    .ToList();
            }
            else if (branchId != null && orgStructureId != null && job != null)
            {
                empPlacement = empPlacement.Where(x => x.BranchId.ToString() == branchId && x.OrgStructureId.ToString() == orgStructureId &&
                    x.LookupId.ToString() == job).ToList();

            }
            else if (branchId != null && orgStructureId == null && job != null)
            {
                empPlacement = empPlacement.Where(x => x.BranchId.ToString() == branchId && x.LookupId.ToString() == job).ToList();

            }
            else if (branchId == null && orgStructureId == null && job != null)
            {
                empPlacement = empPlacement.Where(x => x.LookupId.ToString() == job).ToList();

            }
            else if (branchId == null && orgStructureId != null && job == null)
            {
                empPlacement = empPlacement.Where(x => x.OrgStructureId.ToString() == orgStructureId).ToList();

            }
            else if (branchId == null && orgStructureId != null && job != null)
            {
                empPlacement = empPlacement.Where(x => x.OrgStructureId.ToString() == orgStructureId && x.LookupId.ToString() == job).ToList();

            }
            else
            {
                empPlacement = empPlacement.ToList();
            }

            if (empPlacement.Any())
            {
                var personEmployee = db.Person_Employee.ToList();

                if (status == "Active")
                {
                    personEmployee = personEmployee.Where(x => x.IsActive).ToList();
                }
                if (status == "InActive")
                {
                    personEmployee = personEmployee.Where(x => !x.IsActive).ToList();
                }

                var allLookup = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
                var employeeJobtitle = from ep in empPlacement
                                       join pe in personEmployee
                                           on ep.EmployeeId equals pe.EmployeeId
                                       join lu in allLookup
                                       on ep.LookupId equals lu.LookupId
                                       select (new { pe.Gender, ep.LookupId, pe });

                foreach (var placement in empPlacement)
                {
                    employee = new EmployeeSummary();
                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);

                    employee.SummaryName = lookup?.Description;
                    if (employeeJobtitle.Any())
                    {
                        employee.index = index;
                        employee.NumberOfFemaleEmployee = employeeJobtitle.Where(x => x.LookupId == lookup.LookupId).Count(x => x.Gender == Gender.FEMALE);
                        employee.NumberOfMaleEmployee = employeeJobtitle.Where(x => x.LookupId == lookup.LookupId).Count(x => x.Gender == Gender.MALE);
                        employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;
                    }
                    list.Add(employee);
                    index++;
                }
                int oo = 0;
                var empIdsmale = personEmployee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();

                employee = new EmployeeSummary();
                if (empIdsmale.Any())
                {
                    //var notplacedmale = db.Placements.Where(x => !empIdsmale.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
                    //if (notplacedmale.Any())
                    //{
                    //    employee.SummaryName = "ያልተመደቡ ";
                    //    employee.NumberOfMaleEmployee = notplacedmale.Count();
                    //    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;
                    //}

                    var countM = 0;
                    foreach (var i in empIdsmale)
                    {
                        var place = db.Placements.FirstOrDefault(x => x.EmployeeId == i);
                        if (place == null)
                        {
                            countM = countM + 1;
                        }
                    }

                    employee.SummaryName = "ያልተመደቡ ";
                    employee.NumberOfMaleEmployee = countM;
                    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;
                }
                var empIdsfemale = personEmployee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
                if (empIdsfemale.Any())
                {
                    var notplacedFemale = db.Placements.Where(x => !empIdsfemale.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
                    //if (notplacedFemale.Any())
                    //{
                    //    employee.SummaryName = "ያልተመደቡ ";
                    //    employee.NumberOfFemaleEmployee = notplacedFemale.Count();
                    //    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;


                    //}
                    var countF = 0;
                    foreach(var i in empIdsfemale)
                    {
                        var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == i);
                        if(place == null)
                        {
                             countF = countF + 1;
                        }
                    }
                    employee.SummaryName = "ያልተመደቡ ";
                    employee.NumberOfFemaleEmployee = countF;
                    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;
                }

                employee.index = index++;

                list.Add(employee);
            }
            


            return list.DistinctByField(x => x.SummaryName).ToList();

            #endregion


            #region Filter
            //var palcementList = db.Placements.Where(x=> x.isCurrent ).ToList();
            //emplist = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId, "");
            //var employeeList = db.Person_Employee.Where(x => x.IsActive).ToList();
            //var employeeJobtitle = from e in emplist
            //            join p in palcementList
            //                on e.EmployeeId equals p.EmployeeId
            //            select (new { e.Gender, p });
            #endregion


        }


    }
}