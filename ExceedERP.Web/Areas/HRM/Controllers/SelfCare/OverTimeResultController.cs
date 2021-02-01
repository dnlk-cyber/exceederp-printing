﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.HRM.SelfCare;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class OverTimeResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
        }
        public ActionResult OverTimeJobResults_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<OverTimeJobResult> overtimejobresults = db.OverTimeJobResults.Where (x=> x.OverTimeRequestId == id );
            DataSourceResult result = overtimejobresults.ToDataSourceResult(request, overTimeJobResult => new {
                OverTimeJobResultId = overTimeJobResult.OverTimeJobResultId,
                Date = overTimeJobResult.Date,
                JobResult = overTimeJobResult.JobResult,
                Cost = overTimeJobResult.Cost,
                Benefit = overTimeJobResult.Benefit,
                Remark = overTimeJobResult.Remark,
                OverTimeRequestId = overTimeJobResult.OverTimeRequestId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeJobResults_Create([DataSourceRequest]DataSourceRequest request, OverTimeJobResult overTimeJobResult, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeJobResult
                {
                    Date = overTimeJobResult.Date,
                    JobResult = overTimeJobResult.JobResult,
                    Cost = overTimeJobResult.Cost,
                    Benefit = overTimeJobResult.Benefit,
                    Remark = overTimeJobResult.Remark,
                    OverTimeRequestId = idTwo 
                };

                db.OverTimeJobResults.Add(entity);
                db.SaveChanges();
                overTimeJobResult.OverTimeJobResultId = entity.OverTimeJobResultId;
            }

            return Json(new[] { overTimeJobResult }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeJobResults_Update([DataSourceRequest]DataSourceRequest request, OverTimeJobResult overTimeJobResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeJobResult
                {
                    OverTimeJobResultId = overTimeJobResult.OverTimeJobResultId,
                    Date = overTimeJobResult.Date,
                    JobResult = overTimeJobResult.JobResult,
                    Cost = overTimeJobResult.Cost,
                    Benefit = overTimeJobResult.Benefit,
                    Remark = overTimeJobResult.Remark,
                    OverTimeRequestId = overTimeJobResult.OverTimeRequestId
                };

                db.OverTimeJobResults.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { overTimeJobResult }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeJobResults_Destroy([DataSourceRequest]DataSourceRequest request, OverTimeJobResult overTimeJobResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeJobResult
                {
                    OverTimeJobResultId = overTimeJobResult.OverTimeJobResultId,
                    Date = overTimeJobResult.Date,
                    JobResult = overTimeJobResult.JobResult,
                    Cost = overTimeJobResult.Cost,
                    Benefit = overTimeJobResult.Benefit,
                    Remark = overTimeJobResult.Remark,
                    OverTimeRequestId = overTimeJobResult.OverTimeRequestId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted over time result of over time request" + overTimeJobResult.OverTimeJobResultId);
                #endregion
                db.OverTimeJobResults.Attach(entity);
                db.OverTimeJobResults.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { overTimeJobResult }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
