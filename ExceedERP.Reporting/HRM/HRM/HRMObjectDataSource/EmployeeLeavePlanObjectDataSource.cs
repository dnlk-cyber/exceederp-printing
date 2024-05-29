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
    public class EmployeeLeavePlanObjectDataSource : List<EmployeeLeavePlan>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeLeavePlan> GetLeavePlan(string period, string dept, string branchId)
        {
            List<EmployeeLeavePlan> list = new List<EmployeeLeavePlan>();
            EmployeeLeavePlan employee = new EmployeeLeavePlan();
            db = new ExceedDbContext();
            var periodList = db.GLFiscalYears;
            List<Person_Employee> empList;//= db.Person_Employee.Where(x => x.IsActive).ToList();
            var placments = db.Placements.Where(x => x.isCurrent);
            var departments = db.MainCompanyStructures;
            var allPlacement = db.Placements.ToList();
            var allPlacementHistories = db.PlacementHistories.ToList();

            decimal total = 0;
            #region 

            if (branchId != null && dept != null)
            {
                empList = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && dept == null)
            {
                empList = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId == null && dept != null)
            {
                empList = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else
            {

                empList = db.Person_Employee.Where(x => x.IsActive).ToList();

            }

            if (period != null )
            {
                var periods = periodList.FirstOrDefault(x => x.GlFiscalYearId.ToString() == period);
                var leaveList = db.LeavePlans.ToList();
                leaveList = leaveList.Where(
                    x => x.GlFiscalYearId.ToString() == period).ToList();
                employee = new EmployeeLeavePlan();
                if (leaveList.Any())
                {
                    db = new ExceedDbContext();
                   
                    foreach (var leavePlan in leaveList)
                    {
                        var emp = empList.FirstOrDefault(x => x.EmployeeId == leavePlan.EmployeeId && x.IsActive);
                        if (emp != null)
                        {
                            var place = placments.FirstOrDefault(x => x.EmployeeId == leavePlan.EmployeeId && x.isCurrent);

                            if (place != null)
                            {

                                total = total + leavePlan.LeaveAmount;
                                employee.Total = total;
                            }
                        }
                    }

                    foreach (var t in leaveList)
                    {
                      
                        var emp = empList.FirstOrDefault(x => x.EmployeeId == t.EmployeeId && x.IsActive);
                        if (emp != null)
                        {
                            var place = placments.FirstOrDefault(x => x.EmployeeId == t.EmployeeId && x.isCurrent);
                            if (place != null)
                            {
                                employee = new EmployeeLeavePlan();
                                employee.Id = emp.EmployeeCode;
                                employee.FullName = emp.FullNameAmharic;
                                employee.Total = total;
                                var empWorkUnit =
                                    db.MainCompanyStructures.FirstOrDefault(x =>
                                        x.OrgStructureId == place.OrgStructureId);
                                if (empWorkUnit != null)
                                {
                                    employee.Department = empWorkUnit.Name;
                                }
                                
                                var empLeavePlan = db.LeavePlans.Where(x => x.EmployeeId == place.EmployeeId && x.GlFiscalYearId.ToString()==period).ToList();
                                if (empLeavePlan.Any())
                                {
                                    employee.LeavePlans = new List<LeavePlan>();

                                    foreach (var empleavePlan in empLeavePlan)
                                    {
                                       
                                        LeavePlan leavePlan = new LeavePlan();

                                        leavePlan.LeavePeriod = periods.Name;
                                        leavePlan.Amount = empleavePlan.LeaveAmount;
                                        leavePlan.From = XAPI.EthiopicDateTime.GetEthiopicDate(empleavePlan.From.Day, empleavePlan.From.Month, empleavePlan.From.Year);// empleavePlan.From.ToShortDateString();
                                        leavePlan.To = XAPI.EthiopicDateTime.GetEthiopicDate(empleavePlan.To.Day, empleavePlan.To.Month, empleavePlan.To.Year);//empleavePlan.To.ToShortDateString();

                                        employee.LeavePlans.Add(leavePlan);

                                    }
                                }
                                
                            }

                            employee.Total = total;
                            list.Add(employee);
                        }

                    }
                }
            }
            else
            {
          
                var leaveList = db.LeavePlans.DistinctByField(x => x.EmployeeId).ToList();//.Where(x => periods.Contains(x.GlFiscalYearId)).ToList();


                if (leaveList.Any())
                {
                    db = new ExceedDbContext();
                    var leavePlanList = db.LeavePlans.ToList();
                    foreach (var leavePlan in leavePlanList)
                    {
                        var emp = empList.FirstOrDefault(x => x.EmployeeId == leavePlan.EmployeeId && x.IsActive);
                        if (emp != null)
                        {
                            var place = placments.FirstOrDefault(x => x.EmployeeId == leavePlan.EmployeeId && x.isCurrent);

                            if (place != null)
                            {

                                total = total + leavePlan.LeaveAmount;
                                employee.Total = total;
                            }
                        }
                    }

                    foreach (var t in leaveList)
                    {
                       
                      
                        var emp = empList.FirstOrDefault(x => x.EmployeeId == t.EmployeeId && x.IsActive);
                        if (emp != null)
                        {
                            var place = placments.FirstOrDefault(x => x.EmployeeId == t.EmployeeId && x.isCurrent);

                            if (place != null)
                            {
                               

                                employee = new EmployeeLeavePlan();

                                employee.Id = emp.EmployeeCode;
                                employee.FullName = emp.FullNameAmharic;
                                employee.Total = total;
                                var empWorkUnit =
                                    db.MainCompanyStructures.FirstOrDefault(x =>
                                        x.OrgStructureId == place.OrgStructureId);
                                if (empWorkUnit != null)
                                {
                                    employee.Department = empWorkUnit.Name;
                                }

                                var empLeavePlan = db.LeavePlans.Where(x => x.EmployeeId == place.EmployeeId).ToList();
                                if (empLeavePlan.Any())
                                {
                                    employee.LeavePlans = new List<LeavePlan>();

                                    foreach (var empleavePlan in empLeavePlan)
                                    {

                                        var leavePlan = new LeavePlan();

                                        leavePlan.LeavePeriod = empleavePlan.GlFiscalYear.Name;
                                        leavePlan.Amount = empleavePlan.LeaveAmount;
                                        leavePlan.From = XAPI.EthiopicDateTime.GetEthiopicDate(empleavePlan.From.Day, empleavePlan.From.Month, empleavePlan.From.Year);//empleavePlan.From.ToShortDateString();
                                        leavePlan.To = XAPI.EthiopicDateTime.GetEthiopicDate(empleavePlan.To.Day, empleavePlan.To.Month, empleavePlan.To.Year);//empleavePlan.To.ToShortDateString();

                                        employee.LeavePlans.Add(leavePlan);


                                      
                                    }

                                }
                                list.Add(employee);
                            }

                        }

                    }
                }
            }
            #endregion
            return list.OrderBy(x => x.Id).ToList();
        }
    }
}

