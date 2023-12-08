using ExceedERP.DataAccess.Context;
using System.Linq;

namespace ExceedERP.Helpers.CRM
{
    public class CRMConfigurationHelper
    {
        public static bool IsReplacingBranchSegmentInDistributionDisabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            return db.Configurations
                    .Any(x => x.Category == "CRM"
                            && x.Reference == "DISABLE_REPLACING_BRANCH_SEGMENT"
                            && x.Attribute == "TRUE");

        }
    }
}
