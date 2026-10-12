using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

//using ExceedERP.Reporting.HRM.Payroll;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveOneByOneDetailObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveOneByOne> GetLeaveOneByOneDetail(/*string periodN*/object[] periodNs, string empCode)
        {
            List<LeaveOneByOne> summaries = new List<LeaveOneByOne>();
            LeaveOneByOne sum = new LeaveOneByOne();

            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.ToString() == empCode);
            if (personEmployee != null)
            {
                int id = personEmployee.EmployeeId;
                if (periodNs != null)
                {
                    List<int> gid = new List<int>();
                    foreach (var pp in periodNs)
                    {
                        gid.Add(Convert.ToInt32(pp));
                    }

                    sum = new LeaveOneByOne();
                    sum.LeaveFromOthers = new List<LeaveFromOther>();
                    sum.LeaveTakenDetails = new List<LeaveTakenDetail>();
                    foreach (var periodN in periodNs)
                    {
                        int period = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == periodN.ToString()).GlFiscalYearId;

                        var entitlew =
                            db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == period && x.EmployeeId == id);
                        var taken =
                            db.LeaveRequest.Where(
                                    x => x.GlFiscalYearId == period 
                                    && x.EmployeeId == id 
                                    && !x.IsVoid
                                    && x.Type == LeaveType.ANNUAL
                                    && x.IsOnlineApproved)
                                .ToList();
                        var employees = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
                        //ListOfEmployeeIdView.GetEmployees().FirstOrDefault(x => x.PersonId == id);
                     //   var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.EmployeeId == id && x.Check).ToList();
                        var periods = db.GLFiscalYears.ToList();
                        var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period && x.EmployeeId == id).ToList();
                        var periodName = periods.FirstOrDefault(x => x.GlFiscalYearId == period);

                        if (employees != null)
                        {
                            sum.ID = employees.EmployeeCode;
                            sum.FullName = employees.FullNameaEnglish;
                        }
                        if (periodName != null)
                        {
                            sum.LeavePeriod = sum.LeavePeriod + periodName.Name + ", ";
                        }
                        //if (tranferedList.Any())
                        //{
                        //    sum.TransferedFrom = tranferedList.Sum(x => x.Amount);
                        //}
                        if (paidIncash.Any())
                        {
                            sum.LeavePaidInCash = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));
                        }
                        decimal totalEntitle = 0;
                        var entitles = db.LeavesEntitle.Where(x => x.EmployeeId == id && gid.Contains(x.GlFiscalYearId) && !x.IsExpired);
                        if (entitles.Any())
                        {
                            totalEntitle = entitles.Sum(x => x.Balance);
                        }
                        if (entitlew != null) sum.Entitlment = totalEntitle;// entitlew.Amount;
                      //  sum.TotalAmountTaken = taken.Where(x => x.LeaveType == LeaveType.ANNUAL).Sum(x => x.Amount);
                        decimal subtract = sum.TotalAmountTaken + sum.LeavePaidInCash;
                        sum.Balance = totalEntitle;// - Convert.ToDecimal(paidIncash.Sum(x => x.Amount)) - taken.Sum(x => x.Amount);// - subtract; //(sum.Entitlment + sum.TransferedFrom) - subtract;

                        // LeaveFromOther leaveFromOther = new LeaveFromOther();
                        //foreach (var t in tranferedList)
                        //{
                        //    leaveFromOther = new LeaveFromOther();
                        //    leaveFromOther.TransferredDays = t.Amount;
                        //    var name = periods.Where(x => x.GlFiscalYearId == t.From).FirstOrDefault();
                        //    if (name != null) leaveFromOther.TransferredPeriod = name.Name;
                        //    sum.LeaveFromOthers.Add(leaveFromOther);
                        //}
                        //var leaveEntitles = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired).ToList();


                        var leaveEntitles = db.LeavesEntitle.Where(x => x.EmployeeId == id && x.GlFiscalYearId == period).OrderBy(x=>x.LeaveEntitleId).ToList();
                        if (leaveEntitles.Any())
                        {
                            foreach (var leaveEntitle in leaveEntitles)
                            {
                                var leaveFromOther = new LeaveFromOther();
                                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == leaveEntitle.GlFiscalYearId);
                                if (year != null)
                                {
                                    leaveFromOther.TransferredPeriod = year.Name;
                                }
                                //leaveFromOther.TransferredDays = leaveEntitle.Amount;
                                leaveFromOther.TransferredDays = leaveEntitle.Balance;
                                leaveFromOther.EntitleAmount = leaveEntitle.Amount;
                                if (leaveEntitle.IsExpired)
                                {
                                    leaveFromOther.Status = "Expired";
                                }
                                else
                                {
                                    leaveFromOther.Status = "Active";
                                }

                                sum.LeaveFromOthers.Add(leaveFromOther);
                            }
                        }

                        // LeaveTakenDetail takenDetail = new LeaveTakenDetail();
                        foreach (var t in taken)
                        {
                            var takenDetail = new LeaveTakenDetail();
                            takenDetail.LeaveTakenAmount = t.LeaveDays;
                            takenDetail.LeaveType = t.Type.ToString();
                            var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == t.GlFiscalYearId);
                            if (year != null)
                            {
                                takenDetail.FiscalYear = year.Name;
                            }

                            takenDetail.LeaveTakenDateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(t.From.Day, t.From.Month, t.From.Year);//t.From.ToShortDateString();
                            takenDetail.LeaveTakenDateTo = XAPI.EthiopicDateTime.GetEthiopicDate(t.To.Day, t.To.Month, t.To.Year);//t.To.ToShortDateString();
                          
                                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == t.LeaveRequestId
                                && x.Status == RequestStatus.DEPARTMENT_APPROVAL
                                && x.ReferenceType == "Leave");
                                if (requestStatus != null)
                                {
                                 
                                    takenDetail.DepartmentApprover = GetEmployeeFullName(requestStatus.ApproverId??0);
                               
                                  
                                }

                            var requestStatusFinalApprove = db.RequestesStatus.FirstOrDefault(x => x.RequestId == t.LeaveRequestId
                                && x.Status == RequestStatus.APPROVE
                                && x.ReferenceType == "Leave");
                                if (requestStatusFinalApprove != null)
                                {
                                 
                                    takenDetail.Approver = GetEmployeeFullName(requestStatusFinalApprove.ApproverId ?? 0);
                               
                                  
                                }

                           
                            //var requestStatus = db.RequestesStatus.FirstOrDefault(x=>x. == );

                            sum.LeaveTakenDetails.Add(takenDetail);
                        }


                    }
                    summaries.Add(sum);
                }
            }
            return summaries;
        }
        private string GetEmployeeFullName(int empid)
        {
            var person = db.Person_Employee.Find(empid);
            if (person != null)
            {
                return person.FullNameaEnglish;
            }
            return null;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}