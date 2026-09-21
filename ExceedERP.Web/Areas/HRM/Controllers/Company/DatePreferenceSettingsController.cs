using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin)]
    public class DatePreferenceSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult DatePreferenceSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DatePreferenceSetting> datePreferenceSettings = db.DatePreferenceSettings;
            DataSourceResult result = datePreferenceSettings.ToDataSourceResult(request, datePreferenceSetting => new
            {
                DatePreferenceSettingId = datePreferenceSetting.DatePreferenceSettingId,
                UseEthiopianDate = datePreferenceSetting.UseEthiopianDate,
                Remark = datePreferenceSetting.Remark,
                DateCreated = datePreferenceSetting.DateCreated,
                LastModified = datePreferenceSetting.LastModified,
                CreatedBy = datePreferenceSetting.CreatedBy,
                ModifiedBy = datePreferenceSetting.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DatePreferenceSettings_Create([DataSourceRequest]DataSourceRequest request, DatePreferenceSetting datePreferenceSetting)
        {
            ModelState.Clear();
            var exist = db.DatePreferenceSettings.FirstOrDefault();
            if(exist != null)
            {
                ModelState.AddModelError("", "You can't Add More");
            }

            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(datePreferenceSetting);
                db.DatePreferenceSettings.Add(entity);
                db.SaveChanges();
                datePreferenceSetting.DatePreferenceSettingId = entity.DatePreferenceSettingId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        
                    UserHelper.OperationLog(Request.UserHostAddress, "Probtion Setting ", "Success", User.Identity.Name, User.Identity.Name + " Add new Probtion Setting  " + entity.DatePreferenceSettingId,Modules.HRM);

                        

                #endregion
               
                
            }

            return Json(new[] { datePreferenceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DatePreferenceSettings_Update([DataSourceRequest]DataSourceRequest request, DatePreferenceSetting datePreferenceSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(datePreferenceSetting);
                db.DatePreferenceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);


                UserHelper.OperationLog(Request.UserHostAddress, "Probation Setting ", "Success", User.Identity.Name, User.Identity.Name + " Updated Probation Setting Info of  created by Employee Id" + empId.ToString(),Modules.HRM);

                #endregion

            }

            return Json(new[] { datePreferenceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DatePreferenceSettings_Destroy([DataSourceRequest]DataSourceRequest request, DatePreferenceSetting datePreferenceSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(datePreferenceSetting);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Probation Setting " + datePreferenceSetting.DatePreferenceSettingId);
                #endregion
                db.DatePreferenceSettings.Attach(entity);
                db.DatePreferenceSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { datePreferenceSetting }.ToDataSourceResult(request, ModelState));
        }

        private DatePreferenceSetting GetMappedObject(DatePreferenceSetting datePreferenceSetting)
        {
            var entity = new DatePreferenceSetting
            {
                DatePreferenceSettingId = datePreferenceSetting.DatePreferenceSettingId,
                UseEthiopianDate = datePreferenceSetting.UseEthiopianDate,
                DateCreated = datePreferenceSetting.DateCreated,
                LastModified = datePreferenceSetting.LastModified,
                CreatedBy = datePreferenceSetting.CreatedBy,
                ModifiedBy = datePreferenceSetting.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
