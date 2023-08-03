using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class TenderAwardItemController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }




        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult BidAwardedSupplierItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BidAwardedSupplierItem> bidAwardedSupplierItems = _db.BidAwardedSupplierItems.Where(l => l.BidAwardedSupplierID == id);
            DataSourceResult result = bidAwardedSupplierItems.ToDataSourceResult(request, bidAwardedSupplierItem => new
            {
                BidAwardedSupplierItemId = bidAwardedSupplierItem.BidAwardedSupplierItemId,
                BidAwardedSupplierID = bidAwardedSupplierItem.BidAwardedSupplierID,
                LotItemID = bidAwardedSupplierItem.LotItemID,
                ItemType = bidAwardedSupplierItem.ItemType,
                CatCode = bidAwardedSupplierItem.CatCode,
                CategoryName = bidAwardedSupplierItem.CategoryName,
                ItmCode = bidAwardedSupplierItem.ItmCode,
                ItemName = bidAwardedSupplierItem.ItemName,
                Name = bidAwardedSupplierItem.Name,
                ModelNo = bidAwardedSupplierItem.ModelNo,
                PartNo = bidAwardedSupplierItem.PartNo,
                IsApproved = bidAwardedSupplierItem.IsApproved,
                ItemSpecification = bidAwardedSupplierItem.ItemSpecification,
                UoM = bidAwardedSupplierItem.UoM,
                GLTaxRateID = bidAwardedSupplierItem.GLTaxRateID,
                TaxAmount = bidAwardedSupplierItem.TaxAmount,
                Quanity = bidAwardedSupplierItem.Quanity,
                UPrice = bidAwardedSupplierItem.UPrice,
                Discount = bidAwardedSupplierItem.Discount,
                FinalUPrice = bidAwardedSupplierItem.FinalUPrice,
                TPrice = bidAwardedSupplierItem.TPrice,
                OrgBranchName = bidAwardedSupplierItem.OrgBranchName,
                DateCreated = bidAwardedSupplierItem.DateCreated,
                LastModified = bidAwardedSupplierItem.LastModified,
                CreatedBy = bidAwardedSupplierItem.CreatedBy,
                ModifiedBy = bidAwardedSupplierItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidAwardedSupplierItems_Create([DataSourceRequest]DataSourceRequest request, BidAwardedSupplierItem bidAwardedSupplierItem, int id)
        {

            if (bidAwardedSupplierItem.LotItemID == 0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get lot item result info

            var awardObj = _db.BidAwardedSuppliers.Find(id);
            var lotItemResult = _db.ParticipantsItems
                .Where(x => x.LotID == awardObj.LotID)
                .Where(x => x.SupplierName== awardObj.SupplierName)
                .FirstOrDefault(x => x.LotItemID== bidAwardedSupplierItem.LotItemID);
           
            if (lotItemResult != null)
            {
                bidAwardedSupplierItem.CatCode = lotItemResult.CatCode;
                bidAwardedSupplierItem.CategoryName = lotItemResult.CategoryName;
                bidAwardedSupplierItem.ItmCode = lotItemResult.ItmCode;
                bidAwardedSupplierItem.ItemName = lotItemResult.ItemName;
                bidAwardedSupplierItem.Name = lotItemResult.Name;
                bidAwardedSupplierItem.ModelNo = lotItemResult.ModelNo;

                bidAwardedSupplierItem.Quanity = lotItemResult.Quantity;
                bidAwardedSupplierItem.UPrice = lotItemResult.UnitPrice;
                bidAwardedSupplierItem.UoM = lotItemResult.UoM;
                bidAwardedSupplierItem.Discount = bidAwardedSupplierItem.Discount;//from here user filled
                bidAwardedSupplierItem.GLTaxRateID = lotItemResult.GLTaxRateID;
                bidAwardedSupplierItem.TaxAmount = lotItemResult.TaxAmount;
                bidAwardedSupplierItem.TPrice = lotItemResult.Total;
            }

            #endregion

            if (_db.BidAwardedSupplierItems.Any(x => x.ItmCode == bidAwardedSupplierItem.ItmCode && x.BidAwardedSupplierID == id))
            {
                ModelState.AddModelError("already", "You have already assigned a winning supplier for this Item");
            }

            bidAwardedSupplierItem.FinalUPrice = bidAwardedSupplierItem.UPrice - bidAwardedSupplierItem.Discount;
            var tax = _db.GLTaxRates.Find(bidAwardedSupplierItem.GLTaxRateID);
            if (tax != null)
            {
                bidAwardedSupplierItem.TaxAmount = bidAwardedSupplierItem.FinalUPrice * bidAwardedSupplierItem.Quanity * tax.CalculatedRate;
                bidAwardedSupplierItem.TPrice = bidAwardedSupplierItem.FinalUPrice * bidAwardedSupplierItem.Quanity +
                                            (bidAwardedSupplierItem.FinalUPrice * bidAwardedSupplierItem.Quanity *
                                             tax.CalculatedRate);
            }


            if (ModelState.IsValid)
            {
                var entity = new BidAwardedSupplierItem
                {
                    BidAwardedSupplierID = id,
                    LotItemID = bidAwardedSupplierItem.LotItemID,
                    CatCode = bidAwardedSupplierItem.CatCode,
                    CategoryName = bidAwardedSupplierItem.CategoryName,
                    ItemName = bidAwardedSupplierItem.ItemName,
                    Name = bidAwardedSupplierItem.Name,
                    ModelNo = bidAwardedSupplierItem.ModelNo,
                    PartNo = bidAwardedSupplierItem.PartNo,
                    IsApproved = bidAwardedSupplierItem.IsApproved,
                    ItemSpecification = bidAwardedSupplierItem.ItemSpecification,
                    UoM = bidAwardedSupplierItem.UoM,
                    GLTaxRateID = bidAwardedSupplierItem.GLTaxRateID,
                    TaxAmount = bidAwardedSupplierItem.TaxAmount,
                    Quanity = bidAwardedSupplierItem.Quanity,
                    UPrice = bidAwardedSupplierItem.UPrice,
                    Discount = bidAwardedSupplierItem.Discount,
                    FinalUPrice = bidAwardedSupplierItem.FinalUPrice,
                    TPrice = bidAwardedSupplierItem.TPrice,
                    OrgBranchName = bidAwardedSupplierItem.OrgBranchName,
                    ItmCode = bidAwardedSupplierItem.ItmCode,
                    DateCreated = bidAwardedSupplierItem.DateCreated,
                    LastModified = bidAwardedSupplierItem.LastModified,
                    CreatedBy = bidAwardedSupplierItem.CreatedBy,
                    ModifiedBy = bidAwardedSupplierItem.ModifiedBy
                };

                _db.BidAwardedSupplierItems.Add(entity);
                _db.SaveChanges();
                bidAwardedSupplierItem.BidAwardedSupplierItemId = entity.BidAwardedSupplierItemId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot award item", "Success", User.Identity.Name, $"created lot award {entity.ItemName} under record {entity.BidAwardedSupplierID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { bidAwardedSupplierItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidAwardedSupplierItems_Update([DataSourceRequest]DataSourceRequest request, BidAwardedSupplierItem bidAwardedSupplierItem)
        {
            if (_db.BidAwardedSupplierItems.Any(x => x.BidAwardedSupplierItemId != bidAwardedSupplierItem.BidAwardedSupplierItemId && x.BidAwardedSupplierID == bidAwardedSupplierItem.BidAwardedSupplierID && x.ItmCode == bidAwardedSupplierItem.ItmCode))
            {
                ModelState.AddModelError("already", "You have already assigned a winning supplier for this Item");

            }
            bidAwardedSupplierItem.FinalUPrice = bidAwardedSupplierItem.UPrice - bidAwardedSupplierItem.Discount;
            var tax = _db.GLTaxRates.Find(bidAwardedSupplierItem.GLTaxRateID);
            if (tax != null)
            {
                bidAwardedSupplierItem.TaxAmount = bidAwardedSupplierItem.FinalUPrice * bidAwardedSupplierItem.Quanity * tax.CalculatedRate;
                bidAwardedSupplierItem.TPrice = bidAwardedSupplierItem.FinalUPrice * bidAwardedSupplierItem.Quanity +
                                                (bidAwardedSupplierItem.FinalUPrice * bidAwardedSupplierItem.Quanity *
                                                 tax.CalculatedRate);
            }
            if (ModelState.IsValid)
            {
                var entity = new BidAwardedSupplierItem
                {
                    BidAwardedSupplierItemId = bidAwardedSupplierItem.BidAwardedSupplierItemId,
                    BidAwardedSupplierID = bidAwardedSupplierItem.BidAwardedSupplierID,
                    LotItemID = bidAwardedSupplierItem.LotItemID,
                    ItemType = bidAwardedSupplierItem.ItemType,
                    CatCode = bidAwardedSupplierItem.CatCode,
                    CategoryName = bidAwardedSupplierItem.CategoryName,
                    ItemName = bidAwardedSupplierItem.ItemName,
                    Name = bidAwardedSupplierItem.Name,
                    ModelNo = bidAwardedSupplierItem.ModelNo,
                    PartNo = bidAwardedSupplierItem.PartNo,
                    IsApproved = bidAwardedSupplierItem.IsApproved,
                    ItemSpecification = bidAwardedSupplierItem.ItemSpecification,
                    UoM = bidAwardedSupplierItem.UoM,
                    GLTaxRateID = bidAwardedSupplierItem.GLTaxRateID,
                    TaxAmount = bidAwardedSupplierItem.TaxAmount,
                    Quanity = bidAwardedSupplierItem.Quanity,
                    UPrice = bidAwardedSupplierItem.UPrice,
                    Discount = bidAwardedSupplierItem.Discount,
                    FinalUPrice = bidAwardedSupplierItem.FinalUPrice,
                    TPrice = bidAwardedSupplierItem.TPrice,
                    ItmCode = bidAwardedSupplierItem.ItmCode,
                    OrgBranchName = bidAwardedSupplierItem.OrgBranchName,
                    DateCreated = bidAwardedSupplierItem.DateCreated,
                    LastModified = bidAwardedSupplierItem.LastModified,
                    CreatedBy = bidAwardedSupplierItem.CreatedBy,
                    ModifiedBy = bidAwardedSupplierItem.ModifiedBy
                };

                _db.BidAwardedSupplierItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot award item", "Success", User.Identity.Name, $"updated lot award {entity.ItemName} under record {entity.BidAwardedSupplierID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { bidAwardedSupplierItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidAwardedSupplierItems_Destroy([DataSourceRequest]DataSourceRequest request, BidAwardedSupplierItem bidAwardedSupplierItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new BidAwardedSupplierItem
                {
                    BidAwardedSupplierItemId = bidAwardedSupplierItem.BidAwardedSupplierItemId,
                    ItmCode = bidAwardedSupplierItem.ItmCode,
                    OrgBranchName = bidAwardedSupplierItem.OrgBranchName,
                    DateCreated = bidAwardedSupplierItem.DateCreated,
                    LastModified = bidAwardedSupplierItem.LastModified,
                    CreatedBy = bidAwardedSupplierItem.CreatedBy,
                    ModifiedBy = bidAwardedSupplierItem.ModifiedBy
                };

                _db.BidAwardedSupplierItems.Attach(entity);
                _db.BidAwardedSupplierItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { bidAwardedSupplierItem }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
