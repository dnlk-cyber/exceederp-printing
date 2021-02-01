using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
    public class RecruitmentSettingExternalController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var RecruitmentSetting = Enum.GetValues(typeof(RecruitmentSetting))
                .Cast<RecruitmentSetting>()
                .Select(v => v.ToString())
                .ToList();
            var RecruitmentSettings = RecruitmentSetting.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RecruitmentSetting"] = RecruitmentSettings;
        }
        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "Recruitment_External");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new
            {
                ConfigurationID = configuration.ConfigurationId,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.Configurations.Where(x => x.Category == "Recruitment_External" && x.Reference == "Recruitment" && x.Attribute == configuration.Attribute).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "External Recruitment Setting :-" + configuration.Attribute + " is already registered!!");
                }
                else
                {
                    var entity = new Configuration
                    {
                        Category = "Recruitment_External",
                        Reference = "Recruitment",
                        Attribute = configuration.Attribute,
                        CurrentValue = configuration.CurrentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark
                    };

                    db.Configurations.Add(entity);
                    db.SaveChanges();
                    configuration.ConfigurationId = entity.ConfigurationId;

                    #region operation log
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "External Setting ", "Success", User.Identity.Name, User.Identity.Name + " add External Setting " + entity.Reference + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                    
                    #endregion
                }
            }
            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = "Recruitment_External",
                    Reference = "Recruitment",
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark
                };

                db.Configurations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (name != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "External Setting ", "Success", User.Identity.Name, name.EmployeeCode + " add External Setting " + entity.Reference + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                }


                #endregion
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Destroy([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = "Recruitment_External",
                    Reference = "Recruitment",
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Recruitment external setting of " + configuration.Attribute);
                #endregion
                db.Configurations.Attach(entity);
                db.Configurations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
