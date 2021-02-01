﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize(Roles = HR_Admin)]
    public class CountryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Countries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Country> countries = db.Countries;
            DataSourceResult result = countries.ToDataSourceResult(request, country => new {
                CountryId = country.CountryId,
                Name = country.Name,
                PolticalName = country.PolticalName,
                Continent = country.Continent,
                TelephoneCode = country.TelephoneCode,
                TimeZone = country.TimeZone,
                Nationality = country.Nationality,
                CountryCode = country.CountryCode,
                Remark = country.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Countries_Create([DataSourceRequest]DataSourceRequest request, Country country)
        {
            if (ModelState.IsValid)
            {
                var checkExstance = db.Countries.ToList();
                var jobName = country.Name;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Country already exists in the database!!");
                }
                else
                {
                    var entity = new Country
                    {
                        Name = country.Name,
                        PolticalName = country.PolticalName,
                        Continent = country.Continent,
                        TelephoneCode = country.TelephoneCode,
                        TimeZone = country.TimeZone,
                        Nationality = country.Nationality,
                        CountryCode = country.CountryCode,
                        Remark = country.Remark
                    };

                    db.Countries.Add(entity);
                    db.SaveChanges();
                    country.CountryId = entity.CountryId;
                }
            }

            return Json(new[] { country }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Countries_Update([DataSourceRequest]DataSourceRequest request, Country country)
        {
            if (ModelState.IsValid)
            {
                var entity = new Country
                {
                    CountryId = country.CountryId,
                    Name = country.Name,
                    PolticalName = country.PolticalName,
                    Continent = country.Continent,
                    TelephoneCode = country.TelephoneCode,
                    TimeZone = country.TimeZone,
                    Nationality = country.Nationality,
                    CountryCode = country.CountryCode,
                    Remark = country.Remark
                };

                db.Countries.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { country }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Countries_Destroy([DataSourceRequest]DataSourceRequest request, Country country)
        {
            if (ModelState.IsValid)
            {
                var entity = new Country
                {
                    CountryId = country.CountryId,
                    Name = country.Name,
                    PolticalName = country.PolticalName,
                    Continent = country.Continent,
                    TelephoneCode = country.TelephoneCode,
                    TimeZone = country.TimeZone,
                    Nationality = country.Nationality,
                    CountryCode = country.CountryCode,
                    Remark = country.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Country" + country.Name);
                #endregion
                db.Countries.Attach(entity);
                db.Countries.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { country }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
