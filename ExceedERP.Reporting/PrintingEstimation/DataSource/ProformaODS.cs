using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.PrintingEstimation.DataSource
{
    [DataObject]
    public class ProformaODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProformaVM> GetProforma(string id, string proItemId)
        {
            List<ProformaVM> proformaList = new List<ProformaVM>();
            if (id != null)
            {
                int.TryParse(id, out int proformaId);

                var proforma = db.Proformas.FirstOrDefault(x => x.ProformaId == proformaId);
                if (proforma != null)
                {
                    var pro = new ProformaVM();
                    pro.InvoiceNo = proforma.InvoiceNo;
                    pro.EstNo = db.EstimationForms.Find(proforma.EstimationFormId)?.JobNumber;
                    pro.Date = proforma.InvoiceDate?.ToString("MM-dd-yyyy");
                    pro.TermsOfPayment = proforma.PaymentTerm.ToString() ;
                    pro.ValidityDate = proforma.ValidityDate?.ToString("MM-dd-yyyy");
                    pro.DeliveryDate = proforma.ValidityDate?.ToString("MM-dd-yyyy");
                    //pro.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(proforma.CreatedBy));
                    //pro.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(proforma.CheckedBy));
                    //pro.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(proforma.ApprovedBy));
                    var custDetail = db.OrganizationCustomers.Find(proforma.CustomerId);

                    if(custDetail!=null)
                    {
                        pro.CustomerName = custDetail.TradeName;
                        pro.ContactPhone = db.Addresses.FirstOrDefault(q=>q.Reference== proforma.CustomerId.ToString()
                                         && q.Category==AddressType.MOBILE_PHONE_1)?.Value;
                        pro.Telephone = db.Addresses.FirstOrDefault(q=>q.Reference== proforma.CustomerId.ToString()
                                         && q.Category==AddressType.HOME_PHONE)?.Value;
                        pro.Fax = db.Addresses.FirstOrDefault(q=>q.Reference== proforma.CustomerId.ToString()
                                         && q.Category==AddressType.FAX)?.Value;
                        pro.Email = db.Addresses.FirstOrDefault(q=>q.Reference== proforma.CustomerId.ToString()
                                         && q.Category==AddressType.OFFICE_EMAIL)?.Value;
                    }


                    var proformaItems = db.PFProformaItems.Where(x => x.ProformaId == proformaId).ToList();
                    if (proItemId != null)
                    {
                        int.TryParse(proItemId, out int proformaItemId);
                        proformaItems = proformaItems.Where(x => x.PFProformaItemsId == proformaItemId).ToList();
                    }
                    if (proformaItems.Any())
                    {
                        decimal subtotal = 0;
                        decimal vat = 0;
                        foreach (var item in proformaItems)
                        {
                            subtotal = subtotal + (item.UnitPrice * item.Quantity);
                            var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == item.JobTypeId);
                            if (jobType != null)
                            {
                                var tax = db.GLTaxRates.FirstOrDefault(x=>x.GLTaxRateID == jobType.GLTaxRateID);
                                if(tax != null)
                                {
                                    vat = vat + (item.UnitPrice * item.Quantity * tax.CalculatedRate);
                                }                               
                            }
                        }

                        pro.SubTotal = Math.Round(subtotal,2);
                        pro.Vat = Math.Round(vat,2);
                        pro.Total = Math.Round(pro.Vat + pro.SubTotal,2);
                        pro.PriceInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(pro.Total));
                    }

                    proformaList.Add(pro);
                }

            }

            return proformaList;
        }
    }
}
