using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Web.Areas.Manufacturing.Models;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureDirectLaborCostsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DirectLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int? estimationFormId)
        {
            IQueryable<Core.Domain.Manufacturing.FurnitureEstimation.DirectLaborCost> laborcosts = db.DirectLaborCosts.Where(q => q.FurnitureEstimationId == estimationFormId).OrderByDescending(a => a.DirectLaborCostId);
            DataSourceResult result = laborcosts.ToDataSourceResult(request, laborCost => new DirectLaborCost
            {
                DirectLaborCostId = laborCost.DirectLaborCostId,
                FurnitureEstimationId = laborCost.FurnitureEstimationId,
                
                WorkingTime = laborCost.WorkingTime,
                CostPerHour = laborCost.CostPerHour,
                TotalCost = laborCost.TotalCost,
                DateCreated = laborCost.DateCreated,
                LastModified = laborCost.LastModified,
                TaskType = laborCost.TaskType,
                TaskCategoryId = laborCost.TaskCategoryId

            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DirectLaborCosts_Create([DataSourceRequest]DataSourceRequest request, DirectLaborCost labourCost, int parentId)
        {
            if (ModelState.IsValid)
            {
                var taskCategory = db.ManifucturingTaskCategories.Find(labourCost.TaskCategoryId);
                labourCost.CostPerHour = taskCategory.HourRate;
                labourCost.TotalCost = labourCost.CostPerHour * labourCost.WorkingTime;
                labourCost.FurnitureEstimationId = parentId;
                db.DirectLaborCosts.Attach(labourCost);
                db.DirectLaborCosts.Add(labourCost);
                db.SaveChanges();
            }


            return Json(new[] { labourCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Update([DataSourceRequest]DataSourceRequest request, DirectLaborCost labourCost)
        {
            var taskCategory = db.ManifucturingTaskCategories.Find(labourCost.TaskCategoryId);
            if (taskCategory == null) ModelState.AddModelError(string.Empty, "Task category not found");
            if (ModelState.IsValid)
            {
                labourCost.CostPerHour = taskCategory.HourRate;
                labourCost.TotalCost = labourCost.CostPerHour * labourCost.WorkingTime;
                db.DirectLaborCosts.Attach(labourCost);
                db.Entry(labourCost).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { labourCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Destroy([DataSourceRequest]DataSourceRequest request, DirectLaborCost labourCost)
        {

            var directLabourCost = db.DirectLaborCosts.Find(labourCost.DirectLaborCostId);
            db.DirectLaborCosts.Remove(directLabourCost);
            db.SaveChanges();

            return Json(new[] { labourCost }.ToDataSourceResult(request, ModelState));
        }
        private DirectLaborCost GetMappedObject(DirectLaborCost laborCost)
        {
            var entity = new DirectLaborCost
            {
                DirectLaborCostId = laborCost.DirectLaborCostId,
                FurnitureEstimationId = laborCost.FurnitureEstimationId,
                
                WorkingTime = laborCost.WorkingTime,
                CostPerHour = laborCost.CostPerHour,
                TotalCost = laborCost.TotalCost,
                DateCreated = laborCost.DateCreated,
                LastModified = laborCost.LastModified,
                TaskCategoryId = laborCost.TaskCategoryId
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
