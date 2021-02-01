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
    public class DespensaryStoreObjectDataSourceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DespensaryStoreViewModel> GetDespensaryStore()
        {
            List<DespensaryStoreViewModel> despensaryStoreLists = new List<DespensaryStoreViewModel>();
            var despensaryStores = db.DespensaryStores.ToList();
            foreach (var despensaryStore in despensaryStores)
            {
                var despensary = new DespensaryStoreViewModel();

                var drugLists = db.DrugLists.FirstOrDefault(x => x.DrugListsId == despensaryStore.DrugListsId);
                if (drugLists != null)
                {
                    despensary.DrugCode = drugLists.DrugCode;
                    despensary.DrugName = drugLists.Description;
                    despensary.CurrentAmount = despensaryStore.BegStock;
                    despensary.NewReceived = despensaryStore.NewReceived;
                    despensary.TotalAmount = despensaryStore.Balance;

                    var unit = db.DrugUnits.FirstOrDefault(x=>x.DrugUnitId == despensaryStore.DrugUnitId);
                    if(unit != null)
                    {
                        despensary.Unit = unit.UoMName;
                    }
                }
                despensaryStoreLists.Add(despensary);
            }

            return despensaryStoreLists;
        }
    }
}
