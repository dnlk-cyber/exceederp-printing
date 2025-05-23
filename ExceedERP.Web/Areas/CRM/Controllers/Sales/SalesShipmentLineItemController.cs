using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.ShipmentUser, CRMRoles.ShipmentApprover, FinanceConstants.GeneralSubLedgerTransfer)]
    public class SalesShipmentLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Shipments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesShipmentLineItem> shipments = db.SalesShipmentLineItem.Where(itm => itm.SalesShipmentId == id);
            DataSourceResult result = shipments.ToDataSourceResult(request, shipment => new
            {
                SalesShipmentId = shipment.SalesShipmentId,
                SalesStoreId = shipment.SalesStoreId,
                LineItemId = shipment.LineItemId,
                SalesShipmentLineItemId = shipment.SalesShipmentLineItemId,

                ProductId = shipment.ProductId,
                Price = shipment.Price,
                TotalQuantity = shipment.TotalQuantity,
                ShipedQuantityTilPreShipment = shipment.ShipedQuantityTilPreShipment,
                ShipedQuantity = shipment.ShipedQuantity,
                RemainingQuantity = shipment.RemainingQuantity,
                SubTotal = shipment.SubTotal,
                TaxAmount = shipment.TaxAmount,
                TotalAmount = shipment.TotalAmount,
                Remark = shipment.Remark,
                Class = shipment.Class,
                Variety = shipment.Variety,

                CreatedBy = shipment.CreatedBy,
                DateCreated = shipment.DateCreated,
                LastModified = shipment.LastModified,
                ModifiedBy = shipment.ModifiedBy,
                OrgBranchName = shipment.OrgBranchName,
            });

            return Json(result);
        }
        public ActionResult MissingShipmentLines_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<SalesShipmentLineItem> listToReturn = new List<SalesShipmentLineItem>();
            var allLines = db.SalesShipmentLineItem.Where(itm => itm.SalesShipmentId == id).ToList();
            var binCardLines = db.BinCardLines.Where(b => b.ReferenceDoc == "Shipment " + id).ToList();
            if (allLines.Count != binCardLines.Count && binCardLines.Count > 0)
            {
                foreach (var shipmentLine in allLines)
                {
                    var salesStore = db.SalesStores.Find(shipmentLine.SalesStoreId);

                    InventoryStore Store =
                        db.InventoryStores.FirstOrDefault(
                        x => x.StoreCode == salesStore.StoreCode);
                    Product product =
                        db.Products.FirstOrDefault(x => x.ProductID == shipmentLine.ProductId);

                    StoreItemAssignment storeItem =
                        db.StoreItemAssignments
                        .Where(x => x.InventoryStore.StoreCode == Store.StoreCode &&
                                    x.InventoryItem.ItemCode == product.ItemCode)
                                    .OrderByDescending(x => x.StoreItemAssignmentID).FirstOrDefault();
                    if (storeItem != null)
                    {
                        var respectiveBinOfCurrentItem =
                            binCardLines.FirstOrDefault(b => b.StoreItemAssignmentID == storeItem.StoreItemAssignmentID && b.StoreCode == Store.StoreCode);
                        if (respectiveBinOfCurrentItem == null)
                        {
                            listToReturn.Add(shipmentLine);
                        }
                    }

                }
            }
            //IQueryable<SalesShipmentLineItem> shipments = ;
            DataSourceResult result = listToReturn.AsQueryable().ToDataSourceResult(request, shipment => new
            {
                SalesShipmentId = shipment.SalesShipmentId,
                SalesStoreId = shipment.SalesStoreId,
                LineItemId = shipment.LineItemId,
                SalesShipmentLineItemId = shipment.SalesShipmentLineItemId,

                ProductId = shipment.ProductId,
                Price = shipment.Price,
                TotalQuantity = shipment.TotalQuantity,
                ShipedQuantityTilPreShipment = shipment.ShipedQuantityTilPreShipment,
                ShipedQuantity = shipment.ShipedQuantity,
                RemainingQuantity = shipment.RemainingQuantity,
                SubTotal = shipment.SubTotal,
                TaxAmount = shipment.TaxAmount,
                TotalAmount = shipment.TotalAmount,
                Remark = shipment.Remark,
                Class = shipment.Class,
                Variety = shipment.Variety,

                CreatedBy = shipment.CreatedBy,
                DateCreated = shipment.DateCreated,
                LastModified = shipment.LastModified,
                ModifiedBy = shipment.ModifiedBy,
                OrgBranchName = shipment.OrgBranchName,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Shipment_Create([DataSourceRequest]DataSourceRequest request, SalesShipmentLineItem shipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentLineItem
                {
                    SalesShipmentId = shipment.SalesShipmentId,
                    SalesStoreId = shipment.SalesStoreId,
                    LineItemId = shipment.LineItemId,
                    SalesShipmentLineItemId = shipment.SalesShipmentLineItemId,

                    ProductId = shipment.ProductId,
                    Price = shipment.Price,
                    TotalQuantity = shipment.TotalQuantity,
                    ShipedQuantity = shipment.ShipedQuantity,
                    ShipedQuantityTilPreShipment = shipment.ShipedQuantityTilPreShipment,
                    RemainingQuantity = shipment.RemainingQuantity,
                    SubTotal = shipment.SubTotal,
                    TaxAmount = shipment.TaxAmount,
                    TotalAmount = shipment.TotalAmount,
                    Remark = shipment.Remark,
                    Class = shipment.Class,
                    Variety = shipment.Variety,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                db.SalesShipmentLineItem.Add(entity);
                db.SaveChanges();
                shipment.SalesShipmentId = entity.SalesShipmentId;
                UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Added new  Shipment Line Item   " + shipment.SalesShipmentLineItemId, Modules.CRM);
            }

            return Json(new[] { shipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Shipment_Update([DataSourceRequest]DataSourceRequest request, SalesShipmentLineItem shipment)
        {
            if (ModelState.IsValid)
            {
                if (shipment.TotalQuantity - (shipment.ShipedQuantityTilPreShipment + shipment.ShipedQuantity) < 0)
                {
                    ModelState.AddModelError("", "Shipment Quantity is over limit!");
                }
                else
                {
                    var entity = new SalesShipmentLineItem
                    {
                        SalesShipmentId = shipment.SalesShipmentId,
                        SalesStoreId = shipment.SalesStoreId,
                        LineItemId = shipment.LineItemId,
                        SalesShipmentLineItemId = shipment.SalesShipmentLineItemId,

                        ProductId = shipment.ProductId,
                        Price = shipment.Price,
                        TotalQuantity = shipment.TotalQuantity,
                        ShipedQuantityTilPreShipment = shipment.ShipedQuantityTilPreShipment,
                        ShipedQuantity = shipment.ShipedQuantity,
                        RemainingQuantity = shipment.RemainingQuantity,
                        SubTotal = shipment.SubTotal,
                        TaxAmount = shipment.TaxAmount,
                        TotalAmount = shipment.TotalAmount,
                        Remark = shipment.Remark,
                        Class = shipment.Class,
                        Variety = shipment.Variety,

                        CreatedBy = shipment.CreatedBy,
                        DateCreated = shipment.DateCreated,
                        OrgBranchName = shipment.OrgBranchName,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.RemainingQuantity = entity.TotalQuantity - (entity.ShipedQuantityTilPreShipment + entity.ShipedQuantity);

                    db.SalesShipmentLineItem.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    var header = GetSalesShipment(entity.SalesShipmentId);
                    if (header != null)
                    {
                        new CRMSalesHelper().UpdateInvoiceOrSalesOrderShipmentStatusOnShipmentEdit(header);

                        new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(header);
                    }
                    UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated Shipment Line item  " + shipment.SalesShipmentLineItemId, Modules.CRM);
                }
            }

            return Json(new[] { shipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Shipment_Destroy([DataSourceRequest]DataSourceRequest request, SalesShipmentLineItem shipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentLineItem
                {
                    SalesShipmentId = shipment.SalesShipmentId,
                    SalesStoreId = shipment.SalesStoreId,
                    LineItemId = shipment.LineItemId,
                    SalesShipmentLineItemId = shipment.SalesShipmentLineItemId,

                    ProductId = shipment.ProductId,
                    Price = shipment.Price,
                    TotalQuantity = shipment.TotalQuantity,
                    ShipedQuantityTilPreShipment = shipment.ShipedQuantityTilPreShipment,
                    ShipedQuantity = shipment.ShipedQuantity,
                    RemainingQuantity = shipment.RemainingQuantity,
                    SubTotal = shipment.SubTotal,
                    TaxAmount = shipment.TaxAmount,
                    TotalAmount = shipment.TotalAmount,
                    Remark = shipment.Remark,
                    Class = shipment.Class,
                    Variety = shipment.Variety,

                    CreatedBy = shipment.CreatedBy,
                    DateCreated = shipment.DateCreated,
                    LastModified = shipment.LastModified,
                    ModifiedBy = shipment.ModifiedBy,
                    OrgBranchName = shipment.OrgBranchName,
                };

                db.SalesShipmentLineItem.Attach(entity);
                db.SalesShipmentLineItem.Remove(entity);
                db.SaveChanges();
                var header = GetSalesShipment(entity.SalesShipmentId);
                if (header != null)
                {
                    new CRMSalesHelper().UpdateInvoiceOrSalesOrderShipmentStatusOnShipmentEdit(header);

                    new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(header);
                }
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Shipment Line item  " + shipment.SalesShipmentLineItemId, Modules.CRM);

            }

            return Json(new[] { shipment }.ToDataSourceResult(request, ModelState));
        }

        private SalesShipment GetSalesShipment(int id)
        {
            return db.SalesShipments.Find(id);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}