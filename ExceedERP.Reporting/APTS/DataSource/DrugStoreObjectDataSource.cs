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
    public class DrugStoreObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DrugStoreViewModel> GetDrugStore()
        {
            List<DrugStoreViewModel> drugStoreLists = new List<DrugStoreViewModel>();
            var drugStores = db.DrugStoreItems.ToList();
            foreach (var drugStore in drugStores)
            {
                var drug = new DrugStoreViewModel();
                var drugLists = db.DrugLists.FirstOrDefault(x => x.DrugListsId == drugStore.DrugListsId);
                if (drugLists != null)
                {
                    drug.DrugCode = drugLists.DrugCode;
                    drug.DrugName = drugLists.Description;
                    //drug.CurrentAmount = drugStore.BegStock;
                   
                    drug.TotalAmount = drugStore.Balance;

                    var unit = db.DrugUnits.FirstOrDefault(x=>x.DrugUnitId == drugStore.DrugUnitId);
                    if(unit != null)
                    {
                        drug.Unit = unit.UoMName;
                    }
                }

                drugStoreLists.Add(drug);
            }

            return drugStoreLists;
        }
    }
}
