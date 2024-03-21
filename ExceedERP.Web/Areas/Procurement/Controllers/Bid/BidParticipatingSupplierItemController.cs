using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class BidParticipatingSupplierItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ParticipatingSupplierItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BidParticipatingSupplierItem> participatingSupplierItems = _db.BidParticipatingSupplierItems.Where(p => p.BidPParticipatingSuppliersID == id);
            DataSourceResult result = participatingSupplierItems.ToDataSourceResult(request, participatingItem => new
            {
                BidParticipatingSupplierItemId = participatingItem.BidParticipatingSupplierItemId,
                BidPParticipatingSuppliersID = participatingItem.BidPParticipatingSuppliersID,
                CatCode = participatingItem.CatCode,
                CategoryName = participatingItem.CategoryName,
                ItmCode = participatingItem.ItmCode,
                ItemName = participatingItem.ItemName,
                PartNo = participatingItem.PartNo,
                Name = participatingItem.Name,
                ModelNo = participatingItem.ModelNo,
                ItemSpecification = participatingItem.ItemSpecification,
                UoM = participatingItem.UoM,
                ItemType = participatingItem.ItemType,
                LotItemID = participatingItem.LotItemID,
                GLTaxRateID = participatingItem.GLTaxRateID,
                TaxAmount = participatingItem.TaxAmount,
                Quantity = participatingItem.Quantity,
                UnitPrice = participatingItem.UnitPrice,
                DiscountAmount = participatingItem.DiscountAmount,
                SubTotal = participatingItem.SubTotal,
                Total = participatingItem.Total,
                IsOutOfStock = participatingItem.IsOutOfStock,
                Technical = participatingItem.Technical,
                Remark = participatingItem.Remark,
                DateCreated = participatingItem.DateCreated,
                LastModified = participatingItem.LastModified,
                CreatedBy = participatingItem.CreatedBy,
                ModifiedBy = participatingItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSupplierItems_Create([DataSourceRequest]DataSourceRequest request, BidParticipatingSupplierItem participatingItem, int id)
        {
            ModelState.Clear();

            var tax = _db.GLTaxRates.Find(participatingItem.GLTaxRateID);           
            var lotItem = _db.LotItems.Find(participatingItem.LotItemID);

            if(lotItem!=null)
            {
                participatingItem.CatCode = lotItem.CatCode;
                participatingItem.ItmCode = lotItem.ItmCode;
                participatingItem.CategoryName = lotItem.CategoryName;
                participatingItem.ItemName = lotItem.ItemName;
                participatingItem.Name = lotItem.Name;
                participatingItem.ModelNo = lotItem.ModelNo;
                participatingItem.Quantity = lotItem.Quantity;
            }
            else
            {
                ModelState.AddModelError("null","The lot item does not exist.");
            }

            if(tax==null)
                ModelState.AddModelError("tax", "Please select tax.");

            if (ModelState.IsValid)
            {
                if (tax != null&&lotItem!=null)
                {
                    
                    var exist = _db.BidParticipatingSupplierItems
                        .Where(x => x.BidPParticipatingSuppliersID == id)
                        .FirstOrDefault(x => x.LotItemID == lotItem.LotItemsID);

                    if (exist == null)
                    {
                        var entity = new BidParticipatingSupplierItem
                        {
                            BidPParticipatingSuppliersID = id,
                            LotItemID = lotItem.LotItemsID,
                            CatCode = lotItem.CatCode,
                            CategoryName = lotItem.CategoryName,
                            ItmCode = lotItem.ItmCode,
                            ItemName = lotItem.ItemName,
                            PartNo = lotItem.PartNo,
                            Name = lotItem.Name,
                            ModelNo = lotItem.ModelNo,
                            ItemSpecification = lotItem.ItemSpecification,
                            UoM = lotItem.UoM,
                            ItemType = lotItem.ItemType,
                            GLTaxRateID = participatingItem.GLTaxRateID,
                            Quantity = lotItem.Quantity,
                            UnitPrice = participatingItem.UnitPrice,
                            DiscountAmount = participatingItem.DiscountAmount,
                            TaxAmount = (tax.CalculatedRate * lotItem.Quantity * participatingItem.UnitPrice),
                            SubTotal = lotItem.Quantity * participatingItem.UnitPrice,
                            Total = lotItem.Quantity * participatingItem.UnitPrice + (tax.CalculatedRate * lotItem.Quantity * participatingItem.UnitPrice),


                            IsOutOfStock = participatingItem.IsOutOfStock,
                            Technical = participatingItem.Technical,
                            Remark = participatingItem.Remark,
                            DateCreated = participatingItem.DateCreated,
                            LastModified = participatingItem.LastModified,
                            CreatedBy = participatingItem.CreatedBy,
                            ModifiedBy = participatingItem.ModifiedBy
                        };

                        _db.BidParticipatingSupplierItems.Add(entity);
                        _db.SaveChanges();
                        participatingItem.BidParticipatingSupplierItemId = entity.BidParticipatingSupplierItemId;

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "proforma supplier item", "Success", User.Identity.Name, $"created supplier item {entity.ItemName} under record {entity.BidPParticipatingSuppliersID}", Modules.Procurement);
                        #endregion
                    }

                }
            }

            return Json(new[] { participatingItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSupplierItems_Update([DataSourceRequest]DataSourceRequest request, BidParticipatingSupplierItem participatingItem)
        {
            ModelState.Clear();

            var tax = _db.GLTaxRates.Find(participatingItem.GLTaxRateID);
            var lotItem = _db.LotItems.Find(participatingItem.LotItemID);

            if (lotItem == null)
            {
                ModelState.AddModelError("null", "The lot item does not exist.");
            }
           
            if (tax == null)
                ModelState.AddModelError("tax", "Please select tax.");

            #region get lote item info


            if (lotItem != null)
            {
                participatingItem.CatCode = lotItem.CatCode;
                participatingItem.CategoryName = lotItem.CategoryName;
                participatingItem.ItmCode = lotItem.ItmCode;
                participatingItem.ItemName = lotItem.ItemName;
                participatingItem.Name = lotItem.Name;
                participatingItem.ModelNo = lotItem.ModelNo;
                participatingItem.Quantity = lotItem.Quantity;
            }

            #endregion

           

            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    if (lotItem != null)
                    {
                        var entity = new BidParticipatingSupplierItem
                        {
                            BidParticipatingSupplierItemId = participatingItem.BidParticipatingSupplierItemId,
                            BidPParticipatingSuppliersID = participatingItem.BidPParticipatingSuppliersID,
                            CatCode = participatingItem.CatCode,
                            CategoryName = participatingItem.CategoryName,
                            ItmCode = participatingItem.ItmCode,
                            ItemName = lotItem.ItemName,
                            PartNo = participatingItem.PartNo,
                            Name = lotItem.Name,
                            ModelNo = lotItem.ModelNo,
                            ItemSpecification = participatingItem.ItemSpecification,
                            UoM = participatingItem.UoM,
                            ItemType = participatingItem.ItemType,
                            LotItemID = participatingItem.LotItemID,
                            GLTaxRateID = participatingItem.GLTaxRateID,
                            Quantity = lotItem.Quantity,
                            UnitPrice = participatingItem.UnitPrice,
                            DiscountAmount = participatingItem.DiscountAmount,
                            TaxAmount = (tax.CalculatedRate * lotItem.Quantity * participatingItem.UnitPrice),   
                            SubTotal= lotItem.Quantity * participatingItem.UnitPrice,
                            Total = lotItem.Quantity * participatingItem.UnitPrice + (tax.CalculatedRate * lotItem.Quantity * participatingItem.UnitPrice),
                            
                            IsOutOfStock = participatingItem.IsOutOfStock,
                            Technical = participatingItem.Technical,
                            Remark = participatingItem.Remark,
                            DateCreated = participatingItem.DateCreated,
                            LastModified = participatingItem.LastModified,
                            CreatedBy = participatingItem.CreatedBy,
                            ModifiedBy = participatingItem.ModifiedBy
                        };

                        _db.BidParticipatingSupplierItems.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                        _db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "proforma supplier item", "Success", User.Identity.Name, $"updated supplier item {entity.ItemName} under record {entity.BidPParticipatingSuppliersID}", Modules.Procurement);
                        #endregion
                    }
                }
               
            }

            return Json(new[] { participatingItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSupplierItems_Destroy([DataSourceRequest]DataSourceRequest request, BidParticipatingSupplierItem participatingItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new BidParticipatingSupplierItem
                {
                    BidParticipatingSupplierItemId = participatingItem.BidParticipatingSupplierItemId,
                    BidPParticipatingSuppliersID = participatingItem.BidPParticipatingSuppliersID,
                    CatCode = participatingItem.CatCode,
                    CategoryName = participatingItem.CategoryName,
                    ItmCode = participatingItem.ItmCode,
                    ItemName = participatingItem.ItemName,
                    PartNo = participatingItem.PartNo,
                    ItemSpecification = participatingItem.ItemSpecification,
                    UoM = participatingItem.UoM,
                    ItemType = participatingItem.ItemType,
                    GLTaxRateID = participatingItem.GLTaxRateID,
                    TaxAmount = participatingItem.TaxAmount,
                    Quantity = participatingItem.Quantity,
                    UnitPrice = participatingItem.UnitPrice,
                    SubTotal = participatingItem.SubTotal,
                    DiscountAmount = participatingItem.DiscountAmount,
                    Technical = participatingItem.Technical,
                    Total = participatingItem.Total,
                    LotItemID = participatingItem.LotItemID,
                    Remark = participatingItem.Remark,
                    DateCreated = participatingItem.DateCreated,
                    LastModified = participatingItem.LastModified,
                    CreatedBy = participatingItem.CreatedBy,
                    ModifiedBy = participatingItem.ModifiedBy
                };

                _db.BidParticipatingSupplierItems.Attach(entity);
                _db.BidParticipatingSupplierItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { participatingItem }.ToDataSourceResult(request, ModelState));
        }
        
        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
