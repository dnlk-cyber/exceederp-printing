using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;
using System;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class LaborCostbatchController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DirectLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int estimationFormId)
        {
            IQueryable<DirectLaborCost> directlaborcosts = db.DirectLaborCosts.Where(x => x.FurnitureEstimationId == estimationFormId);
            DataSourceResult result = directlaborcosts.ToDataSourceResult(request, directLaborCost => new DirectLaborCost
            {
                DirectLaborCostId = directLaborCost.DirectLaborCostId,
                
                FurnitureEstimationId = directLaborCost.FurnitureEstimationId,
                WorkingTime = directLaborCost.WorkingTime,
                CostPerHour = directLaborCost.CostPerHour,
                TotalCost = directLaborCost.TotalCost,
                TaskCategoryId = directLaborCost.TaskCategoryId,
                TaskType = directLaborCost.TaskType,
                DateCreated = directLaborCost.DateCreated,
                LastModified = directLaborCost.LastModified,
                CreatedBy = directLaborCost.CreatedBy,
                ModifiedBy = directLaborCost.ModifiedBy,
                
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Create([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DirectLaborCost> directlaborcosts, int estimationFormId)
        {
            var entities = new List<DirectLaborCost>();
            if (directlaborcosts != null && ModelState.IsValid)
            {
                foreach(var directLaborCost in directlaborcosts)
                {
                    var taskCategory = db.ManifucturingTaskCategories.Find(directLaborCost.TaskCategoryId);
                    var entity = new DirectLaborCost
                    {

                            FurnitureEstimationId = estimationFormId, // fk
                            WorkingTime = directLaborCost.WorkingTime,
                            CostPerHour = taskCategory.HourRate, // Hour rate from TaskCategory
                            TotalCost = taskCategory.HourRate * directLaborCost.WorkingTime, // Total
                            TaskCategoryId = directLaborCost.TaskCategoryId,
                            TaskType = directLaborCost.TaskType,
                            DateCreated = DateTime.Today,                        
                            CreatedBy = User.Identity.Name,                         
                    };
                    
                    db.DirectLaborCosts.Add(entity);
                    entities.Add(entity);
                }
                db.SaveChanges();
            }

            return Json(entities.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Update([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DirectLaborCost> directlaborcosts)
        {
            var entities = new List<DirectLaborCost>();
            if (directlaborcosts != null && ModelState.IsValid)
            {
                foreach(var directLaborCost in directlaborcosts)
                {
                    var taskCategory = db.ManifucturingTaskCategories.Find(directLaborCost.TaskCategoryId);
                    var entity = new DirectLaborCost
                    {
                        FurnitureEstimationId = directLaborCost.FurnitureEstimationId, // fk
                        WorkingTime = directLaborCost.WorkingTime,
                        CostPerHour = taskCategory.HourRate, // Hour rate from TaskCategory
                        TotalCost = taskCategory.HourRate * directLaborCost.WorkingTime, // Total
                        TaskCategoryId = directLaborCost.TaskCategoryId,
                        TaskType = directLaborCost.TaskType,
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                    };

                    entities.Add(entity);
                    db.DirectLaborCosts.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;                        
                }
                db.SaveChanges();
            }

            return Json(entities.ToDataSourceResult(request, ModelState));
        } 

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Destroy([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DirectLaborCost> directlaborcosts)
        {
            var entities = new List<DirectLaborCost>();
            if (ModelState.IsValid)
            {
                foreach(var directLaborCost in directlaborcosts)
                {
                    var entity = new DirectLaborCost
                    {
                        DirectLaborCostId = directLaborCost.DirectLaborCostId,
                       
                        FurnitureEstimationId = directLaborCost.FurnitureEstimationId,
                        WorkingTime = directLaborCost.WorkingTime,
                        CostPerHour = directLaborCost.CostPerHour,
                        TotalCost = directLaborCost.TotalCost,
                        TaskCategoryId = directLaborCost.TaskCategoryId,
                        TaskType = directLaborCost.TaskType,
                        DateCreated = directLaborCost.DateCreated,
                        LastModified = directLaborCost.LastModified,
                        CreatedBy = directLaborCost.CreatedBy,
                        ModifiedBy = directLaborCost.ModifiedBy
                    };

                    entities.Add(entity);
                    db.DirectLaborCosts.Attach(entity);
                    db.DirectLaborCosts.Remove(entity);
                }
                db.SaveChanges();
            }

            return Json(entities.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
