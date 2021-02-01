using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.NeedAndPlan
{
   

    public class ProcurementPlanItemController : Controller   
    {
        private ExceedDbContext db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser, ProcurementConstants.ProcurementPlanApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser, ProcurementConstants.ProcurementPlanApprover)]
        public ActionResult PlanItems_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProcurementPlanItem> planitems = db.ProcurementPlanItems.Where(x => x.ProcurementPlanId == id);
            DataSourceResult result = planitems.ToDataSourceResult(request, planItem => new
            {
                ProcurementPlanItemId = planItem.ProcurementPlanItemId,
                ProcurementPlanId = planItem.ProcurementPlanId,
                CatCode = planItem.CatCode,
                CategoryName = planItem.CategoryName,
                ItmCode = planItem.ItmCode,
                ItemName = planItem.ItemName,
                Name = planItem.Name,
                ModelNo = planItem.ModelNo,
                PartNo = planItem.PartNo,
                Quantity = planItem.Quantity,
                UnitPrice = planItem.UnitPrice,
                TotalPrice = planItem.TotalPrice,
                ItemSpecification = planItem.ItemSpecification,
                UoM = planItem.UoM,
                ItemType = planItem.ItemType,
                Type = planItem.Type,
                Period1 = planItem.Period1,
                Period2 = planItem.Period2,
                Period3 = planItem.Period3,
                Period4 = planItem.Period4,
                Period5 = planItem.Period5,
                Period6 = planItem.Period6,
                Period7 = planItem.Period7,
                Period8 = planItem.Period8,
                Period9 = planItem.Period9,
                Period10 = planItem.Period10,
                Period11 = planItem.Period11,
                Period12 = planItem.Period12,
                BudgetSource = planItem.BudgetSource,
                
               ProcurementCategory = planItem.ProcurementCategory,
               
                ProcurementMethod = planItem.ProcurementMethod,
                TenderFloatingTime = planItem.TenderFloatingTime,
                PurchaseDeliveryTime = planItem.PurchaseDeliveryTime,
                Remark = planItem.Remark,

                OrgBranchName = planItem.OrgBranchName,
                DateCreated = planItem.DateCreated,
                LastModified = planItem.LastModified,
                CreatedBy = planItem.CreatedBy,
                ModifiedBy = planItem.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser)]
        public ActionResult PlanItems_Create(int id, ProcurementPlanItem planItem)
        {

            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Plan First");
            }

            var exist = db.ProcurementPlanItems.Where(x => x.ProcurementPlanId == id)
                .Where(x => x.ItmCode == planItem.ItmCode)
                .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + planItem.ItmCode + " already exists");
            }
            #region sum

            decimal total = planItem.Period1 +
                            planItem.Period2 +
                            planItem.Period3 +
                            planItem.Period4 +
                            planItem.Period5 +
                            planItem.Period6 +
                            planItem.Period7 +
                            planItem.Period8 +
                            planItem.Period9 +
                            planItem.Period10 +
                            planItem.Period11 +
                            planItem.Period12;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlanItem
                {
                    ProcurementPlanId = id,
                    CatCode = planItem.CatCode,
                    CategoryName = planItem.CategoryName,
                    ItmCode = planItem.ItmCode,
                    ItemName = planItem.ItemName,
                    Name = planItem.Name,
                    ModelNo = planItem.ModelNo,
                    PartNo = planItem.PartNo,
                    Quantity = total,
                    ItemSpecification = planItem.ItemSpecification,
                    UoM = planItem.UoM,
                    ItemType = planItem.ItemType,
                    Type = planItem.Type,
                    UnitPrice = planItem.UnitPrice,
                    TotalPrice = planItem.UnitPrice* total,
                    Period1 = planItem.Period1,
                    Period2 = planItem.Period2,
                    Period3 = planItem.Period3,
                    Period4 = planItem.Period4,
                    Period5 = planItem.Period5,
                    Period6 = planItem.Period6,
                    Period7 = planItem.Period7,
                    Period8 = planItem.Period8,
                    Period9 = planItem.Period9,
                    Period10 = planItem.Period10,
                    Period11 = planItem.Period11,
                    Period12 = planItem.Period12,
                    BudgetSource = planItem.BudgetSource,
                    ProcurementCategory = planItem.ProcurementCategory,
                    ProcurementMethod = planItem.ProcurementMethod,
                    TenderFloatingTime = planItem.TenderFloatingTime,
                    PurchaseDeliveryTime = planItem.PurchaseDeliveryTime,
                    Remark = planItem.Remark,
                    OrgBranchName = planItem.OrgBranchName,
                    DateCreated = planItem.DateCreated,
                    LastModified = planItem.LastModified,
                    CreatedBy = planItem.CreatedBy,
                    ModifiedBy = planItem.ModifiedBy
                };


                db.ProcurementPlanItems.Add(entity);
                db.SaveChanges();
                planItem.ProcurementPlanItemId = entity.ProcurementPlanItemId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement plan  item", "Success", User.Identity.Name, $"created plan item {entity.ItemName} under record {entity.ProcurementPlanId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { planItem }.ToDataSourceResult(new DataSourceRequest(), ModelState));
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanItems_Update([DataSourceRequest]DataSourceRequest request, ProcurementPlanItem planItem)
        {
            var exist = db.ProcurementPlanItems.Where(x => x.ProcurementPlanId == planItem.ProcurementPlanId)
                   .Where(x => x.ProcurementPlanItemId != planItem.ProcurementPlanItemId)
             .Where(x => x.ItmCode == planItem.ItmCode)
             .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + planItem.ItmCode + " already exists");
            }

            #region sum

            decimal total = planItem.Period1 +
                            planItem.Period2 +
                            planItem.Period3 +
                            planItem.Period4 +
                            planItem.Period5 +
                            planItem.Period6 +
                            planItem.Period7 +
                            planItem.Period8 +
                            planItem.Period9 +
                            planItem.Period10 +
                            planItem.Period11 +
                            planItem.Period12;

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlanItem
                {
                    ProcurementPlanId = planItem.ProcurementPlanId,
                    ProcurementPlanItemId = planItem.ProcurementPlanItemId,
                    CatCode = planItem.CatCode,
                    CategoryName = planItem.CategoryName,
                    ItmCode = planItem.ItmCode,
                    ItemName = planItem.ItemName,
                    Name = planItem.Name,
                    ModelNo = planItem.ModelNo,
                    PartNo = planItem.PartNo,
                    Quantity = total,
                    ItemSpecification = planItem.ItemSpecification,
                    UoM = planItem.UoM,
                    ItemType = planItem.ItemType,
                    Type = planItem.Type,
                    UnitPrice = planItem.UnitPrice,
                    TotalPrice = planItem.UnitPrice * total,
                    Period1 = planItem.Period1,
                    Period2 = planItem.Period2,
                    Period3 = planItem.Period3,
                    Period4 = planItem.Period4,
                    Period5 = planItem.Period5,
                    Period6 = planItem.Period6,
                    Period7 = planItem.Period7,
                    Period8 = planItem.Period8,
                    Period9 = planItem.Period9,
                    Period10 = planItem.Period10,
                    Period11 = planItem.Period11,
                    Period12 = planItem.Period12,
                    BudgetSource = planItem.BudgetSource,
                    ProcurementCategory = planItem.ProcurementCategory,
                    ProcurementMethod = planItem.ProcurementMethod,
                    TenderFloatingTime = planItem.TenderFloatingTime,
                    PurchaseDeliveryTime = planItem.PurchaseDeliveryTime,
                    Remark = planItem.Remark,
                    OrgBranchName = planItem.OrgBranchName,
                    DateCreated = planItem.DateCreated,
                    LastModified = planItem.LastModified,
                    CreatedBy = planItem.CreatedBy,
                    ModifiedBy = planItem.ModifiedBy
                };

                db.ProcurementPlanItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement plan  item", "Success", User.Identity.Name, $"updated plan item {entity.ItemName} under record {entity.ProcurementPlanId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { planItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanItems_Destroy([DataSourceRequest]DataSourceRequest request, ProcurementPlanItem planItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlanItem
                {
                    ProcurementPlanId = planItem.ProcurementPlanId,
                    ProcurementPlanItemId = planItem.ProcurementPlanItemId,
                    CatCode = planItem.CatCode,
                    CategoryName = planItem.CategoryName,
                    ItmCode = planItem.ItmCode,
                    ItemName = planItem.ItemName,
                    PartNo = planItem.PartNo,
                    Quantity = planItem.Quantity,
                    ItemSpecification = planItem.ItemSpecification,
                    UoM = planItem.UoM,
                    ItemType = planItem.ItemType,
                    Type = planItem.Type,
                    UnitPrice = planItem.UnitPrice,
                    Period1 = planItem.Period1,
                    Period2 = planItem.Period2,
                    Period3 = planItem.Period3,
                    Period4 = planItem.Period4,
                    Period5 = planItem.Period5,
                    Period6 = planItem.Period6,
                    Period7 = planItem.Period7,
                    Period8 = planItem.Period8,
                    Period9 = planItem.Period9,
                    Period10 = planItem.Period10,
                    Period11 = planItem.Period11,
                    Period12 = planItem.Period12,
                    BudgetSource = planItem.BudgetSource,
                    ProcurementCategory = planItem.ProcurementCategory,
                    ProcurementMethod = planItem.ProcurementMethod,
                    TenderFloatingTime = planItem.TenderFloatingTime,
                    PurchaseDeliveryTime = planItem.PurchaseDeliveryTime,
                    Remark = planItem.Remark,
                    OrgBranchName = planItem.OrgBranchName,
                    DateCreated = planItem.DateCreated,
                    LastModified = planItem.LastModified,
                    CreatedBy = planItem.CreatedBy,
                    ModifiedBy = planItem.ModifiedBy
                };

                db.ProcurementPlanItems.Attach(entity);
                db.ProcurementPlanItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { planItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementPlanUser)]
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
