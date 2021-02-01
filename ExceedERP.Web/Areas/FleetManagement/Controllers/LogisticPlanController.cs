﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
     [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_User,FleetManagementRoles.Equipment_Plan_Approver)]
    public class LogisticPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["InventoryItem"] = db.InventoryItems.ToList();
            ViewData["Service"] = db.Service.ToList();
            return View();
        }

        public ActionResult LogisticPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LogisticPlan> logisticplans = db.LogisticPlans;
            DataSourceResult result = logisticplans.ToDataSourceResult(request, logisticPlan => new
            {
                LogisticPlanId = logisticPlan.LogisticPlanId,
                GlFiscalYearId = logisticPlan.GlFiscalYearId,
                Type = logisticPlan.Type,
                Period = logisticPlan.GLPeriod.Name,
                Model = logisticPlan.Model,
                Location = logisticPlan.Location,
                FiscalYear = logisticPlan.GLPeriod.GlFiscalYear.Name,
                Status = logisticPlan.Status,
                Remark = logisticPlan.Remark,
                OrgBranchName = logisticPlan.OrgBranchName,
                DateCreated = logisticPlan.DateCreated,
                LastModified = logisticPlan.LastModified,
                CreatedBy = logisticPlan.CreatedBy,
                ModifiedBy = logisticPlan.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogisticPlans_Create([DataSourceRequest]DataSourceRequest request, LogisticPlan logisticPlan)
        {
            var LogisticPlan = db.LogisticPlans.Where(x => x.EquipmentTypeId == logisticPlan.EquipmentTypeId && x.Model == logisticPlan.Model && x.GlFiscalYearId == logisticPlan.GlFiscalYearId && x.GLPeriodId == logisticPlan.GLPeriodId && x.Location == logisticPlan.Location ).ToList();

            if(LogisticPlan.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " Logistic Plan Already Exists !");
                return Json(new[] { logisticPlan }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {

                db = new ExceedDbContext();

                var entity = new LogisticPlan
                {
                    GlFiscalYearId = logisticPlan.GlFiscalYearId,
                    GLPeriodId = logisticPlan.GLPeriodId,
                    EquipmentTypeId = logisticPlan.EquipmentTypeId,
                    Type = logisticPlan.Type,
                    Model = logisticPlan.Model,
                    Status = logisticPlan.Status,
                    Location = logisticPlan.Location,
                    Remark = logisticPlan.Remark,
                    OrgBranchName = logisticPlan.OrgBranchName,
                    DateCreated = logisticPlan.DateCreated,
                    LastModified = logisticPlan.LastModified,
                    CreatedBy = logisticPlan.CreatedBy,
                    ModifiedBy = logisticPlan.ModifiedBy
                };

                db.LogisticPlans.Add(entity);
                db.SaveChanges();
                logisticPlan.LogisticPlanId = entity.LogisticPlanId;
            }

            return Json(new[] { logisticPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogisticPlans_Update([DataSourceRequest]DataSourceRequest request, LogisticPlan logisticPlan)
        {

            var LogisticPlan = db.LogisticPlans.Where(x => x.EquipmentTypeId == logisticPlan.EquipmentTypeId && x.Model == logisticPlan.Model && x.GlFiscalYearId == logisticPlan.GlFiscalYearId && x.GLPeriodId == logisticPlan.GLPeriodId && x.LogisticPlanId != logisticPlan.LogisticPlanId && x.Location == logisticPlan.Location).ToList();

            if (LogisticPlan.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " Logistic Plan Already Exists !");
                return Json(new[] { logisticPlan }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new LogisticPlan
                {
                    LogisticPlanId = logisticPlan.LogisticPlanId,
                    GlFiscalYearId = logisticPlan.GlFiscalYearId,
                    Status = logisticPlan.Status,
                    Remark = logisticPlan.Remark,
                    OrgBranchName = logisticPlan.OrgBranchName,
                    DateCreated = logisticPlan.DateCreated,
                    LastModified = logisticPlan.LastModified,
                    CreatedBy = logisticPlan.CreatedBy,
                    ModifiedBy = logisticPlan.ModifiedBy
                };

                db.LogisticPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { logisticPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogisticPlans_Destroy([DataSourceRequest]DataSourceRequest request, LogisticPlan logisticPlan)
        {

            if (logisticPlan != null)
            {
                db.LogisticPlans.Attach(logisticPlan);
                db.LogisticPlans.Remove(logisticPlan);
                db.SaveChanges();
            }  
            return Json(new[] { logisticPlan }.ToDataSourceResult(request));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
