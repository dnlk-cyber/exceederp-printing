﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureJobAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureJobAssignments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureJobAssignment> furniturejobassignments = db.FurnitureJobAssignments.Where(x => x.FurnitureJobOrderProductionId == id);
            DataSourceResult result = furniturejobassignments.ToDataSourceResult(request, furnitureJobAssignment => new FurnitureJobAssignment
            {
                FurnitureJobAssignmentId = furnitureJobAssignment.FurnitureJobAssignmentId,
                EmployeeId = furnitureJobAssignment.EmployeeId,
                Type = furnitureJobAssignment.Type,
                TaskCategoryId = furnitureJobAssignment.TaskCategoryId,
                TimeSpent = furnitureJobAssignment.TimeSpent,
                TaskAssignedDate = furnitureJobAssignment.TaskAssignedDate,
                TaskEndDate = furnitureJobAssignment.TaskEndDate,
                FurnitureJobOrderProductionId = furnitureJobAssignment.FurnitureJobOrderProductionId,
                DailyProductionPlan = furnitureJobAssignment.DailyProductionPlan,
                CreatedBy = furnitureJobAssignment.CreatedBy,
                DateCreated = furnitureJobAssignment.DateCreated,
                LastModified = furnitureJobAssignment.LastModified,
                ModifiedBy = furnitureJobAssignment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobAssignments_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobAssignment furnitureJobAssignment, int id)
        {
            if (ModelState.IsValid)
            {
                var production = db.FurnitureJobOrderProductions.Find(id);
                
                var entity = new FurnitureJobAssignment
                {
                    EmployeeId = furnitureJobAssignment.EmployeeId,
                    Type = furnitureJobAssignment.Type,
                    TaskCategoryId = furnitureJobAssignment.TaskCategoryId,
                    TimeSpent = furnitureJobAssignment.TimeSpent,
                    TaskAssignedDate = furnitureJobAssignment.TaskAssignedDate,
                    TaskEndDate = furnitureJobAssignment.TaskEndDate,
                    FurnitureJobOrderProductionId = id,
                   
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    
                    
                };
                var totalDays = (entity.TaskEndDate - entity.TaskAssignedDate).Days;
                entity.DailyProductionPlan = Math.Round(production.Quantity / totalDays, 2);

                db.FurnitureJobAssignments.Add(entity);
                db.SaveChanges();
                furnitureJobAssignment.FurnitureJobAssignmentId = entity.FurnitureJobAssignmentId;
            }

            return Json(new[] { furnitureJobAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobAssignments_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobAssignment furnitureJobAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobAssignment
                {
                    FurnitureJobAssignmentId = furnitureJobAssignment.FurnitureJobAssignmentId,
                    EmployeeId = furnitureJobAssignment.EmployeeId,
                    Type = furnitureJobAssignment.Type,
                    TaskCategoryId = furnitureJobAssignment.TaskCategoryId,
                    TimeSpent = furnitureJobAssignment.TimeSpent,
                    TaskAssignedDate = furnitureJobAssignment.TaskAssignedDate,
                    TaskEndDate = furnitureJobAssignment.TaskEndDate,
                    CreatedBy = furnitureJobAssignment.CreatedBy,
                    DateCreated = furnitureJobAssignment.DateCreated,
                    DailyProductionPlan = furnitureJobAssignment.DailyProductionPlan,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                    FurnitureJobOrderProductionId = furnitureJobAssignment.FurnitureJobOrderProductionId
                };

                db.FurnitureJobAssignments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobAssignments_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobAssignment furnitureJobAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobAssignment
                {
                    FurnitureJobAssignmentId = furnitureJobAssignment.FurnitureJobAssignmentId,
                    EmployeeId = furnitureJobAssignment.EmployeeId,
                    Type = furnitureJobAssignment.Type,
                    TaskCategoryId = furnitureJobAssignment.TaskCategoryId,
                    TimeSpent = furnitureJobAssignment.TimeSpent,
                    TaskAssignedDate = furnitureJobAssignment.TaskAssignedDate,
                    TaskEndDate = furnitureJobAssignment.TaskEndDate,
                    FurnitureJobOrderProductionId = furnitureJobAssignment.FurnitureJobOrderProductionId,
                    LastModified = furnitureJobAssignment.LastModified,
                    ModifiedBy = furnitureJobAssignment.ModifiedBy,
                    DailyProductionPlan = furnitureJobAssignment.DailyProductionPlan,
                    CreatedBy = furnitureJobAssignment.CreatedBy,
                    DateCreated = furnitureJobAssignment.DateCreated
                };

                db.FurnitureJobAssignments.Attach(entity);
                db.FurnitureJobAssignments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobAssignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
