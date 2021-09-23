using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Mega.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Mega.DataSource
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

                    pro.CustomerName = proforma.CustomerName;
                    pro.ContactName = proforma.ContactName;
                    pro.ContactPhone = proforma.ContactPhone;

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
                            var jobType = db.JobTypes.FirstOrDefault(x => x.JobTypeId == item.JobTypeId);
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
                        //pro.PriceInWord = ReportHelper.NumberToWords(Convert.ToDouble(pro.Total));
                      //  pro.PriceInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(pro.Total));
                    }

                    proformaList.Add(pro);
                }

            }

            return proformaList;
        }
    }
}
