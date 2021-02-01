using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common; 
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    //   [AuthorizeRoles( CRMRoles.CashSalesUser, CRMRoles.SalesQuoteUser,CRMRoles.SalesOrderUser)] 
    public class LineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();



        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucherID, string voucherDefinitionID)
        {
            IQueryable<LineItem> lineitems = db.LineItems.Where(li => li.Voucher == voucherID.ToString() && li.VoucherDefinitionID == voucherDefinitionID.ToString());
            DataSourceResult result = lineitems.ToDataSourceResult(request, LineItem => new
            {
                LineItemID = LineItem.LineItemID,
                Voucher = LineItem.Voucher,
                ThingID = LineItem.ThingID,
                UnitAmount = LineItem.UnitAmount,
                Quantity = LineItem.CalculatedCost / LineItem.UnitAmount,
                TotalAmount = LineItem.TotalAmount,
                Tax = LineItem.Tax,
                TaxAmount = LineItem.TaxAmount,
                CalculatedCost = LineItem.CalculatedCost,
                References = LineItem.References,
                StoreID = LineItem.StoreID,
                Variety = LineItem.Variety,
                Class = LineItem.Class,
                VoucherDefinitionID = LineItem.VoucherDefinitionID,
                Status = LineItem.Status,
                CreatedBy = LineItem.CreatedBy,
                DateCreated = LineItem.DateCreated,
                LastModified = LineItem.LastModified,
                ModifiedBy = LineItem.ModifiedBy,
                OrgBranchName = LineItem.OrgBranchName,
                PriceTag = LineItem.PriceTag,
                PriceTagDetail = LineItem.PriceTagDetail,
                Remark = LineItem.Remark,
                WithholdingType = LineItem.WithholdingType,
                WithholdingAmount = LineItem.WithholdingAmount,
                WithholdTax = LineItem.WithholdTax,
                WithholdTaxAmount = LineItem.WithholdTaxAmount,
                Vat = LineItem.Vat,

            });

            return Json(result);
        }


        public ActionResult LineItemInventory_Read([DataSourceRequest]DataSourceRequest request, int voucherID, string voucherDefinitionID)
        {
            IQueryable<LineItem> lineitems = db.LineItems.Where(li => li.Voucher == voucherID.ToString() && li.VoucherDefinitionID == voucherDefinitionID.ToString());
            DataSourceResult result = lineitems.ToDataSourceResult(request, LineItem => new
            {
                LineItemID = LineItem.LineItemID,
                Voucher = LineItem.Voucher,
                ThingID = LineItem.ThingID,
                UnitAmount = LineItem.UnitAmount,
                Quantity = LineItem.Quantity,
                TotalAmount = LineItem.TotalAmount,
                Tax = LineItem.Tax,
                TaxAmount = LineItem.TaxAmount,
                CalculatedCost = LineItem.CalculatedCost,
                References = LineItem.References,
                StoreID = LineItem.StoreID,
                Variety = LineItem.Variety,
                Class = LineItem.Class,
                VoucherDefinitionID = LineItem.VoucherDefinitionID,
                Status = LineItem.Status,
                CreatedBy = LineItem.CreatedBy,
                DateCreated = LineItem.DateCreated,
                LastModified = LineItem.LastModified,
                ModifiedBy = LineItem.ModifiedBy,
                OrgBranchName = LineItem.OrgBranchName,
                PriceTag = LineItem.PriceTag,
                PriceTagDetail = LineItem.PriceTagDetail,
                Remark = LineItem.Remark,
                WithholdingType = LineItem.WithholdingType,
                WithholdingAmount = LineItem.WithholdingAmount,
                WithholdTax = LineItem.WithholdTax,
                WithholdTaxAmount = LineItem.WithholdTaxAmount,
                Vat = LineItem.Vat,
            });

            return Json(result);
        }

        public ActionResult LineItems_Create([DataSourceRequest] DataSourceRequest request, LineItemViewModel model)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            db = new ExceedDbContext();
            LineItem Item = db.LineItems.Where(li => li.Voucher == model.Voucher.ToString() && li.ThingID == model.ThingID && li.VoucherDefinitionID == model.VoucherDefinitionId).FirstOrDefault();

            var TaxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == model.Tax).FirstOrDefault();
            var SaleStore = db.SalesStores.Where(x => x.SalesStoreId == model.StoreID).FirstOrDefault();
            var ProductId = db.Products.Where(x => x.ProductID == model.ThingID).FirstOrDefault().ProductID;
            if (!StockBalanceAvailable(SaleStore, ProductId, model.Quantity, model.Variety, model.Class))
            {
                ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");

            }

            else
            {



                var SalesStoreId = SaleStore.SalesStoreId;
                var productName = model.ThingID;

                var priceValue = db.Prices.Where(p => p.ReferenceType == model.PriceTag && p.Reference == model.PriceTagDetail && p.Thing == ProductId.ToString() && p.Variety == model.Variety).ToList();
                var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == model.Variety).FirstOrDefault();

                decimal Price = 0;
                if (priceValue != null)
                {

                    var classPrice = priceValue.Where(x => x.Class == model.Class).FirstOrDefault();
                    if (classPrice != null)
                    {
                        Price = classPrice.Value;
                    }
                    else
                    {
                        Price = priceValue.FirstOrDefault().Value;

                    }
                }
                else if (DefaultpriceValue != null)
                {

                    Price = DefaultpriceValue.Value;
                }

                string TaxId = model.Tax;
                var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                decimal taxPercent = GLTaxRate.Rate / 100;

                decimal TaxAmount = (model.Quantity * Price * taxPercent);
                decimal TotalAmount = (model.Quantity * Price) + TaxAmount;
                decimal CalculatedAmount = TotalAmount - TaxAmount;
               
              
                decimal WithHoldingTaxAmount = decimal.Zero;
                decimal WithHoldingAmount = decimal.Zero;

                var Rates = db.GLTaxRates.ToList();

                if (model.WithholdingType != null)
                {

                    CRMSalesHelper.ConfigureWitholding(model.WithholdTax, model.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, TotalAmount);
                }  
                var entity = new LineItem
                    {
                        Voucher = model.Voucher.ToString(),
                        VoucherDefinitionID = model.VoucherDefinitionId,
                        ThingID = ProductId,
                        StoreID = SalesStoreId,
                        UnitAmount = Price,
                        WithholdingType = model.WithholdingType,
                        WithholdingAmount = WithHoldingAmount,
                        WithholdTax = model.WithholdTax,
                        WithholdTaxAmount = WithHoldingTaxAmount,
                        Vat = TaxRate.Rate,
                        Tax = GLTaxRate.GLTaxRateID.ToString(),
                        TaxAmount = TaxAmount,
                        Quantity = model.Quantity,
                        TotalAmount = TotalAmount,

                        CalculatedCost = CalculatedAmount,
                        Status = CRMConstants.Pending,
                        Variety = model.Variety,
                        Class = model.Class,
                        PriceTag = model.PriceTag,
                        PriceTagDetail = model.PriceTagDetail,

                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,

                    };


                try
                {
                    db.LineItems.Add(entity);
                    int save = db.SaveChanges();
                    if (save == 1)
                    {

                        if (model.AdditionalCostItem.Any())
                        {
                            foreach (var AdditionalCost in model.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                            {

                                var Cost = new AdditionalCostLog
                                {
                                    AdditionalCostId = int.Parse(AdditionalCost.Id),
                                    LineItemId = entity.LineItemID,
                                    Value = int.Parse(AdditionalCost.Name)

                                };
                                db.AdditionalCostLogs.Add(Cost);
                                db.SaveChanges();

                            }

                        }
                    }
                }
                catch (Exception e)
                {
                   
                }
            }


            if (model.VoucherDefinitionId == "0")
            {
                return RedirectToAction("Index", "SalesQuoteVoucher");
            }
            if (model.VoucherDefinitionId == "1")
            {
                return RedirectToAction("Index", "SalesOrderVoucher");
            }
            if (model.VoucherDefinitionId == "2")
            {
                return RedirectToAction("Index", "CashSalesInvoice");
            }
            return View();
        }

        decimal CalculateTax(string taxCode)
        {
            List<Tax> GLTaxRate = new List<Tax>(db.Taxs.Where(t => t.TaxID.ToString() == taxCode));
            var taxPercent = GLTaxRate.FirstOrDefault();
            decimal taxValue = (decimal)(taxPercent != null ? taxPercent.Amount : 0);
            return taxValue;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Update([DataSourceRequest]DataSourceRequest request, LineItemViewModel LineItem)
        {
            ModelState.Remove("StockBalance");

            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();

            db = new ExceedDbContext();
            LineItem Item = db.LineItems.Where(li => li.Voucher == LineItem.Voucher.ToString() && li.LineItemID == LineItem.LineItemID && li.VoucherDefinitionID == LineItem.VoucherDefinitionId).FirstOrDefault();

            if (Item != null)
            {
                var SaleStore = db.SalesStores.Where(x => x.SalesStoreId == LineItem.StoreID).FirstOrDefault();

                if (!StockBalanceAvailable(SaleStore, LineItem.ThingID, LineItem.Quantity, LineItem.Variety, LineItem.Class))
                {
                    ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");

                }
                else
                {




                    string sID = LineItem.StoreID.ToString();
                    var SalesStoreId = SaleStore.SalesStoreId;
                    var productName = LineItem.ThingID;
                    var ProductId = db.Products.Where(x => x.ProductID == LineItem.ThingID).FirstOrDefault().ProductID;
                    var priceValue = db.Prices.Where(p => p.ReferenceType == LineItem.PriceTag && p.Reference == LineItem.PriceTagDetail && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).ToList();
                    var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).FirstOrDefault();

                    decimal Price = 0;
                    if (priceValue != null)
                    {

                        var classPrice = priceValue.Where(x => x.Class == LineItem.Class).FirstOrDefault();
                        if (classPrice != null)
                        {
                            Price = classPrice.Value;
                        }
                        else
                        {
                            Price = priceValue.FirstOrDefault().Value;

                        }
                    }
                    else if (DefaultpriceValue != null)
                    {

                        Price = DefaultpriceValue.Value;
                    }

                    string TaxId = LineItem.Tax;
                    var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                    decimal taxPercent = GLTaxRate.Rate / 100;

                    decimal TaxAmount = taxPercent * LineItem.Quantity * Price;
                    decimal TotalAmount = (LineItem.Quantity * Price) + TaxAmount;
                    decimal CalculatedAmount = TotalAmount - TaxAmount;


                    decimal WithHoldingTaxAmount = decimal.Zero;
                    decimal WithHoldingAmount = decimal.Zero;

                    var Rates = db.GLTaxRates.ToList();

                    if (LineItem.WithholdingType != null)
                    {
                    CRMSalesHelper.ConfigureWitholding(LineItem.WithholdTax, LineItem.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, TotalAmount);
                    }
                    Item.ThingID = ProductId;
                    Item.StoreID = SalesStoreId;
                    Item.UnitAmount = Price;
                    Item.Quantity = LineItem.Quantity;
                    Item.TotalAmount = TotalAmount;
                    Item.WithholdingType = LineItem.WithholdingType;
                    Item.WithholdingAmount = LineItem.WithholdingAmount;
                    Item.WithholdTax = LineItem.WithholdTax;
                    Item.WithholdTaxAmount = LineItem.WithholdTaxAmount;
                    Item.Vat = GLTaxRate.Rate;
                    Item.Tax = GLTaxRate.GLTaxRateID.ToString();
                    Item.TaxAmount = TaxAmount;
                    Item.CalculatedCost = CalculatedAmount;
                    Item.Status = CRMConstants.Pending;
                    Item.PriceTag = LineItem.PriceTag;
                    Item.PriceTagDetail = LineItem.PriceTagDetail;
                    Item.Variety = LineItem.Variety;
                    Item.Class = LineItem.Class;



                    try
                    {
                        db.LineItems.Attach(Item);
                        db.Entry(Item).State = EntityState.Modified;
                        db.SaveChanges();

                        int save = db.SaveChanges();
                        if (save == 1)
                        {
                            if (LineItem.AdditionalCostItem != null)
                            {
                                if (LineItem.AdditionalCostItem.Any())
                                {
                                    db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList().ForEach(a => { db.AdditionalCostLogs.Attach(a); db.AdditionalCostLogs.Remove(a); db.SaveChanges(); });
                                    foreach (var AdditionalCost in LineItem.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                                    {

                                        var Cost = new AdditionalCostLog
                                        {
                                            AdditionalCostId = int.Parse(AdditionalCost.Id),
                                            LineItemId = Item.LineItemID,
                                            Value = int.Parse(AdditionalCost.Name)

                                        };
                                        db.AdditionalCostLogs.Add(Cost);
                                        db.SaveChanges();

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {


                    }
                }
            }


            if (LineItem.VoucherDefinitionId == "0")
            {
                return RedirectToAction("Index", "SalesQuoteVoucher");
            }
            if (LineItem.VoucherDefinitionId == "1")
            {
                return RedirectToAction("Index", "SalesOrderVoucher");
            }
            if (LineItem.VoucherDefinitionId == "2")
            {
                return RedirectToAction("Index", "CashSalesInvoice");
            }
            return View();
        }

        public ActionResult CreditLineItems_Create([DataSourceRequest] DataSourceRequest request, LineItemViewModel model)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            db = new ExceedDbContext();
            LineItem Item = db.LineItems.Where(li => li.Voucher == model.Voucher.ToString() && li.ThingID == model.ThingID && li.VoucherDefinitionID == model.VoucherDefinitionId).FirstOrDefault();

            var TaxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == model.Tax).FirstOrDefault();
            var SaleStore = db.SalesStores.Where(x => x.SalesStoreId == model.StoreID).FirstOrDefault();
            var ProductId = db.Products.Where(x => x.ProductID == model.ThingID).FirstOrDefault().ProductID;
            if (!StockBalanceAvailable(SaleStore, ProductId, model.Quantity, model.Variety, model.Class))
            {
                ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");

            }

            else
            {



                var SalesStoreId = SaleStore.SalesStoreId;
                var productName = model.ThingID;

                var priceValue = db.Prices.Where(p => p.ReferenceType == model.PriceTag && p.Reference == model.PriceTagDetail && p.Thing == ProductId.ToString() && p.Variety == model.Variety).ToList();
                var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == model.Variety).FirstOrDefault();

                decimal Price = 0;
                if (priceValue != null)
                {

                    var classPrice = priceValue.Where(x => x.Class == model.Class).FirstOrDefault();
                    if (classPrice != null)
                    {
                        Price = classPrice.Value;
                    }
                    else
                    {
                        Price = priceValue.FirstOrDefault().Value;

                    }
                }
                else if (DefaultpriceValue != null)
                {

                    Price = DefaultpriceValue.Value;
                }

                string TaxId = model.Tax;
                var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                decimal taxPercent = GLTaxRate.Rate / 100;


                decimal TaxAmount = (model.Quantity * Price * taxPercent);
                decimal TotalAmount = (model.Quantity * Price) + TaxAmount;
                decimal CalculatedAmount = TotalAmount - TaxAmount;


                decimal WithHoldingTaxAmount = decimal.Zero;
                decimal WithHoldingAmount = decimal.Zero;

                var Rates = db.GLTaxRates.ToList();


                if (model.WithholdingType != null)
                {
                    CRMSalesHelper.ConfigureWitholding(model.WithholdTax, model.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, TotalAmount);
                }
                  

                var entity = new LineItem
                {
                    Voucher = model.Voucher.ToString(),
                    VoucherDefinitionID = model.VoucherDefinitionId,
                    ThingID = ProductId,
                    StoreID = SalesStoreId,
                    UnitAmount = Price,
                    WithholdingType = model.WithholdingType,
                    WithholdingAmount = model.WithholdingAmount,
                    WithholdTax = model.WithholdTax,
                    WithholdTaxAmount = model.WithholdTaxAmount,
                    Vat = TaxRate.Rate,
                    Tax = GLTaxRate.GLTaxRateID.ToString(),
                    TaxAmount = TaxAmount,
                    Quantity = model.Quantity,
                    TotalAmount = TotalAmount,

                    CalculatedCost = CalculatedAmount,
                    Status = CRMConstants.Pending,
                    Variety = model.Variety,
                    Class = model.Class,
                    PriceTag = model.PriceTag,
                    PriceTagDetail = model.PriceTagDetail,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };


                try
                {
                    db.LineItems.Add(entity);
                    int save = db.SaveChanges();
                    if (save == 1)
                    {

                        if (model.AdditionalCostItem.Any())
                        {
                            foreach (var AdditionalCost in model.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                            {

                                var Cost = new AdditionalCostLog
                                {
                                    AdditionalCostId = int.Parse(AdditionalCost.Id),
                                    LineItemId = entity.LineItemID,
                                    Value = int.Parse(AdditionalCost.Name)

                                };
                                db.AdditionalCostLogs.Add(Cost);
                                db.SaveChanges();

                            }

                        }
                    }
                }
                catch (Exception e)
                {

                }
            }


            if (model.VoucherDefinitionId == "0")
            {
                return RedirectToAction("Index", "SalesQuoteVoucher");
            }
            if (model.VoucherDefinitionId == "1")
            {
                return RedirectToAction("Index", "SalesOrderVoucher");
            }
            if (model.VoucherDefinitionId == "2")
            {
                return RedirectToAction("Index", "CreditsSalesInvoice");
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditLineItems_Update([DataSourceRequest]DataSourceRequest request, LineItemViewModel LineItem)
        {
            ModelState.Remove("StockBalance");


            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            db = new ExceedDbContext();
            LineItem Item = db.LineItems.Where(li => li.Voucher == LineItem.Voucher.ToString() && li.LineItemID == LineItem.LineItemID && li.VoucherDefinitionID == LineItem.VoucherDefinitionId).FirstOrDefault();

            if (Item != null)
            {
                var SaleStore = db.SalesStores.Where(x => x.SalesStoreId == LineItem.StoreID).FirstOrDefault();

                if (!StockBalanceAvailable(SaleStore, LineItem.ThingID, LineItem.Quantity, LineItem.Variety, LineItem.Class))
                {
                    ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");

                }
                else
                {




                    string sID = LineItem.StoreID.ToString();
                    var SalesStoreId = SaleStore.SalesStoreId;
                    var productName = LineItem.ThingID;
                    var ProductId = db.Products.Where(x => x.ProductID == LineItem.ThingID).FirstOrDefault().ProductID;
                    var priceValue = db.Prices.Where(p => p.ReferenceType == LineItem.PriceTag && p.Reference == LineItem.PriceTagDetail && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).ToList();
                    var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).FirstOrDefault();

                    decimal Price = 0;
                    if (priceValue != null)
                    {

                        var classPrice = priceValue.Where(x => x.Class == LineItem.Class).FirstOrDefault();
                        if (classPrice != null)
                        {
                            Price = classPrice.Value;
                        }
                        else
                        {
                            Price = priceValue.FirstOrDefault().Value;

                        }
                    }
                    else if (DefaultpriceValue != null)
                    {

                        Price = DefaultpriceValue.Value;
                    }

                    string TaxId = LineItem.Tax;
                    var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                    decimal taxPercent = GLTaxRate.Rate / 100;

                    decimal TaxAmount = taxPercent * LineItem.Quantity * Price;
                    decimal TotalAmount = (LineItem.Quantity * Price) + TaxAmount;
                    decimal CalculatedAmount = TotalAmount - TaxAmount;



                    decimal WithHoldingTaxAmount = decimal.Zero;
                    decimal WithHoldingAmount = decimal.Zero;

                    var Rates = db.GLTaxRates.ToList();

                    if (LineItem.WithholdingType != null)
                    {
                        CRMSalesHelper.ConfigureWitholding(LineItem.WithholdTax, LineItem.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, TotalAmount);
                    }

                    Item.WithholdingType = LineItem.WithholdingType;
                    Item.WithholdingAmount = LineItem.WithholdingAmount;
                    Item.WithholdTax = LineItem.WithholdTax;
                    Item.WithholdTaxAmount = LineItem.WithholdTaxAmount;
                    Item.Vat = GLTaxRate.Rate;
                    Item.Tax = GLTaxRate.GLTaxRateID.ToString();
                    Item.TaxAmount = TaxAmount;
                    Item.ThingID = ProductId;
                    Item.StoreID = SalesStoreId;
                    Item.UnitAmount = Price;
                    Item.Quantity = LineItem.Quantity;
                    Item.TotalAmount = TotalAmount;
                    Item.Tax = GLTaxRate.GLTaxRateID.ToString();
                    Item.TaxAmount = TaxAmount;
                    Item.CalculatedCost = CalculatedAmount;
                    Item.Status = CRMConstants.Pending;
                    Item.PriceTag = LineItem.PriceTag;
                    Item.PriceTagDetail = LineItem.PriceTagDetail;
                    Item.Variety = LineItem.Variety;
                    Item.Class = LineItem.Class;



                    try
                    {
                        db.LineItems.Attach(Item);
                        db.Entry(Item).State = EntityState.Modified;
                        db.SaveChanges();

                        int save = db.SaveChanges();
                        if (save == 1)
                        {
                            if (LineItem.AdditionalCostItem != null)
                            {
                                if (LineItem.AdditionalCostItem.Any())
                                {
                                    db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList().ForEach(a => { db.AdditionalCostLogs.Attach(a); db.AdditionalCostLogs.Remove(a); db.SaveChanges(); });
                                    foreach (var AdditionalCost in LineItem.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                                    {

                                        var Cost = new AdditionalCostLog
                                        {
                                            AdditionalCostId = int.Parse(AdditionalCost.Id),
                                            LineItemId = Item.LineItemID,
                                            Value = int.Parse(AdditionalCost.Name)

                                        };
                                        db.AdditionalCostLogs.Add(Cost);
                                        db.SaveChanges();

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {


                    }
                }
            }


            if (LineItem.VoucherDefinitionId == "0")
            {
                return RedirectToAction("Index", "SalesQuoteVoucher");
            }
            if (LineItem.VoucherDefinitionId == "1")
            {
                return RedirectToAction("Index", "SalesOrderVoucher");
            }
            if (LineItem.VoucherDefinitionId == "2")
            {
                return RedirectToAction("Index", "CreditsSalesInvoice");
            }
            return View();
        }






        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region  Support Func
        public bool StockBalanceAvailable(SalesStore Store, int ProductId, decimal quantity, string VarietyID, string Class)
        {
            string SaleQuoteDefination = "0";
            string SalesOrderVoucherDefination = "1";
            string CashInvoiceDefination = "2";

            List<LineItem> saleItem = new List<LineItem>();
            List<LineItem> cashinvoiceItem = new List<LineItem>();
            List<LineItem> SalequoteItem = new List<LineItem>();
            var Product = db.Products.Where(x => x.ProductID == ProductId).FirstOrDefault();
            if (Product != null)
            {
                List<StoreItemAssignment> stockLevel = Store.InventoryStore.StoreItemAssignments.Where(x => x.ItemCode == Product.ItemCode && x.StoreCode == Store.InventoryStore.StoreCode).ToList();

                List<LineItem> SalesOrderItems = db.LineItems.Where(l => (l.ThingID == ProductId && l.VoucherDefinitionID == SalesOrderVoucherDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();
                List<LineItem> SalesQuoteItems = db.LineItems.Where(l => (l.ThingID == ProductId && l.VoucherDefinitionID == SaleQuoteDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();
                List<LineItem> CashInvoiceItems = db.LineItems.Where(l => (l.ThingID == ProductId && l.VoucherDefinitionID == CashInvoiceDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();

                if (!string.IsNullOrEmpty(VarietyID))
                {
                    SalesQuoteItems = SalesQuoteItems.Where(x => x.Variety == VarietyID).ToList();
                    SalesOrderItems = SalesOrderItems.Where(x => x.Variety == VarietyID).ToList();
                    CashInvoiceItems = CashInvoiceItems.Where(x => x.Variety == VarietyID).ToList();
                }


                if (!string.IsNullOrEmpty(Class))
                {
                    SalesQuoteItems = SalesQuoteItems.Where(x => x.Class == Class).ToList();
                    SalesOrderItems = SalesOrderItems.Where(x => x.Class == Class).ToList();
                    CashInvoiceItems = CashInvoiceItems.Where(x => x.Class == Class).ToList();
                }
                foreach (var SalesQuoteItem in SalesQuoteItems)
                {
                    string OrderID = SalesQuoteItem.Voucher;
                    SalesQuoteVoucher sov =
                        db.SalesQuoteVouchers.Where(s => s.SalesQuoteVoucherID.ToString() == OrderID).SingleOrDefault();

                    if (sov != null)
                    {
                        if (sov.LastObjectState == CRMConstants.Forwarded)
                        {

                            SalequoteItem.Add(SalesQuoteItem);
                        }
                    }
                }
                if (SalequoteItem != null)
                {
                    foreach (var sale in SalequoteItem)
                    {
                        SalesQuoteItems.Remove(sale);

                    }
                }



                foreach (var SalesOrderItem in SalesOrderItems)
                {
                    string OrderID = SalesOrderItem.Voucher;
                    SalesOrderVoucher sov =
                        db.SalesOrderVouchers.Where(s => s.SalesOrderVoucherID.ToString() == OrderID).SingleOrDefault();

                    if (sov != null)
                    {
                        if (sov.LastObjectState == CRMConstants.Forwarded)
                        {

                            saleItem.Add(SalesOrderItem);
                        }
                    }
                }
                if (saleItem != null)
                {
                    foreach (var sale in saleItem)
                    {
                        SalesOrderItems.Remove(sale);

                    }
                }

                foreach (var CashInvoiceItem in CashInvoiceItems)
                {
                    string InvoiceID = CashInvoiceItem.Voucher;
                    CashSalesInvoice cInvoice =
                        db.CashSalesInvoices.Where(
                            c =>
                                c.CashSalesInvoiceID.ToString() == InvoiceID).SingleOrDefault();
                    if (cInvoice != null)
                    {
                        if (cInvoice.LastObjectState == CRMConstants.Forwarded)
                        {

                            cashinvoiceItem.Add(CashInvoiceItem);
                        }
                    }

                }
                if (cashinvoiceItem != null)
                {
                    foreach (var invoice in cashinvoiceItem)
                    {

                        CashInvoiceItems.Remove(invoice);
                    }
                }

                decimal QuoteQuantity = 0;
                if (SalesQuoteItems != null)
                {
                    QuoteQuantity = SalesQuoteItems.Sum(s => s.Quantity);
                }
                else
                {
                    QuoteQuantity = 0;
                }

                decimal OrderedQuantity = 0;
                if (SalesOrderItems != null)
                {
                    OrderedQuantity = SalesOrderItems.Sum(s => s.Quantity);
                }
                else
                {
                    OrderedQuantity = 0;
                }
                decimal InvoiceChecKQuantity = 0;
                if (SalesOrderItems != null)
                {
                    InvoiceChecKQuantity = CashInvoiceItems.Sum(s => s.Quantity);
                }
                else
                {
                    InvoiceChecKQuantity = 0;
                }
                if (stockLevel != null)
                {
                    List<StoreItemAssignment> stockBalance = new List<StoreItemAssignment>();
                    if (!string.IsNullOrEmpty(Class))
                    {
                        stockBalance = stockLevel.Where(x => x.ItemCategoryCode == Class).ToList();
                        if (stockBalance == null)
                        {
                            stockBalance = stockLevel;
                        }
                    }
                    else
                    {

                        stockBalance = stockLevel;

                    }

                    decimal stocktotal = stockBalance.Sum(s => s.Balance) - OrderedQuantity - InvoiceChecKQuantity - QuoteQuantity;

                    decimal reserved = OrderedQuantity + InvoiceChecKQuantity + QuoteQuantity;

                    if (stocktotal < 0)
                    {
                        return false;
                    }

                    if (stocktotal < quantity)
                    {
                        return false;
                    }
                    if ((stocktotal + reserved) > quantity)
                    {
                        return true;
                    }

                }

                return false;

            }
            return false;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Destroy([DataSourceRequest]DataSourceRequest request, LineItem LineItem)
        {
            LineItem Item = db.LineItems.Where(x => x.LineItemID == LineItem.LineItemID).FirstOrDefault();
            db.LineItems.Attach(Item);
            db.LineItems.Remove(Item);
            db.SaveChanges();

            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Line Item number " + LineItem.LineItemID);


            return Json(new[] { LineItem }.ToDataSourceResult(request));
        }


        /*     public bool StockBalanceAvailable(string storeID, string productID,decimal quantity , string Variety, string Class)
             {
                 string SaleQuoteDefination = "0";
                 string SalesOrderVoucherDefination = "1";
                 string CashInvoiceDefination = "2";
                 string productName = db.Products.Where(p => p.ProductID.ToString() == productID).SingleOrDefault().Name;
                 var stockLevel = db.StockBalances.Where(sl => sl.StoreID == storeID && sl.Thing == productID && sl.Variety ==Variety && sl.StockBalanceType == "BEGINNING_BALANCE").ToList();
                 List<LineItem> SalesOrderItems = db.LineItems.Where(l => l.ThingID == productName && l.VoucherDefinitionID == SalesOrderVoucherDefination && l.Variety ==Variety).ToList();
                 List<LineItem> SalesQuoteItems = db.LineItems.Where(l => l.ThingID == productName && l.VoucherDefinitionID == SaleQuoteDefination && l.Variety ==Variety).ToList();

                 List<LineItem> CashInvoiceItems = db.LineItems.Where(l => l.ThingID == productName && l.VoucherDefinitionID == CashInvoiceDefination && l.Variety ==Variety ).ToList();

                 List<LineItem> saleItem = new List<LineItem>();
                 List<LineItem> cashinvoiceItem = new List<LineItem>();
                 List<LineItem> SalequoteItem = new List<LineItem>();

                 foreach (var SalesQuoteItem in SalesQuoteItems)
                 {
                     string OrderID = SalesQuoteItem.Voucher;
                     SalesQuoteVoucher sov =
                         db.SalesQuoteVouchers.Where(s => s.SalesQuoteVoucherID.ToString() == OrderID).SingleOrDefault();

                     if (sov != null)
                     {
                         if (sov.LastObjectState == CRMConstants.Forwarded)
                         {

                             SalequoteItem.Add(SalesQuoteItem);
                         }
                     }
                 }
                 if (SalequoteItem != null)
                 {
                     foreach (var sale in SalequoteItem)
                     {
                         SalesQuoteItems.Remove(sale);

                     }
                 }



                 foreach (var SalesOrderItem in SalesOrderItems)
                 {
                     string OrderID = SalesOrderItem.Voucher;
                     SalesOrderVoucher sov =
                         db.SalesOrderVouchers.Where(s => s.SalesOrderVoucherID.ToString() == OrderID).SingleOrDefault();

                     if (sov != null)
                     {
                         if (sov.LastObjectState == CRMConstants.Forwarded)
                         {

                             saleItem.Add(SalesOrderItem);
                         }
                     }
                 }
                 if (saleItem != null)
                 {
                     foreach (var sale in saleItem)
                     {
                         SalesOrderItems.Remove(sale);

                     }
                 }

                 foreach (var CashInvoiceItem in CashInvoiceItems)
                 {
                     string InvoiceID = CashInvoiceItem.Voucher;
                     CashSalesInvoice cInvoice =
                         db.CashSalesInvoices.Where(
                             c =>
                                 c.CashSalesInvoiceID.ToString() == InvoiceID).SingleOrDefault();
                     if (cInvoice != null)
                     {
                         if (cInvoice.LastObjectState == CRMConstants.Forwarded)
                         {

                             cashinvoiceItem.Add(CashInvoiceItem);
                         }
                     }

                 }
                 if (cashinvoiceItem != null)
                 {
                     foreach (var invoice in cashinvoiceItem)
                     {

                         CashInvoiceItems.Remove(invoice);
                     }
                 }

                 decimal QuoteQuantity = 0;
                 if (SalesQuoteItems != null)
                 {
                     QuoteQuantity = SalesQuoteItems.Sum(s => s.Quantity);
                 }
                 else
                 {
                     QuoteQuantity = 0;
                 }

                 decimal OrderedQuantity = 0;
                 if (SalesOrderItems != null)
                 {
                     OrderedQuantity = SalesOrderItems.Sum(s => s.Quantity);
                 }
                 else
                 {
                     OrderedQuantity = 0;
                 }
                 decimal InvoiceChecKQuantity = 0;
                 if (SalesOrderItems != null)
                 {
                     InvoiceChecKQuantity = CashInvoiceItems.Sum(s => s.Quantity);
                 }
                 else
                 {
                     InvoiceChecKQuantity = 0;
                 }
                 if (stockLevel != null)
                 {
                     decimal stocktotal = stockLevel.Sum(s => s.Quantity) - OrderedQuantity - InvoiceChecKQuantity - QuoteQuantity;

                     decimal reserved = OrderedQuantity + InvoiceChecKQuantity + QuoteQuantity;
                     if (stocktotal-reserved > quantity)
                         {
                             return true;
                         }
              
                 }
                 return false;
             }
             */



        #endregion
    }
}
