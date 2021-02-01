using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class BranchDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BranchVm> GetBrancheshs()
        {
            List<BranchVm> branches = db.Branch
                .Select(x=> new BranchVm {BranchId=x.BranchId,Name=x.Name,LocationAccount=x.LocationAccount}).ToList();
            return branches;
        }


    }

    public class BranchVm
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string LocationAccount { get; set; }
        
    }
}
