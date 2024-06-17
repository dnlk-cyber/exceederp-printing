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
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class PlanningDetailWithSubInsicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

      
        public ActionResult PlanningDetailWithSubInsicators_Read([DataSourceRequest]DataSourceRequest request,int pDetailId,int parentId)
        {
            IQueryable<PlanningDetail> planningdetails = db.PlanningDetails.Where(p=>p.BSCPlanId==pDetailId&& p.ParentId==parentId);
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
        public ActionResult PlanningDetailWithSubInsicators_Create([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail,int pDetailId)
        {
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
        public ActionResult PlanningDetailWithSubInsicators_Update([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(planningDetail);
                db.PlanningDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { planningDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanningDetailWithSubInsicators_Destroy([DataSourceRequest]DataSourceRequest request, PlanningDetail planningDetail)
        {
            var monthlyDetails = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
            if (monthlyDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(planningDetail);
                db.PlanningDetails.Attach(entity);
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
