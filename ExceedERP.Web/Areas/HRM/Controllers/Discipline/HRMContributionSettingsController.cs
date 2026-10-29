using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
    public class HRMContributionSettingsController : BaseController
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Contributiontype"] = db.ContributionTypes.Select(x => new { x.ContributionTypeId, Name = x.Description + " " + x.ContributionCategory.ToString() }).AsQueryable();

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
        public JsonResult HRMContributionSettings_Create([DataSourceRequest] DataSourceRequest request, HRMContributionSetting health)
        {
            var exist = db.HRMContributionSettings.Any();
            if (exist)
            {
                ModelState.AddModelError("eexist", " already Setted Once you can modify it");
            }
            if(ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HRMContributionSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Health benefit setting", "Success", User.Identity.Name, User.Identity.Name + " add health benefit setting for  " + " at date " + DateTime.Today, Modules.Health);
                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HRMContributionSettings_Update([DataSourceRequest] DataSourceRequest request, HRMContributionSetting health)
        {

            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HRMContributionSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Health benefit setting", "Success", User.Identity.Name, User.Identity.Name + " update health benefit setting of  " + health.Description + " at date " + DateTime.Today, Modules.Health);
                

                #endregion

            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
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