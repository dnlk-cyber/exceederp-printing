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
using ExceedERP.Core.Domain.Planning;
using System.IO;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [Authorize]
    public class MonthlyPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly ExceedERP.Helpers.Planning.PlanningApprovalHelper auth;

        private const string NotEditableMessage =
            "You can change figures only on your own work unit's plan, and only while it is a draft or has been returned for revision.";

        public MonthlyPlansController()
        {
            auth = new ExceedERP.Helpers.Planning.PlanningApprovalHelper(db);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyPlans_Read([DataSourceRequest]DataSourceRequest request,int pMId)
        {
            if (!CanViewDetail(pMId))
                return Json(Enumerable.Empty<MonthlyPlan>().ToDataSourceResult(request));

            IQueryable<MonthlyPlan> monthlyplans = db.MonthlyPlans.Where(x => x.PlanningDetailId == pMId);
            DataSourceResult result = monthlyplans.ToDataSourceResult(request, monthlyPlan => new {
                MonthlyPlanId = monthlyPlan.MonthlyPlanId,
                PlanningDetailId = monthlyPlan.PlanningDetailId,
                GLPeriodId = monthlyPlan.GLPeriodId,
                Value = monthlyPlan.Value,
                ActualValue = monthlyPlan.ActualValue,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyPlans_Update([DataSourceRequest]DataSourceRequest request, MonthlyPlan monthlyPlan)
        {
            monthlyPlan.ActualValue = 0;
            var entity = db.MonthlyPlans.Find(monthlyPlan.MonthlyPlanId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The monthly plan was not found.");
            else if (IsPlanLocked(entity.PlanningDetailId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            if (ModelState.IsValid)
            {
                entity.Value = monthlyPlan.Value;
                entity.ActualValue = 0;
                db.SaveChanges();
            }

            return Json(new[] { monthlyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyPlans_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyPlan monthlyPlan)
        {
            var entity = db.MonthlyPlans.Find(monthlyPlan.MonthlyPlanId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The monthly plan was not found.");
            else if (IsPlanLocked(entity.PlanningDetailId))
                ModelState.AddModelError(string.Empty, NotEditableMessage);

            var weeklyDetails = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyPlan.MonthlyPlanId).ToList();
            if (weeklyDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this! you have to delete child Data first");
            }

            if (ModelState.IsValid)
            {
                db.MonthlyPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { monthlyPlan }.ToDataSourceResult(request, ModelState));
        }
        private MonthlyPlan GetMappedObject(MonthlyPlan monthlyPlan)
        {
            var entity = new MonthlyPlan
            {
                MonthlyPlanId = monthlyPlan.MonthlyPlanId,
                PlanningDetailId = monthlyPlan.PlanningDetailId,
                GLPeriodId = monthlyPlan.GLPeriodId,
                Value = monthlyPlan.Value,
                ActualValue = monthlyPlan.ActualValue,


            };
            return entity;
        }

        private bool IsPlanLocked(int planningDetailId)
        {
            var plan = auth.GetPlanOfDetail(planningDetailId);
            var placement = auth.GetCurrentPlacement(ExceedERP.Web.Helpers.UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return !auth.CanEditFigures(plan, placement);
        }

        private bool CanViewDetail(int planningDetailId)
        {
            var plan = auth.GetPlanOfDetail(planningDetailId);
            if (plan != null && PlanningSelfCareHelper.CanViewAllWorkUnits(User))
                return true;

            var placement = auth.GetCurrentPlacement(ExceedERP.Web.Helpers.UserInformationHelper.GetEmployeeId(User.Identity.Name));
            return plan != null && placement != null && auth.CanViewPlan(plan, placement);
        }
        public ActionResult UploadDocument(int mothlyPlanId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + mothlyPlanId.ToString() + "-" + AttachmentType.DOCUMENT), mothlyPlanId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + mothlyPlanId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = mothlyPlanId.ToString(),
                            Description = mothlyPlanId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = mothlyPlanId.ToString()
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
                return RedirectToAction("Index","Plans");
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
