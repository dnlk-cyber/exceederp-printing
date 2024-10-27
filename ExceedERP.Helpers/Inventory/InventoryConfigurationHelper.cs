using ExceedERP.DataAccess.Context;
using System.Linq;

namespace ExceedERP.Helpers.Inventory
{
    public class InventoryConfigurationHelper
    {
        public static string GetIssueDistributionCostCenterDebitReplace()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "REPLACE_COSTCENTER_ON_ISSUE_DEBIT")
                    .FirstOrDefault();

                return conf?.Attribute ?? "";
            }
        }

        public static string GetBranchSubAccountReplace()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "APPLY_BRANCH_SUBACCOUNT_FOR_STORE_TRANSACTION")
                    .FirstOrDefault();

                return conf?.Attribute ?? "";
            }
        }

        public static string GetBinCardRSortOrder()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "BINCARD_SORT_ORDER")
                    .FirstOrDefault();

                return conf?.Attribute ?? "";
            }
        }

        public static bool IsBinCardRecalculateAllowed()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "ALLOW_BINCARD_RECALCULATE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool RecalculateSourceTransactionOnBincardUpdate()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "RECALCULATE_SOURCE_TRANSACTION_ON_BINCARD_UPDATE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsHidePriceOnPrintEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "HIDE_PRICE_ON_PRINT")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsElectronicSignatureEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "APPLY_ELECTRONIC_SIGNATURE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsFreshBazaarEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "ENABLE_FRESH_BAZAAR")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }
    }

}
