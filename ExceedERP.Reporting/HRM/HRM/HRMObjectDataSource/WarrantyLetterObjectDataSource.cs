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
        public List<WarrantyLetterViewModel> GetWarrantyLetter(string employee, bool header)
        {
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

            var warrReq = db.ExprianceRequests.FirstOrDefault(x => x.EmployeeId.ToString() == employee);
            if (warrReq != null)
            {
                //warranty = new WarrantyLetterViewModel();
                var empName = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == warrReq.EmployeeId);
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId.ToString() == employee && x.isCurrent);
                if (empName != null)
                {
                    warranty.Name = empName.FullNameAmharic;
                    warranty.Company = "";
                    warranty.Adress = "";
                    warranty.RequestDate = warrReq.ApplicationDateAm;
                    //warranty.EmployeeStatus = empName.Status;
                    if (warranty.EmployeeStatus == EmployeeStatus.PERMANENT.ToString())
                    {
                        warranty.EmployeeStatus = "ቋሚ";
                    }
                    if (warranty.EmployeeStatus == EmployeeStatus.CONTRACT.ToString())
                    {
                        warranty.EmployeeStatus = "ጊዜያዊ";
                    }
                    if (placement != null)
                    {
                        warranty.Salary = placement.Salary;

                        warranty.SalaryInWord =
                                     Convert.ToInt32(warranty.Salary).ToWordAmharic();
                    }
                    var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.WarrantyAm);
                    if (template != null)
                    {
                        warranty.TempOne = template.TempOne;
                    }
                }

            }
            warrantyList.Add(warranty);
            return warrantyList;
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
    }
}
