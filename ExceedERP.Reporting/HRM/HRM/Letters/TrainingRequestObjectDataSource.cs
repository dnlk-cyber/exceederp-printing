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
    public class TrainingRequestObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingReqeustVM> GetTrainingRequest(string id, bool header)
        {
            List<TrainingReqeustVM> EmployeeInfoList = new List<TrainingReqeustVM>();
            TrainingReqeustVM EmployeeInfo = new TrainingReqeustVM();
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

            EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            var trainingRequest = db.TrainingApplicants.FirstOrDefault(x => x.TrainingApplicantId.ToString() == id);
            if (trainingRequest != null)
            {
                var depo = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingRequest.OrgStructureId);
                if (depo != null)
                {
                    EmployeeInfo.Department = depo.Name;
                }
                EmployeeInfo.Date = trainingRequest.DateAmharic;
                //var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == trainingRequest.GlFiscalYearId);
                //if (year != null)
                //{
                //    EmployeeInfo.Year = year.Name;
                //}
                //var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent);
                //if(placement != null)
                //{
                //    var placeHead = db.Placements.FirstOrDefault(x=>x.OrgStructureId == placement.OrgStructureId && x.isCurrent && x.IsHead);
                //    if(placeHead != null)
                //    {
                //        var headPerson = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == placeHead.EmployeeId);
                //        if(headPerson != null)
                //        {
                //            EmployeeInfo.Approver = headPerson.FullNameaEnglish;
                //        }
                //    }
                //}
                var requstStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == trainingRequest.TrainingApplicantId && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE);
                if (requstStatus != null)
                {
                    EmployeeInfo.Status = requstStatus.Status.ToString();

                    var empApp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requstStatus.ApproverId);

                    if (empApp != null)
                    {

                        EmployeeInfo.Approver = empApp.FullNameaEnglish;


                    }
                    var reqDep = db.RequestesStatus.FirstOrDefault(x => x.RequestId == trainingRequest.TrainingApplicantId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);

                    if (reqDep != null)
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
                    var depApp = db.RequestesStatus.FirstOrDefault(x => x.RequestId == trainingRequest.TrainingApplicantId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                    if (depApp != null)
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
                        var reject = db.RequestesStatus.FirstOrDefault(x => x.RequestId == trainingRequest.TrainingApplicantId && x.ReferenceType == "Leave" && x.Status == RequestStatus.REJECT);
                        if (reject != null)
                        {
                            EmployeeInfo.Status = reject.Status.ToString();

                        }
                        else
                        {
                            var pend = db.RequestesStatus.FirstOrDefault(x => x.RequestId == trainingRequest.TrainingApplicantId && x.ReferenceType == "Leave" && x.Status == RequestStatus.PEND);
                            if (pend != null)
                            {
                                EmployeeInfo.Status = pend.Status.ToString();

                            }
                            else
                            {
                                var waiting = db.RequestesStatus.FirstOrDefault(x => x.RequestId == trainingRequest.TrainingApplicantId && x.ReferenceType == "Leave" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                                if (waiting != null)
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
}
