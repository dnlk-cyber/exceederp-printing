﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class PlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var objectives = db.Objectives.Select(O => new
            {
                O.ObjectiveId,
                O.Name,
             

            }).ToList();
            var indicators = db.Indicators.Select(I => new
            {
                I.IndicatorId,
                I.Name,
            
            }).ToList();
            var glFiscalYears = db.GLFiscalYears.Select(g => new
            {
                g.GlFiscalYearId,
                g.Name,

            }).ToList();
            ViewData["Objectives"] = objectives;
            ViewData["Indicators"] = indicators;
            ViewData["GlobalIndicators"] = db.GlobalIndicators.Select(x=>new { x.GlobalIndicatorId,x.Indicator});
            ViewData["GlFiscalYears"] = glFiscalYears;
            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId, period = x.Name }).ToList();
            ViewData["departmentLists"] = db.MainCompanyStructures.Select(x=>new { x.OrgStructureId,x.Name}).ToList();

            return View();
        }

        public ActionResult Plans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Plan> planns = db.Plans/*.Where(x=>x.IsDepartnental)*/;
            DataSourceResult result = planns.ToDataSourceResult(request, plan => new {
                PlanId = plan.PlanId,
                GlFiscalYearId = plan.GlFiscalYearId,
                IsDepartnental = plan.IsDepartnental,
                IsPersonal = plan.IsPersonal,
                DateCreated = plan.DateCreated,
                CreatedBy = plan.CreatedBy,
                LastModified = plan.LastModified,
                ModifiedBy = plan.ModifiedBy,
                Description = plan.Description,
                Reason = plan.Reason
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plans_Create([DataSourceRequest]DataSourceRequest request, Plan plan)
        {
            var exists = db.Plans.Any(s => s.GlFiscalYearId ==plan.GlFiscalYearId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            if (ModelState.IsValid)
            {
                plan.IsDepartnental = true;
                plan.DateCreated = DateTime.Now.Date;
                plan.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(plan);
                db.Plans.Add(entity);
                db.SaveChanges();
                plan.PlanId = entity.PlanId;

                #region operation log
                var fiscalYear = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                if (fiscalYear != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Plan", "Success", User.Identity.Name, fiscalYear.Name + " is Created By  " + User.Identity.Name, Modules.Planning);

                }

                #endregion
            }
          

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plans_Update([DataSourceRequest]DataSourceRequest request, Plan plan)
        {
            var exists = db.Plans/*.Where(x => x.IsDepartnental)*/.Any(s => s.GlFiscalYearId == plan.GlFiscalYearId/*&&s.IsDepartnental*/&&s.PlanId !=plan.PlanId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            if (ModelState.IsValid)
            {
                plan.LastModified = DateTime.Now.Date;
                plan.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(plan);
                db.Plans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Plan", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plans_Destroy([DataSourceRequest]DataSourceRequest request, Plan plan)
        {
            var bscPlans = db.BSCPlans.Where(x => x.PlanId == plan.PlanId).ToList();
            if (bscPlans.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(plan);
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(plan.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    Fiscal year " + delete);
                #endregion
                db.Plans.Attach(entity);
                db.Plans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }
        private Plan GetMappedObject(Plan plan)
        {
            var entity = new Plan
            {
                PlanId = plan.PlanId,
                GlFiscalYearId = plan.GlFiscalYearId,
                IsDepartnental = plan.IsDepartnental,
                DateCreated = plan.DateCreated,
                CreatedBy = plan.CreatedBy,
                LastModified = plan.LastModified,
                ModifiedBy = plan.ModifiedBy,
                Description = plan.Description,
                Reason = plan.Reason
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
