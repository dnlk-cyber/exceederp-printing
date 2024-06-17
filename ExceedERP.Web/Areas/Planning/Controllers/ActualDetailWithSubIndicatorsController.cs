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
    [AuthorizeRoles(PlanningRoles.Planning_Admin)]
    public class ActualDetailWithSubIndicatorsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActualDetailWithSubIndicators_Read([DataSourceRequest]DataSourceRequest request,int aDetailId,int parentId)
        {
            IQueryable<ActualDetail> actualdetails = db.ActualDetails.Where(s=>s.BSCActualId == aDetailId&& s.ParentId==parentId);
            DataSourceResult result = actualdetails.ToDataSourceResult(request, actualDetail => new {
                ActualDetailId = actualDetail.ActualDetailId,
                ParentId = actualDetail.ParentId,
                BSCActualId = actualDetail.BSCActualId,
                Unit = actualDetail.Unit,
                KPIType = actualDetail.KPIType,
                IndicatorId = actualDetail.IndicatorId,
                GlobalIndicatorId = actualDetail.GlobalIndicatorId,
                HasSubIndicator = actualDetail.HasSubIndicator,
                Plan = actualDetail.Plan,
                Baseline = actualDetail.Baseline,
                Target = actualDetail.Target
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AActualDetailWithSubIndicators_Create([DataSourceRequest]DataSourceRequest request, ActualDetail actualDetail,int aDetailId)
        {
            if (ModelState.IsValid)
            {
                
                actualDetail.BSCActualId = aDetailId;
                var entity = GetMappedObject(actualDetail);
                db.ActualDetails.Add(entity);
                db.SaveChanges();
                actualDetail.ActualDetailId = entity.ActualDetailId;
            }

            return Json(new[] { actualDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualDetailWithSubIndicators_Update([DataSourceRequest]DataSourceRequest request, ActualDetail actualDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(actualDetail);
                db.ActualDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { actualDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualDetailWithSubIndicators_Destroy([DataSourceRequest]DataSourceRequest request, ActualDetail actualDetail)
        {
            var monthlyActuals = db.MonthlyActuals.Where(x => x.ActualDetailId == actualDetail.ActualDetailId).ToList();
            if (monthlyActuals.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(actualDetail);
                db.ActualDetails.Attach(entity);
                db.ActualDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { actualDetail }.ToDataSourceResult(request, ModelState));
        }
        private ActualDetail GetMappedObject(ActualDetail actualDetail)
        {
            var entity = new ActualDetail
            {
                ActualDetailId = actualDetail.ActualDetailId,
                ParentId = actualDetail.ParentId,
                BSCActualId = actualDetail.BSCActualId,
                Unit = actualDetail.Unit,
                KPIType = actualDetail.KPIType,
                IndicatorId = actualDetail.IndicatorId,
                GlobalIndicatorId = actualDetail.GlobalIndicatorId,
                HasSubIndicator = actualDetail.HasSubIndicator,
                Plan = actualDetail.Plan,
                Baseline = actualDetail.Baseline,
                Target = actualDetail.Target


            };
            return entity;
        }
        //private string getSubIndigator(int indicatorid)
        //{
        //    var subindicators = db.Indicators.Find(indicatorid);
        //    if (subindicators != null)
        //    {
        //        return subindicators.SubIndicator;
        //    }
        //    return null;
        //}
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
                return RedirectToAction("Index", "Actuals");
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
