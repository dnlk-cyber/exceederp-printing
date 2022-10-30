using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.CRM;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    //   [AuthorizeRoles( CRMRoles.CashSalesUser, CRMRoles.SalesQuoteUser,CRMRoles.SalesOrderUser)] 
    [Authorize]
    public class LineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();



        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucherID, string voucherDefinitionID)
        {
            var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();
            IQueryable<LineItem> lineitems = db.LineItems.Where(li => li.Voucher == voucherID.ToString() && li.VoucherDefinitionID == voucherDefinitionID.ToString() && assignedStores.Contains(li.StoreID));
            DataSourceResult result = lineitems.ToDataSourceResult(request, LineItem => new
            {
                LineItemID = LineItem.LineItemID,
                Voucher = LineItem.Voucher,
                ThingID = LineItem.ThingID,
                UnitAmount = LineItem.UnitAmount,
                //Quantity = LineItem.CalculatedCost / LineItem.UnitAmount,
                Quantity = LineItem.Quantity,
                TotalAmount = LineItem.TotalAmount,
                Tax = LineItem.Tax,
                TaxAmount = LineItem.TaxAmount,
                CalculatedCost = LineItem.CalculatedCost,
                Discount = LineItem.Discount,
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
                ShipmentNumber = LineItem.ShipmentNumber,
                StoreRequisitionNumber = LineItem.StoreRequisitionNumber,

                //PriceTagDetail = LineItem.PriceTagDetail,
                Remark = LineItem.Remark,
                WithholdingType = LineItem.WithholdingType,
                WithholdingAmount = LineItem.WithholdingAmount,
                WithholdTax = LineItem.WithholdTax,
                WithholdTaxAmount = LineItem.WithholdTaxAmount,
                Vat = LineItem.Vat,
                UOM = LineItem.Product.UOM,

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
                Discount = LineItem.Discount,
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
                ShipmentNumber = LineItem.ShipmentNumber,
                StoreRequisitionNumber = LineItem.StoreRequisitionNumber,

                //PriceTagDetail = LineItem.PriceTagDetail,
                Remark = LineItem.Remark,
                WithholdingType = LineItem.WithholdingType,
                WithholdingAmount = LineItem.WithholdingAmount,
                WithholdTax = LineItem.WithholdTax,
                WithholdTaxAmount = LineItem.WithholdTaxAmount,
                Vat = LineItem.Vat,
            });

            return Json(result);
        }

        public ActionResult LineItems_Create([DataSourceRequest] DataSourceRequest request, LineItemViewModel model, string voucherID, string voucherDefinitionIDCreate)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            db = new ExceedDbContext();
            //LineItem Item = await db.LineItems.FirstOrDefaultAsync(li => li.Voucher == model.Voucher.ToString() && li.ThingID == model.ThingID && li.VoucherDefinitionID == model.VoucherDefinitionId);

            var TaxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == model.Tax).FirstOrDefault();
            var SaleStore = db.SalesStores.Where(x => x.SalesStoreId == model.StoreID).FirstOrDefault();
            //var ProductId = db.Products.Where(x => x.ProductID == model.ThingID).FirstOrDefault().ProductID;

            //if (!StockBalanceAvailable(SaleStore, model.ThingID, model.Quantity, model.Variety, model.Class))
            //{
            //    ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");
            //}

            //if (string.IsNullOrEmpty(variety))
            //{
            //    variety = null;
            //}
            //if (string.IsNullOrEmpty(className))
            //{
            //    className = null;
            //}
            var existingItem = db.LineItems.FirstOrDefault(l => l.Voucher == voucherID &&
                                          l.VoucherDefinitionID == voucherDefinitionIDCreate &&
                                          l.ThingID == model.ThingID &&
                                          l.Variety == model.Variety &&
                                          l.ShipmentNumber == model.ShipmentNumber &&
                                          l.StoreRequisitionNumber == model.StoreRequisitionNumber &&
                                          l.Class == model.Class);
            if (existingItem != null)
            {
                ModelState.AddModelError("", "Duplicate line item not allowed");
            }
            if (ModelState.IsValid)
            {
                var SalesStoreId = SaleStore.SalesStoreId;
                var productName = model.ThingID;

                //var priceValue = db.Prices.Where(p => p.ReferenceType == model.PriceTag  && p.Thing == ProductId.ToString() && p.Variety == model.Variety).ToList();
                //var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == model.Variety).FirstOrDefault();

                decimal unitPrice = model.UnitAmount;

                //decimal Price = 0;
                //if (priceValue != null)
                //{

                //    var classPrice = priceValue.Where(x => x.Class == model.Class).FirstOrDefault();
                //    if (classPrice != null)
                //    {
                //        Price = classPrice.Value;
                //    }
                //    else
                //    {
                //        var priceObj = priceValue.FirstOrDefault();
                //        if(priceObj != null)
                //        {
                //            Price = priceObj.Value;
                //        }

                //    }
                //}
                //else if (DefaultpriceValue != null)
                //{

                //    Price = DefaultpriceValue.Value;
                //}

                string TaxId = model.Tax;
                var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                decimal taxPercent = GLTaxRate.Rate / 100;

                decimal TaxAmount = (model.Quantity * unitPrice * taxPercent);
                decimal TotalAmount = (model.Quantity * unitPrice) + TaxAmount - model.Discount;
                decimal CalculatedAmount = model.Quantity * unitPrice;


                decimal WithHoldingTaxAmount = decimal.Zero;
                decimal WithHoldingAmount = decimal.Zero;

                var Rates = db.GLTaxRates.ToList();

                if (model.WithholdingType != null)
                {

                    CRMSalesHelper.ConfigureWitholding(model.WithholdTax, model.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, CalculatedAmount);
                }
                var entity = new LineItem
                {
                    Voucher = voucherID,
                    VoucherDefinitionID = voucherDefinitionIDCreate,
                    ThingID = model.ThingID,
                    StoreID = SalesStoreId,
                    UnitAmount = unitPrice,
                    WithholdingType = model.WithholdingType,
                    WithholdingAmount = WithHoldingAmount,
                    WithholdTax = model.WithholdTax,
                    WithholdTaxAmount = WithHoldingTaxAmount,
                    Vat = TaxRate.Rate,
                    Tax = GLTaxRate.GLTaxRateID.ToString(),
                    TaxAmount = TaxAmount,
                    Quantity = model.Quantity,
                    TotalAmount = TotalAmount,
                    Discount = model.Discount,

                    CalculatedCost = CalculatedAmount,
                    Status = CRMConstants.Pending,
                    Variety = model.Variety,
                    Class = model.Class,
                    PriceTag = model.PriceTag,
                    //PriceTagDetail = model.PriceTagDetail,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };


                try
                {
                    db.LineItems.Add(entity);
                    int save = db.SaveChanges();
                    model.Voucher = entity.Voucher;
                    //if (save == 1)
                    //{

                    //    if (model.AdditionalCostItem.Any())
                    //    {
                    //        foreach (var AdditionalCost in model.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                    //        {

                    //            var Cost = new AdditionalCostLog
                    //            {
                    //                AdditionalCostId = int.Parse(AdditionalCost.Id),
                    //                LineItemId = entity.LineItemID,
                    //                CostAmount = int.Parse(AdditionalCost.Name)

                    //            };
                    //            db.AdditionalCostLogs.Add(Cost);
                    //            db.SaveChanges();

                    //        }

                    //    }
                    //}
                }
                catch (Exception e)
                {

                }
                #region Operation log
                if (voucherDefinitionIDCreate == "0")
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Sales Quote Line Create", "Success", User.Identity.Name, $"Sales Quote line for quote no. {entity.Voucher} created. Line item id: {entity.LineItemID}", Modules.CRM);
                }
                else if (voucherDefinitionIDCreate == "1")
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Sales Order Line Create", "Success", User.Identity.Name, $"Sales Order line for order no. {entity.Voucher} created. Line item id: {entity.LineItemID}", Modules.CRM);
                }
                else if (voucherDefinitionIDCreate == "2")
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Invoice Line Create", "Success", User.Identity.Name, $"Invoice line for invoice no. {entity.Voucher} created. Line item id: {entity.LineItemID}", Modules.CRM);
                }
                #endregion
            }


            //if (model.VoucherDefinitionId == "0")
            //{
            //    return RedirectToAction("Index", "SalesQuoteVoucher");
            //}
            //if (model.VoucherDefinitionId == "1")
            //{
            //    return RedirectToAction("Index", "SalesOrderVoucher");
            //}
            if (voucherDefinitionIDCreate == "2")
            {
                var invoiceHeader = db.CashSalesInvoices.FirstOrDefault(inv => inv.CashSalesInvoiceID.ToString() == model.Voucher);
                if (invoiceHeader != null)
                {
                    CRMSalesHelper.UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);

                    new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoiceHeader);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
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

                //if (!StockBalanceAvailable(SaleStore, LineItem.ThingID, LineItem.Quantity, LineItem.Variety, LineItem.Class))
                //{
                //    ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");

                //}
                var existingItem = db.LineItems.FirstOrDefault(l => l.Voucher == LineItem.Voucher &&
                                          l.VoucherDefinitionID == LineItem.VoucherDefinitionId &&
                                          l.ThingID == LineItem.ThingID &&
                                          l.Variety == LineItem.Variety &&
                                          l.Class == LineItem.Class &&
                                          l.ShipmentNumber == LineItem.ShipmentNumber &&
                                          l.StoreRequisitionNumber == LineItem.StoreRequisitionNumber &&
                                          l.LineItemID != LineItem.LineItemID);
                if (existingItem != null)
                {
                    ModelState.AddModelError("", "Duplicate line item not allowed");
                }
                if (ModelState.IsValid)
                {




                    string sID = LineItem.StoreID.ToString();
                    var SalesStoreId = SaleStore.SalesStoreId;
                    var productName = LineItem.ThingID;
                    var ProductId = db.Products.Where(x => x.ProductID == LineItem.ThingID).FirstOrDefault().ProductID;

                    //var priceValue = db.Prices.Where(p => p.ReferenceType == LineItem.PriceTag && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).ToList();
                    //var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).FirstOrDefault();

                    decimal unitPrice = LineItem.UnitAmount;
                    //if (priceValue != null)
                    //{

                    //    var classPrice = priceValue.Where(x => x.Class == LineItem.Class).FirstOrDefault();
                    //    if (classPrice != null)
                    //    {
                    //        Price = classPrice.Value;
                    //    }
                    //    else
                    //    {
                    //        Price = priceValue.FirstOrDefault().Value;

                    //    }
                    //}
                    //else if (DefaultpriceValue != null)
                    //{

                    //    Price = DefaultpriceValue.Value;
                    //}

                    string TaxId = LineItem.Tax;
                    var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                    decimal taxPercent = GLTaxRate.Rate / 100;

                    decimal TaxAmount = (LineItem.Quantity * unitPrice * taxPercent);
                    decimal TotalAmount = (LineItem.Quantity * unitPrice) + TaxAmount - LineItem.Discount;
                    decimal CalculatedAmount = LineItem.Quantity * unitPrice;


                    decimal WithHoldingTaxAmount = decimal.Zero;
                    decimal WithHoldingAmount = decimal.Zero;

                    var Rates = db.GLTaxRates.ToList();

                    if (LineItem.WithholdingType != null)
                    {
                        CRMSalesHelper.ConfigureWitholding(LineItem.WithholdTax, LineItem.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, CalculatedAmount);
                    }
                    Item.ThingID = ProductId;
                    Item.StoreID = SalesStoreId;
                    Item.UnitAmount = unitPrice;
                    Item.Quantity = LineItem.Quantity;
                    Item.TotalAmount = TotalAmount;
                    Item.WithholdingType = LineItem.WithholdingType;
                    Item.WithholdingAmount = WithHoldingAmount;
                    Item.WithholdTax = LineItem.WithholdTax;
                    Item.WithholdTaxAmount = WithHoldingTaxAmount;
                    Item.Vat = GLTaxRate.Rate;
                    Item.Discount = LineItem.Discount;

                    Item.Tax = GLTaxRate.GLTaxRateID.ToString();
                    Item.TaxAmount = TaxAmount;
                    Item.CalculatedCost = CalculatedAmount;
                    Item.Status = CRMConstants.Pending;
                    Item.PriceTag = LineItem.PriceTag;
                    //Item.PriceTagDetail = LineItem.PriceTagDetail;
                    Item.Variety = LineItem.Variety;
                    Item.Class = LineItem.Class;



                    try
                    {
                        db.LineItems.Attach(Item);
                        db.Entry(Item).State = EntityState.Modified;
                        db.SaveChanges();

                        int save = db.SaveChanges();
                        //if (save == 1)
                        //{
                        //    if (LineItem.AdditionalCostItem != null)
                        //    {
                        //        if (LineItem.AdditionalCostItem.Any())
                        //        {
                        //            db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList().ForEach(a => { db.AdditionalCostLogs.Attach(a); db.AdditionalCostLogs.Remove(a); db.SaveChanges(); });
                        //            foreach (var AdditionalCost in LineItem.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                        //            {

                        //                var Cost = new AdditionalCostLog
                        //                {
                        //                    AdditionalCostId = int.Parse(AdditionalCost.Id),
                        //                    LineItemId = Item.LineItemID,
                        //                    CostAmount = int.Parse(AdditionalCost.Name)

                        //                };
                        //                db.AdditionalCostLogs.Add(Cost);
                        //                db.SaveChanges();

                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {


                    }

                    #region Operation log
                    if (Item.VoucherDefinitionID == "0")
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Sales Quote Line Edit", "Success", User.Identity.Name, $"Sales Quote line for quote no. {Item.Voucher} updated. Line item id: {Item.LineItemID}", Modules.CRM);
                    }
                    else if (Item.VoucherDefinitionID == "1")
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Sales Order Line Edit", "Success", User.Identity.Name, $"Sales Order line for order no. {Item.Voucher} updated. Line item id: {Item.LineItemID}", Modules.CRM);
                    }
                    else if (Item.VoucherDefinitionID == "2")
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Invoice Line Edit", "Success", User.Identity.Name, $"Invoice line for invoice no. {Item.Voucher} updated. Line item id: {Item.LineItemID}", Modules.CRM);
                    }
                    #endregion
                }
            }


            //if (LineItem.VoucherDefinitionId == "0")
            //{
            //    return RedirectToAction("Index", "SalesQuoteVoucher");
            //}
            //if (LineItem.VoucherDefinitionId == "1")
            //{
            //    return RedirectToAction("Index", "SalesOrderVoucher");
            //}
            if (LineItem.VoucherDefinitionId == "2")
            {
                var invoiceHeader = db.CashSalesInvoices.FirstOrDefault(inv => inv.CashSalesInvoiceID.ToString() == LineItem.Voucher);
                if (invoiceHeader != null)
                {
                    CRMSalesHelper.UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);

                    new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoiceHeader);
                }
                //return RedirectToAction("Index", "CashSalesInvoice");

            }
            return Json(new[] { LineItem }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult CreditLineItems_Create([DataSourceRequest] DataSourceRequest request, LineItemViewModel model, string voucherID, string voucherDefinitionIDCreate)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            db = new ExceedDbContext();
            LineItem Item = db.LineItems.Where(li => li.Voucher == model.Voucher.ToString() && li.ThingID == model.ThingID && li.VoucherDefinitionID == model.VoucherDefinitionId).FirstOrDefault();

            var TaxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == model.Tax).FirstOrDefault();
            var SaleStore = db.SalesStores.Where(x => x.SalesStoreId == model.StoreID).FirstOrDefault();
            //var ProductId = db.Products.Where(x => x.ProductID == model.ThingID).FirstOrDefault().ProductID;
            ////if (!StockBalanceAvailable(SaleStore, model.ThingID, model.Quantity, model.Variety, model.Class))
            ////{
            ////    ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");

            ////}
            var existingItem = db.LineItems.FirstOrDefault(l => l.Voucher == voucherID &&
                                          l.VoucherDefinitionID == voucherDefinitionIDCreate &&
                                          l.ThingID == model.ThingID &&
                                          l.Variety == model.Variety &&
                                          l.Class == model.Class);
            if (existingItem != null)
            {
                ModelState.AddModelError("", "Duplicate line item not allowed");
            }
            if (ModelState.IsValid)
            {
                var SalesStoreId = SaleStore.SalesStoreId;
                var productName = model.ThingID;

                //var priceValue = db.Prices.Where(p => p.ReferenceType == model.PriceTag  && p.Thing == ProductId.ToString() && p.Variety == model.Variety).ToList();
                //var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == model.Variety).FirstOrDefault();

                decimal unitPrice = model.UnitAmount;
                //if (priceValue != null)
                //{

                //    var classPrice = priceValue.Where(x => x.Class == model.Class).FirstOrDefault();
                //    if (classPrice != null)
                //    {
                //        Price = classPrice.Value;
                //    }
                //    else
                //    {
                //        Price = priceValue.FirstOrDefault().Value;

                //    }
                //}
                //else if (DefaultpriceValue != null)
                //{

                //    Price = DefaultpriceValue.Value;
                //}

                string TaxId = model.Tax;
                var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                decimal taxPercent = GLTaxRate.Rate / 100;


                decimal TaxAmount = (model.Quantity * unitPrice * taxPercent);
                decimal TotalAmount = (model.Quantity * unitPrice) + TaxAmount - model.Discount;
                decimal CalculatedAmount = model.Quantity * unitPrice;



                decimal WithHoldingTaxAmount = decimal.Zero;
                decimal WithHoldingAmount = decimal.Zero;

                var Rates = db.GLTaxRates.ToList();


                if (model.WithholdingType != null)
                {
                    CRMSalesHelper.ConfigureWitholding(model.WithholdTax, model.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, CalculatedAmount);
                }


                var entity = new LineItem
                {
                    Voucher = voucherID,
                    VoucherDefinitionID = voucherDefinitionIDCreate,
                    ThingID = model.ThingID,
                    StoreID = SalesStoreId,
                    UnitAmount = unitPrice,
                    WithholdingType = model.WithholdingType,
                    WithholdingAmount = model.WithholdingAmount,
                    WithholdTax = model.WithholdTax,
                    WithholdTaxAmount = model.WithholdTaxAmount,
                    Vat = TaxRate.Rate,
                    Tax = GLTaxRate.GLTaxRateID.ToString(),
                    TaxAmount = TaxAmount,
                    Quantity = model.Quantity,
                    TotalAmount = TotalAmount,
                    Discount = model.Discount,

                    CalculatedCost = CalculatedAmount,
                    Status = CRMConstants.Pending,
                    Variety = model.Variety,
                    Class = model.Class,
                    PriceTag = model.PriceTag,
                    //PriceTagDetail = model.PriceTagDetail,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };


                try
                {
                    db.LineItems.Add(entity);
                    int save = db.SaveChanges();
                    model.Voucher = entity.Voucher;
                    //if (save == 1)
                    //{

                    //    if (model.AdditionalCostItem.Any())
                    //    {
                    //        foreach (var AdditionalCost in model.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                    //        {

                    //            var Cost = new AdditionalCostLog
                    //            {
                    //                AdditionalCostId = int.Parse(AdditionalCost.Id),
                    //                LineItemId = entity.LineItemID,
                    //                CostAmount = int.Parse(AdditionalCost.Name)

                    //            };
                    //            db.AdditionalCostLogs.Add(Cost);
                    //            db.SaveChanges();

                    //        }

                    //    }
                    //}
                }
                catch (Exception e)
                {

                }
                #region Operation log
                
                if (entity.VoucherDefinitionID == "2")
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Credit Sales Line Create", "Success", User.Identity.Name, $"Invoice line for invoice no. {entity.Voucher} created. Line item id: {entity.LineItemID}", Modules.CRM);
                }
                #endregion
            }


            //if (model.VoucherDefinitionId == "0")
            //{
            //    return RedirectToAction("Index", "SalesQuoteVoucher");
            //}
            //else if (model.VoucherDefinitionId == "1")
            //{
            //    return RedirectToAction("Index", "SalesOrderVoucher");
            //}
            if (voucherDefinitionIDCreate == "2")
            {
                var invoiceHeader = db.CashSalesInvoices.FirstOrDefault(inv => inv.CashSalesInvoiceID.ToString() == model.Voucher);
                if (invoiceHeader != null)
                {
                    CRMSalesHelper.UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);

                    new CRMPaymentDistibutionHelper().DistributeCreditInvoice(invoiceHeader);
                }

                //return RedirectToAction("Index", "CreditsSalesInvoice");
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
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

                //if (!StockBalanceAvailable(SaleStore, LineItem.ThingID, LineItem.Quantity, LineItem.Variety, LineItem.Class))
                //{
                //    ModelState.AddModelError("Quantityexceed", " Quantity Exceed Stock Balance ! ");
                //}
                var existingItem = db.LineItems.FirstOrDefault(l => l.Voucher == LineItem.Voucher &&
                                          l.VoucherDefinitionID == LineItem.VoucherDefinitionId &&
                                          l.ThingID == LineItem.ThingID &&
                                          l.Variety == LineItem.Variety &&
                                          l.Class == LineItem.Class &&
                                          l.ShipmentNumber == LineItem.ShipmentNumber &&
                                          l.StoreRequisitionNumber == LineItem.StoreRequisitionNumber &&
                                          l.LineItemID != LineItem.LineItemID);
                if (existingItem != null)
                {
                    ModelState.AddModelError("", "Duplicate line item not allowed");
                }
                if (ModelState.IsValid)
                {
                    string sID = LineItem.StoreID.ToString();
                    var SalesStoreId = SaleStore.SalesStoreId;
                    var productName = LineItem.ThingID;
                    var ProductId = db.Products.Where(x => x.ProductID == LineItem.ThingID).FirstOrDefault().ProductID;
                    var priceValue = db.Prices.Where(p => p.ReferenceType == LineItem.PriceTag && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).ToList();
                    var DefaultpriceValue = db.Prices.Where(p => p.IsDefault == true && p.Thing == ProductId.ToString() && p.Variety == LineItem.Variety).FirstOrDefault();

                    decimal unitPrice = LineItem.UnitAmount;
                    //if (priceValue != null)
                    //{

                    //    var classPrice = priceValue.Where(x => x.Class == LineItem.Class).FirstOrDefault();
                    //    if (classPrice != null)
                    //    {
                    //        Price = classPrice.Value;
                    //    }
                    //    else
                    //    {
                    //        Price = priceValue.FirstOrDefault().Value;

                    //    }
                    //}
                    //else if (DefaultpriceValue != null)
                    //{

                    //    Price = DefaultpriceValue.Value;
                    //}

                    string TaxId = LineItem.Tax;
                    var GLTaxRate = db.GLTaxRates.Where(t => t.GLTaxRateID.ToString() == TaxId).FirstOrDefault();

                    decimal taxPercent = GLTaxRate.Rate / 100;

                    decimal TaxAmount = (LineItem.Quantity * unitPrice * taxPercent);
                    decimal TotalAmount = (LineItem.Quantity * unitPrice) + TaxAmount - LineItem.Discount;
                    decimal CalculatedAmount = LineItem.Quantity * unitPrice;


                    decimal WithHoldingTaxAmount = decimal.Zero;
                    decimal WithHoldingAmount = decimal.Zero;

                    var Rates = db.GLTaxRates.ToList();

                    if (LineItem.WithholdingType != null)
                    {
                        CRMSalesHelper.ConfigureWitholding(LineItem.WithholdTax, LineItem.WithholdingType, Rates, out WithHoldingAmount, out WithHoldingTaxAmount, TaxAmount, CalculatedAmount);
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
                    Item.UnitAmount = unitPrice;
                    Item.Quantity = LineItem.Quantity;
                    Item.TotalAmount = TotalAmount;
                    Item.Tax = GLTaxRate.GLTaxRateID.ToString();
                    Item.TaxAmount = TaxAmount;
                    Item.Discount = LineItem.Discount;
                    Item.CalculatedCost = CalculatedAmount;
                    Item.Status = CRMConstants.Pending;
                    Item.PriceTag = LineItem.PriceTag;
                    //Item.PriceTagDetail = LineItem.PriceTagDetail;
                    Item.Variety = LineItem.Variety;
                    Item.Class = LineItem.Class;
                    try
                    {
                        db.LineItems.Attach(Item);
                        db.Entry(Item).State = EntityState.Modified;
                        db.SaveChanges();

                        int save = db.SaveChanges();
                        //if (save == 1)
                        //{
                        //    if (LineItem.AdditionalCostItem != null)
                        //    {
                        //        if (LineItem.AdditionalCostItem.Any())
                        //        {
                        //            db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList().ForEach(a => { db.AdditionalCostLogs.Attach(a); db.AdditionalCostLogs.Remove(a); db.SaveChanges(); });
                        //            foreach (var AdditionalCost in LineItem.AdditionalCostItem.Where(x => x.Name != "0").ToList())
                        //            {

                        //                var Cost = new AdditionalCostLog
                        //                {
                        //                    AdditionalCostId = int.Parse(AdditionalCost.Id),
                        //                    LineItemId = Item.LineItemID,
                        //                    CostAmount = int.Parse(AdditionalCost.Name)

                        //                };
                        //                db.AdditionalCostLogs.Add(Cost);
                        //                db.SaveChanges();

                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {


                    }
                    #region Operation log
                    if (Item.VoucherDefinitionID == "2")
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Credit Sales Line Edit", "Success", User.Identity.Name, $"Invoice line for invoice no. {Item.Voucher} updated. Line item id: {Item.LineItemID}", Modules.CRM);
                    }
                    #endregion
                }
            }


            //if (LineItem.VoucherDefinitionId == "0")
            //{
            //    return RedirectToAction("Index", "SalesQuoteVoucher");
            //}
            //if (LineItem.VoucherDefinitionId == "1")
            //{
            //    return RedirectToAction("Index", "SalesOrderVoucher");
            //}
            if (LineItem.VoucherDefinitionId == "2")
            {
                var invoiceHeader = db.CashSalesInvoices.FirstOrDefault(inv => inv.CashSalesInvoiceID.ToString() == LineItem.Voucher);
                if (invoiceHeader != null)
                {
                    CRMSalesHelper.UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);

                    new CRMPaymentDistibutionHelper().DistributeCreditInvoice(invoiceHeader);
                }
                //return RedirectToAction("Index", "CreditsSalesInvoice");
            }
            return Json(new[] { LineItem }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region  Support Func
        public bool StockBalanceAvailable(SalesStore Store, int ProductId, decimal quantity, string VarietyID, string Class)
        {
            //string SaleQuoteDefination = "0";
            //string SalesOrderVoucherDefination = "1";
            string CashInvoiceDefination = "2";

            //List<LineItem> saleItem = new List<LineItem>();
            List<LineItem> invoiceLineItemsToProcess = new List<LineItem>();
            //List<LineItem> SalequoteItem = new List<LineItem>();
            var Product = db.Products.Where(x => x.ProductID == ProductId).FirstOrDefault();
            if (Product != null)
            {
                List<StoreItemAssignment> stockLevel = Store.InventoryStore.StoreItemAssignments.Where(x => x.ItemCode == Product.ItemCode && x.StoreCode == Store.InventoryStore.StoreCode).ToList();

                //List<LineItem> SalesOrderItems = db.LineItems.Where(l => (l.ThingID == ProductId && l.VoucherDefinitionID == SalesOrderVoucherDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();
                //List<LineItem> SalesQuoteItems = db.LineItems.Where(l => (l.ThingID == ProductId && l.VoucherDefinitionID == SaleQuoteDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();

                List<LineItem> invoiceLineItems = db.LineItems.Where(l => (l.ThingID == ProductId && l.VoucherDefinitionID == CashInvoiceDefination && l.StoreID == Store.SalesStoreId && l.Status != CRMConstants.ProductIssued)).ToList();

                if (!string.IsNullOrEmpty(VarietyID))
                {
                    //SalesQuoteItems = SalesQuoteItems.Where(x => x.Variety == VarietyID).ToList();
                    //SalesOrderItems = SalesOrderItems.Where(x => x.Variety == VarietyID).ToList();
                    invoiceLineItems = invoiceLineItems.Where(x => x.Variety == VarietyID).ToList();
                }


                if (!string.IsNullOrEmpty(Class))
                {
                    //SalesQuoteItems = SalesQuoteItems.Where(x => x.Class == Class).ToList();
                    //SalesOrderItems = SalesOrderItems.Where(x => x.Class == Class).ToList();
                    invoiceLineItems = invoiceLineItems.Where(x => x.Class == Class).ToList();
                }
                //foreach (var SalesQuoteItem in SalesQuoteItems)
                //{
                //    string OrderID = SalesQuoteItem.Voucher;
                //    SalesQuoteVoucher sov =
                //        db.SalesQuoteVouchers.Where(s => s.SalesQuoteVoucherID.ToString() == OrderID).SingleOrDefault();

                //    if (sov != null)
                //    {
                //        if (sov.LastObjectState == CRMConstants.Forwarded)
                //        {

                //            SalequoteItem.Add(SalesQuoteItem);
                //        }
                //    }
                //}
                //if (SalequoteItem != null)
                //{
                //    foreach (var sale in SalequoteItem)
                //    {
                //        SalesQuoteItems.Remove(sale);

                //    }
                //}



                //foreach (var SalesOrderItem in SalesOrderItems)
                //{
                //    string OrderID = SalesOrderItem.Voucher;
                //    SalesOrderVoucher sov =
                //        db.SalesOrderVouchers.Where(s => s.SalesOrderVoucherID.ToString() == OrderID).SingleOrDefault();

                //    if (sov != null)
                //    {
                //        if (sov.LastObjectState == CRMConstants.Forwarded)
                //        {

                //            saleItem.Add(SalesOrderItem);
                //        }
                //    }
                //}
                //if (saleItem != null)
                //{
                //    foreach (var sale in saleItem)
                //    {
                //        SalesOrderItems.Remove(sale);

                //    }
                //}

                foreach (var item in invoiceLineItems)
                {
                    string InvoiceID = item.Voucher;
                    CashSalesInvoice cInvoice =
                        db.CashSalesInvoices.Where(
                            c =>
                                c.CashSalesInvoiceID.ToString() == InvoiceID).SingleOrDefault();
                    if (cInvoice != null)
                    {
                        if (cInvoice.LastObjectState == CRMConstants.Forwarded)
                        {

                            invoiceLineItemsToProcess.Add(item);
                        }
                    }

                }
                if (invoiceLineItemsToProcess.Any())
                {
                    foreach (var invoice in invoiceLineItemsToProcess)
                    {

                        invoiceLineItems.Remove(invoice);
                    }
                }

                //decimal QuoteQuantity = 0;
                //if (SalesQuoteItems != null)
                //{
                //    QuoteQuantity = SalesQuoteItems.Sum(s => s.Quantity);
                //}
                //else
                //{
                //    QuoteQuantity = 0;
                //}

                //decimal OrderedQuantity = 0;
                //if (SalesOrderItems != null)
                //{
                //    OrderedQuantity = SalesOrderItems.Sum(s => s.Quantity);
                //}
                //else
                //{
                //    OrderedQuantity = 0;
                //}
                decimal InvoiceChecKQuantity = 0;
                if (invoiceLineItems.Any())
                {
                    InvoiceChecKQuantity = invoiceLineItems.Sum(s => s.Quantity);
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

                    //decimal stocktotal = stockBalance.Sum(s => s.Balance) - OrderedQuantity - InvoiceChecKQuantity - QuoteQuantity;
                    decimal stocktotal = stockBalance.Sum(s => s.Balance) - InvoiceChecKQuantity;

                    //decimal reserved = OrderedQuantity + InvoiceChecKQuantity + QuoteQuantity;
                    decimal reserved = InvoiceChecKQuantity;

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

            var invoiceHeader = db.CashSalesInvoices.FirstOrDefault(inv => inv.CashSalesInvoiceID.ToString() == LineItem.Voucher);
            if (invoiceHeader != null && invoiceHeader.PaymentMethod == CRMConstants.CreditPaymentMethod)
            {
                new CRMSalesHelper().UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);

                new CRMPaymentDistibutionHelper().DistributeCreditInvoice(invoiceHeader);
            }
            else if (invoiceHeader != null && invoiceHeader.PaymentMethod == CRMConstants.CashPaymentMethod)
            {
                new CRMSalesHelper().UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);

                new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoiceHeader);
            }

            if(Item.VoucherDefinitionID == "0")
            {
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete Sales Quote Line", "Success", User.Identity.Name, $"Sales quote line of quote no. {Item.Voucher} deleted." + LineItem.LineItemID);
            }
            else if (Item.VoucherDefinitionID == "1")
            {
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete Sales Order Line", "Success", User.Identity.Name, $"Sales order line of order no. {Item.Voucher} deleted." + LineItem.LineItemID);
            }
            else if (Item.VoucherDefinitionID == "2")
            {
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete Invoice Line", "Success", User.Identity.Name, $"Invoice line of invoice no. {Item.Voucher} deleted." + LineItem.LineItemID);
            }

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
