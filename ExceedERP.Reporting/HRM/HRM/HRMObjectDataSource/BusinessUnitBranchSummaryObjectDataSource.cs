using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class BusinessUnitBranchSummaryObjectDataSource: IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BusinessUnitBRanchSummaryVM> GetBusinessUnitBranchSummary(string businessUnit)
        {
            IQueryable<Placement> placements = db.Placements;

            // Filter by Business Unit
            if (!string.IsNullOrWhiteSpace(businessUnit) &&
                int.TryParse(businessUnit, out int businessUnitId))
            {
                placements = placements
                    .Where(x => x.BusinessUnitId == businessUnitId);
            }

            var result = (
                from p in placements

                group p by p.BusinessUnitId into g

                join bu in db.BusinessUnits
                    on g.Key equals bu.BusinessUnitId

                select new BusinessUnitBRanchSummaryVM
                {
                    Code = bu.Accountcode,
                    BusinessUnitName = bu.Name,

                    NumberofBranchs = g
                        .Where(x => x.BusinessUnitId==g.Key)
                        .Select(x => x.BranchId)
                        .Distinct()
                        .Count()
                })
                .OrderBy(x => x.BusinessUnitName)
                .ToList();

            return result;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}