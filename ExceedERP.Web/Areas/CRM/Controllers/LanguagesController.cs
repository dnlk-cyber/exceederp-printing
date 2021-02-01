﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     

﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
﻿using ExceedERP.DataAccess.Context;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
           [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class LanguagesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var countries = db.Countries;
            ViewData["CountryList"] = countries;
            var LanguageTypes = Enum.GetValues(typeof(ExceedERP.Core.Domain.Common.LanguageType))
                     .Cast<ExceedERP.Core.Domain.Common.LanguageType>()
                   
                     .ToList();
            var businessList = LanguageTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LanguageTypes"] = businessList;
            return View();
        }

        public ActionResult Languages_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Language> languages = db.Languages;
            DataSourceResult result = languages.ToDataSourceResult(request, language => new {
                LanguageID = language.LanguageId,
                Description = language.Description,
                LanguageType = language.LanguageType,
                Country = language.Country,
            
                Remark = language.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Languages_Create([DataSourceRequest]DataSourceRequest request, Language language)
        {
            if (ModelState.IsValid)
            {
                var entity = new Language
                {
                    Description = language.Description,
                    LanguageType  = language.LanguageType ,
                    Country = language.Country,
                  
                    Remark = language.Remark
                };

                db.Languages.Add(entity);
                db.SaveChanges();
                language.LanguageId = entity.LanguageId;
            }

            return Json(new[] { language }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Languages_Update([DataSourceRequest]DataSourceRequest request, Language language)
        {
            if (ModelState.IsValid)
            {
                var entity = new Language
                {
                    LanguageId = language.LanguageId,
                    Description = language.Description,
                    LanguageType  = language.LanguageType ,
                    Country = language.Country,
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
            if (ModelState.IsValid)
            {
                var entity = new Language
                {
                    LanguageId = language.LanguageId,
                    Description = language.Description,
                    LanguageType  = language.LanguageType ,
                    Country = language.Country,
                    Remark = language.Remark
                };

                db.Languages.Attach(entity);
                db.Languages.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Language number " + language.LanguageId);
      
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
