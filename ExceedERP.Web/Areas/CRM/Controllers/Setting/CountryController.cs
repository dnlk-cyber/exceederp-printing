using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class CountryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Countries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Country> countries = db.Countries;
            DataSourceResult result = countries.ToDataSourceResult(request, country => new Country
            {
                CountryId = country.CountryId,
                Name = country.Name,
                PolticalName = country.PolticalName,
                Continent = country.Continent,
                TelephoneCode = country.TelephoneCode,
                TimeZone = country.TimeZone,
                Nationality = country.Nationality,
                CountryCode = country.CountryCode,
                //CreatedBy = country.CreatedBy,
                //DateCreated = country.DateCreated,
                //LastModified = country.LastModified,
                //ModifiedBy = country.ModifiedBy,
                //OrgBranchName = country.OrgBranchName,
                Remark = country.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Countries_Create([DataSourceRequest]DataSourceRequest request, Country country)
        {
            List<Country> countrynames = db.Countries.Where(s => s.Name == country.Name).ToList();
            List<Country> countrycodes = db.Countries.Where(s => s.Name == country.Name).ToList();
            if (countrynames.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Country with Name Already Exist");
            }
            else if (countrynames.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Country with Code Already Exist");
            }

            else
            {
                if (ModelState.IsValid)
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
                        //CreatedBy = User.Identity.Name,
                        //DateCreated = DateTime.Now,
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
            List<Country> countrynames = db.Countries.Where(s => s.Name == country.Name).ToList();
            List<Country> countrycodes = db.Countries.Where(s => s.Name == country.Name).ToList();
            if (countrynames.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Country with Name Already Exist");
            }
            else if (countrynames.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Country with Code Already Exist");
            }

            else
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

                db.Countries.Attach(entity);
                db.Countries.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Country number " + country.CountryId);
		
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
