﻿using System;
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
using ExceedERP.Web.Areas.Printing.Models;
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingDailyLaborRate)]
    public class DLRHourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private UpdateDLR dlr = new UpdateDLR();
        private DLRSalary salary = new DLRSalary();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DLRHours_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<DLRHour> dlrhours = db.DLRHours.Where(q=>q.DailyLaborRateId == id);
            DataSourceResult result = dlrhours.ToDataSourceResult(request, dLRHour => new {
                DLRHourId = dLRHour.DLRHourId,
                DailyLaborRateId = dLRHour.DailyLaborRateId,
                NoOfDay = dLRHour.NoOfDay,
                NoOfWorkingHour = dLRHour.NoOfWorkingHour,
                NoOfEmployee = dLRHour.NoOfEmployee,
                ExpectedTime = dLRHour.ExpectedTime,
                DateCreated = dLRHour.DateCreated,
                LastModified = dLRHour.LastModified,
                CreatedBy = dLRHour.CreatedBy,
                ModifiedBy = dLRHour.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult DLRHours_Create([DataSourceRequest]DataSourceRequest request, DLRHour dLRHour, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (DLRHour)this.GetObject(dLRHour, true, id);
                dlr.UpdateDLRRow(salary = null, entity, db);
                db.DLRHours.Add(entity);
                db.SaveChanges();
                dLRHour.DLRHourId = entity.DLRHourId;
            }

            return Json(new[] { dLRHour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DLRHours_Update([DataSourceRequest]DataSourceRequest request, DLRHour dLRHour)
        {
            if (ModelState.IsValid)
            {
                var entity = (DLRHour)this.GetObject(dLRHour, false);
                db.DLRHours.Attach(entity);
                dlr.UpdateDLRRow(salary = null, entity, db);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dLRHour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DLRHours_Destroy([DataSourceRequest]DataSourceRequest request, DLRHour dLRHour)
        {

            if (ModelState.IsValid)
            {
                var entity = (DLRHour)this.GetObject(dLRHour, false);

                db.DLRHours.Attach(entity);
                dlr.UpdateDLRRow(salary = null, entity, db, false);

                db.DLRHours.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { dLRHour }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(DLRHour dLRHour, bool status, int id = 0)
        {
            if (status)
            {
                dLRHour.DailyLaborRateId = id;
            }
            var entity = new DLRHour
            {
                DLRHourId = dLRHour.DLRHourId,
                DailyLaborRateId = dLRHour.DailyLaborRateId,
                NoOfDay = dLRHour.NoOfDay,
                NoOfWorkingHour = dLRHour.NoOfWorkingHour,
                NoOfEmployee = dLRHour.NoOfEmployee,
                ExpectedTime = dLRHour.ExpectedTime,
                DateCreated = dLRHour.DateCreated,
                LastModified = dLRHour.LastModified,
                CreatedBy = dLRHour.CreatedBy,
                ModifiedBy = dLRHour.ModifiedBy

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
