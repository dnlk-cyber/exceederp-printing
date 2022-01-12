using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Inventory
{
    public class InventoryConfigurationHelper
    {      

        public static string GetBinCardRSortOrder()
        {
            ExceedDbContext db = new ExceedDbContext();
            string sortOder = "";

            var conf = db.Configurations
                    .Where(x => x.Category == "Inventory")
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
                    .Where(x => x.Category == "Inventory")
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
                    .Where(x => x.Category == "Inventory")
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
                    .Where(x => x.Category == "Inventory")
                    .Where(x => x.Reference == "HIDE_PRICE_ON_PRINT")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

            if (conf != null)
                return true;
            else
                return false;
        }
    }
}
