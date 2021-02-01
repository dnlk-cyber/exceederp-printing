using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Inventory.DataSource
{

    [DataObject]
    public class MaterialConsumptionObjectDataSource : List<MaterialConsumptionViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaterialConsumptionViewModel> GetMaterialConsumption(string periodfrom, string periodto)
        {
            var consumptionLists = new List<MaterialConsumptionViewModel>();

            if (!string.IsNullOrEmpty(periodfrom) && !string.IsNullOrEmpty(periodto))
            {
                var from = int.Parse(periodfrom);
                var to = int.Parse(periodto);
                var dt11 = db.GLPeriods.Find(from).DateFrom;
                var dt22 = db.GLPeriods.Find(to).DateTo;

               

                var issues = db.IssueItems
                    .Where(x => x.Issue.IssueDate >= dt11 && x.Issue.IssueDate <= dt22)
                    .Where(x => x.Issue.IsVoid == false)
                    .Where(x => x.Issue.IsOnlineApproved).ToList();

                var stores = db.InventoryStores.ToList();
                 var catagories = db.ItemCategorys.OrderBy(x=>x.Name).ToList();
                var branches = db.Branch.OrderBy(x=>x.Name).ToList();

                foreach (var catagory in catagories)
                {
                    foreach (var branch in branches)
                    {
                        var branchstores = stores.Where(x => x.BranchId == branch.BranchId).Select(x=>x.StoreCode).ToList();
                        var consumptionViewModel = new MaterialConsumptionViewModel
                        {
                            Description = catagory.Name,
                            Branch = branch.Name,
                            Amount = issues.Where(x => x.ItemCategoryCode == catagory.ItemCategoryCode)
                                .Where(x => branchstores.Contains(x.Issue.StoreCode)).Sum(x => x.Total)
                        };

                        consumptionLists.Add(consumptionViewModel);
                    }

                }
            }
            return consumptionLists;
        }
    }

}