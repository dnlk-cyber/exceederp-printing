using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
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

namespace ExceedERP.Web.Areas.Health.Controllers.Setting
{
    public class PaymentCoverageSettingsController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        public static DateTime RenewalDate { get; set; }
        public ActionResult Index()
        {
            //PopulateViewData();
            return View();
        }
        //public void PopulateViewData()
        //{
        //    ViewData["Rank"] = db.Ranks.ToList();

        //}
        
        public JsonResult PaymentCoverageSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var paymentCoverages = db.PaymentCoverageSettings.ToList();
            var coverageSettings = paymentCoverages.Select(paymentCoverage => new PaymentCoverageSetting
            {

                PaymentCoverageSettingId = paymentCoverage.PaymentCoverageSettingId,
                Payment = paymentCoverage.Payment,
                Description = paymentCoverage.Description               


            }).ToList();

            return Json(coverageSettings.ToDataSourceResult(request));
        }
        public JsonResult PaymentCoverageSettings_Create([DataSourceRequest] DataSourceRequest request, PaymentCoverageSetting paymentCoverage)
        {
            
            if(ModelState.IsValid)
            {
                var entity = new PaymentCoverageSetting
                {
                    PaymentCoverageSettingId = paymentCoverage.PaymentCoverageSettingId,
                    Payment = paymentCoverage.Payment,
                    Description = paymentCoverage.Description
                };
                db.PaymentCoverageSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Payment Coverage setting", "Success", User.Identity.Name, User.Identity.Name + " add Payment Coverage setting for "  + " at date " + DateTime.Today, Modules.Health);
                

                #endregion
            }

            return Json(new[] { paymentCoverage }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PaymentCoverageSettings_Update([DataSourceRequest] DataSourceRequest request, PaymentCoverageSetting paymentCoverage)
        {

           
            if (ModelState.IsValid)
            {
                var entity = new PaymentCoverageSetting
                {
                    PaymentCoverageSettingId = paymentCoverage.PaymentCoverageSettingId,
                    Payment = paymentCoverage.Payment,
                    Description = paymentCoverage.Description
                };
                db.PaymentCoverageSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
               
                    UserHelper.OperationLog(Request.UserHostAddress, "PaymentCoverage setting", "Success", User.Identity.Name, User.Identity.Name + " update Payment Coverage setting  "  + " at date " + DateTime.Today, Modules.Health);
                

                #endregion

            }

            return Json(new[] { paymentCoverage }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValueSetting_Destroy([DataSourceRequest] DataSourceRequest request, PaymentCoverageSetting paymentCoverage)
        {

            if (ModelState.IsValid)
            {
                var entity = new PaymentCoverageSetting
                {
                    PaymentCoverageSettingId = paymentCoverage.PaymentCoverageSettingId,
                    Payment = paymentCoverage.Payment,
                    Description = paymentCoverage.Description
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment Coverage setting " );
                #endregion
                db.PaymentCoverageSettings.Attach(entity);
                db.PaymentCoverageSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { paymentCoverage }.ToDataSourceResult(request));
        }
    }
}