using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{


    public class InventoryVariationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private static int _countId = 0;

         [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        public ActionResult Index()
            {
            _countId = 1; 
            return View();
         }

        private List<PropertyCountItem> PropertyCountItems()
            {
                List<PropertyCountItem> propertyCountItems =db.PropertyCountItems.Where( x => x.Status).ToList();
                return propertyCountItems; 
            }

         [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        public ActionResult PropertyCountItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<VariationVM> variationVms  = new List<VariationVM>();
            foreach (var count in PropertyCountItems())
                {
                   InventoryItem inventory = db.InventoryItems.Find(count.ItemCode);
                   VariationVM variation = new VariationVM();
                    if (count.PreviousBalance < count.Count)
                        {
                            variation.OveragePrice = (count.Count - count.PreviousBalance) * count.Price;
                            variation.OverageQty = count.Count - count.PreviousBalance;
                            variation.Amouont = count.Count*count.Price;
                            variation.TotalAmount = count.Count*count.PreviousBalance;
                            if (inventory != null)
                            {
                                variation.PhysicalAmount = count.PreviousBalance*inventory.AvgPrice; 
                                variation.Price = inventory.AvgPrice; 
                                variation.ShortageQty = 0;
                                variation.ShortagePrice = 0;
                                variation.PreviousBalance = count.PreviousBalance; 
                                variation.ItemCode = count.ItemCode;
                                variation.ItemCategoryCode = inventory.ItemCategoryCode;
                                variation.Count = count.Count;
                                variation.Uom = inventory.UoMName;
                                variation.ItemDescription = inventory.Name;
                            }
                            variationVms.Add(variation);

                        }
                else if (count.PreviousBalance > count.Count)
                    {
                        variation.ShortagePrice = (count.PreviousBalance - count.Count) * count.Price;
                        variation.ShortageQty =   count.PreviousBalance - count.Count;
                        variation.Amouont = count.Count * count.Price;
                        if (inventory != null)
                        {
                            variation.PhysicalAmount = count.PreviousBalance * inventory.AvgPrice;
                            variation.TotalAmount = count.Count * count.PreviousBalance;
                            variation.OverageQty = 0;
                            variation.Price = inventory.AvgPrice;
                            variation.OveragePrice = 0;
                            variation.PreviousBalance = count.PreviousBalance;
                            variation.ItemCode = count.ItemCode;
                            variation.ItemCategoryCode = inventory.ItemCategoryCode;
                            variation.Count = count.Count;
                            variation.Uom = inventory.UoMName;
                            variation.ItemDescription = inventory.Name;
                        }
                        variationVms.Add(variation);

                    }
                else
                    {
                        variation.ShortagePrice = 0;
                        variation.ShortageQty = 0;
                        variation.Amouont = count.Count * count.Price;
                        variation.OverageQty = 0;
                        variation.PhysicalAmount = count.PreviousBalance * inventory.AvgPrice;
                        variation.TotalAmount = count.Count * count.PreviousBalance;
                        variation.PreviousBalance = count.PreviousBalance;
                        variation.Price = inventory.AvgPrice;
                        variation.OveragePrice = 0;
                        variation.ItemCode = count.ItemCode;
                        variation.ItemCategoryCode = inventory.ItemCategoryCode;
                        variation.Count = count.Count;
                        variation.Uom = inventory.UoMName;
                        variation.ItemDescription = inventory.Name;
                        variationVms.Add(variation);

                    }
                
                }
            IQueryable<VariationVM> propertycountitems = variationVms.AsQueryable();
            DataSourceResult result = propertycountitems.ToDataSourceResult(request, variationVM => new {
                PropertyCountItemId = variationVM.PropertyCountItemId,
                ItemDescription = variationVM.ItemDescription,
                ItemCode = variationVM.ItemCode,
                Uom = variationVM.Uom,
               
                PreviousBalance = variationVM.PreviousBalance,
                Count = variationVM.Count,
                Status = variationVM.Status,
                Price = variationVM.Price,
                ItemCategoryCode = variationVM.ItemCategoryCode,
                ShortageQty = variationVM.ShortageQty,
                OverageQty = variationVM.OverageQty,
                ShortagePrice = variationVM.ShortagePrice,
                OveragePrice = variationVM.OveragePrice,
                Amouont = variationVM.Amouont,
                PhysicalAmount = variationVM.PhysicalAmount
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

         [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
