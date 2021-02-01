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
     [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
    public class LanguageProficiencyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Profile_Admin = "HR_Profile_Admin";
        private const string HR_Profile_User = "HR_Profile_User";
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            return View();
        }
           [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
        public ActionResult LanguageOfProficiencies_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LanguageProficiency> languageofproficiencies = db.LanguageOfProficiencies.Where(l => l.EmployeeId == id);
            DataSourceResult result = languageofproficiencies.ToDataSourceResult(request, languageProficiency => new {
                LanguageProficiencyId = languageProficiency.LanguageProficiencyId,
                EmployeeId = languageProficiency.EmployeeId,
                LanguageId = languageProficiency.LanguageId,
                Reading = languageProficiency.Reading,
                Listening = languageProficiency.Listening,
                Writing = languageProficiency.Writing,
                Speaking = languageProficiency.Speaking,
                Remark = languageProficiency.Remark,
                DateCreated = languageProficiency.DateCreated,
                LastModified = languageProficiency.LastModified,
                CreatedBy = languageProficiency.CreatedBy,
                ModifiedBy = languageProficiency.ModifiedBy
            });

            return Json(result);
        }
          [Authorize(Roles =HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LanguageOfProficiencies_Create([DataSourceRequest]DataSourceRequest request, LanguageProficiency languageProficiency, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.LanguageOfProficiencies.Where(x => x.LanguageId == languageProficiency.LanguageId && x.EmployeeId == id).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "This Language is already registered!!");
                }
                else
                {
                    var entity = new LanguageProficiency
                    {
                        EmployeeId = id,
                        LanguageId = languageProficiency.LanguageId,
                        Reading = languageProficiency.Reading,
                        Listening = languageProficiency.Listening,
                        Writing = languageProficiency.Writing,
                        Speaking = languageProficiency.Speaking,
                        Remark = languageProficiency.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = languageProficiency.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = languageProficiency.ModifiedBy
                    };

                    db.LanguageOfProficiencies.Add(entity);
                    db.SaveChanges();
                    languageProficiency.LanguageProficiencyId = entity.LanguageProficiencyId;
                }
        }
            return Json(new[] { languageProficiency }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Profile_User)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LanguageOfProficiencies_Update([DataSourceRequest]DataSourceRequest request, LanguageProficiency languageProficiency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LanguageProficiency
                {
                    LanguageProficiencyId = languageProficiency.LanguageProficiencyId,
                    EmployeeId = languageProficiency.EmployeeId,
                    LanguageId = languageProficiency.LanguageId,
                    Reading = languageProficiency.Reading,
                    Listening = languageProficiency.Listening,
                    Writing = languageProficiency.Writing,
                    Speaking = languageProficiency.Speaking,
                    Remark = languageProficiency.Remark,
                    DateCreated = languageProficiency.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = languageProficiency.CreatedBy,
                    ModifiedBy = User .Identity .Name 
                };

                db.LanguageOfProficiencies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { languageProficiency }.ToDataSourceResult(request, ModelState));
        }
           [Authorize(Roles = HR_Profile_Admin + "," + HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LanguageOfProficiencies_Destroy([DataSourceRequest]DataSourceRequest request, LanguageProficiency languageProficiency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LanguageProficiency
                {
                    LanguageProficiencyId = languageProficiency.LanguageProficiencyId,
                    EmployeeId = languageProficiency.EmployeeId,
                    LanguageId = languageProficiency.LanguageId,
                    Reading = languageProficiency.Reading,
                    Listening = languageProficiency.Listening,
                    Writing = languageProficiency.Writing,
                    Speaking = languageProficiency.Speaking,
                    Remark = languageProficiency.Remark,
                    DateCreated = languageProficiency.DateCreated,
                    LastModified = languageProficiency.LastModified,
                    CreatedBy = languageProficiency.CreatedBy,
                    ModifiedBy = languageProficiency.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Language Proficiency of Employee Id" + languageProficiency.EmployeeId);
                #endregion
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
