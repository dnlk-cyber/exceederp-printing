﻿using System;
using System.Collections.Generic;

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
    public class MonthlyLeavePlanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["AllEmployeeList"] = db.Person_Employee.AsQueryable();
            ViewData["PeriodList"] = db.GLFiscalYears.AsQueryable();
            ViewData["Period"] = db.GLPeriods.AsQueryable();
            return View();
        }

        public ActionResult MonthlyLeavePlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MonthlyLeavePlan> monthlyleaveplans = db.MonthlyLeavePlans;
            DataSourceResult result = monthlyleaveplans.ToDataSourceResult(request, monthlyLeavePlan => new {
                MonthlyLeavePlanId = monthlyLeavePlan.MonthlyLeavePlanId,
                EmployeeId = monthlyLeavePlan.EmployeeId,
                GlFiscalYearId = monthlyLeavePlan.GlFiscalYearId,
                OrgBranchName = monthlyLeavePlan.OrgBranchName,
                DateCreated = monthlyLeavePlan.DateCreated,
                LastModified = monthlyLeavePlan.LastModified,
                CreatedBy = monthlyLeavePlan.CreatedBy,
                ModifiedBy = monthlyLeavePlan.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyLeavePlans_Create([DataSourceRequest]DataSourceRequest request, MonthlyLeavePlan monthlyLeavePlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyLeavePlan
                {
                    EmployeeId = monthlyLeavePlan.EmployeeId,
                    GlFiscalYearId = monthlyLeavePlan.GlFiscalYearId,
                    OrgBranchName = monthlyLeavePlan.OrgBranchName,
                    DateCreated = monthlyLeavePlan.DateCreated,
                    LastModified = monthlyLeavePlan.LastModified,
                    CreatedBy = monthlyLeavePlan.CreatedBy,
                    ModifiedBy = monthlyLeavePlan.ModifiedBy
                };

                db.MonthlyLeavePlans.Add(entity);
                db.SaveChanges();
                monthlyLeavePlan.MonthlyLeavePlanId = entity.MonthlyLeavePlanId;
            }

            return Json(new[] { monthlyLeavePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyLeavePlans_Update([DataSourceRequest]DataSourceRequest request, MonthlyLeavePlan monthlyLeavePlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyLeavePlan
                {
                    MonthlyLeavePlanId = monthlyLeavePlan.MonthlyLeavePlanId,
                    EmployeeId = monthlyLeavePlan.EmployeeId,
                    GlFiscalYearId = monthlyLeavePlan.GlFiscalYearId,
                    OrgBranchName = monthlyLeavePlan.OrgBranchName,
                    DateCreated = monthlyLeavePlan.DateCreated,
                    LastModified = monthlyLeavePlan.LastModified,
                    CreatedBy = monthlyLeavePlan.CreatedBy,
                    ModifiedBy = monthlyLeavePlan.ModifiedBy
                };

                db.MonthlyLeavePlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { monthlyLeavePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyLeavePlans_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyLeavePlan monthlyLeavePlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyLeavePlan
                {
                    MonthlyLeavePlanId = monthlyLeavePlan.MonthlyLeavePlanId,
                    EmployeeId = monthlyLeavePlan.EmployeeId,
                    GlFiscalYearId = monthlyLeavePlan.GlFiscalYearId,
                    OrgBranchName = monthlyLeavePlan.OrgBranchName,
                    DateCreated = monthlyLeavePlan.DateCreated,
                    LastModified = monthlyLeavePlan.LastModified,
                    CreatedBy = monthlyLeavePlan.CreatedBy,
                    ModifiedBy = monthlyLeavePlan.ModifiedBy
                };

                db.MonthlyLeavePlans.Attach(entity);
                db.MonthlyLeavePlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { monthlyLeavePlan }.ToDataSourceResult(request, ModelState));
        }
        public void GenerateLeavePlanPeriods(int id)
        {
            var leavePlan = db.MonthlyLeavePlans.Find(id);
            if(leavePlan != null)
            {
                var glPeriods = db.GLPeriods.Where(x=>x.GlFiscalYearId == leavePlan.GlFiscalYearId).ToList();
                foreach(var period in glPeriods)
                {
                    var checkDuplicate = db.MonthlyLeavePlanDetails.FirstOrDefault(x=>x.GlFiscalYearId == period.GlFiscalYearId && x.GLPeriodId == period.GLPeriodId && x.MonthlyLeavePlanId == leavePlan.MonthlyLeavePlanId);
                    if(checkDuplicate == null)
                    {
                        var entity = new MonthlyLeavePlanDetail
                        {
                            MonthlyLeavePlanId = leavePlan.MonthlyLeavePlanId,
                            GlFiscalYearId = period.GlFiscalYearId,
                            GLPeriodId = period.GLPeriodId,
                            Amount = 0,
                        };

                        db.MonthlyLeavePlanDetails.Add(entity);
                        db.SaveChanges();
                    }
                   
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
