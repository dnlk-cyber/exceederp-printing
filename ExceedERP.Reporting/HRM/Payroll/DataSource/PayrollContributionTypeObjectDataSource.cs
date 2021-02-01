using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollContributionTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPayrollContributionTypes()
        {
            List<string> list = new List<string> { };
            var contributionTypes = db.ContributionTypes.ToList().Select(s => new
            {
                nameSelector = s.Description
            }).OrderBy(x => x.nameSelector);

            if (!contributionTypes.Any())
            {
                return list;
            }
            return contributionTypes.Select(x => x.nameSelector).ToList();
        }
    }
}