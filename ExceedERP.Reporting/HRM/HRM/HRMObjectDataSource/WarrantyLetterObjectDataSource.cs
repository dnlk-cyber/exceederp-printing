using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.Common;
using System.IO;
using System.Drawing;

using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.HRM;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class WarrantyLetterObjectDataSource : List<WarrantyLetterViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WarrantyVM> GetWarrantyLetter(string employee, bool header, string toCompany, string toPerson, string companyAddress)
        {
            List<WarrantyVM> warrantyOnePerson = new List<WarrantyVM>();

            List<WarrantyLetterViewModel> warrantyList = new List<WarrantyLetterViewModel>();

            var warranty = new WarrantyLetterViewModel();

            if (header)
            {
                var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                if (organization != null)
                {
                    warranty.TradeName = organization.TradeName;
                    warranty.AmharicName = organization.AmharicName;
                    var address =
                        db.Addresses.Where(
                            a =>
                                a.Reference == organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Count > 0)
                    {
                        foreach (var a in address)
                        {

                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                warranty.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                warranty.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (warranty.Address == null)
                                {
                                    warranty.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                warranty.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                warranty.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                warranty.WebSite = "Website:" + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                warranty.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                warranty.Tel = "Tel:  " + a.Value;

                            }

                        }

                    }
                    var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        warranty.LogoFileName = ByteArrayToImage(logo.BinaryFile);
                    }
                }
                // warrantyList.Add(warranty);
            }

            if (employee != null)
            {
                int.TryParse(employee, out int empId);
                var warrReq = db.ExprianceRequests.FirstOrDefault(x => x.EmployeeId == empId);
                if (warrReq != null)
                {
                    //warranty = new WarrantyLetterViewModel();
                    var empName = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == warrReq.EmployeeId);

                    if (empName != null)
                    {
                        string title = "";
                        var lookUP = db.Lookups.FirstOrDefault(x=>x.LookupId == empName.Title);
                        if(lookUP != null)
                        {
                            title = lookUP.Description;
                        }
                        warranty.Name = title + " " + empName.FullNameAmharic;
                        warranty.Company = "";
                        warranty.Adress = "";
                        warranty.RequestDate = warrReq.ApplicationDateAm;
                        //warranty.EmployeeStatus = empName.Status;
                        if (warranty.EmployeeStatus == EmployeeStatus.PERMANENT.ToString())
                        {
                            warranty.EmployeeStatus = "ቋሚ";
                          warranty.EmployeeStatus = EmployeeStatus.PERMANENT.ToString().ToLower();
                        }
                        if (warranty.EmployeeStatus == EmployeeStatus.CONTRACT.ToString())
                        {
                            warranty.EmployeeStatus = "ጊዜያዊ";
                            //warranty.EmployeeStatus = EmployeeStatus.CONTRACT.ToString().ToLower();
                        }

                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                        if (placement != null)
                        {
                            warranty.Salary = placement.Salary;

                            warranty.SalaryInWord = Convert.ToInt32(warranty.Salary).ToWordAmharic();
                        }
                        var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.WarrantyAm);
                        if (template != null)
                        {
                            warranty.TempOne = template.TempOne;
                        }
                    }

                }
                warrantyList.Add(warranty);



                foreach (var war in warrantyList)
                {
                    var letter = new WarrantyVM();

                    #region Headers

                    letter.Abbreviation = war.Abbreviation;
                    letter.Address = war.Address;
                    letter.AmharicName = war.AmharicName;
                    letter.City = war.City;
                    letter.Email = war.Email;
                    letter.Fax = war.Fax;
                    letter.LogoFileName = war.LogoFileName;
                    letter.AmharicName = war.AmharicName;
                    letter.TradeName = war.TradeName;
                    letter.Pobox = war.Pobox;
                    letter.Tel = war.Tel;
                    letter.WebSite = war.WebSite;

                    #endregion

                    letter.Title = "ዋስ ስለመሆን ይመለከታል፡፡";

                    var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.WarrantyAm);
                    if (template != null)
                    {
                        string doc = "";

                        doc = template.Document.Replace("-ApplicantName-", war.Name);
                        doc = doc.Replace("-ToName-", toPerson);
                        doc = doc.Replace("-ApplicationDate-", war.RequestDate);
                        string salary = war.Salary + "/" + war.SalaryInWord + "/";
                        doc = doc.Replace("-Salary-", salary);

                        letter.Warranty = doc;

                        warrantyOnePerson.Add(letter);
                    }
                }
            }

            return warrantyOnePerson;
        }
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }

        public class WarrantyVM
        {
            public string Warranty { get; set; }
            public string Title { get; set; }
            public string TradeName { get; set; }
            public string AmharicName { get; set; }
            public string Abbreviation { get; set; }
            public Image LogoFileName { get; set; }
            public string OrgAddress { get; set; }

            public string Email { get; set; }
            public string WebSite { get; set; }
            public string City { get; set; }
            public string Fax { get; set; }
            public string Pobox { get; set; }
            public string Tel { get; set; }
            public string Address { get; set; }
        }
    }
}
