using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
 
    [DataObject]
    public class ProjectObjectDataSource : List<Branch>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> GetTranscript(object[] project)
        {

            Branch branch = new Branch();
            List<Branch> branchList = new List<Branch>();
            if (project != null)
            {
                if (project.Any())
                {
                    foreach (var bra in project)
                    {
                        var selectedBranch = db.Branch
                            .FirstOrDefault(x => x.BranchId.ToString() == bra.ToString());
                        branch = new Branch();
                        branch.BranchId = selectedBranch.BranchId;
                        branch.AmharicName = selectedBranch.AmharicName;

                        branchList.Add(branch);
                    }


                }
            }

            return branchList;
        }
    }
}
