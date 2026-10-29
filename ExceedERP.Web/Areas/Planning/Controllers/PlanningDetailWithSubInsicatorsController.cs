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
using ExceedERP.Core.Domain.Common;
using System.IO;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [Authorize]
    public class PlanningDetailWithSubInsicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly ExceedERP.Helpers.Planning.PlanningApprovalHelper auth;

        private const string NotEditableMessage =
            "You can change figures only on your own work unit's plan, and only while it is a draft or has been returned for revision.";

        public PlanningDetailWithSubInsicatorsController()
        {
            auth = new ExceedERP.Helpers.Planning.PlanningApprovalHelper(db);
        }

        public ActionResult PlanningDetailWithSubInsicators_Read([DataSourceRequest]DataSourceRequest request,int pDetailId,int parentId)
        {
            if (!CanViewPlan(pDetailId))
                return Json(Enumerable.Empty<PlanningDetail>().ToDataSourceResult(request));

            IQueryable<PlanningDetail> planningdetails = db.PlanningDetails.Where(p=>p.BSCPlanId==pDetailId&& p.ParentId==parentId&&p.GetIndicator.IsActive);
            DataSourceResult result = planningdetails.ToDataSourceResult(request, planningDetail => new {
                PlanningDetailId = planningDetail.PlanningDetailId,
                BSCPlanId = planningDetail.BSCPlanId,
                ParentId = planningDetail.ParentId,
                Unit = planningDetail.Unit,
                KPIType = planningDetail.KPIType,
                IndicatorId = planningDetail.IndicatorId,
                GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                HasSubIndicator = planningDetail.HasSubIndicator,
                Plan = planningDetail.Plan,
                Baseline = planningDetail.Baseline,
                Target = planningDetail.Target                
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlanningDetailWithSubInsicators_Create([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail,int pDetailId)
        {
            if (IsPlanLocked(pDetailId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
                planningDetail.BSCPlanId = pDetailId;
                var entity = GetMappedObject(planningDetail);
                db.PlanningDetails.Add(entity);
                db.SaveChanges();
                planningDetail.PlanningDetailId = entity.PlanningDetailId;
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlanningDetailWithSubInsicators_Update([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail)
        {
            var entity = db.PlanningDetails.Find(planningDetail.PlanningDetailId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The planning detail was not found.");
            else if (IsPlanLocked(entity.BSCPlanId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
                entity.Plan = planningDetail.Plan;
                entity.Baseline = planningDetail.Baseline;
                entity.Target = planningDetail.Target;
                db.SaveChanges();
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlanningDetailWithSubInsicators_Destroy([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail)
        {
            var entity = db.PlanningDetails.Find(planningDetail.PlanningDetailId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The planning detail was not found.");
            else if (IsPlanLocked(entity.BSCPlanId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            var monthlyDetails = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
            if (monthlyDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                db.PlanningDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }
        private PlanningDetail GetMappedObject(PlanningDetail planningDetail)
        {
            var entity = new PlanningDetail
            {
                PlanningDetailId = planningDetail.PlanningDetailId,
                BSCPlanId = planningDetail.BSCPlanId,
                ParentId = planningDetail.ParentId,
                GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                Unit = planningDetail.Unit,
                KPIType = planningDetail.KPIType,
                IndicatorId = planningDetail.IndicatorId,
                HasSubIndicator = planningDetail.HasSubIndicator,
                Plan = planningDetail.Plan,
                Baseline = planningDetail.Baseline,
                Target = planningDetail.Target
            };
            return entity;
        }

        private bool IsPlanLocked(int bscPlanId)
        {
            var plan = db.BSCPlans.Find(bscPlanId);
            var placement = auth.GetCurrentPlacement(UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return !auth.CanEditFigures(plan, placement);
        }

        private bool CanViewPlan(int bscPlanId)
        {
            var plan = db.BSCPlans.Find(bscPlanId);
            if (plan != null && PlanningSelfCareHelper.CanViewAllWorkUnits(User))
                return true;

            var placement = auth.GetCurrentPlacement(UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return plan != null && placement != null && auth.CanViewPlan(plan, placement);
        }
        public ActionResult UploadDocument(int subIndicatorId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + subIndicatorId.ToString() + "-" + AttachmentType.DOCUMENT), subIndicatorId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + subIndicatorId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = subIndicatorId.ToString(),
                            Description = subIndicatorId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = subIndicatorId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index", "Plans");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            int.TryParse(attachId, out int attachmentId);
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
