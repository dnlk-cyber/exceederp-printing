using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class EarnedValueManagementTermsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/EarnedValueManagementTerms
        public ActionResult Index()
        {
            var termDetails = db.EarnedValueManagementTerms.ToList();
            ViewData["EVMTerms"] = termDetails;
            return View();
        }

        public ActionResult EarnedValueManagementTerms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EarnedValueManagementTerm> terms = db.EarnedValueManagementTerms.OrderBy(t => t.EarnedValueManagementTermId);
            DataSourceResult result = terms.ToDataSourceResult(request, earnedValueTerm => new {
                EarnedValueManagementTermId = earnedValueTerm.EarnedValueManagementTermId,
                Category = earnedValueTerm.Category,
                Term = earnedValueTerm.Term,
                Abbreviation = earnedValueTerm.Abbreviation,
                Definition = earnedValueTerm.Definition,
                Formula = earnedValueTerm.Formula
            });

            return Json(result);
        }

        
        private bool CheckExistance(EarnedValueManagementTerm model)
        {
            var valueExists = false;
            var existingTerm = db.EarnedValueManagementTerms.Where(evmTerm => evmTerm.Term == model.Term || evmTerm.Abbreviation == model.Abbreviation).FirstOrDefault();
            if (existingTerm != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValueManagementTerm_Create([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTerm earnedValueTerm)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(earnedValueTerm))
                {
                    ModelState.AddModelError("", "EVM Terms should be uniquely Identifiable!!");
                }
                else
                {


                    var entity = new EarnedValueManagementTerm
                    {
                        //EarnedValueManagementTermsId = term.EarnedValueManagementTermsId,
                        Category = earnedValueTerm.Category,
                        Term = earnedValueTerm.Term,
                        Abbreviation = earnedValueTerm.Abbreviation,
                        Definition = earnedValueTerm.Definition,
                        Formula = earnedValueTerm.Formula
                    };

                    db.EarnedValueManagementTerms.Add(entity);
                    db.SaveChanges();
                    earnedValueTerm.EarnedValueManagementTermId = entity.EarnedValueManagementTermId;

                }
            }

            return Json(new[] { earnedValueTerm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValueManagementTerm_Update([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTerm earnedValueTerm)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementTerm
                {
                    EarnedValueManagementTermId = earnedValueTerm.EarnedValueManagementTermId,
                    Category = earnedValueTerm.Category,
                    Term = earnedValueTerm.Term,
                    Abbreviation = earnedValueTerm.Abbreviation,
                    Definition = earnedValueTerm.Definition,
                    Formula = earnedValueTerm.Formula
                };

                db.EarnedValueManagementTerms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { earnedValueTerm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValueManagementTerm_Destroy([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTerm earnedValueTerm)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementTerm
                {
                    EarnedValueManagementTermId = earnedValueTerm.EarnedValueManagementTermId,
                    Category = earnedValueTerm.Category,
                    Term = earnedValueTerm.Term,
                    Abbreviation = earnedValueTerm.Abbreviation,
                    Definition = earnedValueTerm.Definition,
                    Formula = earnedValueTerm.Formula
                };

                db.EarnedValueManagementTerms.Attach(entity);
                db.EarnedValueManagementTerms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { earnedValueTerm }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}