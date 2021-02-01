using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseFollowUpItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForeignPurchaseFollowUpItems_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ForeignPurchaseFollowUpItems> foreignpurchasefollowupitems = db.ForeignPurchaseFollowUpItems.Where(x=>x.ForeignPurchaseId==id);
            DataSourceResult result = foreignpurchasefollowupitems.ToDataSourceResult(request, foreignPurchaseFollowUpItems => new {
                ForeignPurchaseFollowUpItemsID = foreignPurchaseFollowUpItems.ForeignPurchaseFollowUpItemsID,
                CatCode = foreignPurchaseFollowUpItems.CatCode,
                CategoryName = foreignPurchaseFollowUpItems.CategoryName,
                ItmCode = foreignPurchaseFollowUpItems.ItmCode,
                ItemName = foreignPurchaseFollowUpItems.ItemName,
                Name = foreignPurchaseFollowUpItems.Name,
                ModelNo = foreignPurchaseFollowUpItems.ModelNo,
                ItemType = foreignPurchaseFollowUpItems.ItemType,
                PartNo = foreignPurchaseFollowUpItems.PartNo,
                Quantity = foreignPurchaseFollowUpItems.Quantity,
                UnitPrice = foreignPurchaseFollowUpItems.UnitPrice,
                GLTaxRateID = foreignPurchaseFollowUpItems.GLTaxRateID,
                TaxAmount = foreignPurchaseFollowUpItems.TaxAmount,
                SubTotal = foreignPurchaseFollowUpItems.SubTotal,
                AdditionalUnitCost = foreignPurchaseFollowUpItems.AdditionalUnitCost,
                FinalUnitPrice = foreignPurchaseFollowUpItems.FinalUnitPrice,
                Total = foreignPurchaseFollowUpItems.Total,
                ItemSpecification = foreignPurchaseFollowUpItems.ItemSpecification,
                UoM = foreignPurchaseFollowUpItems.UoM
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpItems_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpItems foreignPurchaseFollowUpItems)
        {
            #region calculate dta
            foreignPurchaseFollowUpItems.SubTotal = foreignPurchaseFollowUpItems.Quantity * foreignPurchaseFollowUpItems.UnitPrice;
            var tax = db.GLTaxRates.Find(foreignPurchaseFollowUpItems.GLTaxRateID);
            if (tax != null)
                foreignPurchaseFollowUpItems.TaxAmount = tax.CalculatedRate * foreignPurchaseFollowUpItems.SubTotal;

            foreignPurchaseFollowUpItems.Total = foreignPurchaseFollowUpItems.Quantity * (foreignPurchaseFollowUpItems.UnitPrice + foreignPurchaseFollowUpItems.AdditionalUnitCost) + foreignPurchaseFollowUpItems.TaxAmount;
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpItems
                {
                    ForeignPurchaseId = id,
                    CatCode = foreignPurchaseFollowUpItems.CatCode,
                    CategoryName = foreignPurchaseFollowUpItems.CategoryName,
                    ItmCode = foreignPurchaseFollowUpItems.ItmCode,
                    ItemName = foreignPurchaseFollowUpItems.ItemName,
                    Name = foreignPurchaseFollowUpItems.Name,
                    ModelNo = foreignPurchaseFollowUpItems.ModelNo,
                    ItemType = foreignPurchaseFollowUpItems.ItemType,
                    PartNo = foreignPurchaseFollowUpItems.PartNo,
                    Quantity = foreignPurchaseFollowUpItems.Quantity,
                    UnitPrice = foreignPurchaseFollowUpItems.UnitPrice,
                    GLTaxRateID = foreignPurchaseFollowUpItems.GLTaxRateID,
                    TaxAmount = foreignPurchaseFollowUpItems.TaxAmount,
                    SubTotal = foreignPurchaseFollowUpItems.SubTotal,
                    AdditionalUnitCost = foreignPurchaseFollowUpItems.AdditionalUnitCost,
                    FinalUnitPrice = foreignPurchaseFollowUpItems.FinalUnitPrice,
                    Total = foreignPurchaseFollowUpItems.Total,
                    ItemSpecification = foreignPurchaseFollowUpItems.ItemSpecification,
                    UoM = foreignPurchaseFollowUpItems.UoM
                };

                db.ForeignPurchaseFollowUpItems.Add(entity);
                db.SaveChanges();
                foreignPurchaseFollowUpItems.ForeignPurchaseFollowUpItemsID = entity.ForeignPurchaseFollowUpItemsID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up item", "Success", User.Identity.Name, $"created item {entity.ItemName} under record {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseFollowUpItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpItems_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpItems foreignPurchaseFollowUpItems)
        {
            #region calculate dta
            foreignPurchaseFollowUpItems.SubTotal =  foreignPurchaseFollowUpItems.Quantity * foreignPurchaseFollowUpItems.UnitPrice;
            var tax = db.GLTaxRates.Find(foreignPurchaseFollowUpItems.GLTaxRateID);
            if (tax != null)
                foreignPurchaseFollowUpItems.TaxAmount = tax.CalculatedRate * foreignPurchaseFollowUpItems.SubTotal;

            foreignPurchaseFollowUpItems.Total = foreignPurchaseFollowUpItems.Quantity * (foreignPurchaseFollowUpItems.UnitPrice+foreignPurchaseFollowUpItems.AdditionalUnitCost)+foreignPurchaseFollowUpItems.TaxAmount;
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpItems
                {
                    ForeignPurchaseId = foreignPurchaseFollowUpItems.ForeignPurchaseId,
                    ForeignPurchaseFollowUpItemsID = foreignPurchaseFollowUpItems.ForeignPurchaseFollowUpItemsID,
                    CatCode = foreignPurchaseFollowUpItems.CatCode,
                    CategoryName = foreignPurchaseFollowUpItems.CategoryName,
                    ItmCode = foreignPurchaseFollowUpItems.ItmCode,
                    ItemName = foreignPurchaseFollowUpItems.ItemName,
                    Name = foreignPurchaseFollowUpItems.Name,
                    ModelNo = foreignPurchaseFollowUpItems.ModelNo,
                    ItemType = foreignPurchaseFollowUpItems.ItemType,
                    PartNo = foreignPurchaseFollowUpItems.PartNo,
                    Quantity = foreignPurchaseFollowUpItems.Quantity,
                    UnitPrice = foreignPurchaseFollowUpItems.UnitPrice,
                    GLTaxRateID = foreignPurchaseFollowUpItems.GLTaxRateID,
                    TaxAmount = foreignPurchaseFollowUpItems.TaxAmount,
                    SubTotal = foreignPurchaseFollowUpItems.SubTotal,
                    AdditionalUnitCost = foreignPurchaseFollowUpItems.AdditionalUnitCost,
                    FinalUnitPrice = foreignPurchaseFollowUpItems.FinalUnitPrice,
                    Total = foreignPurchaseFollowUpItems.Total,
                    ItemSpecification = foreignPurchaseFollowUpItems.ItemSpecification,
                    UoM = foreignPurchaseFollowUpItems.UoM
                };

                db.ForeignPurchaseFollowUpItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up item", "Success", User.Identity.Name, $"updated item {entity.ItemName} under record {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseFollowUpItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpItems_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpItems foreignPurchaseFollowUpItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpItems
                {
                    ForeignPurchaseId = foreignPurchaseFollowUpItems.ForeignPurchaseId,
                    ForeignPurchaseFollowUpItemsID = foreignPurchaseFollowUpItems.ForeignPurchaseFollowUpItemsID,
                    CatCode = foreignPurchaseFollowUpItems.CatCode,
                    CategoryName = foreignPurchaseFollowUpItems.CategoryName,
                    ItmCode = foreignPurchaseFollowUpItems.ItmCode,
                    ItemName = foreignPurchaseFollowUpItems.ItemName,
                    ItemType = foreignPurchaseFollowUpItems.ItemType,
                    PartNo = foreignPurchaseFollowUpItems.PartNo,
                    Quantity = foreignPurchaseFollowUpItems.Quantity,
                    UnitPrice = foreignPurchaseFollowUpItems.UnitPrice,
                    GLTaxRateID = foreignPurchaseFollowUpItems.GLTaxRateID,
                    TaxAmount = foreignPurchaseFollowUpItems.TaxAmount,
                    SubTotal = foreignPurchaseFollowUpItems.SubTotal,
                    AdditionalUnitCost = foreignPurchaseFollowUpItems.AdditionalUnitCost,
                    FinalUnitPrice = foreignPurchaseFollowUpItems.FinalUnitPrice,
                    Total = foreignPurchaseFollowUpItems.Total,
                    ItemSpecification = foreignPurchaseFollowUpItems.ItemSpecification,
                    UoM = foreignPurchaseFollowUpItems.UoM
                };

                db.ForeignPurchaseFollowUpItems.Attach(entity);
                db.ForeignPurchaseFollowUpItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseFollowUpItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
