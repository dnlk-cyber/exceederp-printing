using System;
using System.Collections.Generic;
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
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class CheckUpController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/CheckUp
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckUp_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CheckUp> checkups = db.CheckUps;
            DataSourceResult result = checkups.ToDataSourceResult(request, check => new {
                CheckUpId = check.CheckUpId,
                HealthCheck = check.HealthCheck,
                CriminalCheck = check.CriminalCheck,
                Offer = check.Offer,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckUp_Create([DataSourceRequest]DataSourceRequest request, CheckUp checkup)
        {
            if (ModelState.IsValid)
            {
                
                    var entity = new CheckUp
                    {
                        HealthCheck = checkup.HealthCheck,
                        CriminalCheck = checkup.CriminalCheck,
                        Offer = checkup.Offer
                    };

                    db.CheckUps.Add(entity);
                    db.SaveChanges();
                    checkup.CheckUpId = entity.CheckUpId;
                
            }

            return Json(new[] { checkup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckUp_Update([DataSourceRequest]DataSourceRequest request, CheckUp checkup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new CheckUp
                {
                    CheckUpId = checkup.CheckUpId,
                    HealthCheck = checkup.HealthCheck,
                    CriminalCheck = checkup.CriminalCheck,
                    Offer = checkup.Offer
                };

                db.CheckUps.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { checkup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckUp_Destroy([DataSourceRequest]DataSourceRequest request, CheckUp checkup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new CheckUp
                {
                    CheckUpId = checkup.CheckUpId,
                    HealthCheck = checkup.HealthCheck,
                    CriminalCheck = checkup.CriminalCheck,
                    Offer = checkup.Offer
                };
                db.CheckUps.Attach(entity);
                db.CheckUps.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { checkup }.ToDataSourceResult(request, ModelState));
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
