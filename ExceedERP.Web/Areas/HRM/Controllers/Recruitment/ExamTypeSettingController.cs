using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ExamTypeSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ExamTypeSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExamTypeSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ExamTypeSetting> settings = db.ExamTypeSettings;
            DataSourceResult result = settings.ToDataSourceResult(request, setting => new
            {
                ExamTypeSettingId = setting.ExamTypeSettingId,
                ExamType = setting.ExamType,
                SettingVlaue = setting.SettingVlaue,
                ExamSource = setting.ExamSource,
                Remark = setting.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExamTypeSetting_Create([DataSourceRequest]DataSourceRequest request, ExamTypeSetting setting)
        {
            ModelState.Clear();
            var checkExit = db.ExamTypeSettings.FirstOrDefault(x=>x.ExamSource == setting.ExamSource && x.ExamType == setting.ExamType);
            if(checkExit != null)
            {
                ModelState.AddModelError("","this setting is already exist");
            }
            if (ModelState.IsValid)
            {
               
                    var entity = new ExamTypeSetting
                    {
                        ExamTypeSettingId = setting.ExamTypeSettingId,
                        ExamType = setting.ExamType,
                        SettingVlaue = setting.SettingVlaue,
                        ExamSource = setting.ExamSource,
                        Remark = setting.Remark
                    };

                    db.ExamTypeSettings.Add(entity);
                    db.SaveChanges();
                
            }
            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExamTypeSetting_Update([DataSourceRequest]DataSourceRequest request, ExamTypeSetting setting)
        {
            ModelState.Clear();
            var checkExit = db.ExamTypeSettings.FirstOrDefault(x => x.ExamSource == setting.ExamSource && x.ExamType == setting.ExamType && x.ExamTypeSettingId != setting.ExamTypeSettingId);
            if (checkExit != null)
            {
                ModelState.AddModelError("", "this setting is already exist");
            }
            if (ModelState.IsValid)
            {
                var entity = new ExamTypeSetting
                {
                    ExamTypeSettingId = setting.ExamTypeSettingId,
                    ExamType = setting.ExamType,
                    SettingVlaue = setting.SettingVlaue,
                    ExamSource = setting.ExamSource,
                    Remark = setting.Remark
                };

                db.ExamTypeSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExamTypeSetting_Destroy([DataSourceRequest]DataSourceRequest request, ExamTypeSetting setting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ExamTypeSetting
                {
                    ExamTypeSettingId = setting.ExamTypeSettingId,
                    ExamType = setting.ExamType,
                    SettingVlaue = setting.SettingVlaue,
                    ExamSource = setting.ExamSource,
                    Remark = setting.Remark
                };
                #region delete log
              //  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Recruitment setting of " + configuration.Attribute);
                #endregion
                db.ExamTypeSettings.Attach(entity);
                db.ExamTypeSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
