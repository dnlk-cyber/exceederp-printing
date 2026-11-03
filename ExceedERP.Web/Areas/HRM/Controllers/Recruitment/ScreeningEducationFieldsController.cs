using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ScreeningEducationFieldsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EducationLevelSettings
        public ActionResult Index()
        {
            return View();
        }

        // GET: HRM/EducationLevelSettings/Details/5
        public ActionResult ScreeningEducationFields_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            int sId = 0;
            if (!String.IsNullOrEmpty(id))
            {
                sId = int.Parse(id);
            }
            IQueryable<ScreeningEducationField> applicantEducations = db.ScreeningEducationFields.Where(x=>x.ScreeingSettingId == sId);


            DataSourceResult result = applicantEducations.ToDataSourceResult(request, screeningEducationField => new
            {
                ScreeningEducationFieldId = screeningEducationField.ScreeningEducationFieldId,
                ScreeingSettingId = screeningEducationField.ScreeingSettingId,
                EducationFieldId = screeningEducationField.EducationFieldId,
                Remark = screeningEducationField.Remark,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningEducationFields_Create([DataSourceRequest]DataSourceRequest request, ScreeningEducationField screeningEducationField, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningEducationField
                {
                    ScreeningEducationFieldId = screeningEducationField.ScreeningEducationFieldId,
                    ScreeingSettingId = id,
                    EducationFieldId = screeningEducationField.EducationFieldId,
                    Remark = screeningEducationField.Remark,
                };

                db.ScreeningEducationFields.Add(entity);
                db.SaveChanges();
                screeningEducationField.ScreeningEducationFieldId = entity.ScreeningEducationFieldId;


            }
            return Json(new[] { screeningEducationField }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningEducationFields_Update([DataSourceRequest]DataSourceRequest request, ScreeningEducationField screeningEducationField)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ScreeningEducationField
                {
                    ScreeningEducationFieldId = screeningEducationField.ScreeningEducationFieldId,
                    ScreeingSettingId = screeningEducationField.ScreeingSettingId,
                    EducationFieldId = screeningEducationField.EducationFieldId,
                    Remark = screeningEducationField.Remark,
                };

                db.ScreeningEducationFields.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { screeningEducationField }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScreeningEducationFields_Destroy([DataSourceRequest]DataSourceRequest request, ScreeningEducationField screeningEducationField)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                var entity = new ScreeningEducationField
                {
                    ScreeningEducationFieldId = screeningEducationField.ScreeningEducationFieldId,
                    ScreeingSettingId = screeningEducationField.ScreeingSettingId,
                    EducationFieldId = screeningEducationField.EducationFieldId,
                    Remark = screeningEducationField.Remark,
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level setting" + screeningEducationField.ScreeningEducationFieldId);
                #endregion
                db.ScreeningEducationFields.Attach(entity);
                db.ScreeningEducationFields.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { screeningEducationField }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
