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

        public ActionResult FurnitureJobAssignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobAssignment> furniturejobassignments = db.FurnitureJobAssignments;
            DataSourceResult result = furniturejobassignments.ToDataSourceResult(request, furnitureJobAssignment => new {
                FurnitureJobAssignmentId = furnitureJobAssignment.FurnitureJobAssignmentId,
                EmployeeId = furnitureJobAssignment.EmployeeId,
                Type = furnitureJobAssignment.Type,
                TaskCategoryId = furnitureJobAssignment.TaskCategoryId,
                TimeSpent = furnitureJobAssignment.TimeSpent,
                TaskAssignedDate = furnitureJobAssignment.TaskAssignedDate,
                TaskEndDate = furnitureJobAssignment.TaskEndDate,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobAssignments_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobAssignment furnitureJobAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobAssignment
                {
                    EmployeeId = furnitureJobAssignment.EmployeeId,
                    Type = furnitureJobAssignment.Type,
                    TaskCategoryId = furnitureJobAssignment.TaskCategoryId,
                    TimeSpent = furnitureJobAssignment.TimeSpent,
                    TaskAssignedDate = furnitureJobAssignment.TaskAssignedDate,
                    TaskEndDate = furnitureJobAssignment.TaskEndDate,
                };

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
