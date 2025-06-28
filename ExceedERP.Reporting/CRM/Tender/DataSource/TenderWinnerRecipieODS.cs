using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.Tender.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ExceedERP.Reporting.CRM.Tender.DataSource
{
    [DataObject]
    public class TenderWinnerRecipieODS
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WinnerCustomerNameVM> GetSaleTenderWinnerRecipie(string id, string tin)
        {
            int.TryParse(id, out int slotId);
            var winners = db.Winners
                .Where(x => x.SalesLotId == slotId)
                .ToList();

            if (!winners.Any())
                return new List<WinnerCustomerNameVM>();

            var productIds = winners.Select(w => w.ProductID).Distinct().ToList();
            var productDict = db.Products
                .Where(p => productIds.Contains(p.ProductID))
                .ToDictionary(p => p.ProductID);

            var salesLotItems = db.SalesLotItems
                .Where(s => s.SalesLotId == slotId)
                .ToDictionary(s => s.ProductID, s => s.Quantity);

            var groupedWinners = winners.GroupBy(w => w.TinNumber);

            var result = new List<WinnerCustomerNameVM>();

            foreach (var group in groupedWinners)
            {
                var firstWinner = group.First();
                var customerVM = new WinnerCustomerNameVM
                {
                    CustomerName = firstWinner.CustomerName,
                    TinNumber = group.Key,
                    winnerRecipies = new List<WinnerRecipieVM>()
                };

                foreach (var item in group)
                {
                    var quantity = salesLotItems.TryGetValue(item.ProductID, out var qty) ? qty : 0;
                    var unit = productDict.TryGetValue(item.ProductID, out var product) ? product.UOM : null;

                    customerVM.winnerRecipies.Add(new WinnerRecipieVM
                    {
                        CustomerName = item.CustomerName,
                        Unit = unit,
                        UnitPrice = item.UnitPrice,
                        ItemName = item.ProductName,
                        Quantity = quantity,
                        TotalPrice = item.UnitPrice * quantity
                    });
                }

                result.Add(customerVM);
            }

            return result;
        }
        //public List<WinnerRecipieVM> GetSaleTenderWinnerRecipie(string id,string Tin)
        //{
        //    List<WinnerRecipieVM> list = new List<WinnerRecipieVM>();
        //    var FetchWinnerItems = db.Winners.Where(x=>x.TinNumber==Tin&&x.SalesLotId.ToString()==id);
        //    if(FetchWinnerItems.Any())
        //    {
        //        foreach(var Items in FetchWinnerItems)
        //        {
        //            var Item=new WinnerRecipieVM();
        //            Item.CustomerName = Items.CustomerName;
        //            Item.Unit = db.Products.FirstOrDefault(x=>x.ProductID==Items.ProductID).UOM;
        //            Item.UnitPrice = Items.UnitPrice;
        //            Item.ItemName = Items.ProductName;
        //            Item.Quantity = db.SalesLotItems.FirstOrDefault(x=>x.ProductID==Items.ProductID&&x.SalesLotId==Items.SalesLotId).Quantity;
        //            Item.TotalPrice = Items.UnitPrice * Item.Quantity;
        //            list.Add(Item);
        //        }
        //    }

        //    return list;
        //}
    }
    public class WinnerCustomerNameVM
    {
        public string CustomerName { get; set; }
        public string TinNumber { get; set; }
        public List<WinnerRecipieVM> winnerRecipies { get; set; }

    }


    public class WinnerRecipieVM
    {
        public string CustomerName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemName { get; set; }
        public decimal TotalPrice { get; set; }
    }
}