using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Construction.Controllers.Estimation;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class PieceRateWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int Category { get; set; }
        public static decimal OverHeadCost { get; set; }
        public static decimal ProfitMargin { get; set; }
        public static string CurrentUSer { get; set; }
        ConstructionInputs inputs;

        public ActionResult Index(string name)
        {

            SetDefaultValue();
            return View();
        }

        public JsonResult GetWorkItemsItems(int WorkCategoryId)
        {
            var items = db.AreaWorkItemes.ToList().Where(x => x.WorkCategoryId == WorkCategoryId).ToList();

            ViewData["WorkItems"] = items.Select(p => new { ItemName = p.WorkItemName, AreaWorkItemId = p.AreaWorkItemId });
            return Json(items.Select(p => new { ItemName = p.WorkItemName, AreaWorkItemId = p.AreaWorkItemId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategory()
        {
            var items = db.ConstructionItemCategories.ToList();
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AreaWorkItem_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AreaWorkItem> AreaWorkItems = db.AreaWorkItemes.AsQueryable();
            SetDefaultValue();
            //if (ProjectId > 0)
            //{
            AreaWorkItems =
                            db.AreaWorkItemes;
                                    //db.AreaWorkItemes.Where(x => x.ProjectInformationId == WorkItemPerProjectController.ProjectId);
            DataSourceResult result = AreaWorkItems.ToDataSourceResult(request,
                    AreaWorkItem => new
                    {
                        AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                        WorkCategoryId = AreaWorkItem.WorkCategoryId,
                        Price = AreaWorkItem.Price,
                        WorkItemName = AreaWorkItem.WorkItemName,
                        ItemCode = AreaWorkItem.ItemCode,
                        DirectCost = AreaWorkItem.DirectCost,
                        Width = AreaWorkItem.Width,
                        Height = AreaWorkItem.Height,
                        Area = AreaWorkItem.Area,
                        CylinderLock = AreaWorkItem.CylinderLock,
                        OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                        ProjectInformationId = AreaWorkItem.ProjectInformationId,
                        MaterialCost = AreaWorkItem.MaterialCost,
                        OrgBranchName = AreaWorkItem.OrgBranchName,
                        DateCreated = AreaWorkItem.DateCreated,
                        LastModified = AreaWorkItem.LastModified,
                        CreatedBy = AreaWorkItem.CreatedBy,
                        ModifiedBy = AreaWorkItem.ModifiedBy,
                        TransportCost = AreaWorkItem.TransportCost,
                    });

            return Json(result);
            //}

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Create([DataSourceRequest]DataSourceRequest request, AreaWorkItem AreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                //if (ProjectEstimationWorkItemController.ProjectId == 0)
                //{

                //    return new JavaScriptResult() { Script = "alert('" + "Please select the project first!!" + "');" };
                //}
                var entity = new AreaWorkItem
                {
                    AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                    Price = AreaWorkItem.Price,
                    WorkCategoryId = AreaWorkItem.WorkCategoryId,
                    WorkItemName = AreaWorkItem.WorkItemName,
                    ItemCode = AreaWorkItem.ItemCode,
                    DirectCost = AreaWorkItem.DirectCost,
                    Width = AreaWorkItem.Width,
                    Height = AreaWorkItem.Height,
                    Area = AreaWorkItem.Area,
                    CylinderLock = AreaWorkItem.CylinderLock,
                    OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                    TransportCost = AreaWorkItem.TransportCost,
                    ProjectInformationId = ProjectEstimationCopyController.ProjectId,
                    MaterialCost = AreaWorkItem.MaterialCost,

                    OrgBranchName = AreaWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = AreaWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = AreaWorkItem.ModifiedBy
                };
                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCost / 100)) +
                                         (entity.DirectCost * (ProfitMargin / 100));
                db.AreaWorkItemes.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                AreaWorkItem.AreaWorkItemId = entity.AreaWorkItemId;

            }

            return Json(new[] { AreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Update([DataSourceRequest]DataSourceRequest request, AreaWorkItem AreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AreaWorkItem
                {
                    AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                    WorkCategoryId = AreaWorkItem.WorkCategoryId,
                    Price = AreaWorkItem.Price,
                    WorkItemName = AreaWorkItem.WorkItemName,
                    ItemCode = AreaWorkItem.ItemCode,
                    DirectCost = AreaWorkItem.DirectCost,
                    Width = AreaWorkItem.Width,
                    Height = AreaWorkItem.Height,
                    Area = AreaWorkItem.Area,
                    TransportCost = AreaWorkItem.TransportCost,
                    CylinderLock = AreaWorkItem.CylinderLock,
                    OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,

                    ProjectInformationId = AreaWorkItem.ProjectInformationId,
                    MaterialCost = AreaWorkItem.MaterialCost,

                    OrgBranchName = AreaWorkItem.OrgBranchName,
                    DateCreated = AreaWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = AreaWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCost / 100)) +
                                            (entity.DirectCost * (ProfitMargin / 100));
                db = new ExceedDbContext();
                db.AreaWorkItemes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { AreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, AreaWorkItem AreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AreaWorkItem
                {
                    AreaWorkItemId = AreaWorkItem.AreaWorkItemId,
                    WorkCategoryId = AreaWorkItem.WorkCategoryId,
                    WorkItemName = AreaWorkItem.WorkItemName,
                    ItemCode = AreaWorkItem.ItemCode,
                    DirectCost = AreaWorkItem.DirectCost,
                    Width = AreaWorkItem.Width,
                    Height = AreaWorkItem.Height,
                    Price = AreaWorkItem.Price,
                    Area = AreaWorkItem.Area,
                    CylinderLock = AreaWorkItem.CylinderLock,
                    OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                    TransportCost = AreaWorkItem.TransportCost,
                    ProjectInformationId = AreaWorkItem.ProjectInformationId,
                    MaterialCost = AreaWorkItem.MaterialCost,
                    OrgBranchName = AreaWorkItem.OrgBranchName,
                    DateCreated = AreaWorkItem.DateCreated,
                    LastModified = AreaWorkItem.LastModified,
                    CreatedBy = AreaWorkItem.CreatedBy,
                    ModifiedBy = AreaWorkItem.ModifiedBy
                };

                db.AreaWorkItemes.Attach(entity);
                db.AreaWorkItemes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { AreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        private void SetDefaultValue()
        {
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                var ovH = configartion.Where(x => x.Attribute == "OVER_HEAD_COST_RATE").FirstOrDefault();
                if (ovH != null)
                {
                    OverHeadCost = Convert.ToDecimal(ovH.CurrentValue);
                }
                var profMa = configartion.Where(x => x.Attribute == "PROFIT_COST_RATE").FirstOrDefault();
                if (profMa != null)
                {
                    ProfitMargin = Convert.ToDecimal(profMa.CurrentValue);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
