using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.APTS.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.DataSource
{
 

    [DataObject]
    public class ExpireDateObjectDatasource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExpireDrugViewModel> GetDrugByExpiryDate(DateTime from, DateTime to)
        {
            List<ExpireDrugViewModel> drugExpire = new List<ExpireDrugViewModel>();

            var receivals = db.DrugReceivalDetails.Where(x => x.ExpiryDate >= from && x.ExpiryDate <= to && x.Balance > 0);
            foreach (var receival in receivals)
            {
                ExpireDrugViewModel expire = new ExpireDrugViewModel();
                var drug = db.DrugLists.FirstOrDefault(x => x.DrugListsId == receival.DrugListsId);
                if (drug != null)
                {
                    expire.DrugCode = drug.DrugCode;
                    expire.DrugName = drug.Description;

                }
                expire.Amount = receival.Balance;
                expire.ExpireDate = receival.ExpiryDateAmh;
                var receivedD = db.DrugReceivalModel19s.FirstOrDefault(x=>x.DrugReceivalModel19Id == receival.DrugReceivalModel19Id);
                if(receivedD != null)
                {
                    expire.EntryDate = receivedD.ReceivingDateAmh;
                }
               

                decimal dateLeft = (receival.ExpiryDate - DateTime.Now).Days;
                expire.DateLeft = dateLeft;
                drugExpire.Add(expire);
            }

            return drugExpire;
        }
    }
}