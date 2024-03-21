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
    
    public class PoChangeOfOrderItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PoChangeOfOrderItems_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PoChangeOfOrderItem> PoChangeOfOrderItems = db.PoChangeOfOrderItems.Where(x => x.PoChangeOforderId == id);
            DataSourceResult result = PoChangeOfOrderItems.ToDataSourceResult(request, purchaseOrderItem => new
            {
                PoChangeOfOrderItemId = purchaseOrderItem.PoChangeOfOrderItemId,
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
                PoChangeOforderId = purchaseOrderItem.PoChangeOforderId,
                TaxAmount = purchaseOrderItem.TaxAmount,
                SubTotal = purchaseOrderItem.SubTotal,
                Total = purchaseOrderItem.Total,
                Discount = purchaseOrderItem.Discount,
                Remark = purchaseOrderItem.Remark,
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
        public ActionResult PoChangeOfOrderItems_Create(int id, [DataSourceRequest]DataSourceRequest request, PoChangeOfOrderItem purchaseOrderItem)
        {


            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Purchase Order First");
            }

            var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);

           


            if (ModelState.IsValid)
            {
                var entity = new PoChangeOfOrderItem
                {
                    PoChangeOforderId = id,
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

                db.PoChangeOfOrderItems.Add(entity);
                db.SaveChanges();
                purchaseOrderItem.PoChangeOfOrderItemId = entity.PoChangeOfOrderItemId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase change of order item ", "Success", User.Identity.Name, $"created item {entity.ItemName} under record {entity.PoChangeOforderId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseOrderItem }.ToDataSourceResult(request, ModelState));
        }
        

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOfOrderItems_Update([DataSourceRequest]DataSourceRequest request, PoChangeOfOrderItem purchaseOrderItem)
        {

            var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);

           
            if (ModelState.IsValid)
            {
                var entity =new PoChangeOfOrderItem
                {
                        PoChangeOfOrderItemId = purchaseOrderItem.PoChangeOfOrderItemId,
                        PoChangeOforderId = purchaseOrderItem.PoChangeOforderId,
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


                db.PoChangeOfOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase change of order item", "Success", User.Identity.Name, $"updated item {entity.ItemName} under record {entity.PoChangeOforderId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseOrderItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOfOrderItems_Destroy([DataSourceRequest]DataSourceRequest request, PoChangeOfOrderItem purchaseOrderItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoChangeOfOrderItem
                {
                    PoChangeOfOrderItemId = purchaseOrderItem.PoChangeOfOrderItemId,
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
                    OrgBranchName = purchaseOrderItem.OrgBranchName,
                    DateCreated = purchaseOrderItem.DateCreated,
                    LastModified = purchaseOrderItem.LastModified,
                    CreatedBy = purchaseOrderItem.CreatedBy,
                    ModifiedBy = purchaseOrderItem.ModifiedBy
                };

                db.PoChangeOfOrderItems.Attach(entity);
                db.PoChangeOfOrderItems.Remove(entity);
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
