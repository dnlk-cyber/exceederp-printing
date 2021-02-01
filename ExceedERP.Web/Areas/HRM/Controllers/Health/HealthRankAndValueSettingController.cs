using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class HealthRankAndValueSettingController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        public static DateTime RenewalDate { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Rank"] = db.Ranks.ToList();

        }
        
        public JsonResult HealthRankAndValueSetting_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healths = db.HealthRankAndValueSettings.ToList();
            var accidents = healths.Select(health => new HealthRankAndValueSetting
            {

                HealthRankAndValueSettingId = health.HealthRankAndValueSettingId,
                RankId = health.RankId,
                CurrentValue = health.CurrentValue               


            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult HealthRankAndValueSetting_Create([DataSourceRequest] DataSourceRequest request, HealthRankAndValueSetting health)
        {
            
            if(ModelState.IsValid)
            {
                var entity = new HealthRankAndValueSetting
                {
                    HealthRankAndValueSettingId = health.HealthRankAndValueSettingId,
                    RankId = health.RankId,
                    CurrentValue = health.CurrentValue
                };
                db.HealthRankAndValueSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
                var rank = db.Ranks.FirstOrDefault(x=>x.RankId == entity.RankId);
                if(rank != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Value by rank setting", "Success", User.Identity.Name, User.Identity.Name + " add health value setting for rank " + rank.RankName + " at date " + DateTime.Today, Modules.HealthAndInsurance);
                }

                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValueSetting_Update([DataSourceRequest] DataSourceRequest request, HealthRankAndValueSetting health)
        {

           
            if (ModelState.IsValid)
            {
                var entity = new HealthRankAndValueSetting
                {
                    HealthRankAndValueSettingId = health.HealthRankAndValueSettingId,
                    RankId = health.RankId,
                    CurrentValue = health.CurrentValue
                };
                db.HealthRankAndValueSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
                var rank = db.Ranks.FirstOrDefault(x => x.RankId == entity.RankId);
                if (rank != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Value by rank setting", "Success", User.Identity.Name, User.Identity.Name + " update health value setting of rank " + rank.RankName + " at date " + DateTime.Today, Modules.HealthAndInsurance);
                }

                #endregion

            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValueSetting_Destroy([DataSourceRequest] DataSourceRequest request, HealthRankAndValueSetting health)
        {

            if (ModelState.IsValid)
            {
                var entity = new HealthRankAndValueSetting
                {
                    HealthRankAndValueSettingId = health.HealthRankAndValueSettingId,
                    RankId = health.RankId,
                    CurrentValue = health.CurrentValue
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted health setting " + health.RankId);
                #endregion
                db.HealthRankAndValueSettings.Attach(entity);
                db.HealthRankAndValueSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }
    }
}