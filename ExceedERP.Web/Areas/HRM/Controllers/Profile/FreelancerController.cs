﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class FreelancerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        

        public ActionResult Freelancers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Freelancer> freelancers = db.Freelancers.Where(x=>x.EmployeeId == id);
            DataSourceResult result = freelancers.ToDataSourceResult(request, freelancer => new {
                FreelancerId = freelancer.FreelancerId,
                EmployeeId = freelancer.EmployeeId,
                From = freelancer.From,
                To = freelancer.To,
                FromAm = freelancer.FromAm,
                ToAm = freelancer.ToAm
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Freelancers_Create([DataSourceRequest]DataSourceRequest request, Freelancer freelancer, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new Freelancer
                {
                    EmployeeId = id,
                    From = freelancer.From,
                    To = freelancer.To,
                    FromAm = freelancer.FromAm,
                    ToAm = freelancer.ToAm
                };

                db.Freelancers.Add(entity);
                db.SaveChanges();
                freelancer.FreelancerId = entity.FreelancerId;
            }

            return Json(new[] { freelancer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Freelancers_Update([DataSourceRequest]DataSourceRequest request, Freelancer freelancer)
        {
            if (ModelState.IsValid)
            {
                var entity = new Freelancer
                {
                    FreelancerId = freelancer.FreelancerId,
                    EmployeeId = freelancer.EmployeeId,
                    From = freelancer.From,
                    To = freelancer.To,
                    FromAm = freelancer.FromAm,
                    ToAm = freelancer.ToAm
                };

                db.Freelancers.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { freelancer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Freelancers_Destroy([DataSourceRequest]DataSourceRequest request, Freelancer freelancer)
        {
            if (ModelState.IsValid)
            {
                var entity = new Freelancer
                {
                    FreelancerId = freelancer.FreelancerId,
                    EmployeeId = freelancer.EmployeeId,
                    From = freelancer.From,
                    To = freelancer.To,
                    FromAm = freelancer.FromAm,
                    ToAm = freelancer.ToAm
                };

                db.Freelancers.Attach(entity);
                db.Freelancers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { freelancer }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
