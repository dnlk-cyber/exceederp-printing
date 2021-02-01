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
    public class PMScheduleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["EquipmentType"] = db.EquipmentType.ToList();
            ViewData["Fleet"] = db.Fleets.ToList();
            ViewData["Service"] = db.Service.ToList();
            ViewData["GLPeriod"] = db.GLPeriods.ToList();
            return View();
        }

        public ActionResult PMSchedules_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PMSchedule> pmschedules = db.PMSchedules;
            DataSourceResult result = pmschedules.ToDataSourceResult(request, pMSchedule => new
            {
                PMScheduleId = pMSchedule.PMScheduleId,
                GlFiscalYearId = pMSchedule.GlFiscalYearId,
                EquipmentTypeId = pMSchedule.EquipmentTypeId,
                Type = pMSchedule.Type,
                Year = pMSchedule.GlFiscalYear.Name,
                Location = pMSchedule.Location,
                Status = pMSchedule.Status,
                Remark = pMSchedule.Remark,
                OrgBranchName = pMSchedule.OrgBranchName,
                DateCreated = pMSchedule.DateCreated,
                LastModified = pMSchedule.LastModified,
                CreatedBy = pMSchedule.CreatedBy,
                ModifiedBy = pMSchedule.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PMSchedules_Create([DataSourceRequest]DataSourceRequest request, PMSchedule pMSchedule)
        {
            var PMSchedules = db.PMSchedules.Where(x => x.EquipmentTypeId == pMSchedule.EquipmentTypeId && x.Location == pMSchedule.Location && x.GlFiscalYearId == pMSchedule.GlFiscalYearId).ToList();

            if (PMSchedules.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " PM Plan Already Exists !");
                return Json(new[] { pMSchedule }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new PMSchedule
                {
                    GlFiscalYearId = pMSchedule.GlFiscalYearId,
                    Location = pMSchedule.Location,
                    EquipmentTypeId = pMSchedule.EquipmentTypeId,
                    Status = Core.Domain.Common.ApprovalStatuses.Pending,
                    Remark = pMSchedule.Remark,
                    OrgBranchName = pMSchedule.OrgBranchName,
                    DateCreated = DateTime.Now,
                    Type = pMSchedule.Type,
                    LastModified = pMSchedule.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = pMSchedule.ModifiedBy
                };

                db.PMSchedules.Add(entity);
                db.SaveChanges();
                pMSchedule.PMScheduleId = entity.PMScheduleId;
            }

            return Json(new[] { pMSchedule }.ToDataSourceResult(request, ModelState));
        }

       

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PMSchedules_Update([DataSourceRequest]DataSourceRequest request, PMSchedule pMSchedule)
        {

            var PMSchedules = db.PMSchedules.Where(x => x.EquipmentTypeId == pMSchedule.EquipmentTypeId && x.GlFiscalYearId == pMSchedule.GlFiscalYearId && x.Location == pMSchedule.Location && x.PMScheduleId != pMSchedule.PMScheduleId).ToList();

            if (PMSchedules.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " PM Plan Already Exists !");
                return Json(new[] { pMSchedule }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new PMSchedule
                {
                    PMScheduleId = pMSchedule.PMScheduleId,
                    GlFiscalYearId = pMSchedule.GlFiscalYearId,
                    Location = pMSchedule.Location,
                    Status = pMSchedule.Status,
                    Remark = pMSchedule.Remark,
                    OrgBranchName = pMSchedule.OrgBranchName,
                    DateCreated = pMSchedule.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = pMSchedule.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PMSchedules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { pMSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PMSchedules_Destroy([DataSourceRequest]DataSourceRequest request, PMSchedule pMSchedule)
        {

            if (pMSchedule != null)
            {
                db.PMSchedules.Attach(pMSchedule);
                db.PMSchedules.Remove(pMSchedule);
                db.SaveChanges();
            }


            return Json(new[] { pMSchedule }.ToDataSourceResult(request));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
