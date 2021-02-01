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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_User, FleetManagementRoles.Equipment_Plan_Approver)]
    public class LogisticPlanDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogisticPlanDetails_Read([DataSourceRequest]DataSourceRequest request , string Id)
        {
            IQueryable<LogisticPlanDetail> logisticplandetails = db.LogisticPlanDetails.Where(x=>x.LogisticPlanId.ToString() == Id);
            DataSourceResult result = logisticplandetails.ToDataSourceResult(request, logisticPlanDetail => new {
                LogisticPlanDetailId = logisticPlanDetail.LogisticPlanDetailId,
                LogisticPlanId = logisticPlanDetail.LogisticPlanId,
                Weeks = logisticPlanDetail.Weeks,
                ItemCode =logisticPlanDetail.ItemCode,
                Quantity = logisticPlanDetail.Quantity,
                ServiceId = logisticPlanDetail.ServiceId,
                Status = logisticPlanDetail.Status,
                Remark = logisticPlanDetail.Remark,
                OrgBranchName = logisticPlanDetail.OrgBranchName,
                DateCreated = logisticPlanDetail.DateCreated,
                LastModified = logisticPlanDetail.LastModified,
                CreatedBy = logisticPlanDetail.CreatedBy,
                ModifiedBy = logisticPlanDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogisticPlanDetails_Create([DataSourceRequest]DataSourceRequest request, LogisticPlanDetail logisticPlanDetail, string Id)
        {
            if (ModelState.IsValid)
            {
                var entity = new LogisticPlanDetail
                {
                    LogisticPlanId =  int.Parse(Id),
                    Weeks = logisticPlanDetail.Weeks,
                    ServiceId = logisticPlanDetail.ServiceId,
                    Quantity = logisticPlanDetail.Quantity,
                    ItemCode = logisticPlanDetail.ItemCode,
                    Status = logisticPlanDetail.Status,
                    Remark = logisticPlanDetail.Remark,
                    OrgBranchName = logisticPlanDetail.OrgBranchName,
                    DateCreated = logisticPlanDetail.DateCreated,
                    LastModified = logisticPlanDetail.LastModified,
                    CreatedBy = logisticPlanDetail.CreatedBy,
                    ModifiedBy = logisticPlanDetail.ModifiedBy
                };

                db.LogisticPlanDetails.Add(entity);
                db.SaveChanges();
                logisticPlanDetail.LogisticPlanDetailId = entity.LogisticPlanDetailId;
            }

            return Json(new[] { logisticPlanDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogisticPlanDetails_Update([DataSourceRequest]DataSourceRequest request, LogisticPlanDetail logisticPlanDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new LogisticPlanDetail
                {
                    LogisticPlanDetailId = logisticPlanDetail.LogisticPlanDetailId,
                    LogisticPlanId = logisticPlanDetail.LogisticPlanId,
                    Weeks = logisticPlanDetail.Weeks,
                    Service = logisticPlanDetail.Service,
                    Status = logisticPlanDetail.Status,
                    Remark = logisticPlanDetail.Remark,
                    OrgBranchName = logisticPlanDetail.OrgBranchName,
                    DateCreated = logisticPlanDetail.DateCreated,
                    LastModified = logisticPlanDetail.LastModified,
                    CreatedBy = logisticPlanDetail.CreatedBy,
                    ModifiedBy = logisticPlanDetail.ModifiedBy
                };

                db.LogisticPlanDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { logisticPlanDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogisticPlanDetails_Destroy([DataSourceRequest]DataSourceRequest request, LogisticPlanDetail logisticPlanDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new LogisticPlanDetail
                {
                    LogisticPlanDetailId = logisticPlanDetail.LogisticPlanDetailId,
                    LogisticPlanId = logisticPlanDetail.LogisticPlanId,
                    Weeks = logisticPlanDetail.Weeks,
                    Service = logisticPlanDetail.Service,
                    Status = logisticPlanDetail.Status,
                    Remark = logisticPlanDetail.Remark,
                    OrgBranchName = logisticPlanDetail.OrgBranchName,
                    DateCreated = logisticPlanDetail.DateCreated,
                    LastModified = logisticPlanDetail.LastModified,
                    CreatedBy = logisticPlanDetail.CreatedBy,
                    ModifiedBy = logisticPlanDetail.ModifiedBy
                };

                db.LogisticPlanDetails.Attach(entity);
                db.LogisticPlanDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { logisticPlanDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
