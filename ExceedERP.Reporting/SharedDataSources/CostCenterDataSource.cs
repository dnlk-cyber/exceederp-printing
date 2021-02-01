using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class CostCenterDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BranchCostCenter> GetCostCenters()
        {
            List<BranchCostCenter> branchCostCenters = db.BranchCostCenter.ToList();
            return branchCostCenters;
        }

    }
}
