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
    class LaboratoryFormRequestHeadersObjectDataSource:IDisposable
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LaboratoryRequestFormEmployeeVM> GetRequesterEmployees(string id, bool header)
        {
            List<LaboratoryRequestFormEmployeeVM> requesterEmployee = new List<LaboratoryRequestFormEmployeeVM>();

            LaboratoryRequestFormEmployeeVM employee = new LaboratoryRequestFormEmployeeVM();

            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();

            if (header)
            {
                if (Organization != null)
                {
                    employee.Name = Organization.TradeName;
                    employee.AmharicName = Organization.AmharicName;
                    employee.Abbreviation = Organization.Abbreviation;
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
                                employee.Address = "Subcity: " + a.Value + " ";
                            }

                            if (a.Category == AddressType.KEBELE)
                            {
                                employee.Address += "Kebele: " + a.Value + " ";
                            }

                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (employee.Address == null)
                                {
                                    employee.Address += a.Value + ", Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                employee.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                employee.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                employee.WebSite = "Website:" + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                employee.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                employee.Tel = "Tel:  " + a.Value;

                            }

                        }

                    }

                    var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        employee.LogoFileName =
                            OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }

            }

            if (id != null)
            {

                var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x => x.PatientCardDetailId.ToString() == id);

                if (patientCardDetail != null)
                {
                    var patientCard = db.PatientCard.FirstOrDefault(x => x.PatientId == patientCardDetail.PatientId);

                    if (patientCard != null)
                    {
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == patientCard.EmployeeId);
                        if (person != null)
                        {
                            employee.FullName = person.FullNameaEnglish;
                            employee.Sex = person.Gender.ToString();
                            employee.CardNo = patientCard.PatientCardId;
                            employee.Age = (DateTime.Today.Year - person.DateOfBirth.Year).ToString();
                            employee.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day,DateTime.Today.Month,DateTime.Today.Year);
                        }
                    }
                }
            }

            requesterEmployee.Add(employee);

            return requesterEmployee;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
