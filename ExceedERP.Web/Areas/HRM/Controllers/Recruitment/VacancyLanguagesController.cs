using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System.Data.Entity;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    [Authorize]
    public class VacancyLanguagesController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult VacancyLanguages_Read([DataSourceRequest] DataSourceRequest request,int masterId)
        {
            var vacancyLanguages = db.VacancyLanguages
                .Where(x => x.VacancyId == masterId);

            var result = vacancyLanguages.ToDataSourceResult(request, language => new
            {
                VacancyLanguageId = language.VacancyLanguageId,
                VacancyId = language.VacancyId,
                LanguageName = language.LanguageName,
                Description = language.Description,
                ProficiencyLevel = language.ProficiencyLevel,
                IsRequired = language.IsRequired,
                CreatedBy = language.CreatedBy,
                DateCreated = language.DateCreated,
                ModifiedBy = language.ModifiedBy,
                LastModified = language.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VacancyLanguages_Create([DataSourceRequest] DataSourceRequest request,VacancyLanguage vacancyLanguage, int masterId)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                vacancyLanguage.VacancyId = masterId;
                vacancyLanguage.DateCreated = DateTime.Now;
                vacancyLanguage.CreatedBy = User.Identity.Name;

                db.VacancyLanguages.Add(vacancyLanguage);
                db.SaveChanges();
            }

            return Json(new[] { vacancyLanguage }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult VacancyLanguages_Update([DataSourceRequest] DataSourceRequest request, VacancyLanguage vacancyLanguage)
        {
            ModelState.Clear();

            var entity = db.VacancyLanguages.FirstOrDefault(v=>v.VacancyLanguageId == vacancyLanguage.VacancyLanguageId);

            if (entity != null)
            {
                entity.LanguageName = vacancyLanguage.LanguageName;
                entity.Description = vacancyLanguage.Description;
                entity.ProficiencyLevel = vacancyLanguage.ProficiencyLevel;
                entity.IsRequired = vacancyLanguage.IsRequired;

                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Now;

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                // Log

                UserHelper.OperationLog(
                            Request.UserHostAddress,
                            "Vacancy Language",
                            "Success",
                            User.Identity.Name,
                            $"{User.Identity.Name} updated vacancy language {entity.LanguageName}",
                            Modules.HRM);
            }
           
            return Json(new[] { vacancyLanguage }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult VacancyLanguages_Destroy([DataSourceRequest] DataSourceRequest request, VacancyLanguage vacancyLanguage)
        {
            if (ModelState.IsValid)
            {
                var entity = db.VacancyLanguages
                    .Find(vacancyLanguage.VacancyLanguageId);

                if (entity != null)
                {
                    // Delete Log
                    UserHelper.DeleteLog(
                        Request.UserHostAddress,
                        "Delete",
                        "Success",
                        User.Identity.Name,
                        $"Deleted Vacancy Language with ID {entity.VacancyLanguageId}"
                    );

                    db.VacancyLanguages.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { vacancyLanguage }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

