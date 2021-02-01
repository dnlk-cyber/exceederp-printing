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
    public class LeaveObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetLeaveSummary(string leaveType, string period, string branchId, string orgStructureId,string status)
         {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
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

            #region set Leave

            LeaveType? leave = null;
            switch (leaveType)
            {
                case "ANNUAL":
                    leave = LeaveType.ANNUAL;
                    break;
                case "OTHER":
                    leave = LeaveType.OTHER;
                    break;
                case "MARRIAGE":
                    leave = LeaveType.MARRIAGE;
                    break;
                case "MATERNITY":
                    leave = LeaveType.MATERNITY;
                    break;
                case "PATERNITY":
                    leave = LeaveType.PATERNITY;
                    break;
                case "SICK_LEAVE":
                    leave = LeaveType.SICK_LEAVE;
                    break;


            }

            #endregion

            if (period != null && leaveType != null)
            {
                var leaveList = db.Leaves.Where(
                    x => x.LeaveType == leave && x.GlFiscalYearId.ToString() == period).Select(x => x.EmployeeId).Distinct().ToList();

               

                employee = new EmployeeSummary();

                if (leaveList.Count() > 0)
                {
                    db = new ExceedDbContext();
                   
                    foreach (var t in leaveList)
                    {

                        var empList = employees.FirstOrDefault(x => x.EmployeeId == t);
                        //foreach (var e in empList)
                        //{
                        // employee.Criteria = "Job Title";

                        // employee.SummaryName = Name.Description;
                        if (empList != null)
                        {
                            if (empList.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1;

                            }
                            if (empList.Gender == Gender.MALE)
                            {
                                employee.NumberOfMaleEmployee += 1;

                            }
                        }

                        //}
                       
                    }
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    list.Add(employee);
                }

               
            }
            else if(period == null && leaveType != null)
            {
                var leaveList = db.Leaves.Where(x => x.LeaveType == leave ).Select(x => x.EmployeeId).Distinct().ToList();

                var leaveRequests = db.LeaveRequest.Where(x => x.Type == leave).ToList();
                foreach (var leaveRequest in leaveRequests)
                {
                    var checkStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.Status == RequestStatus.APPROVE);
                    if (checkStatus != null)
                    {
                        leaveList.Add(leaveRequest.EmployeeId);
                    }
                }

                leaveList = leaveList.Distinct().ToList();

                employee = new EmployeeSummary();

                if (leaveList.Count() > 0)
                {
                    db = new ExceedDbContext();

                    foreach (var t in leaveList)
                    {

                        var empList = employees.FirstOrDefault(x => x.EmployeeId == t);
                        //foreach (var e in empList)
                        //{
                        // employee.Criteria = "Job Title";

                        // employee.SummaryName = Name.Description;
                        if (empList != null)
                        {
                            if (empList.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1;

                            }
                            if (empList.Gender == Gender.MALE)
                            {
                                employee.NumberOfMaleEmployee += 1;

                            }
                        }

                        //}

                    }
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    list.Add(employee);
                }
            }
            else if (period != null && leaveType == null)
            {
                var leaveList = db.Leaves.Where(x => x.GlFiscalYearId.ToString() == period).Select(x => x.EmployeeId).Distinct().ToList();


                var leaveRequests = db.LeaveRequest.Where(x => x.GlFiscalYearId.ToString() == period).ToList();
                foreach (var leaveRequest in leaveRequests)
                {
                    var checkStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.Status == RequestStatus.APPROVE);
                    if (checkStatus != null)
                    {
                        leaveList.Add(leaveRequest.EmployeeId);
                    }
                }

                leaveList = leaveList.Distinct().ToList();

                employee = new EmployeeSummary();

                if (leaveList.Count() > 0)
                {
                    db = new ExceedDbContext();

                    foreach (var t in leaveList)
                    {

                        var empList = employees.FirstOrDefault(x => x.EmployeeId == t);
                        //foreach (var e in empList)
                        //{
                        // employee.Criteria = "Job Title";

                        // employee.SummaryName = Name.Description;
                        if (empList != null)
                        {
                            if (empList.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1;

                            }
                            if (empList.Gender == Gender.MALE)
                            {
                                employee.NumberOfMaleEmployee += 1;

                            }
                        }

                        //}

                    }
                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    list.Add(employee);
                }
            }
            else
            {
                var leaveList = db.Leaves.DistinctByField(x=>x.EmployeeId).Select(x => x.EmployeeId).ToList();


                var leaveRequests = db.LeaveRequest.ToList();
                foreach (var leaveRequest in leaveRequests)
                {
                    var checkStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.Status == RequestStatus.APPROVE);
                    if (checkStatus != null)
                    {
                        leaveList.Add(leaveRequest.EmployeeId);
                    }
                }

                leaveList = leaveList.Distinct().ToList();

                leaveList = leaveList.ToList();
                employee = new EmployeeSummary();
                if (leaveList.Count() > 0)
                {
                    db = new ExceedDbContext();

                    foreach (var t in leaveList)
                    {

                        var empList = employees.FirstOrDefault(x => x.EmployeeId == t);
                        //foreach (var e in empList)
                        //{
                            // employee.Criteria = "Job Title";

                            // employee.SummaryName = Name.Description;
                            if(empList != null)
                            {
                                if (empList.Gender == Gender.FEMALE)
                                {
                                    employee.NumberOfFemaleEmployee += 1;

                                }
                                if (empList.Gender == Gender.MALE)
                                {
                                    employee.NumberOfMaleEmployee += 1;

                                }
                            }     

                        //}
                       
                    }

                    employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                    list.Add(employee);
                }
            }
            return list;
        }
    }
}