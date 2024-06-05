using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetRegistration
{
    public class FleetBoloRegistrationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetBoloRegistration
        public ActionResult Index()
        {
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber,
                Value = s.FleetsID
            }).ToList();
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FleetBoloRegistration> bolo = db.FleetBoloRegistrations;
            DataSourceResult result = bolo.ToDataSourceResult(request, model => new FleetBoloRegistration
            {
                FleetBoloRegistrationId = model.FleetBoloRegistrationId,
                FleetId = model.FleetId,
                BoloExpireDate = model.BoloExpireDate,
                BoloRegistrationDate = model.BoloRegistrationDate,
                Remark = model.Remark,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        {
            var bolo = db.FleetBoloRegistrations.Where(x => x.FleetId == model.FleetId).ToList();
            
            if (bolo.Any())
            {
                ModelState.AddModelError("", "This vehicle already exists");
            }
            if(model.BoloRegistrationDate == model.BoloExpireDate)
            {
                ModelState.AddModelError("", "Registration Date and Expiration Date can't be equal");
            }
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;

                db.FleetBoloRegistrations.Add(model);
                db.SaveChanges();
               
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        {
            var bolo = db.FleetBoloRegistrations.Where(x => x.FleetId == model.FleetId).ToList();

            if (bolo.Any())
            {
                ModelState.AddModelError("", "This vehicle already exists");
            }
            if (model.BoloRegistrationDate == model.BoloExpireDate)
            {
                ModelState.AddModelError("", "Registration Date and Expiration Date can't be equal");
            }
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        {
            if (ModelState.IsValid)
            {

                var bolo = db.FleetBoloRegistrations.Find(model.FleetBoloRegistrationId);
                db.FleetBoloRegistrations.Remove(bolo);
                db.SaveChanges();

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}