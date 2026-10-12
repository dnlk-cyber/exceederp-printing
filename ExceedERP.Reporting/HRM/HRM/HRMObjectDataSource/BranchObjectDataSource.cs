using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class BranchObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> GetBranch()
        {
            List<Branch> list = new List<Branch> { };

            var branchs = db.Branch.ToList();

            if (!branchs.Any())
            {
                return list;
            }

            return branchs;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}