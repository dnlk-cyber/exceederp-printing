using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    public class TenderParticipantItemsController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ParticipantsItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ParticipantsItem> participantsitems = _db.ParticipantsItems.Where(x => x.LotID == id);
            DataSourceResult result = participantsitems.ToDataSourceResult(request, participantsItem => new
            {
                ParticipantsItemId = participantsItem.ParticipantsItemId,
                LotItemID = participantsItem.LotItemID,
                CatCode = participantsItem.CatCode,
                CategoryName = participantsItem.CategoryName,
                ItmCode = participantsItem.ItmCode,
                ItemName = participantsItem.ItemName,
                Name = participantsItem.Name,
                ModelNo = participantsItem.ModelNo,
                PartNo = participantsItem.PartNo,
                ItemSpecification = participantsItem.ItemSpecification,
                UoM = participantsItem.UoM,
                ItemType = participantsItem.ItemType,
                Quantity = participantsItem.Quantity,
                GLTaxRateID = participantsItem.GLTaxRateID,
                SupplierId = participantsItem.SupplierId,
                IsNewSupplier = participantsItem.IsNewSupplier,
                SupplierName = participantsItem.SupplierName,
                UnitPrice = participantsItem.UnitPrice,
                TaxAmount = participantsItem.TaxAmount,
                SubTotal = participantsItem.SubTotal,
                Technical = participantsItem.Technical,
                Total = participantsItem.Total,
                DiscountAmount = participantsItem.DiscountAmount,
                LotID = participantsItem.LotID,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipantsItems_Create([DataSourceRequest]DataSourceRequest request, ParticipantsItem participantsItem, int id)
        {

            var exists = _db.ParticipantsItems
               .Where(x => x.ItmCode == participantsItem.ItmCode)
               .Where(x => x.Name == participantsItem.Name)
               .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)
               .Where(x => x.ModelNo == participantsItem.ModelNo)
               .Where(x => x.SupplierName == participantsItem.SupplierName)
               .Where(x => x.LotID == participantsItem.LotID)
               .Where(x => x.ParticipantsItemId != participantsItem.ParticipantsItemId);

            if (exists.Any())
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");

            if (participantsItem.UnitPrice == 0)
                ModelState.AddModelError("unitPrice", "Please Insert Unit Price");

            if (ModelState.IsValid)
            {
                LotItems lotItems = _db.LotItems
                    .FirstOrDefault(x => x.LotID == id && x.ItmCode == participantsItem.ItmCode && x.ItemSpecification == participantsItem.ItemSpecification);
                var tax = _db.GLTaxRates.Find(participantsItem.GLTaxRateID);
                if (tax != null)
                {
                    if (lotItems != null)
                    {
                        var entity = new ParticipantsItem
                        {
                            LotID = id,
                            LotItemID = participantsItem.LotItemID,
                            Technical = participantsItem.Technical,
                            GLTaxRateID = tax.GLTaxRateID,
                            CatCode = participantsItem.CatCode,
                            CategoryName = participantsItem.CategoryName,
                            ItmCode = participantsItem.ItmCode,
                            Name = participantsItem.Name,
                            ModelNo = participantsItem.ModelNo,
                            ItemName = participantsItem.ItemName,
                            PartNo = participantsItem.PartNo,
                            ItemSpecification = participantsItem.ItemSpecification,
                            UoM = participantsItem.UoM,
                            ItemType = participantsItem.ItemType,
                            SupplierId = participantsItem.SupplierId,
                            IsNewSupplier = participantsItem.IsNewSupplier,
                            SupplierName = participantsItem.SupplierName,
                            Quantity = lotItems.Quantity,
                            UnitPrice = participantsItem.UnitPrice,
                            SubTotal = lotItems.Quantity * participantsItem.UnitPrice,
                            TaxAmount = lotItems.Quantity * tax.CalculatedRate * participantsItem.UnitPrice,
                            Total = lotItems.Quantity * tax.CalculatedRate * participantsItem.UnitPrice + lotItems.Quantity * participantsItem.UnitPrice,

                            DiscountAmount = participantsItem.DiscountAmount
                        };

                        _db.ParticipantsItems.Add(entity);
                        _db.SaveChanges();
                        participantsItem.ParticipantsItemId = entity.ParticipantsItemId;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "tender lot participant", "Success", User.Identity.Name, $"created lot participant {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                        #endregion
                    }
                }
            }

            return Json(new[] { participantsItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipantsItems_Update([DataSourceRequest]DataSourceRequest request, ParticipantsItem participantsItem)
        {
            var exists = _db.ParticipantsItems
                .Where(x => x.ItmCode == participantsItem.ItmCode)
                .Where(x => x.Name == participantsItem.Name)
                .Where(x => x.ModelNo == participantsItem.ModelNo)
                .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)
                .Where(x => x.SupplierName == participantsItem.SupplierName)
                .Where(x => x.LotID == participantsItem.LotID)
                .Where(x => x.ParticipantsItemId != participantsItem.ParticipantsItemId);
            if(exists.Any())
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");

            if (participantsItem.UnitPrice == 0)
                ModelState.AddModelError("unitPrice", "Please Insert Unit Price");

            if (ModelState.IsValid)
            {
                var lotItems = _db.LotItems.FirstOrDefault(x => x.LotID == participantsItem.LotID && x.ItmCode == participantsItem.ItmCode && x.ItemSpecification == participantsItem.ItemSpecification);
                GLTaxRate tax = _db.GLTaxRates.Find(participantsItem.GLTaxRateID);
                if (lotItems != null)
                {
                    if (tax != null)
                    {
                        var entity = new ParticipantsItem
                        {
                            LotID = participantsItem.LotID,
                            LotItemID = participantsItem.LotItemID,
                            ParticipantsItemId = participantsItem.ParticipantsItemId,
                            GLTaxRateID = participantsItem.GLTaxRateID,
                            Technical = participantsItem.Technical,
                            CatCode = participantsItem.CatCode,
                            CategoryName = participantsItem.CategoryName,
                            ItmCode = participantsItem.ItmCode,
                            ItemName = participantsItem.ItemName,
                            Name = participantsItem.Name,
                            ModelNo = participantsItem.ModelNo,
                            PartNo = participantsItem.PartNo,
                            ItemSpecification = participantsItem.ItemSpecification,
                            UoM = participantsItem.UoM,
                            SupplierId = participantsItem.SupplierId,
                            IsNewSupplier = participantsItem.IsNewSupplier,
                            SupplierName = participantsItem.SupplierName,
                            UnitPrice = participantsItem.UnitPrice,
                            Quantity = lotItems.Quantity,
                            SubTotal = lotItems.Quantity * participantsItem.UnitPrice,
                            TaxAmount = lotItems.Quantity * tax.CalculatedRate * participantsItem.UnitPrice,
                            Total = lotItems.Quantity * tax.CalculatedRate * participantsItem.UnitPrice + lotItems.Quantity * participantsItem.UnitPrice,
                            DiscountAmount = participantsItem.DiscountAmount
                        };

                        _db.ParticipantsItems.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "tender lot participant", "Success", User.Identity.Name, $"updated lot participant {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                        #endregion
                    }
                }
                _db.SaveChanges();
            }

            return Json(new[] { participantsItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipantsItems_Destroy([DataSourceRequest]DataSourceRequest request, ParticipantsItem participantsItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ParticipantsItem
                {
                    ParticipantsItemId = participantsItem.ParticipantsItemId,
                    LotItemID = participantsItem.LotItemID,
                    CatCode = participantsItem.CatCode,
                    CategoryName = participantsItem.CategoryName,
                    ItmCode = participantsItem.ItmCode,
                    ItemName = participantsItem.ItemName,
                    PartNo = participantsItem.PartNo,
                    ItemSpecification = participantsItem.ItemSpecification,
                    UoM = participantsItem.UoM,
                    Technical = participantsItem.Technical,
                    LotID = participantsItem.LotID,
                    SupplierId = participantsItem.SupplierId,
                    IsNewSupplier = participantsItem.IsNewSupplier,
                    SupplierName = participantsItem.SupplierName,
                    UnitPrice = participantsItem.UnitPrice
                };

                _db.ParticipantsItems.Attach(entity);
                _db.ParticipantsItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { participantsItem }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetItemParticipants(string itemcode)
        {
            List<BidPurchasingSuppliers> purchasingSupplier = new List<BidPurchasingSuppliers>();
            var participants = _db.ParticipantsItems.Where(x => x.ItmCode == itemcode);
            foreach (var participant in participants)
            {
                BidPurchasingSuppliers suppliers = _db.BidPurchasingSupplierses.FirstOrDefault(x=>x.SupplierName==participant.SupplierName);
                purchasingSupplier.Add(suppliers);
            }
            return Json(purchasingSupplier.Select(x => new { 
                    supplierId = x.BidPurchasingSuppliersID,
                    Name = x.SupplierName }),
                JsonRequestBehavior.AllowGet);
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
