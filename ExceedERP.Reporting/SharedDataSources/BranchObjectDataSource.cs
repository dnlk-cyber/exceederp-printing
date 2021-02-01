using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class BranchObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> GetBranch()
        {

            List<Branch> list = new List<Branch> { };


            var branches = db.Branch.ToList();
            if (!branches.Any())
            {
                return list;
            }

            return branches;
        }
    }
}