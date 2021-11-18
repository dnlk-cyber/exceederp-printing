using ExceedERP.Core.Domain.ProductionFollowUp;
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
   public class ProformaItemsListParameterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProformaItemVM> GetProformaItems(string proId)
        {
            List<ProformaItemVM> itemList = new List<ProformaItemVM>();
            if(proId != null)
            {
                int.TryParse(proId, out int proformaId);
                var proformaItem = db.PFProformaItems.Where(x=>x.ProformaId == proformaId).ToList();
                foreach (var pro in proformaItem)
                {
                    var proforma = new ProformaItemVM();

                    //if(pro.JobTypeItemId != null)
                    //{
                    //    var item = db.JobTypeItems.FirstOrDefault(x => x.JobTypeId == pro.JobTypeId && x.JobTypeItemId == pro.JobTypeItemId);
                    //    if (item != null)
                    //    {
                    //        proforma.Description = item.Name;
                    //        proforma.ProformaItemId = pro.ProformaItemsId;

                    //        itemList.Add(proforma);
                    //    }
                    //}                   
                }

            }

            return itemList;
        }
    }
}
