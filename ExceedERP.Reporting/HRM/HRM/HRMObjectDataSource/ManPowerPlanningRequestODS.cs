using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ManPowerPlanningRequestODS:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ManPowerRequestVM> GetManPowerRequest(string id, bool header)
        {
            List<ManPowerRequestVM> manpowerRequestList = new List<ManPowerRequestVM>();

            var manPower = new ManPowerRequestVM();

            if (header)
            {
                var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
                if (Organization != null)
                {
                    manPower.Name = Organization.TradeName;
                    manPower.AmharicName = Organization.AmharicName;
                    manPower.Abbreviation = Organization.Abbreviation;
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
                                manPower.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                manPower.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (manPower.Address == null)
                                {
                                    manPower.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                manPower.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                manPower.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                manPower.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                manPower.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                manPower.Tel = "Tel:  " + a.Value;

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
                        manPower.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            manPower.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);


            int.TryParse(id, out int requestId);

            var manPowerRequest = db.ManPowerPlanningRequests.FirstOrDefault(x => x.ManPowerPlanningRequestId == requestId);

            if (manPowerRequest != null)
            {
                manPower.NoOfEmployees = manPowerRequest.NumberOfStaff;
                manPower.EmployeementType = manPowerRequest.EmploymentType.ToString();

                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == manPowerRequest.OrgStructureId);
                if(mainCompany != null)
                {
                    manPower.Department = mainCompany.Name;
                }
                var branch = db.Branch.FirstOrDefault(x=>x.BranchId == manPowerRequest.BranchId);
                if(branch != null)
                {
                    manPower.Branch = branch.Name;
                }
                var job = db.Lookups.FirstOrDefault(x=>x.LookupId == manPowerRequest.LookupId);
                if(job != null)
                {
                    manPower.JobTitle = job.Description;
                }
                var period = db.GLFiscalYears.FirstOrDefault(x=>x.GlFiscalYearId == manPowerRequest.GlFiscalYearId);
                if(period != null)
                {
                    manPower.Period = period.Name;
                }
                var requstStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == manPowerRequest.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.APPROVE);
                if (requstStatus != null)
                {
                    manPower.Status = requstStatus.Status.ToString();

                    var empApp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requstStatus.ApproverId);

                    if (empApp != null)
                    {
                        manPower.Approver = empApp.FullNameaEnglish;
                    }
                    var reqDep = db.RequestesStatus.FirstOrDefault(x => x.RequestId == manPowerRequest.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);

                    if (reqDep != null)
                    {
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == reqDep.ApproverId);
                        if (emp != null)
                        {
                            manPower.DepApprover = emp.FullNameaEnglish;
                        }
                    }

                }
                else
                {
                    var depApp = db.RequestesStatus.FirstOrDefault(x => x.RequestId == manPowerRequest.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                    if (depApp != null)
                    {
                        manPower.Status = depApp.Status.ToString();
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requstStatus.ApproverId);
                        if (emp != null)
                        {
                            manPower.DepApprover = emp.FullNameaEnglish;
                        }
                    }
                    else
                    {
                        var reject = db.RequestesStatus.FirstOrDefault(x => x.RequestId == manPowerRequest.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.REJECT);
                        if (reject != null)
                        {
                            manPower.Status = reject.Status.ToString();
                        }
                        else
                        {
                            var pend = db.RequestesStatus.FirstOrDefault(x => x.RequestId == manPowerRequest.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.PEND);
                            if (pend != null)
                            {
                                manPower.Status = pend.Status.ToString();

                            }
                            else
                            {
                                var waiting = db.RequestesStatus.FirstOrDefault(x => x.RequestId == manPowerRequest.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                                if (waiting != null)
                                {
                                    manPower.Status = waiting.Status.ToString();
                                }
                                else
                                {
                                    manPower.Status = "Not Sent For Approval";
                                }
                            }
                        }
                    }


                }
            }

            manpowerRequestList.Add(manPower);

            return manpowerRequestList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

}
