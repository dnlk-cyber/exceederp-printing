using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
    public class PassSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["Vacancy"] = db.VaccancyPlans.ToList();

            return View();
        }

        public ActionResult PassSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RecruitmentApplicantPassSetting> passSettings = db.RecruitmentApplicantPassSettings;
            DataSourceResult result = passSettings.ToDataSourceResult(request, passSetting => new {
                RecruitmentApplicantPassSettingId = passSetting.RecruitmentApplicantPassSettingId,
                VacancyId = passSetting.VacancyId,
                TotalValue = passSetting.TotalValue,
                PassScore = passSetting.PassScore,
                Remark = passSetting.Remark,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PassSetting_Create([DataSourceRequest]DataSourceRequest request, RecruitmentApplicantPassSetting passSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
               
                    var entity = new RecruitmentApplicantPassSetting
                    {
                        VacancyId = passSetting.VacancyId,
                        TotalValue = passSetting.TotalValue,
                        PassScore = passSetting.PassScore,
                        Remark = passSetting.Remark,
                    };

                    db.RecruitmentApplicantPassSettings.Add(entity);
                    db.SaveChanges();
                    passSetting.RecruitmentApplicantPassSettingId = entity.RecruitmentApplicantPassSettingId;
                
            }

            return Json(new[] { passSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PassSetting_Update([DataSourceRequest]DataSourceRequest request, RecruitmentApplicantPassSetting passSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentApplicantPassSetting
                {
                    RecruitmentApplicantPassSettingId = passSetting.RecruitmentApplicantPassSettingId,
                    VacancyId = passSetting.VacancyId,
                    TotalValue = passSetting.TotalValue,
                    PassScore = passSetting.PassScore,
                    Remark = passSetting.Remark,
                };

                db.RecruitmentApplicantPassSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { passSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PassSetting_Destroy([DataSourceRequest]DataSourceRequest request, RecruitmentApplicantPassSetting passSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentApplicantPassSetting
                {
                    RecruitmentApplicantPassSettingId = passSetting.RecruitmentApplicantPassSettingId,
                    VacancyId = passSetting.VacancyId,
                    TotalValue = passSetting.TotalValue,
                    PassScore = passSetting.PassScore,
                    Remark = passSetting.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Recruitment general setting of " + passSetting.RecruitmentApplicantPassSettingId );
                #endregion
                db.RecruitmentApplicantPassSettings.Attach(entity);
                db.RecruitmentApplicantPassSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { passSetting }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCascadeAttributes(string Reference)
        {
            List<string> attributes = new List<string>();
            switch (Reference)
            {


                case "NOOF_PROMOTION_PER_YEAR":

           
                          
                    break;
                

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();

            attributes =
                  Enum.GetValues(typeof(PromotionSetting))
                      .Cast<PromotionSetting>()
                      .Select(v => v.ToString())
                      .ToList();


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
