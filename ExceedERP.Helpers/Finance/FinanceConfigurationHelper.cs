using ExceedERP.DataAccess.Context;
using System.Linq;

namespace ExceedERP.Helpers.Finance
{
    public class FinanceConfigurationHelper
    {
        public static bool AutoCalculatetravelPerdiem()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "AUTO_CALCULATE_TRAVEL_PERDIEM")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }
        public static bool AllowtravelClaimForOthers()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ALLOW_TRAVEL_CLAIM_FOR_OTHERS")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool ApplyClearingAccountInGRN()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "APPLY_CLEARING_ACCOUNT_GRN_DISTRIBUTION")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsGrnDistributionDisabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "DISABLE_GRN_DISTRIBUTION")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsTelebirrPaymentMethodEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_TELEBIRR_PAYMENT_METHOD")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsPRBudgetControlEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_PR_BUDGET_CONTROL")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsPaymentBudgetControlEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_PAYMENT_BUDGET_CONTROL")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IsPaymentRequestAuthorizationEnabled()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_PAYMENT_REQUEST_AUTHORIZATION")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static string GetBranchSubAccountReplace()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
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
                    .Where(x => x.Category == "Finance")
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
                    .Where(x => x.Category == "Finance")
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
                    .Where(x => x.Category == "Finance")
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
                    .Where(x => x.Category == "Finance")
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
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "APPLY_ELECTRONIC_SIGNATURE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IgnorePerdiemInClaim()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "IGNORE_PERDIEM_IN_CLAIM")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool IgnoreTravelDeparture()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "IGNORE_TRAVEL_DEPARTURE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }
    }

}
