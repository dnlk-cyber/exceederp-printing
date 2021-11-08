using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
   public class RecruitmentRequestLetterODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RecruitmentRequestVM> GetRecruitmentRequestLetter(string id, bool header)
        {
            List<RecruitmentRequestVM> EmployeeInfoList = new List<RecruitmentRequestVM>();
            RecruitmentRequestVM EmployeeInfo = new RecruitmentRequestVM();
            if (header)
            {
                var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
                if (Organization != null)
                {
                    EmployeeInfo.Name = Organization.TradeName;
                    EmployeeInfo.AmharicName = Organization.AmharicName;
                    EmployeeInfo.Abbreviation = Organization.Abbreviation;
                    var address =
                        db.Addresses.Where(
                            a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Any())
                    {
                        foreach (var a in address)
                        {
                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                EmployeeInfo.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                EmployeeInfo.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (EmployeeInfo.Address == null)
                                {
                                    EmployeeInfo.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                EmployeeInfo.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                EmployeeInfo.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                EmployeeInfo.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                EmployeeInfo.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                EmployeeInfo.Tel = "Tel:  " + a.Value;

                            }
                        }

                    }
                    var logo =
                        db.Attachments.Where(
                            a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                    if (logo != null)
                    {
                        EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year);

            
            var leaveRequest = db.RecruitmentRequest.FirstOrDefault(x=>x.RecruitmentRequestId.ToString() == id);
            if(leaveRequest != null)
            {
                var company = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == leaveRequest.OrgStructureId);
                if(company != null)
                {
                    EmployeeInfo.Department = company.Name;
                }
                var job = db.Lookups.FirstOrDefault(x=>x.LookupId == leaveRequest.LookupId);
                if(job != null)
                {
                    EmployeeInfo.JobTitle = job.Description;
                }
                var branch = db.Branch.FirstOrDefault(x=>x.BranchId == leaveRequest.BranchId);
                if(branch != null)
                {
                    EmployeeInfo.Branch = branch.Name;
                }
               
                var requstStatus = db.RequestesStatus.FirstOrDefault(x=>x.RequestId == leaveRequest.RecruitmentRequestId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.APPROVE);
                if(requstStatus != null)
                {
                    EmployeeInfo.Status = requstStatus.Status.ToString();

                    var empApp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requstStatus.ApproverId);

                    if (empApp != null)
                    {
                        
                            EmployeeInfo.Approver = empApp.FullNameaEnglish;                          
                        
                    }
                    var reqDep = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.RecruitmentRequestId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);

                    if(reqDep != null)
                    {
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == reqDep.ApproverId);
                        if (emp != null)
                        {                           
                            EmployeeInfo.DepApprover = emp.FullNameaEnglish;
                        }
                    }
                   
                }
                else
                {
                    var depApp = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.RecruitmentRequestId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                    if(depApp != null)
                    {
                        EmployeeInfo.Status = depApp.Status.ToString();
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requstStatus.ApproverId);
                        if (emp != null)
                        {
                            EmployeeInfo.DepApprover = emp.FullNameaEnglish;
                        }
                    }
                    else
                    {
                        var reject = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.RecruitmentRequestId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.REJECT);
                        if(reject != null)
                        {
                            EmployeeInfo.Status = reject.Status.ToString();
                            
                        }
                        else
                        {
                            var pend = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.RecruitmentRequestId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.PEND);
                            if(pend != null)
                            {
                                EmployeeInfo.Status = pend.Status.ToString();                               
                            }
                            else
                            {
                                var waiting = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.RecruitmentRequestId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                                if(waiting != null)
                                {
                                    EmployeeInfo.Status = waiting.Status.ToString();
                                }
                                else
                                {
                                    EmployeeInfo.Status = "Not Sent For Approval";
                                }
                            }
                        }
                    }
                }

                EmployeeInfoList.Add(EmployeeInfo);
            }
         
            return EmployeeInfoList;
        }
    }

    public class RecruitmentRequestVM
    {
        public string Department { get; set; }
        public string Head { get; set; }
        public string ApprovedBy { get; set; }
        public string Branch { get; set; }
        public string JobTitle { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string Date { get; set; }
        public string LeaveType { get; set; }
        public string Approver { get; set; }
        public string DepApprover { get; set; }
        public string Status { get; set; }
    }
}
