using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    
    public class PurchaseOrderItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult PurchaseOrderItems_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchaseOrderItem> purchaseorderitems = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id);
            DataSourceResult result = purchaseorderitems.ToDataSourceResult(request, purchaseOrderItem => new
            {
                PurchaseOrderItemID = purchaseOrderItem.PurchaseOrderItemID,
                CatCode = purchaseOrderItem.CatCode,
                CategoryName = purchaseOrderItem.CategoryName,
                ItmCode = purchaseOrderItem.ItmCode,
                ItemName = purchaseOrderItem.ItemName,
                Name = purchaseOrderItem.Name,
                ModelNo = purchaseOrderItem.ModelNo,
                PartNo = purchaseOrderItem.PartNo,
                Quantity = purchaseOrderItem.Quantity,
                IsApproved = purchaseOrderItem.IsApproved,
                ItemSpecification = purchaseOrderItem.ItemSpecification,
                UoM = purchaseOrderItem.UoM,
                ItemType = purchaseOrderItem.ItemType,
                UnitPrice = purchaseOrderItem.UnitPrice,
                GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                PurchaseOrderID = purchaseOrderItem.PurchaseOrderID,
                TaxAmount = purchaseOrderItem.TaxAmount,
                SubTotal = purchaseOrderItem.SubTotal,
                Total = purchaseOrderItem.Total,
                Remark = purchaseOrderItem.Remark,
                FromPreviousPurchase = purchaseOrderItem.FromPreviousPurchase,
                OrgBranchName = purchaseOrderItem.OrgBranchName,
                DateCreated = purchaseOrderItem.DateCreated,
                LastModified = purchaseOrderItem.LastModified,
                CreatedBy = purchaseOrderItem.CreatedBy,
                ModifiedBy = purchaseOrderItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderItems_Create(int id, [DataSourceRequest]DataSourceRequest request, PurchaseOrderItem purchaseOrderItem)
        {

            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Purchase Order First");
            }

            var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);

            if (ModelState.IsValid)
            {
                var entity = new PurchaseOrderItem
                {
                    PurchaseOrderID = id,
                    CatCode = purchaseOrderItem.CatCode,
                    CategoryName = purchaseOrderItem.CategoryName,
                    ItmCode = purchaseOrderItem.ItmCode,
                    ItemName = purchaseOrderItem.ItemName,
                    Name = purchaseOrderItem.Name,
                    ModelNo = purchaseOrderItem.ModelNo,
                    PartNo = purchaseOrderItem.PartNo,
                    Quantity = purchaseOrderItem.Quantity,
                    IsApproved = purchaseOrderItem.IsApproved,
                    ItemSpecification = purchaseOrderItem.ItemSpecification,
                    UoM = purchaseOrderItem.UoM,
                    ItemType = purchaseOrderItem.ItemType,
                    UnitPrice = purchaseOrderItem.UnitPrice,
                    GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                    FromPreviousPurchase = purchaseOrderItem.FromPreviousPurchase,
                };
                if (tax != null)
                {
                    entity.TaxAmount = tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.SubTotal = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.Total = (purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice) +
                                   (tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice);
                }
                entity.Remark = purchaseOrderItem.Remark;
                entity.OrgBranchName = purchaseOrderItem.OrgBranchName;
                entity.DateCreated = purchaseOrderItem.DateCreated;
                entity.LastModified = DateTime.Today;
                entity.CreatedBy = purchaseOrderItem.CreatedBy;
                entity.ModifiedBy = purchaseOrderItem.ModifiedBy;

                db.PurchaseOrderItems.Add(entity);
                db.SaveChanges();
                purchaseOrderItem.PurchaseOrderItemID = entity.PurchaseOrderItemID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order item", "Success", User.Identity.Name, $"created item {entity.ItemName} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseOrderItem }.ToDataSourceResult(request, ModelState));
        }
        

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderItems_Update([DataSourceRequest]DataSourceRequest request, PurchaseOrderItem purchaseOrderItem)
        {

            var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);
            if (ModelState.IsValid)
            {
                PurchaseOrderItem entity =
                    new PurchaseOrderItem
                    {
                        PurchaseOrderItemID = purchaseOrderItem.PurchaseOrderItemID,
                        PurchaseOrderID = purchaseOrderItem.PurchaseOrderID,
                        CatCode = purchaseOrderItem.CatCode,
                        CategoryName = purchaseOrderItem.CategoryName,
                        ItmCode = purchaseOrderItem.ItmCode,
                        ItemName = purchaseOrderItem.ItemName,
                        Name = purchaseOrderItem.Name,
                        ModelNo = purchaseOrderItem.ModelNo,
                        PartNo = purchaseOrderItem.PartNo,
                        Quantity = purchaseOrderItem.Quantity,
                        IsApproved = purchaseOrderItem.IsApproved,
                        ItemSpecification = purchaseOrderItem.ItemSpecification,
                        UoM = purchaseOrderItem.UoM,
                        ItemType = purchaseOrderItem.ItemType,
                        UnitPrice = purchaseOrderItem.UnitPrice,
                        GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                        FromPreviousPurchase = purchaseOrderItem.FromPreviousPurchase,
                    };

                if (tax != null)
                {
                    entity.TaxAmount = tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.SubTotal = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.Total = (purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice) +
                                   (tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice);
                }
                entity.Remark = purchaseOrderItem.Remark;
                entity.OrgBranchName = purchaseOrderItem.OrgBranchName;
                entity.DateCreated = purchaseOrderItem.DateCreated;
                entity.LastModified = DateTime.Today;
                entity.CreatedBy = purchaseOrderItem.CreatedBy;
                entity.ModifiedBy = User.Identity.Name;


                db.PurchaseOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order item", "Success", User.Identity.Name, $"updated item {entity.ItemName} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseOrderItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderItems_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseOrderItem purchaseOrderItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseOrderItem
                {
                    PurchaseOrderItemID = purchaseOrderItem.PurchaseOrderItemID,
                    CatCode = purchaseOrderItem.CatCode,
                    CategoryName = purchaseOrderItem.CategoryName,
                    ItmCode = purchaseOrderItem.ItmCode,
                    ItemName = purchaseOrderItem.ItemName,
                    PartNo = purchaseOrderItem.PartNo,
                    Quantity = purchaseOrderItem.Quantity,
                    IsApproved = purchaseOrderItem.IsApproved,
                    ItemSpecification = purchaseOrderItem.ItemSpecification,
                    UoM = purchaseOrderItem.UoM,
                    ItemType = purchaseOrderItem.ItemType,
                    UnitPrice = purchaseOrderItem.UnitPrice,
                    GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                    TaxAmount = purchaseOrderItem.TaxAmount,
                    SubTotal = purchaseOrderItem.SubTotal,
                    Total = purchaseOrderItem.Total,
                    Remark = purchaseOrderItem.Remark,
                    FromPreviousPurchase = purchaseOrderItem.FromPreviousPurchase,
                    OrgBranchName = purchaseOrderItem.OrgBranchName,
                    DateCreated = purchaseOrderItem.DateCreated,
                    LastModified = purchaseOrderItem.LastModified,
                    CreatedBy = purchaseOrderItem.CreatedBy,
                    ModifiedBy = purchaseOrderItem.ModifiedBy
                };

                db.PurchaseOrderItems.Attach(entity);
                db.PurchaseOrderItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { purchaseOrderItem }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(ProcurementConstants.ProcurementAdmin, ProcurementConstants.ProcurementTenderApprover, ProcurementConstants.ProcurementTenderUser)]

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementConstants.ProcurementAdmin, ProcurementConstants.ProcurementTenderApprover, ProcurementConstants.ProcurementTenderUser)]

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
