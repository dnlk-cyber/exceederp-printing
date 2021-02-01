using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class ScheduleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/Schedule
        public ActionResult Index()
        {
            return View();
        }

        public virtual JsonResult TasksFromEstimation_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<ProjectEstimationWorkItem> workItems = db.ProjectEstimationWorkItem;
            DataSourceResult result = workItems.ToDataSourceResult(request, wi => new TaskFromEstimation
            {
                TaskFromEstimationId = wi.ProjectEstimationWorkItemId,
                WorkCategoryId = wi.WorkCategoryId,
                Title = wi.ItemName,
                Category = GetCategory(wi.WorkCategoryId).Name,
                Start = DateTime.Now,
                End = DateTime.Now,
                EstimatedTotalCost = wi.LabourHourlyOutPut > 0? (wi.TotalUnitCost + ((wi.OverHeadCost * wi.TotalUnitCost) / 100) + ((wi.ProfitCost * wi.TotalUnitCost) / 100)) / wi.LabourHourlyOutPut:
                                        wi.TotalUnitCost + ((wi.OverHeadCost * wi.TotalUnitCost) / 100) + ((wi.ProfitCost * wi.TotalUnitCost) / 100)
            });

            return (Json(result));
        }

        public WorkCategory GetCategory(int id)
        {
            WorkCategory cat = new WorkCategory();
            var category = db.WorkCategory.Find(id);
            if (category != null)
            {
                cat = category;
            }
            return cat;
        }
    }
}