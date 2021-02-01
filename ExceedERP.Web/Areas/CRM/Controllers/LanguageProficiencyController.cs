﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain.Common;
 using ExceedERP.Core.Domain.Common.HRM;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class LanguageProficiencyController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LanguageOfProficiencies_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LanguageProficiency> languageofproficiencies = db.LanguageOfProficiencies.Where( l=> l.Person == id.ToString( ));
            DataSourceResult result = languageofproficiencies.ToDataSourceResult(request, languageProficiency => new {
                LanguageProficiencyID = languageProficiency.LanguageProficiencyID,
                Person = languageProficiency.Person,
                Languge = languageProficiency.Languge,
                Reading = languageProficiency.Reading,
                Listening = languageProficiency.Listening,
                Writing = languageProficiency.Writing,
                Speaking = languageProficiency.Speaking,
                CreatedBy = languageProficiency.CreatedBy,
                DateCreated = languageProficiency.DateCreated,
                LastModified = languageProficiency.LastModified,
                ModifiedBy = languageProficiency.ModifiedBy,
                OrgBranchName = languageProficiency.OrgBranchName,
                Remark = languageProficiency.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LanguageOfProficiencies_Create([DataSourceRequest]DataSourceRequest request, LanguageProficiency languageProficiency, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new LanguageProficiency
                {
                    Person = id.ToString( ),
                    Languge = languageProficiency.Languge,
                    Reading = languageProficiency.Reading,
                    Listening = languageProficiency.Listening,
                    Writing = languageProficiency.Writing,
                    Speaking = languageProficiency.Speaking,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = languageProficiency.Remark
                };

                db.LanguageOfProficiencies.Add(entity);
                db.SaveChanges();
                languageProficiency.LanguageProficiencyID = entity.LanguageProficiencyID;
            }

            return Json(new[] { languageProficiency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LanguageOfProficiencies_Update([DataSourceRequest]DataSourceRequest request, LanguageProficiency languageProficiency)
        {
            if (ModelState.IsValid)
            {
                var entity = new LanguageProficiency
                {
                    LanguageProficiencyID = languageProficiency.LanguageProficiencyID,
                    Person = languageProficiency.Person,
                    Languge = languageProficiency.Languge,
                    Reading = languageProficiency.Reading,
                    Listening = languageProficiency.Listening,
                    Writing = languageProficiency.Writing,
                    Speaking = languageProficiency.Speaking,
                    Remark = languageProficiency.Remark
                };

                db.LanguageOfProficiencies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { languageProficiency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LanguageOfProficiencies_Destroy([DataSourceRequest]DataSourceRequest request, LanguageProficiency languageProficiency)
        {
            if (ModelState.IsValid)
            {
                var entity = new LanguageProficiency
                {
                    LanguageProficiencyID = languageProficiency.LanguageProficiencyID,
                    Person = languageProficiency.Person,
                    Languge = languageProficiency.Languge,
                    Reading = languageProficiency.Reading,
                    Listening = languageProficiency.Listening,
                    Writing = languageProficiency.Writing,
                    Speaking = languageProficiency.Speaking,
                    Remark = languageProficiency.Remark
                };

                db.LanguageOfProficiencies.Attach(entity);
                db.LanguageOfProficiencies.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { languageProficiency }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
