using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.NeedAndPlan
{


    public class ProcurementNeedItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementUserDepartmentApprover, ProcurementConstants.ProcurementPlanApprover, ProcurementConstants.ProcurementPlanUser)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementUserDepartmentApprover, ProcurementConstants.ProcurementPlanApprover, ProcurementConstants.ProcurementPlanUser)]
        public ActionResult AnnualNeedAssesmentItems_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AnnualNeedAssesmentItem> annualneedassesmentitems = db.AnnualNeedAssesmentItems.Where(x => x.AnnualNeedAssesmentID == id);
            DataSourceResult result = annualneedassesmentitems.ToDataSourceResult(request, needAssesmentItem => new
            {
                AnnualNeedAssesmentID = needAssesmentItem.AnnualNeedAssesmentID,
                AnnualNeedAssesmentItemID = needAssesmentItem.AnnualNeedAssesmentItemID,
                CatCode = needAssesmentItem.CatCode,
                CategoryName = needAssesmentItem.CategoryName,
                ItmCode = needAssesmentItem.ItmCode,
                ItemName = needAssesmentItem.ItemName,
                Name = needAssesmentItem.Name,
                ModelNo = needAssesmentItem.ModelNo,
                PartNo = needAssesmentItem.PartNo,
                Quantity = needAssesmentItem.Quantity,
                ItemSpecification = needAssesmentItem.ItemSpecification,
                UoM = needAssesmentItem.UoM,
                Price = needAssesmentItem.Price,
                ItemType = needAssesmentItem.ItemType,
                Type = needAssesmentItem.Type,
                Period1 = needAssesmentItem.Period1,
                Period2 = needAssesmentItem.Period2,
                Period3 = needAssesmentItem.Period3,
                Period4 = needAssesmentItem.Period4,
                Period5 = needAssesmentItem.Period5,
                Period6 = needAssesmentItem.Period6,
                Period7 = needAssesmentItem.Period7,
                Period8 = needAssesmentItem.Period8,
                Period9 = needAssesmentItem.Period9,
                Period10 = needAssesmentItem.Period10,
                Period11 = needAssesmentItem.Period11,
                Period12 = needAssesmentItem.Period12,
                Function = needAssesmentItem.Function,
                PerformanceRequirement = needAssesmentItem.PerformanceRequirement,
                PhysicalCharacteristics = needAssesmentItem.PhysicalCharacteristics,
                QuantityRequired = needAssesmentItem.QuantityRequired,
                MinimumQualityRequired = needAssesmentItem.MinimumQualityRequired,
                BudgetSource = needAssesmentItem.BudgetSource,
                ProcurementCategory = needAssesmentItem.ProcurementCategory,
                EstimatedBudget = needAssesmentItem.EstimatedBudget,
                EstimatedTimeTheItemIsNeeded = needAssesmentItem.EstimatedTimeTheItemIsNeeded,
                Unit = needAssesmentItem.UoM,
                OrgBranchName = needAssesmentItem.OrgBranchName,
                DateCreated = needAssesmentItem.DateCreated,
                LastModified = needAssesmentItem.LastModified,
                CreatedBy = needAssesmentItem.CreatedBy,
                ModifiedBy = needAssesmentItem.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentItems_Create(int id, [DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentItem annualNeedAssesmentItem)
        {

            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Annual Need Assesment First");
            }

            if (!string.IsNullOrEmpty(annualNeedAssesmentItem.Type))
            {
                switch (annualNeedAssesmentItem.Type)
                {
                    case "Stock":
                        annualNeedAssesmentItem.ItemType = PurchaseItemType.Stock;
                        break;

                    case "FixedAsset":
                        annualNeedAssesmentItem.ItemType = PurchaseItemType.FixedAsset;
                        break;

                    case "Service":
                        annualNeedAssesmentItem.ItemType = PurchaseItemType.Service;
                        break;
                }
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

            if (annualNeedAssesmentItem.ItemType ==PurchaseItemType.Stock)
            {
                var itemCode = annualNeedAssesmentItem.ItmCode.Split('_')[0];
                var item = db.InventoryItems.FirstOrDefault(x => x.ItemCode == itemCode);
                annualNeedAssesmentItem.UoM = item?.UoMName;
            }
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentItem
                {
                    AnnualNeedAssesmentID = id,
                    CatCode = annualNeedAssesmentItem.CatCode.Split('_')[0],
                    CategoryName = annualNeedAssesmentItem.CatCode.Split('_')[1],
                    ItmCode = annualNeedAssesmentItem.ItmCode.Split('_')[0],
                    ItemName = annualNeedAssesmentItem.ItmCode.Split('_')[1],
                    Name = annualNeedAssesmentItem.Name,
                    ModelNo = annualNeedAssesmentItem.ModelNo,
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
                UserHelper.OperationLog(Request.UserHostAddress, "procurement need management item", "Success", User.Identity.Name, $"created need item {entity.ItemName} under record {entity.AnnualNeedAssesmentID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { annualNeedAssesmentItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementPlanUser)]
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
            if (annualNeedAssesmentItem.ItemType == PurchaseItemType.Stock)
            {
                var itemCode = annualNeedAssesmentItem.ItmCode.Split('_')[0];
                var item = db.InventoryItems.FirstOrDefault(x => x.ItemCode == itemCode);
                annualNeedAssesmentItem.UoM = item?.UoMName;
            }
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentItem
                {
                    AnnualNeedAssesmentID = annualNeedAssesmentItem.AnnualNeedAssesmentID,
                    AnnualNeedAssesmentItemID = annualNeedAssesmentItem.AnnualNeedAssesmentItemID,
                    CatCode = annualNeedAssesmentItem.CatCode.Split('_')[0],
                    CategoryName = annualNeedAssesmentItem.CatCode.Split('_')[1],
                    ItmCode = annualNeedAssesmentItem.ItmCode.Split('_')[0],
                    ItemName = annualNeedAssesmentItem.ItmCode.Split('_')[1],
                    Name = annualNeedAssesmentItem.Name,
                    ModelNo = annualNeedAssesmentItem.ModelNo,
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
                UserHelper.OperationLog(Request.UserHostAddress, "procurement need management item", "Success", User.Identity.Name, $"updated need item {entity.ItemName} under record {entity.AnnualNeedAssesmentID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { annualNeedAssesmentItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementPlanUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentItems_Destroy([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentItem annualNeedAssesmentItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentItem
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
