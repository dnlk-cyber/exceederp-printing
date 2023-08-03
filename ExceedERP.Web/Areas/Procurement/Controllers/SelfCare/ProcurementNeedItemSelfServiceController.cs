using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare
{


    public class ProcurementNeedItemSelfServiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementPlanApprover, ProcurementRoles.ProcurementPlanUser)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementPlanApprover, ProcurementRoles.ProcurementPlanUser)]
        public ActionResult AnnualNeedAssesmentItems_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AnnualNeedAssesmentItem> annualneedassesmentitems = db.AnnualNeedAssesmentItems.Where(x => x.AnnualNeedAssesmentID == id);
            DataSourceResult result = annualneedassesmentitems.ToDataSourceResult(request, annualNeedAssesmentItem => new
            {

                AnnualNeedAssesmentItemID = annualNeedAssesmentItem.AnnualNeedAssesmentItemID,
                CatCode = annualNeedAssesmentItem.CatCode,
                CategoryName = annualNeedAssesmentItem.CategoryName,
                ItmCode = annualNeedAssesmentItem.ItmCode,
                ItemName = annualNeedAssesmentItem.ItemName,
                PartNo = annualNeedAssesmentItem.PartNo,
                Quantity = annualNeedAssesmentItem.Quantity,
                ItemSpecification = annualNeedAssesmentItem.ItemSpecification,
                UoM = annualNeedAssesmentItem.UoM,
                Price = annualNeedAssesmentItem.Price,
                ItemType = annualNeedAssesmentItem.ItemType,
                Type = annualNeedAssesmentItem.Type,
                Period1 = annualNeedAssesmentItem.Period1,
                Period2 = annualNeedAssesmentItem.Period2,
                Period3 = annualNeedAssesmentItem.Period3,
                Period4 = annualNeedAssesmentItem.Period4,
                Period5 = annualNeedAssesmentItem.Period5,
                Period6 = annualNeedAssesmentItem.Period6,
                Period7 = annualNeedAssesmentItem.Period7,
                Period8 = annualNeedAssesmentItem.Period8,
                Period9 = annualNeedAssesmentItem.Period9,
                Period10 = annualNeedAssesmentItem.Period10,
                Period11 = annualNeedAssesmentItem.Period11,
                Period12 = annualNeedAssesmentItem.Period12,
                Function = annualNeedAssesmentItem.Function,
                PerformanceRequirement = annualNeedAssesmentItem.PerformanceRequirement,
                PhysicalCharacteristics = annualNeedAssesmentItem.PhysicalCharacteristics,
                QuantityRequired = annualNeedAssesmentItem.QuantityRequired,
                MinimumQualityRequired = annualNeedAssesmentItem.MinimumQualityRequired,
                BudgetSource = annualNeedAssesmentItem.BudgetSource,
                ProcurementCategory = annualNeedAssesmentItem.ProcurementCategory,
                EstimatedBudget = annualNeedAssesmentItem.EstimatedBudget,
                EstimatedTimeTheItemIsNeeded = annualNeedAssesmentItem.EstimatedTimeTheItemIsNeeded,
                Unit = annualNeedAssesmentItem.UoM,
                OrgBranchName = annualNeedAssesmentItem.OrgBranchName,
                DateCreated = annualNeedAssesmentItem.DateCreated,
                LastModified = annualNeedAssesmentItem.LastModified,
                CreatedBy = annualNeedAssesmentItem.CreatedBy,
                ModifiedBy = annualNeedAssesmentItem.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentItems_Create(int id, [DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentItem annualNeedAssesmentItem)
        {

            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Annual Need Assesment First");
            }

            #region sum

            decimal total = annualNeedAssesmentItem.Period1 +
                            annualNeedAssesmentItem.Period2 +
                            annualNeedAssesmentItem.Period3 +
                            annualNeedAssesmentItem.Period4 +
                            annualNeedAssesmentItem.Period5 +
                            annualNeedAssesmentItem.Period6 +
                            annualNeedAssesmentItem.Period7 +
                            annualNeedAssesmentItem.Period8 +
                            annualNeedAssesmentItem.Period9 +
                            annualNeedAssesmentItem.Period10 +
                            annualNeedAssesmentItem.Period11 +
                            annualNeedAssesmentItem.Period12;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentItem
                {
                    AnnualNeedAssesmentID = id,
                    CatCode = annualNeedAssesmentItem.CatCode.Split('_')[0],
                    CategoryName = annualNeedAssesmentItem.CatCode.Split('_')[1],
                    ItmCode = annualNeedAssesmentItem.ItmCode.Split('_')[0],
                    ItemName = annualNeedAssesmentItem.ItmCode.Split('_')[1],
                    PartNo = annualNeedAssesmentItem.PartNo,
                    Quantity = total,
                    ItemSpecification = annualNeedAssesmentItem.ItemSpecification,
                    UoM = annualNeedAssesmentItem.UoM,
                    Price = annualNeedAssesmentItem.Price,
                    ItemType = annualNeedAssesmentItem.ItemType,
                    Type = annualNeedAssesmentItem.Type,
                    Period1 = annualNeedAssesmentItem.Period1,
                    Period2 = annualNeedAssesmentItem.Period2,
                    Period3 = annualNeedAssesmentItem.Period3,
                    Period4 = annualNeedAssesmentItem.Period4,
                    Period5 = annualNeedAssesmentItem.Period5,
                    Period6 = annualNeedAssesmentItem.Period6,
                    Period7 = annualNeedAssesmentItem.Period7,
                    Period8 = annualNeedAssesmentItem.Period8,
                    Period9 = annualNeedAssesmentItem.Period9,
                    Period10 = annualNeedAssesmentItem.Period10,
                    Period11 = annualNeedAssesmentItem.Period11,
                    Period12 = annualNeedAssesmentItem.Period12,
                    Function = annualNeedAssesmentItem.Function,
                    PerformanceRequirement = annualNeedAssesmentItem.PerformanceRequirement,
                    PhysicalCharacteristics = annualNeedAssesmentItem.PhysicalCharacteristics,
                    QuantityRequired = annualNeedAssesmentItem.QuantityRequired,
                    MinimumQualityRequired = annualNeedAssesmentItem.MinimumQualityRequired,
                    BudgetSource = annualNeedAssesmentItem.BudgetSource,
                    ProcurementCategory = annualNeedAssesmentItem.ProcurementCategory,
                    EstimatedBudget = annualNeedAssesmentItem.EstimatedBudget,
                    EstimatedTimeTheItemIsNeeded = annualNeedAssesmentItem.EstimatedTimeTheItemIsNeeded,
                    OrgBranchName = annualNeedAssesmentItem.OrgBranchName,
                    DateCreated = annualNeedAssesmentItem.DateCreated,
                    LastModified = annualNeedAssesmentItem.LastModified,
                    CreatedBy = annualNeedAssesmentItem.CreatedBy,
                    ModifiedBy = annualNeedAssesmentItem.ModifiedBy
                };

                db.AnnualNeedAssesmentItems.Add(entity);
                db.SaveChanges();
                annualNeedAssesmentItem.AnnualNeedAssesmentItemID = entity.AnnualNeedAssesmentItemID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement need item self care", "Success", User.Identity.Name, $"created need item {entity.ItemName} under record {entity.AnnualNeedAssesmentID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { annualNeedAssesmentItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentItems_Update([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentItem annualNeedAssesmentItem)
        {
            #region sum

            decimal total = annualNeedAssesmentItem.Period1 +
                            annualNeedAssesmentItem.Period2 +
                            annualNeedAssesmentItem.Period3 +
                            annualNeedAssesmentItem.Period4 +
                            annualNeedAssesmentItem.Period5 +
                            annualNeedAssesmentItem.Period6 +
                            annualNeedAssesmentItem.Period7 +
                            annualNeedAssesmentItem.Period8 +
                            annualNeedAssesmentItem.Period9 +
                            annualNeedAssesmentItem.Period10 +
                            annualNeedAssesmentItem.Period11 +
                            annualNeedAssesmentItem.Period12;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentItem
                {
                    AnnualNeedAssesmentItemID = annualNeedAssesmentItem.AnnualNeedAssesmentItemID,
                    CatCode = annualNeedAssesmentItem.CatCode.Split('_')[0],
                    CategoryName = annualNeedAssesmentItem.CatCode.Split('_')[1],
                    ItmCode = annualNeedAssesmentItem.ItmCode.Split('_')[0],
                    ItemName = annualNeedAssesmentItem.ItmCode.Split('_')[1],
                    PartNo = annualNeedAssesmentItem.PartNo,
                    Quantity = total,
                    ItemSpecification = annualNeedAssesmentItem.ItemSpecification,
                    UoM = annualNeedAssesmentItem.UoM,
                    Price = annualNeedAssesmentItem.Price,
                    ItemType = annualNeedAssesmentItem.ItemType,
                    Type = annualNeedAssesmentItem.Type,
                    Period1 = annualNeedAssesmentItem.Period1,
                    Period2 = annualNeedAssesmentItem.Period2,
                    Period3 = annualNeedAssesmentItem.Period3,
                    Period4 = annualNeedAssesmentItem.Period4,
                    Period5 = annualNeedAssesmentItem.Period5,
                    Period6 = annualNeedAssesmentItem.Period6,
                    Period7 = annualNeedAssesmentItem.Period7,
                    Period8 = annualNeedAssesmentItem.Period8,
                    Period9 = annualNeedAssesmentItem.Period9,
                    Period10 = annualNeedAssesmentItem.Period10,
                    Period11 = annualNeedAssesmentItem.Period11,
                    Period12 = annualNeedAssesmentItem.Period12,
                    Function = annualNeedAssesmentItem.Function,
                    PerformanceRequirement = annualNeedAssesmentItem.PerformanceRequirement,
                    PhysicalCharacteristics = annualNeedAssesmentItem.PhysicalCharacteristics,
                    QuantityRequired = annualNeedAssesmentItem.QuantityRequired,
                    MinimumQualityRequired = annualNeedAssesmentItem.MinimumQualityRequired,
                    BudgetSource = annualNeedAssesmentItem.BudgetSource,
                    ProcurementCategory = annualNeedAssesmentItem.ProcurementCategory,
                    EstimatedBudget = annualNeedAssesmentItem.EstimatedBudget,
                    EstimatedTimeTheItemIsNeeded = annualNeedAssesmentItem.EstimatedTimeTheItemIsNeeded,
                    OrgBranchName = annualNeedAssesmentItem.OrgBranchName,
                    DateCreated = annualNeedAssesmentItem.DateCreated,
                    LastModified = annualNeedAssesmentItem.LastModified,
                    CreatedBy = annualNeedAssesmentItem.CreatedBy,
                    ModifiedBy = annualNeedAssesmentItem.ModifiedBy
                };

                db.AnnualNeedAssesmentItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement need item self care", "Success", User.Identity.Name, $"updated need item {entity.ItemName} under record {entity.AnnualNeedAssesmentID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { annualNeedAssesmentItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentItems_Destroy([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentItem annualNeedAssesmentItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentItem
                {
                    AnnualNeedAssesmentItemID = annualNeedAssesmentItem.AnnualNeedAssesmentItemID,
                    CatCode = annualNeedAssesmentItem.CatCode.Split('_')[0],
                    CategoryName = annualNeedAssesmentItem.CatCode.Split('_')[1],
                    ItmCode = annualNeedAssesmentItem.ItmCode.Split('_')[0],
                    ItemName = annualNeedAssesmentItem.ItmCode.Split('_')[1],
                    PartNo = annualNeedAssesmentItem.PartNo,
                    Quantity = annualNeedAssesmentItem.Quantity,
                    ItemSpecification = annualNeedAssesmentItem.ItemSpecification,
                    UoM = annualNeedAssesmentItem.UoM,
                    Price = annualNeedAssesmentItem.Price,
                    ItemType = annualNeedAssesmentItem.ItemType,
                    Type = annualNeedAssesmentItem.Type,
                    Period1 = annualNeedAssesmentItem.Period1,
                    Period2 = annualNeedAssesmentItem.Period2,
                    Period3 = annualNeedAssesmentItem.Period3,
                    Period4 = annualNeedAssesmentItem.Period4,
                    Period5 = annualNeedAssesmentItem.Period5,
                    Period6 = annualNeedAssesmentItem.Period6,
                    Period7 = annualNeedAssesmentItem.Period7,
                    Period8 = annualNeedAssesmentItem.Period8,
                    Period9 = annualNeedAssesmentItem.Period9,
                    Period10 = annualNeedAssesmentItem.Period10,
                    Period11 = annualNeedAssesmentItem.Period11,
                    Period12 = annualNeedAssesmentItem.Period12,
                    Function = annualNeedAssesmentItem.Function,
                    PerformanceRequirement = annualNeedAssesmentItem.PerformanceRequirement,
                    PhysicalCharacteristics = annualNeedAssesmentItem.PhysicalCharacteristics,
                    QuantityRequired = annualNeedAssesmentItem.QuantityRequired,
                    MinimumQualityRequired = annualNeedAssesmentItem.MinimumQualityRequired,
                    BudgetSource = annualNeedAssesmentItem.BudgetSource,
                    ProcurementCategory = annualNeedAssesmentItem.ProcurementCategory,
                    EstimatedBudget = annualNeedAssesmentItem.EstimatedBudget,
                    EstimatedTimeTheItemIsNeeded = annualNeedAssesmentItem.EstimatedTimeTheItemIsNeeded,
                    OrgBranchName = annualNeedAssesmentItem.OrgBranchName,
                    DateCreated = annualNeedAssesmentItem.DateCreated,
                    LastModified = annualNeedAssesmentItem.LastModified,
                    CreatedBy = annualNeedAssesmentItem.CreatedBy,
                    ModifiedBy = annualNeedAssesmentItem.ModifiedBy
                };

                db.AnnualNeedAssesmentItems.Attach(entity);
                db.AnnualNeedAssesmentItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { annualNeedAssesmentItem }.ToDataSourceResult(request, ModelState));
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
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
