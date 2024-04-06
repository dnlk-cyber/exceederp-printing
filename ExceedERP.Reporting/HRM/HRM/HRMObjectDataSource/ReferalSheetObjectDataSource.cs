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
    public class ReferalSheetObjectDataSource : List<ReferalSheetVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReferalSheetVM> GetReferalSheets(string id, DateTime date, bool header)
        {

            ReferalSheetVM refer = new ReferalSheetVM();
            List<ReferalSheetVM> referalSheets = new List<ReferalSheetVM>();
            if (header)
            {
                var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
                if (Organization != null)
                {
                    refer.Name = Organization.TradeName;
                    refer.AmharicName = Organization.AmharicName;
                    refer.Abbreviation = Organization.Abbreviation;
                    var address = db.Addresses.Where(a =>a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Any())
                    {
                        foreach (var a in address)
                        {
                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                refer.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                refer.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (refer.Address == null)
                                {
                                    refer.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                refer.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                refer.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                refer.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                refer.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                refer.Tel = "Tel:  " + a.Value;

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
                        refer.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }


            DateTime dateD = new DateTime(date.Year, date.Month, date.Day);

            if (id != null)
            {

                var patientCard = db.PatientCard.FirstOrDefault(x => x.PatientId.ToString() == id /*&& x.Date == dateD*/);

                if (patientCard != null)
                {
                    var refaralSheet = db.ReferralSheet.FirstOrDefault(x=>x.Date == dateD);

                    if(refaralSheet != null)
                    {
                        var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == patientCard.EmployeeId);
                        if(person != null)
                        {
                            refer.FullName = person.FullNameaEnglish;
                        }

                        refer.Age = patientCard.Age.ToString();
                        refer.Sex = patientCard.Sex;

                        refer.DateRefer = refaralSheet.Date;
                        refer.ReferredFrom = refaralSheet.ReferredFrom;

                        var hospital = db.Companies.FirstOrDefault(x => x.Category == "2" && x.MainCompanyId == refaralSheet.MainCompanyId);

                        if(hospital != null)
                        {
                            refer.ReferredTo = hospital.TradeName;
                        }
                       
                        refer.Diagnosis = refaralSheet.Diagnosis;
                        refer.TreatmentGiven = refaralSheet.TreatmentGiven;                        
                    }
                }
            }

            referalSheets.Add(refer);

            return referalSheets;

        }
    }
}
