using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class DelegationCCODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DelegationCCVM> GetDeligation(string id)
        {
            List<DelegationCCVM> delegationCCList = new List<DelegationCCVM>();

            var delegationCCs = db.DelegationCCs.Where(x => x.DelegationId.ToString() == id).ToList();

            foreach (var del in delegationCCs)
            {
                var delgat = new DelegationCCVM();

                var comp = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == del.CC);
                if(comp != null)
                {
                    delgat.CC = comp.Name;
                }

                delegationCCList.Add(delgat);
            }

            return delegationCCList;
        }
    }
}
