using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using System.Linq;

namespace ExceedERP.Helpers.Inventory
{
    public static class PropertyCountHelper
    {
        public static void UpdatePropertyCountItemPreviousBalanceFromBinCard(BinCardLine countBinCard, BinCardLine latestBinCard)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var countLine =
                            db.PropertyCountItems
                            .FirstOrDefault(c => c.PropertyCountId.ToString() == countBinCard.Reference &&
                            c.ItemCode == countBinCard.StoreItemAssignment.ItemCode &&
                            c.PropertyCount.StoreCode == countBinCard.StoreItemAssignment.StoreCode);
                if (countLine != null)
                {
                    countLine.PreviousBalance = latestBinCard.BalanceQTY;

                    db.PropertyCountItems.Attach(countLine);
                    db.Entry(countLine).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
    }
}
