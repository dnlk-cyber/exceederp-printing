using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeRankSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeRankSummary(string branchId, string orgStructureId,string status, string job = "")
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();
            List<Person_Employee> employees;
            if (branchId != null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {

                employees = db.Person_Employee.ToList();

            }
            if (status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList();
            }
            #region All Rank

            #region Rank

            if (job != null)
            {
                db = new ExceedDbContext();
                var rankList = db.Ranks.Where(x => x.RankId.ToString() == job).ToList();
                foreach (var r in rankList)
                {
                    employee = new EmployeeSummary();
                    employee.Criteria = "Rank";

                    employee.SummaryName = r.RankName;
                    var palcementList = db.Placements.Where(x => x.RankId == r.RankId).ToList();
                    foreach (var l in palcementList)
                    {
                        var emplist = employees.FirstOrDefault(x => x.EmployeeId == l.EmployeeId && x.IsActive);
                        if (emplist != null)
                        {
                            if (emplist.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1; 

                            }
                            if (emplist.Gender == Gender.MALE)
                            {
                                employee.NumberOfMaleEmployee += 1;

                            }

                        }

                    }
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                    list.Add(employee);
                }


            }
            else
            {

                db = new ExceedDbContext();
                var rankList = db.Ranks.ToList();
                foreach (var r in rankList)
                {
                    employee = new EmployeeSummary();
                    employee.Criteria = "Rank";

                    employee.SummaryName = r.RankName;
                    var palcementList = db.Placements.Where(x => x.RankId == r.RankId && x.isCurrent).ToList();
                    foreach (var l in palcementList)
                    {
                        var emplist = employees.FirstOrDefault(x => x.EmployeeId == l.EmployeeId && x.IsActive);
                        if (emplist != null)
                        {
                            if (emplist.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1;

                            }
                            if (emplist.Gender == Gender.MALE)
                            {
                                employee.NumberOfMaleEmployee += 1;

                            }
                        }


                    }
                    employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;
                    list.Add(employee);
                }
            }

            #endregion

            #endregion



            return list;
        }
    }
}