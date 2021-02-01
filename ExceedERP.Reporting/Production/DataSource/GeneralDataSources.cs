using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Production.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Production.DataSource
{
    //class GeneralDataSources
    //{
    //}
    [DataObject]
    public class ProductionBranchOds : List<BranchViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BranchViewModel> GetAllProductionBranches()
        {
            List<BranchViewModel> list = new List<BranchViewModel>();
            var productionOrders = db.ProductionOrders.ToList();
            var ids = new HashSet<int>(productionOrders.Select(p => p.BranchId));
            var branches = db.Branch.Where(b => (ids.Contains(b.BranchId))).ToList();
            if (branches.Any())
            {
                foreach (var branch in branches)
                {
                    var record = new BranchViewModel
                    {
                        Id = branch.BranchId,
                        Name = branch.Name
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }
}
