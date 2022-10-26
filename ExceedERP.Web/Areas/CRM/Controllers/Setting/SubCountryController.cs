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

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class SubCountryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["CountryList"] = db.Countries.AsQueryable();
            ViewData["SubCountryList"] = db.SubCountries.ToList();
            return View();
        }

        public ActionResult SubCountries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SubCountry> subcountries = db.SubCountries;
            DataSourceResult result = subcountries.ToDataSourceResult(request, subCountry => new {
                SubCountryID = subCountry.SubCountryID,
                CountryID = subCountry.CountryID,
                Name = subCountry.Name,
                AlternativeName = subCountry.AlternativeName,
                Parent = subCountry.Parent,
                CreatedBy = subCountry.CreatedBy,
                DateCreated = subCountry.DateCreated,
                LastModified = subCountry.LastModified,
                ModifiedBy = subCountry.ModifiedBy,
                OrgBranchName = subCountry.OrgBranchName,
                Remark = subCountry.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubCountries_Create([DataSourceRequest]DataSourceRequest request, SubCountry subCountry)
        {
             List<SubCountry> countrynames = db.SubCountries.Where(s => s.Name == subCountry.Name).ToList();
              List<SubCountry> countryAlternativeNames = db.SubCountries.Where(s => s.AlternativeName == subCountry.AlternativeName).ToList();
              if (countrynames.Any())
              {
                  ModelState.AddModelError("DuplicateEntire", "SubCountry with Name Already Exist");
              }
              else if (countryAlternativeNames.Any())
              {
                  ModelState.AddModelError("DuplicateEntire", "SubCountry with Alternative Name Already Exist");
              }

              else
              {
                  if (ModelState.IsValid)
                  {
                      var entity = new SubCountry
                      {
                          CountryID = subCountry.CountryID,
                          Name = subCountry.Name,
                          AlternativeName = subCountry.AlternativeName,
                          Parent = subCountry.Parent,
                          Remark = subCountry.Remark
                      };

                      db.SubCountries.Add(entity);
                      db.SaveChanges();
                      subCountry.SubCountryID = entity.SubCountryID;
                  }
              }
            return Json(new[] { subCountry }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubCountries_Update([DataSourceRequest]DataSourceRequest request, SubCountry subCountry)
        {
             List<SubCountry> countrynames = db.SubCountries.Where(s => s.Name == subCountry.Name).ToList();
              List<SubCountry> countryAlternativeNames = db.SubCountries.Where(s => s.AlternativeName == subCountry.AlternativeName).ToList();
              if (countrynames.Any())
              {
                  ModelState.AddModelError("DuplicateEntire", "SubCountry with Name Already Exist");
              }
              else if (countryAlternativeNames.Any())
              {
                  ModelState.AddModelError("DuplicateEntire", "SubCountry with Alternative Name Already Exist");
              }

              else
              {
                  if (ModelState.IsValid)
                  {
                      var entity = new SubCountry
                      {
                          SubCountryID = subCountry.SubCountryID,
                          CountryID = subCountry.CountryID,
                          Name = subCountry.Name,
                          AlternativeName = subCountry.AlternativeName,
                          Parent = subCountry.Parent,
                          Remark = subCountry.Remark
                      };

                      db.SubCountries.Attach(entity);
                      db.Entry(entity).State = EntityState.Modified;
                      db.SaveChanges();
                  }
              }
            return Json(new[] { subCountry }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubCountries_Destroy([DataSourceRequest]DataSourceRequest request, SubCountry subCountry)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubCountry
                {
                    SubCountryID = subCountry.SubCountryID,
                    CountryID = subCountry.CountryID,
                    Name = subCountry.Name,
                    AlternativeName = subCountry.AlternativeName,
                    Parent = subCountry.Parent,
                    Remark = subCountry.Remark
                };

                db.SubCountries.Attach(entity);
                db.SubCountries.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Sub Country number " + subCountry.SubCountryID);
            }

            return Json(new[] { subCountry }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
