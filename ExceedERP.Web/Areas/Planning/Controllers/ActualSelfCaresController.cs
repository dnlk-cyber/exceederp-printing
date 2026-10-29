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
using System.IO;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
                    PlanningRoles.Planning_User,
                    HrmConstants.HRDepartmentHead,
                    HrmConstants.HREmployeeSelfService)]
    public class ActualSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var objectives = PlanningSelfCareHelper.GetDepartmentObjectiveList(db, User.Identity.Name);
            var indicators = PlanningSelfCareHelper.GetDepartmentIndicatorList(db, User.Identity.Name);
            var fiscalYears = from f in db.GLFiscalYears
                              join
                    p in db.Plans on f.GlFiscalYearId equals p.GlFiscalYearId
                              select new
                              {
                                  f.GlFiscalYearId,
                                  f.Name,

                              };
            ViewData["GlFiscalYears"] = fiscalYears.DistinctByField(x => x.GlFiscalYearId).ToList();
            ViewData["Objectives"] = objectives;
            ViewData["Indicators"] = indicators;
            ViewData["GlobalIndicators"] = PlanningSelfCareHelper.GetDepartmentGlobalIndicatorList(db, User.Identity.Name);
            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId, period = x.Name }).ToList();
            ViewData["departmentLists"] = db.MainCompanyStructures.ToList();

            return View();
        }

        public ActionResult ActualSelfCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            //var actualIds = db.BSCActuals.Where(x => x.OrgStructureId == workUnitId).Select(x => x.ActualId);
            IQueryable<Actual> acuals = db.Actuals/*.Where(x=>actualIds.Contains(x.ActualId))*//*.Where(x=>x.IsPersonal)*/;
            
            DataSourceResult result = acuals.ToDataSourceResult(request, actual => new {
                ActualId = actual.ActualId,
                GlFiscalYearId = actual.GlFiscalYearId,
                PlanId = actual.PlanId,
                IsPersonal = actual.IsPersonal,
                IsDepartnental = actual.IsDepartnental,
                DateCreated = actual.DateCreated,
                CreatedBy = actual.CreatedBy,
                LastModified = actual.LastModified,
                ModifiedBy = actual.ModifiedBy,
                Description = actual.Description
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualSelfCares_Create([DataSourceRequest]DataSourceRequest request, Actual actual)
        {
            var exists = db.Actuals/*.Where(x => x.IsPersonal)*/.Any(s => s.GlFiscalYearId == actual.GlFiscalYearId/*&&s.IsPersonal*/);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            var plan = db.Plans/*.Where(x => x.IsPersonal)*/.FirstOrDefault(x => x.GlFiscalYearId == actual.GlFiscalYearId/*&&x.IsPersonal*/);
            if (plan != null)
            {
                actual.PlanId = plan.PlanId;
            }
            else if (plan == null)
            {
                ModelState.AddModelError("exists", "you have to plan first for this fiscal year ");

            }
            if (ModelState.IsValid)
            {
                actual.IsPersonal = true;
                actual.CreatedBy = User.Identity.Name;
                actual.DateCreated = DateTime.Now.Date;
                var entity = GetMappedObject(actual);
                db.Actuals.Add(entity);
                db.SaveChanges();
                actual.ActualId = entity.ActualId;
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion
            }

            return Json(new[] { actual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualSelfCares_Update([DataSourceRequest]DataSourceRequest request, Actual actual)
        {
            var exists = db.Actuals.Any(x => x.GlFiscalYearId == actual.GlFiscalYearId && x.ActualId != actual.ActualId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            var plan = db.Plans.FirstOrDefault(x => x.GlFiscalYearId == actual.GlFiscalYearId);
            if (plan != null)
            {
                actual.PlanId = plan.PlanId;
            }
            if (plan == null)
            {
                ModelState.AddModelError("exist", "you have to add a Plan for this fiscal year first");
            }
            if (ModelState.IsValid)
            {
                actual.ModifiedBy = User.Identity.Name;
                actual.LastModified = DateTime.Now.Date;
                var entity = GetMappedObject(actual);
                db.Actuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Actual", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion
            }

            return Json(new[] { actual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, Actual actual)
        {
            var bscActuals = db.BSCActuals.Where(x => x.ActualId == actual.ActualId).ToList();
            if (bscActuals.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(actual);
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(actual.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " year:" + delete);
                #endregion
                db.Actuals.Attach(entity);
                db.Actuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { actual }.ToDataSourceResult(request, ModelState));
        }
        private Actual GetMappedObject(Actual actual)
        {
            var entity = new Actual
            {
                ActualId = actual.ActualId,
                GlFiscalYearId = actual.GlFiscalYearId,
                PlanId = actual.PlanId,
                IsPersonal = actual.IsPersonal,
                DateCreated = actual.DateCreated,
                CreatedBy= actual.CreatedBy,
                LastModified=actual.LastModified,
                ModifiedBy=actual.ModifiedBy,
                Description = actual.Description
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
                return RedirectToAction("Index", "ActualSelfCares");
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
