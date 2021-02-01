using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ClearanceProviderObjectDataSource : List<ClearanceProvider>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ClearanceProvider> GetClearanceProvider()
        {
            ClearanceProvider provider = new ClearanceProvider();
            List<ClearanceProvider> list = new List<ClearanceProvider>();
            List<ClearanceProvider> allDepartment;
            var lookups = db.MainCompanyStructures.ToList();
            allDepartment = new List<ClearanceProvider>();
            allDepartment = db.ClearanceProvideres.ToList();
            if (allDepartment.Any())
            {
                foreach (var a in allDepartment)
                {
                    provider = new ClearanceProvider();
                    if (a.WorkUnit == null)
                    {
                        provider.Name = a.Name;
                    }
                    else
                    {

                        provider.Name = a.WorkUnit;


                    }
                    list.Add(provider);
                }
            }

            return list;
        }
    }
}