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
    public class MaterialConsumptionByDepartmentOds : List<MaterialConsumptionViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaterialConsumptionViewModel> GetMaterialConsumption(string periodfrom, string periodto,string branchid)
        {
            var consumptionLists = new List<MaterialConsumptionViewModel>();

            if (!string.IsNullOrEmpty(periodfrom) && !string.IsNullOrEmpty(periodto))
            {
                var from = int.Parse(periodfrom);
                var to = int.Parse(periodto);
                var dt11 = db.GLPeriods.Find(from).DateFrom;
                var dt22 = db.GLPeriods.Find(to).DateTo;


                List<string> permanntStorelist = db.InventoryStores.Where(x => x.IsTemp == false).Select(x => x.StoreCode).ToList();

                var issues = db.IssueItems
                    .Where(x => x.Issue.IssueDate >= dt11 && x.Issue.IssueDate <= dt22)
                    .Where(x => x.Issue.IsVoid == false)                   
                    .Where(x => x.Issue.IsOnlineApproved)
                    .Where(x => permanntStorelist.Contains(x.Issue.StoreCode)).ToList();

                if (!string.IsNullOrEmpty(branchid))
                {
                    int did = int.Parse(branchid);
                    var branchstores = db.InventoryStores.Where(x => x.BranchId ==did).Select(x => x.StoreCode).ToList();
                    issues = issues.Where(x => branchstores.Contains(x.Issue.StoreCode)).ToList();
                }

                   
                 var catagories = db.ItemCategorys.OrderBy(x=>x.Name).ToList();
                var departments = db.MainCompanyStructures.OrderBy(x=>x.Name).ToList();

                foreach (var catagory in catagories)
                {
                    foreach (var department in departments)
                    {
                       
                        var consumptionViewModel = new MaterialConsumptionViewModel
                        {
                            Description = catagory.Name,
                            Branch = department.Name,
                            Amount = issues.Where(x => x.ItemCategoryCode == catagory.ItemCategoryCode)
                                .Where(x => x.Issue.OrgStructureId==department.OrgStructureId).Sum(x => x.Total)
                        };

                        if(consumptionViewModel.Amount>0)
                         consumptionLists.Add(consumptionViewModel);
                    }

                }
            }
            return consumptionLists;
        }
    }

}