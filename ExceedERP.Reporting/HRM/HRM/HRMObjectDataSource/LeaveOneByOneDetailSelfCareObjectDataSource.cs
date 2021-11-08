using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.DataAccess.Context;
using System;
using System.Linq;
using ExceedERP.Core.Domain.Common;

//using ExceedERP.Reporting.HRM.Payroll;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveOneByOneDetailSelfCareObjectDataSource : List<LeaveOneByOne>
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveOneByOne> GetLeaveOneByOneDetail(/*string periodN*/object[] periodNs, string id)
        {
            List<LeaveOneByOne> summaries = new List<LeaveOneByOne>();
            LeaveOneByOne sum = new LeaveOneByOne();

            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.ToString() == id);
            if (personEmployee != null)
            {
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
                        var fiscalYearId = Convert.ToInt32(periodN);
                        int period = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId).GlFiscalYearId;

                        var entitlew =
                            db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == period && x.EmployeeId == personEmployee.EmployeeId);
                        var taken =
                            db.Leaves.Where(
                                    x => x.GlFiscalYearId == period && x.EmployeeId == personEmployee.EmployeeId/* && x.LeaveType == LeaveType.ANNUAL*/)
                                .ToList();
                        var employees = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == personEmployee.EmployeeId);
                           // ListOfEmployeeIdView.GetEmployees().FirstOrDefault(x => x.PersonId == personEmployee.EmployeeId);
                       // var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.EmployeeId == personEmployee.EmployeeId && x.Check).ToList();
                        var periods = db.GLFiscalYears.ToList();
                        var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period && x.EmployeeId == personEmployee.EmployeeId).ToList();
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
                        var entitles = db.LeavesEntitle.Where(x => x.EmployeeId == personEmployee.EmployeeId && gid.Contains(x.GlFiscalYearId) && !x.IsExpired);
                        if (entitles.Any())
                        {
                            totalEntitle = entitles.Sum(x => x.Balance);
                        }
                        if (entitlew != null) sum.Entitlment = totalEntitle;// entitlew.Amount;
                        sum.TotalAmountTaken = taken.Where(x => x.LeaveType == LeaveType.ANNUAL).Sum(x => x.Amount);
                        decimal subtract = sum.TotalAmountTaken + sum.LeavePaidInCash;
                        sum.Balance = totalEntitle;


                        var leaveEntitles = db.LeavesEntitle.Where(x => x.EmployeeId == personEmployee.EmployeeId && x.GlFiscalYearId == period).ToList();
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
                            takenDetail.LeaveTakenAmount = t.Amount;
                            takenDetail.LeaveType = t.LeaveType.ToString();
                            var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == t.GlFiscalYearId);
                            if (year != null)
                            {
                                takenDetail.FiscalYear = year.Name;
                            }

                            takenDetail.LeaveTakenDateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(t.From.Day, t.From.Month, t.From.Year);//t.From.ToShortDateString();
                            takenDetail.LeaveTakenDateTo = XAPI.EthiopicDateTime.GetEthiopicDate(t.To.Day, t.To.Month, t.To.Year);//t.To.ToShortDateString();
                            if (t.RequestId != null)
                            {
                                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == t.RequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Leave");
                                if (requestStatus != null)
                                {
                                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requestStatus.ApproverId);
                                    if (person != null)
                                    {
                                        takenDetail.Approver = person.FullNameaEnglish;
                                    }
                                }

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
    }
}