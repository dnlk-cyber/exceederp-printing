using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
    public class ProformaItemsODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProformaItemVM> GetProformaItems(string id, string proItemId)
        {
            List<ProformaItemVM> proformaItemList = new List<ProformaItemVM>();

            var proformaItems = db.PFProformaItems.ToList();

            if (id != null)
            {
                int.TryParse(id, out int proformaId);

                proformaItems = proformaItems.Where(x => x.ProformaId == proformaId).ToList();
            }
            if (proItemId != null)
            {
                int.TryParse(proItemId, out int proformaItemId);

                proformaItems = proformaItems.Where(x => x.PFProformaItemsId == proformaItemId).ToList();
            }
            foreach (var pro in proformaItems)
            {
                var proforma = new ProformaItemVM();

                proforma.Quantity = pro.Quantity;
                proforma.UnitPrice = pro.UnitPrice;

                proforma.BeforeVat = pro.UnitPrice * pro.Quantity;
             
               // proforma.Description = pro.Description;
                proforma.Size = pro.Size;
                //proforma.Page = pro.Page;

                var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == pro.JobTypeId);
                if (jobType != null)
                {
                    proforma.Description = jobType.JobName;

                    //var jobTypeItem = db.JobTypeItems.FirstOrDefault(x => x.JobTypeItemId == pro.JobTypeItemId);
                    //if (jobTypeItem != null)
                    //{
                    //    proforma.Description = proforma.Description + " - " + jobTypeItem.Name;
                    //}

                    var tax = db.GLTaxRates.FirstOrDefault(x=>x.GLTaxRateID == jobType.GLTaxRateID);
                    if(tax != null)
                    {
                        proforma.TaxPercent = tax.Rate + " %";
                        if(tax.Rate == 0)
                        {
                            proforma.TaxPercent = "Free";
                        }
                        proforma.TotalPrice = (pro.UnitPrice * pro.Quantity) + (pro.UnitPrice * pro.Quantity * tax.CalculatedRate);
                    }
                }

                proformaItemList.Add(proforma);
            }



            return proformaItemList;
        }
    }
}
