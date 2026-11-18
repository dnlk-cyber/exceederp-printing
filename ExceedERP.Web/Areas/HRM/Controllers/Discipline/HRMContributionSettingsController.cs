using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRDisciplineUser)]
    public class HRMContributionSettingsController : BaseController
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Contributiontype"] = db.ContributionTypes
                .Where(x => x.ContributionCategory == ContributionCategory.FIXED)
                .Select(x => new { x.ContributionTypeId, Name = x.Description }).ToList();

            return View();
        }
       
        public JsonResult HRMContributionSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var hRMContributionSettings = db.HRMContributionSettings.ToList();
            var accidents = hRMContributionSettings.Select(healthContribution => new HRMContributionSetting
            {

                HRMContributionSettingId = healthContribution.HRMContributionSettingId,
                ContributionTypeId = healthContribution.ContributionTypeId,
                DateCreated = healthContribution.DateCreated,
                CreatedBy = healthContribution.CreatedBy,
                LastModified = healthContribution.LastModified,
                ModifiedBy = healthContribution.ModifiedBy,
                Description = healthContribution.Description


            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult HRMContributionSettings_Create([DataSourceRequest] DataSourceRequest request, HRMContributionSetting health)
        {
            ValidateContributionType(health);
            var exist = db.HRMContributionSettings.Any();
            if (exist)
            {
                ModelState.AddModelError("", "A discipline contribution is already configured. Edit the existing setting.");
            }
            if(ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HRMContributionSettings.Add(entity);
                db.SaveChanges();
                health.HRMContributionSettingId = entity.HRMContributionSettingId;

                #region operation log                
              
                UserHelper.OperationLog(Request.UserHostAddress, "Discipline contribution setting", "Success", User.Identity.Name, "Created the discipline payroll contribution setting.", Modules.HRM);
                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult HRMContributionSettings_Update([DataSourceRequest] DataSourceRequest request, HRMContributionSetting health)
        {
            ValidateContributionType(health);

            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HRMContributionSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
               
                UserHelper.OperationLog(Request.UserHostAddress, "Discipline contribution setting", "Success", User.Identity.Name, "Updated the discipline payroll contribution setting.", Modules.HRM);
                

                #endregion

            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult HRMContributionSettings_Destroy([DataSourceRequest] DataSourceRequest request, HRMContributionSetting health)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted health benefit setting " );
                #endregion
                db.HRMContributionSettings.Attach(entity);
                db.HRMContributionSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }
        private void ValidateContributionType(HRMContributionSetting setting)
        {
            if (!db.ContributionTypes.Any(x => x.ContributionTypeId == setting.ContributionTypeId
                && x.ContributionCategory == ContributionCategory.FIXED))
                ModelState.AddModelError("ContributionTypeId", "Select a fixed contribution type defined in Payroll.");
        }

        private HRMContributionSetting GetMappedObject(HRMContributionSetting healthContribution)
        {
            var entity = new HRMContributionSetting
            {
                HRMContributionSettingId = healthContribution.HRMContributionSettingId,
                ContributionTypeId = healthContribution.ContributionTypeId,
                DateCreated = healthContribution.DateCreated,
                CreatedBy = healthContribution.CreatedBy,
                LastModified = healthContribution.LastModified,
                ModifiedBy = healthContribution.ModifiedBy,
                Description = healthContribution.Description
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
