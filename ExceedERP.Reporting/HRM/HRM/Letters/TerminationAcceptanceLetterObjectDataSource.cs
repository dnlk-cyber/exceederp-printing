using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class TerminationAcceptanceLetterObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TerminationAcceptanceLetterVM> GetTerminationAcceptance(string id, bool header)
        {
            List<TerminationAcceptanceLetterVM> acceptanceList = new List<TerminationAcceptanceLetterVM>();
            TerminationAcceptanceLetterVM acceptance = new TerminationAcceptanceLetterVM();

            if (header)
            {
                var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
                if (Organization != null)
                {
                    acceptance.Name = Organization.TradeName;
                    acceptance.AmharicName = Organization.AmharicName;
                    acceptance.Abbreviation = Organization.Abbreviation;
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
                                acceptance.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                acceptance.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (acceptance.Address == null)
                                {
                                    acceptance.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                acceptance.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                acceptance.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                acceptance.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                acceptance.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                acceptance.Tel = "Tel:  " + a.Value;

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
                        acceptance.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            acceptance.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);

            var termination = db.Terminations.FirstOrDefault(x=>x.EmployeeId.ToString() == id);
            if(termination != null)
            {
                var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == termination.EmployeeId);
                if(person != null)
                {
                    acceptance.EmpName = person.FullNameAmharic;
                    acceptance.RequestDate = termination.ApplicationDateAm;
                    acceptance.TerminationDate = termination.TerminationDateAm;
                    var look = db.Lookups.FirstOrDefault(x=>x.LookupId == person.Title && x.LookupType == LookUpTypes.PERSON_TITLE);
                    if(look != null)
                    {
                        acceptance.Title = look.Description; ;
                    }
                    
                }
            }

            var template = db.Templates.FirstOrDefault(x=>x.TemplateType == TemplateTypes.TerminationAcceptanceAm);
            if(template != null)
            {
                acceptance.TempOne = template.TempOne;
            }

            acceptanceList.Add(acceptance);

            return acceptanceList;
        }
    }
}
