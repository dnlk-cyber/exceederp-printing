﻿using System;



﻿using System.Data.Entity;
﻿using System.Linq;

using System.Web.Mvc;

﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize(Roles = HR_Admin)]
    public class LanguagesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            var countries = db.Countries;
            ViewData["CountryList"] = countries;
            var LanguageTypes = Enum.GetValues(typeof(LanguageType))
                     .Cast<LanguageType>()
                   
                     .ToList();
            var businessList = LanguageTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LanguageTypes"] = businessList;
            return View();
        }

        public ActionResult Languages_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Language> languages = db.Languages;
            DataSourceResult result = languages.ToDataSourceResult(request, language => new {
                LanguageId = language.LanguageId,
                Description = language.Description,
                LanguageType = language.LanguageType,
                CountryId = language.CountryId,
                Remark = language.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Languages_Create([DataSourceRequest]DataSourceRequest request, Language language)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExstance = db.Languages.ToList();
                 var jobName = language.Description ;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description .ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Country is already exists in the database!!");
                }
                else
                {
                    var entity = new Language
                    {
                        Description = language.Description,
                        LanguageType = language.LanguageType,
                        CountryId = language.CountryId,
                        Remark = language.Remark
                    };

                    db.Languages.Add(entity);
                    db.SaveChanges();
                    language.LanguageId = entity.LanguageId;
                }
            }

            return Json(new[] { language }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Languages_Update([DataSourceRequest]DataSourceRequest request, Language language)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Language
                {
                    LanguageId = language.LanguageId,
                    Description = language.Description,
                    LanguageType  = language.LanguageType ,
                    CountryId = language.CountryId,
                    Remark = language.Remark
                };

                db.Languages.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { language }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Languages_Destroy([DataSourceRequest]DataSourceRequest request, Language language)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Language
                {
                    LanguageId = language.LanguageId,
                    Description = language.Description,
                    LanguageType  = language.LanguageType ,
                    CountryId = language.CountryId,
                    Remark = language.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Language " + language.Description);
                #endregion
                db.Languages.Attach(entity);
                db.Languages.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { language }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
