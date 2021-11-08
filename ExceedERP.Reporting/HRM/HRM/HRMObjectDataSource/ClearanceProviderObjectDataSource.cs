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
        public List<ClearanceProvider> GetClearanceProvider(string empId)
        {
            List<ClearanceProvider> list = new List<ClearanceProvider>();

            var allProviders = db.ClearanceProvideres.ToList();

            foreach (var a in allProviders)
            {
                var provider = new ClearanceProvider();

                provider.Name = a.Name;

                list.Add(provider);
            }
            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                var placement = db.Placements.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                if (placement != null)
                {
                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                    if (mainCompany != null)
                    {
                        var checkDup = list.FirstOrDefault(x => x.Name.Trim().ToLower() == mainCompany.Name.Trim().ToLower());
                        if (checkDup == null)
                        {
                            var pro = new ClearanceProvider();

                            pro.Name = mainCompany.Name;

                            list.Add(pro);
                        }
                    }
                }
            }

            return list;
        }
    }
}