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
    public class ComplainODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ComplainVM> GetComplain(string id, bool header)
        {
            List<ComplainVM> singleComplain = new List<ComplainVM>();
            ComplainVM comp = new ComplainVM();
            var complain = db.Complains.FirstOrDefault(x => x.ComplainId.ToString() == id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");// CompanyCategory.MAIN_COMPANY);

            if (header)
            {

                if (organization != null)
                {
                    comp.Name = organization.TradeName;
                    comp.AmharicName = organization.AmharicName;
                    comp.Abbreviation = organization.Abbreviation;

                    var address =
                   db.Addresses.Where(
                       a =>
                           a.Reference == organization.MainCompanyId.ToString() &&
                           a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Any())
                    {
                        foreach (var a in address)
                        {
                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                comp.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                comp.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (comp.Address == null)
                                {
                                    comp.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                comp.POBox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                comp.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                comp.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                comp.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                comp.Tel = "Tel:  " + a.Value;

                            }
                        }

                    }


                    var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        comp.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
                comp.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }

            if (complain != null)
            {
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == complain.EmployeeId);
                if (person != null)
                {
                    comp.EmpName = person.FullNameAmharic;
                }
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == complain.EmployeeId && x.isCurrent);
                if (place != null)
                {
                    var main = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                    if (main != null)
                    {
                        comp.Department = main.Name;
                    }
                    var lookJob = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (lookJob != null)
                    {
                        comp.JobTitle = lookJob.Description;
                    }
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                    if (branch != null)
                    {
                        comp.Branch = branch.AmharicName;
                    }
                }

                comp.ComplainDate = complain.DateAmharic;
                comp.ComplainDescription = complain.ComplainDescription;

                var look = db.Lookups.FirstOrDefault(x => x.LookupId == complain.LookupId && x.LookupType == LookUpTypes.COMPLAIN_TYPE);
                if (look != null)
                {
                    comp.ComplainType = look.Description;
                }

                var responses = db.ComplainDetails.Where(x => x.ComplainId == complain.ComplainId);
                var response = "";
                foreach (var res in responses)
                {
                    response = response + " \n " + res.Response;
                }

                comp.Response = response;

                singleComplain.Add(comp);
            }

            return singleComplain;
        }
    }
}
