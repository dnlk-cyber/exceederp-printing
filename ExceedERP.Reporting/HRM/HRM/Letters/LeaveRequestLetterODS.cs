using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
   public class LeaveRequestLetterODS:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveRequestLetterVM> GetLeaveRequestLetter(string id, bool header)
        {
            List<LeaveRequestLetterVM> employeeInfoList = new List<LeaveRequestLetterVM>();
            LeaveRequestLetterVM employeeInfo = new LeaveRequestLetterVM();

            if (header)
            {
                var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                if (organization != null)
                {
                    employeeInfo.Name = organization.TradeName;
                    employeeInfo.AmharicName = organization.AmharicName;
                    employeeInfo.Abbreviation = organization.Abbreviation;

                    var addresses = db.Addresses
                        .Where(a => a.Reference == organization.MainCompanyId.ToString() && a.ReferenceType.ToLower() == "company")
                        .ToDictionary(a => a.Category, a => a.Value);

                    if (addresses.TryGetValue(AddressType.KIFLEKETEMA, out var kifleketema))
                    {
                        employeeInfo.Address = "Subcity: " + kifleketema + " ";
                    }
                    if (addresses.TryGetValue(AddressType.KEBELE, out var kebele))
                    {
                        employeeInfo.Address += "Kebele: " + kebele + " ";
                    }
                    if (addresses.TryGetValue(AddressType.CITYPROVINCE, out var cityProvince))
                    {
                        employeeInfo.Address += cityProvince + " -Ethiopia";
                    }
                    if (addresses.TryGetValue(AddressType.POBOX, out var pobox))
                    {
                        employeeInfo.Pobox = "P.O.Box: " + pobox;
                    }
                    if (addresses.TryGetValue(AddressType.OFFICE_EMAIL, out var email))
                    {
                        employeeInfo.Email = "Email: " + email;
                    }
                    if (addresses.TryGetValue(AddressType.WEBSITE, out var website))
                    {
                        employeeInfo.WebSite = "Website: " + website;
                    }
                    if (addresses.TryGetValue(AddressType.FAX, out var fax))
                    {
                        employeeInfo.Fax = "Fax: " + fax;
                    }
                    if (addresses.TryGetValue(AddressType.OFFICE_PHONE, out var officePhone))
                    {
                        employeeInfo.Tel = "Tel: " + officePhone;
                    }

                    var logo = db.Attachments
                        .FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() && a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        employeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            employeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            if (int.TryParse(id, out int requestId))
            {
                var leaveRequest = db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestId);
                if (leaveRequest != null)
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId);
                    if (person != null)
                    {
                        employeeInfo.FullName = person.FullNameAmharic;
                    }

                    employeeInfo.From = leaveRequest.FromAmharic;
                    employeeInfo.To = leaveRequest.ToAmharic;
                    employeeInfo.LeaveDays = leaveRequest.LeaveDays.ToString();
                    employeeInfo.LeaveType = leaveRequest.Type.ToString();

                    var leaves = db.LeaveRequest
                        .Where(x => x.EmployeeId == leaveRequest.EmployeeId)
                        .ToList();
                    var leaveIds = leaves
                        .Select(x => x.LeaveRequestId)
                        .ToList();

                    leaveIds = db.RequestesStatus
                        .Where(x => leaveIds.Contains(x.RequestId) && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE)
                        .Select(x => x.RequestId)
                        .ToList();

                    //employeeInfo.TotalLeaveTaken = leaves
                    //    .Where(x => leaveIds.Contains(x.LeaveRequestId))
                    //    .Sum(x => x.LeaveBalance);

                    var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == leaveRequest.GlFiscalYearId);
                    if (year != null)
                    {
                        employeeInfo.Year = year.Name;
                    }

                    var requestStatus = db.RequestesStatus
                        .FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE);

                    if (requestStatus != null)
                    {
                        employeeInfo.TotalLeaveTaken = leaveRequest.LeaveDays;
                        employeeInfo.Status = requestStatus.Status.ToString();

                        var empApp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requestStatus.ApproverId);
                        if (empApp != null)
                        {
                            employeeInfo.Approver = empApp.FullNameaEnglish;
                            var approvedBy = UserHelper.GetUserNameByEmployeeId(empApp.EmployeeId);
                            employeeInfo.ApproverSign = SignatureHelper.GetUserSignature(approvedBy);
                        }

                        var reqDep = db.RequestesStatus
                            .FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);

                        if (reqDep != null)
                        {
                            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == reqDep.ApproverId);
                            if (emp != null)
                            {
                                employeeInfo.DepApprover = emp.FullNameaEnglish;
                            }
                        }
                        employeeInfo.Remark = requestStatus.Remark;
                    }
                    else
                    {
                        var depApp = db.RequestesStatus
                            .FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                        if (depApp != null)
                        {
                            employeeInfo.Status = depApp.Status.ToString();
                            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == depApp.ApproverId);
                            if (emp != null)
                            {
                                employeeInfo.DepApprover = emp.FullNameaEnglish;
                            }
                            var approvedDepBy = UserHelper.GetUserNameByEmployeeId(depApp.ApproverId ?? 0);
                            employeeInfo.DepApproverSign = SignatureHelper.GetUserSignature(approvedDepBy);
                            employeeInfo.Remark = depApp.Remark;
                        }
                        else
                        {
                            var reject = db.RequestesStatus
                                .FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId 
                                && x.ReferenceType == "Leave" 
                                && x.Status == RequestStatus.REJECT);
                            if (reject != null)
                            {
                                employeeInfo.Status = reject.Status.ToString();
                            }
                            else
                            {
                                var pend = db.RequestesStatus
                                    .FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId 
                                    && x.ReferenceType == "Leave" 
                                    && x.Status == RequestStatus.PEND);
                                if (pend != null)
                                {
                                    employeeInfo.Status = pend.Status.ToString();
                                    employeeInfo.Remark = pend.Remark;

                                }
                                else
                                {
                                    var waiting = db.RequestesStatus
                                        .FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId 
                                        && x.ReferenceType == "Leave" 
                                        && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                                    if (waiting != null)
                                    {
                                        employeeInfo.Status = waiting.Status.ToString();
                                        employeeInfo.Remark = waiting.Remark;

                                    }
                                    else
                                    {
                                        employeeInfo.Status = "Not Sent For Approval";
                                    }
                                }
                            }
                        }
                    }

                    var fiscalYears = db.GLFiscalYears.ToList();
                    var leavesEntitle = db.LeavesEntitle
                        .Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x=>!x.IsExpired)
                        .GroupBy(x => x.GlFiscalYearId)
                        .ToList();

                    employeeInfo.RequestDetails = leavesEntitle
                        .Select(g =>
                        {
                            var fiscalYear = fiscalYears.FirstOrDefault(y => y.GlFiscalYearId == g.Key);
                            return new LeaveRequestDetails
                            {
                                FiscalYear = fiscalYear?.Name ?? "Unknown Fiscal Year",
                                Balance = g.Sum(x => x.Balance)
                            };
                        })
                        .ToList();

                    employeeInfo.TotalLeaveBalance = employeeInfo.RequestDetails.Any()?employeeInfo.RequestDetails.Sum(x => x.Balance):0;
                    employeeInfo.RemainingBalance = employeeInfo.TotalLeaveBalance > employeeInfo.TotalLeaveTaken? employeeInfo.TotalLeaveBalance - employeeInfo.TotalLeaveTaken:0;

                    employeeInfoList.Add(employeeInfo);
                }
            }

            return employeeInfoList;
        }
    
            public void Dispose()
            {
                db.Dispose();
            }
        

        //public List<LeaveRequestLetterVM> GetLeaveRequestLetter(string id, bool header)
        //{
        //    List<LeaveRequestLetterVM> EmployeeInfoList = new List<LeaveRequestLetterVM>();
        //    LeaveRequestLetterVM EmployeeInfo = new LeaveRequestLetterVM();
        //    if (header)
        //    {
        //        var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
        //        if (Organization != null)
        //        {
        //            EmployeeInfo.Name = Organization.TradeName;
        //            EmployeeInfo.AmharicName = Organization.AmharicName;
        //            EmployeeInfo.Abbreviation = Organization.Abbreviation;
        //            var address =
        //                db.Addresses.Where(
        //                    a =>
        //                        a.Reference == Organization.MainCompanyId.ToString() &&
        //                        a.ReferenceType.ToLower() == "company").ToList();
        //            if (address.Any())
        //            {
        //                foreach (var a in address)
        //                {
        //                    if (a.Category == AddressType.KIFLEKETEMA)
        //                    {
        //                        EmployeeInfo.Address = "Subcity: " + a.Value + " ";
        //                    }
        //                    if (a.Category == AddressType.KEBELE)
        //                    {
        //                        EmployeeInfo.Address += "Kebele: " + a.Value + " ";
        //                    }
        //                    if (a.Category == AddressType.CITYPROVINCE)
        //                    {
        //                        if (EmployeeInfo.Address == null)
        //                        {
        //                            EmployeeInfo.Address += a.Value + " -Ethiopia";
        //                        }
        //                    }
        //                    else if (a.Category == AddressType.POBOX)
        //                    {

        //                        EmployeeInfo.Pobox = "P.O.Box: " + a.Value;

        //                    }
        //                    else if (a.Category == AddressType.OFFICE_EMAIL)
        //                    {

        //                        EmployeeInfo.Email = "Email: " + a.Value;

        //                    }
        //                    else if (a.Category == AddressType.WEBSITE)
        //                    {

        //                        EmployeeInfo.WebSite = "Website: " + a.Value;

        //                    }
        //                    else if (a.Category == AddressType.FAX)
        //                    {

        //                        EmployeeInfo.Fax = "Fax:  " + a.Value;

        //                    }
        //                    else if (a.Category == AddressType.OFFICE_PHONE)
        //                    {

        //                        EmployeeInfo.Tel = "Tel:  " + a.Value;

        //                    }
        //                }

        //            }
        //            var logo =
        //                db.Attachments.Where(
        //                    a =>
        //                        a.Reference == Organization.MainCompanyId.ToString() &&
        //                        a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
        //            if (logo != null)
        //            {
        //                EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
        //            }
        //        }
        //    }

        //    EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year);

        //    int.TryParse(id,out int requestId);
        //    var leaveRequest = db.LeaveRequest.FirstOrDefault(x=>x.LeaveRequestId == requestId);
        //    if(leaveRequest != null)
        //    {
        //        var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == leaveRequest.EmployeeId);
        //        if(person != null)
        //        {
        //            EmployeeInfo.FullName = person.FullNameAmharic;
        //        }
        //        EmployeeInfo.From = leaveRequest.FromAmharic;
        //        EmployeeInfo.To = leaveRequest.ToAmharic;
        //        EmployeeInfo.LeaveDays = leaveRequest.LeaveDays.ToString();
        //        EmployeeInfo.LeaveType = leaveRequest.Type.ToString();
        //        var leaves = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId).ToList();
        //        var leaveIds = leaves.Select(x => x.LeaveRequestId);
        //        leaveIds = db.RequestesStatus.Where(x => leaveIds.Contains(x.RequestId) && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE).Select(x=>x.RequestId);
        //        EmployeeInfo.TotalLeaveTaken = leaves.Where(x=> leaveIds.Contains(x.LeaveRequestId)).Any() ? leaves.Where(x=>leaveIds.Contains(x.LeaveRequestId)).Sum(x => x.LeaveBalance) : 0;

        //        var year = db.GLFiscalYears.FirstOrDefault(x=>x.GlFiscalYearId == leaveRequest.GlFiscalYearId);
        //        if(year != null)
        //        {
        //            EmployeeInfo.Year = year.Name;
        //        }

        //        var requstStatus = db.RequestesStatus.FirstOrDefault(x=>x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE);
        //        if(requstStatus != null)
        //        {

        //            EmployeeInfo.Status = requstStatus.Status.ToString();

        //            var empApp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requstStatus.ApproverId);

        //            if (empApp != null)
        //            {

        //                    EmployeeInfo.Approver = empApp.FullNameaEnglish;


        //            }
        //            var reqDep = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);

        //            if(reqDep != null)
        //            {
        //                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == reqDep.ApproverId);
        //                if (emp != null)
        //                {

        //                    EmployeeInfo.DepApprover = emp.FullNameaEnglish;
        //                }
        //            }

        //        }
        //        else
        //        {
        //            var depApp = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
        //            if(depApp != null)
        //            {
        //                EmployeeInfo.Status = depApp.Status.ToString();
        //                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == depApp.ApproverId);
        //                if (emp != null)
        //                {
        //                    EmployeeInfo.DepApprover = emp.FullNameaEnglish;
        //                }
        //            }
        //            else
        //            {
        //                var reject = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.REJECT);
        //                if(reject != null)
        //                {
        //                    EmployeeInfo.Status = reject.Status.ToString();

        //                }
        //                else
        //                {
        //                    var pend = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.PEND);
        //                    if(pend != null)
        //                    {
        //                        EmployeeInfo.Status = pend.Status.ToString();

        //                    }
        //                    else
        //                    {
        //                        var waiting = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.ReferenceType == "Leave" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
        //                        if(waiting != null)
        //                        {
        //                            EmployeeInfo.Status = waiting.Status.ToString();
        //                        }
        //                        else
        //                        {
        //                            EmployeeInfo.Status = "Not Sent For Approval";
        //                        }
        //                    }
        //                }
        //            }


        //        }
        //        EmployeeInfo.RequestDetails = new List<LeaveRequestDetails>();
        //        var leaveEntitles = db.LeavesEntitle.Where(x => x.EmployeeId == leaveRequest.EmployeeId).OrderBy(x=>x.GlFiscalYearId).ToList();
        //        var leaveWithDistictYears = leaveEntitles.DistinctByField(x=>x.GlFiscalYearId).ToList();
        //        foreach(var leaveYear in leaveWithDistictYears)
        //        {
        //            var leaveDetail = new  LeaveRequestDetails();
        //            var fiscalYear = db.GLFiscalYears.Find(leaveYear.GlFiscalYearId);
        //            if (fiscalYear != null)
        //            {
        //                leaveDetail.FiscalYear = fiscalYear.Name;

        //                leaveDetail.Balance = leaveEntitles.Where(x => x.GlFiscalYearId == leaveYear.GlFiscalYearId).Sum(x => x.Balance);
        //                EmployeeInfo.RequestDetails.Add(leaveDetail);
        //            }

        //        }

        //        EmployeeInfoList.Add(EmployeeInfo);
        //    }
        //    EmployeeInfo.TotalLeaveBalance = EmployeeInfo.RequestDetails.Any() ? EmployeeInfo.RequestDetails.Sum(x => x.Balance):0;
        //    EmployeeInfo.RemainingBalance = EmployeeInfo.TotalLeaveBalance - EmployeeInfo.TotalLeaveBalance;
        //    return EmployeeInfoList;
        //}
    }
}
