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
    public class TransferToClinicObjectDataSource : List<TransferToClinic>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TransferToClinic> GetTransferToClinic(string id, bool header)
        {
            List<TransferToClinic> transferList = new List<TransferToClinic>();
            TransferToClinic transfer = new TransferToClinic();
         
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
            if (header)
            {

                if (Organization != null)
                {
                    transfer.Name = Organization.TradeName;
                    transfer.AmharicName = Organization.AmharicName;
                    transfer.Abbreviation = Organization.Abbreviation;
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
                                transfer.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                transfer.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (transfer.Address == null)
                                {
                                    transfer.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                transfer.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                transfer.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                transfer.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                transfer.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                transfer.Tel = "Tel:  " + a.Value;

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
                        transfer.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            if (id != null)
            {
                int.TryParse(id, out int empId);
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (person != null)
                {
                    db = new ExceedDbContext();

                    transfer.FullName = person.FullNameAmharic;

                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId && x.isCurrent);

                    if (placement != null)
                    {
                        var lookUp = db.Lookups.FirstOrDefault(x=>x.LookupId == placement.LookupId);
                        if(lookUp != null)
                        {
                            transfer.JobTitle = lookUp.Description;
                        }

                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == placement.OrgStructureId);
                        if(mainCompany != null)
                        {
                            transfer.Department = mainCompany.Name;
                        }
                    }

                    transfer.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    var hour = DateTime.Now.TimeOfDay.Hours.ToString();
                    var minut = DateTime.Now.TimeOfDay.Minutes.ToString();
                    transfer.Time = hour + ":" + minut;

                    transferList.Add(transfer);

                }
            }

            return transferList;
        }
    }
}
