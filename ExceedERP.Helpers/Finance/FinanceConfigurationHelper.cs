using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Finance
{
    public class FinanceConfigurationHelper
    {
        public static bool ApplyClearingAccountInGRN()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "APPLY_CLEARING_ACCOUNT_GRN_DISTRIBUTION")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static bool IsGrnDistributionDisabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "DISABLE_GRN_DISTRIBUTION")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static bool IsTelebirrPaymentMethodEnabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_TELEBIRR_PAYMENT_METHOD")
                   .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }

        public static bool IsPRBudgetControlEnabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_PR_BUDGET_CONTROL")
                   .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static bool IsPaymentRequestAuthorizationEnabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category == "Finance")
                    .Where(x => x.Reference == "ENABLE_PAYMENT_REQUEST_AUTHORIZATION")
                   .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static string GetBranchSubAccountReplace()
        {
            ExceedDbContext db = new ExceedDbContext();
            string value = "";

            var conf = db.Configurations
                    .Where(x => x.Category ==  "Finance")
                    .Where(x => x.Reference == "APPLY_BRANCH_SUBACCOUNT_FOR_STORE_TRANSACTION")
                    .FirstOrDefault();

            if (conf != null)
                value = conf.Attribute;

            return value;
        }
        public static string GetBinCardRSortOrder()
        {
            ExceedDbContext db = new ExceedDbContext();
            string sortOder = "";

            var conf = db.Configurations
                    .Where(x => x.Category ==  "Finance")
                    .Where(x => x.Reference == "BINCARD_SORT_ORDER")
                    .FirstOrDefault();

            if (conf != null)
                sortOder=conf.Attribute;

            return sortOder;
        }
        public static bool IsBinCardRecalculateAllowed()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category ==  "Finance")
                    .Where(x => x.Reference == "ALLOW_BINCARD_RECALCULATE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static bool RecalculateSourceTransactionOnBincardUpdate()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category ==  "Finance")
                    .Where(x => x.Reference == "RECALCULATE_SOURCE_TRANSACTION_ON_BINCARD_UPDATE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static bool IsHidePriceOnPrintEnabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category ==  "Finance")
                    .Where(x => x.Reference == "HIDE_PRICE_ON_PRINT")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
        public static bool IsElectronicSignatureEnabled()
        {
            ExceedDbContext db = new ExceedDbContext();

            var conf = db.Configurations
                    .Where(x => x.Category ==  "Finance")
                    .Where(x => x.Reference == "APPLY_ELECTRONIC_SIGNATURE")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
    }
}
