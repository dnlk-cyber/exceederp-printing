﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class LeaveEntitleStartingDateController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FisicalYear"] = db.GLFiscalYears.ToList();
            return View();
        }

        public ActionResult LeaveEntitleStartingDate_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LeaveEntitleStartingDate> leaveEntitleStartingDates = db.LeaveEntitleStartingDates;

            DataSourceResult result = leaveEntitleStartingDates.ToDataSourceResult(request, leaveEntitleStartingDate => new {
                LeaveEntitleStartingDateId = leaveEntitleStartingDate.LeaveEntitleStartingDateId,
                Start = leaveEntitleStartingDate.Start,
                Remark = leaveEntitleStartingDate.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveEntitleStartingDate_Create([DataSourceRequest]DataSourceRequest request, LeaveEntitleStartingDate leaveEntitleStartingDate)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveEntitleStartingDate
                {
                    Start = leaveEntitleStartingDate.Start,
                    Remark = leaveEntitleStartingDate.Remark
                };

                db.LeaveEntitleStartingDates.Add(entity);
                db.SaveChanges();
                leaveEntitleStartingDate.LeaveEntitleStartingDateId = entity.LeaveEntitleStartingDateId;
            }

            return Json(new[] { leaveEntitleStartingDate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveEntitleStartingDate_Update([DataSourceRequest]DataSourceRequest request, LeaveEntitleStartingDate leaveEntitleStartingDate)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveEntitleStartingDate
                {
                    LeaveEntitleStartingDateId = leaveEntitleStartingDate.LeaveEntitleStartingDateId,
                    Start = leaveEntitleStartingDate.Start,
                    Remark = leaveEntitleStartingDate.Remark
                };

                db.LeaveEntitleStartingDates.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { leaveEntitleStartingDate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveEntitleStartingDate_Destroy([DataSourceRequest]DataSourceRequest request, LeaveEntitleStartingDate leaveEntitleStartingDate)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveEntitleStartingDate
                {
                    LeaveEntitleStartingDateId = leaveEntitleStartingDate.LeaveEntitleStartingDateId,
                    Start = leaveEntitleStartingDate.Start,
                    Remark = leaveEntitleStartingDate.Remark
                };

                db.LeaveEntitleStartingDates.Attach(entity);
                db.LeaveEntitleStartingDates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leaveEntitleStartingDate }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
