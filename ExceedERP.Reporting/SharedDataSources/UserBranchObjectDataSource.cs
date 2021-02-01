using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class UserBranchObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> GetBranch(string user)
        {
            List<string> myBranches = UserHelper.GetUserBranchId(user);

            List<Branch> list = new List<Branch> { };
            var branches = db.Branch.Where(x => myBranches.Contains(x.BranchId.ToString())).ToList();
            if (!branches.Any())
            {
                return list;
            }

            return branches;
        }

    }
}
