using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{

    public class PurchaseOrderItemDirectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PurchaseOrderItemDirects_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchaseOrderItem> purchaseOrderItemDirects = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id);
            DataSourceResult result = purchaseOrderItemDirects.ToDataSourceResult(request, purchaseOrderItem => new
            {
                PurchaseOrderItemID = purchaseOrderItem.PurchaseOrderItemID,
                CatCode = purchaseOrderItem.CatCode,
                CategoryName = purchaseOrderItem.CategoryName,
                ItmCode = purchaseOrderItem.ItmCode,
                ItemName = purchaseOrderItem.ItemName,
                Name = purchaseOrderItem.Name,
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
                Discount = purchaseOrderItem.Discount,
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

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderItemDirects_Create(int id, [DataSourceRequest]DataSourceRequest request, PurchaseOrderItem purchaseOrderItem)
        {


            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Purchase Order First");
            }

            var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);

            #region validate quantity

            //var purchaseOrder = db.PurchaseOrders.Find(id);

            //var prid = db.PurchaseRequisitions.Find(purchaseOrder?.PurchaseRequisitionID)?.PurchaseRequisitionID;
            //var pritem = db.PurchaseRequisitionItems
            //    .Where(x => x.PurchaseRequisitionID == prid).FirstOrDefault(x => x.ItmCode == purchaseOrderItem.ItmCode);
            //if (purchaseOrderItem.Quantity > pritem.Quantity)
            //    ModelState.AddModelError("above", "Quanity of item is above requested quanity.");


            List<int> assignmentIds = db.PoPurchaseRequisitions
               .Where(x => x.PurchaseOrderID == id).Select(x=>x.PurchaseRequisitionAssignmentId).ToList();

            var pritems=db.PurchaseRequisitionReassignmentItems
                .Where(x=>assignmentIds.Contains(x.PurchaseRequisitionAssignmentId))
                .Where(x=>x.ItmCode== purchaseOrderItem.ItmCode)
                .Where(x=>x.ItemSpecification==purchaseOrderItem.ItemSpecification).AsQueryable();

            if(purchaseOrderItem.ItemType==PurchaseItemType.FixedAsset)
            {
                pritems = pritems.Where(x => x.Name == purchaseOrderItem.Name);
            }
           
            var filteredPrItems=pritems.ToList();

            if(filteredPrItems.Any())
            {
                decimal requestedTotal = filteredPrItems.Sum(x => x.Quantity);
                if (purchaseOrderItem.Quantity > requestedTotal)
                    ModelState.AddModelError("above", "Quanity of item is above requested quanity.");
            }

            #endregion

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
                    PartNo = purchaseOrderItem.PartNo,
                    Quantity = purchaseOrderItem.Quantity,
                    IsApproved = purchaseOrderItem.IsApproved,
                    ItemSpecification = purchaseOrderItem.ItemSpecification,
                    UoM = purchaseOrderItem.UoM,
                    ItemType = purchaseOrderItem.ItemType,
                    UnitPrice = purchaseOrderItem.UnitPrice,
                    Discount = purchaseOrderItem.Discount,
                    GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                    FromPreviousPurchase = purchaseOrderItem.FromPreviousPurchase,
                };
                if (tax != null)
                {
                    entity.TaxAmount = tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.SubTotal = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.Total = (purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice) 
                                   +(tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice)
                                   - purchaseOrderItem.Discount; 
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
        

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderItemDirects_Update([DataSourceRequest]DataSourceRequest request, PurchaseOrderItem purchaseOrderItem)
        {

            var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);

            #region validate quantity

            //var purchaseOrder = db.PurchaseOrders.Find(purchaseOrderItem.PurchaseOrderID);

            //var prid = db.PurchaseRequisitions.Find(purchaseOrder?.PurchaseRequisitionID)?.PurchaseRequisitionID;
            //var pritem = db.PurchaseRequisitionItems
            //    .Where(x => x.PurchaseRequisitionID == prid).FirstOrDefault(x => x.ItmCode == purchaseOrderItem.ItmCode);
            //if (purchaseOrderItem.Quantity > pritem.Quantity)
            //    ModelState.AddModelError("above", "Quanity of item is above requested quanity.");

            List<int> assignmentIds = db.PoPurchaseRequisitions
               .Where(x => x.PurchaseOrderID == purchaseOrderItem.PurchaseOrderID).Select(x => x.PurchaseRequisitionAssignmentId).ToList();

            var pritems = db.PurchaseRequisitionReassignmentItems
                .Where(x => assignmentIds.Contains(x.PurchaseRequisitionAssignmentId))
                .Where(x => x.ItmCode == purchaseOrderItem.ItmCode)
                .Where(x => x.ItemSpecification == purchaseOrderItem.ItemSpecification).AsQueryable();

            if (purchaseOrderItem.ItemType == PurchaseItemType.FixedAsset)
            {
                pritems = pritems.Where(x => x.Name == purchaseOrderItem.Name);
            }

            var filteredPrItems = pritems.ToList();

            if (filteredPrItems.Any())
            {
                decimal requestedTotal = filteredPrItems.Sum(x => x.Quantity);
                if (purchaseOrderItem.Quantity > requestedTotal)
                    ModelState.AddModelError("above", "Quanity of item is above requested quanity.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                PurchaseOrderItem entity =new PurchaseOrderItem
                    {
                        PurchaseOrderItemID = purchaseOrderItem.PurchaseOrderItemID,
                        PurchaseOrderID = purchaseOrderItem.PurchaseOrderID,
                        CatCode = purchaseOrderItem.CatCode,
                        CategoryName = purchaseOrderItem.CategoryName,
                        ItmCode = purchaseOrderItem.ItmCode,
                        ItemName = purchaseOrderItem.ItemName,
                        Name = purchaseOrderItem.Name,
                        PartNo = purchaseOrderItem.PartNo,
                        Quantity = purchaseOrderItem.Quantity,
                        IsApproved = purchaseOrderItem.IsApproved,
                        ItemSpecification = purchaseOrderItem.ItemSpecification,
                        UoM = purchaseOrderItem.UoM,
                        ItemType = purchaseOrderItem.ItemType,
                        UnitPrice = purchaseOrderItem.UnitPrice,
                        Discount = purchaseOrderItem.Discount,
                        GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                        FromPreviousPurchase = purchaseOrderItem.FromPreviousPurchase,
                    };

                if (tax != null)
                {
                    entity.TaxAmount = tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.SubTotal = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    entity.Total = (purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice) 
                                   +(tax.CalculatedRate * purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice)
                                   - purchaseOrderItem.Discount; 
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

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderItemDirects_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseOrderItem purchaseOrderItem)
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
                    Name = purchaseOrderItem.Name,
                    PartNo = purchaseOrderItem.PartNo,
                    Quantity = purchaseOrderItem.Quantity,
                    Discount = purchaseOrderItem.Discount,
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
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementTenderApprover, ProcurementRoles.ProcurementTenderUser)]

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementTenderApprover, ProcurementRoles.ProcurementTenderUser)]

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
