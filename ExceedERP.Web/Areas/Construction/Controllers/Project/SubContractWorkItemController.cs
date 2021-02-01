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
    public class SubContractWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       public  static int Category { get; set; }
       public static decimal OverHeadCost { get; set; }
       public static decimal ProfitMargin { get; set; }
       public static string CurrentUSer { get; set; }
       ConstructionInputs inputs ;
       
        public ActionResult Index(string name)
        {

            SetDefaultValue();
            return View();
        }

        
   public ActionResult SubContractWorkItem_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SubContractWorkItem > SubContractWorkItems = db.SubContractWorkItem.AsQueryable();
            SetDefaultValue();
            //if (ProjectId > 0)
            //{
            SubContractWorkItems =
                    db.SubContractWorkItem;
            //db.SubContractWorkItem.Where(x => x.ProjectInformationId == WorkItemPerProjectController.ProjectId);
            DataSourceResult result = SubContractWorkItems.ToDataSourceResult(request,
                    SubContractWorkItem => new
                    {
                        SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                        WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = SubContractWorkItem.WorkItemName,
                        ItemCode = SubContractWorkItem.ItemCode,
                        Quntity = SubContractWorkItem.Quntity,
                        SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                        SelectedCost = SubContractWorkItem.SelectedCost,
                        TotalCost = SubContractWorkItem.TotalCost,
                        ProjectInformationId = SubContractWorkItem.ProjectInformationId,
                       
                       OrgBranchName = SubContractWorkItem.OrgBranchName,
                        DateCreated = SubContractWorkItem.DateCreated,
                        LastModified = SubContractWorkItem.LastModified,
                        CreatedBy = SubContractWorkItem.CreatedBy,
                        ModifiedBy = SubContractWorkItem.ModifiedBy ,
                       
                    });
               
                return Json(result);
            //}
            
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Create([DataSourceRequest]DataSourceRequest request, SubContractWorkItem SubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                //if (ProjectEstimationWorkItemController.ProjectId == 0)
                //{
                   
                //    return new JavaScriptResult() { Script = "alert('" + "Please select the project first!!" + "');" };
                //}
                var entity = new SubContractWorkItem
                {
                    SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                    WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = SubContractWorkItem.WorkItemName,
                    ItemCode = SubContractWorkItem.ItemCode,
                    Quntity = SubContractWorkItem.Quntity,
                    SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                    SelectedCost = SubContractWorkItem.SelectedCost,
                    TotalCost = SubContractWorkItem.TotalCost,
                    ProjectInformationId = ProjectEstimationCopyController.ProjectId,

                    OrgBranchName = SubContractWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = SubContractWorkItem.LastModified,
                    CreatedBy =User.Identity.Name,
                    ModifiedBy = SubContractWorkItem.ModifiedBy,
                };
                entity.SelectedCost = (entity.SubContractCostOne+entity.SubContractCostTwo+entity.SubContractCostThree)/3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCost / 100)) +
                                          (entity.SelectedCost * (ProfitMargin / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;
               
                db.SubContractWorkItem.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                SubContractWorkItem.SubContractWorkItemId = entity.SubContractWorkItemId;
                
            }

            return Json(new[] { SubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Update([DataSourceRequest]DataSourceRequest request, SubContractWorkItem SubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubContractWorkItem
                {
                    SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                    WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = SubContractWorkItem.WorkItemName,
                    ItemCode = SubContractWorkItem.ItemCode,
                    Quntity = SubContractWorkItem.Quntity,
                    SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                    SelectedCost = SubContractWorkItem.SelectedCost,
                    TotalCost = SubContractWorkItem.TotalCost,
                    ProjectInformationId = SubContractWorkItem.ProjectInformationId,

                    OrgBranchName = SubContractWorkItem.OrgBranchName,
                    DateCreated = SubContractWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = SubContractWorkItem.CreatedBy ,
                    ModifiedBy = User.Identity.Name 
                };

                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCost / 100)) +
                                          (entity.SelectedCost * (ProfitMargin / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;
                db = new ExceedDbContext();
                db.SubContractWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
               
            }

            return Json(new[] { SubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, SubContractWorkItem SubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubContractWorkItem
                {
                    SubContractWorkItemId = SubContractWorkItem.SubContractWorkItemId,
                    WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = SubContractWorkItem.WorkItemName,
                    ItemCode = SubContractWorkItem.ItemCode,
                    Quntity = SubContractWorkItem.Quntity,
                    SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                    SelectedCost = SubContractWorkItem.SelectedCost,
                    TotalCost = SubContractWorkItem.TotalCost,
                    ProjectInformationId = SubContractWorkItem.ProjectInformationId,
                    OrgBranchName = SubContractWorkItem.OrgBranchName,
                    DateCreated = SubContractWorkItem.DateCreated,
                    LastModified = SubContractWorkItem.LastModified,
                    CreatedBy = SubContractWorkItem.CreatedBy,
                    ModifiedBy = SubContractWorkItem.ModifiedBy 
                };

                db.SubContractWorkItem.Attach(entity);
                db.SubContractWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { SubContractWorkItem }.ToDataSourceResult(request, ModelState));
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
