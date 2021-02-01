using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{

    [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]
    public class DepreciationRuleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateData();
            return View();
        }

        public void PopulateData()
        {
            var fixedAssetCategories = db.FixedAssetCategories;
            ViewData["fixedAssetCategories"] = fixedAssetCategories.ToList();

            //var fixedAssetSubCategories = db.FixedAssetSubCategories;
            //ViewData["fixedAssetSubCategories"] = fixedAssetSubCategories.ToList();

            var fixedAssetGroup = db.FixedAssetGroups;
            ViewData["fixedAssetGroupsa"] = fixedAssetGroup.ToList();
        }
        public ActionResult DepretiationRules_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DepreciationRule> depretiationrules = db.DepretiationRules;
            DataSourceResult result = depretiationrules.ToDataSourceResult(request, depretiationRule => new
            {
                DepreciationRuleId = depretiationRule.DepreciationRuleId,
                CategoryCode = depretiationRule.CategoryCode,
                AccumulatedAccount = depretiationRule.AccumulatedAccount,
                AccountCode = depretiationRule.AccountCode,
                DepreciationCode = depretiationRule.DepreciationCode,
                FixedAssetGroupID = depretiationRule.FixedAssetGroupID,
                ClearingAccount = depretiationRule.ClearingAccount
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepretiationRules_Create([DataSourceRequest]DataSourceRequest request, DepreciationRule depreciationRule)
        {
            #region validation

            var exists = db.DepretiationRules.Where(x => x.CategoryCode == depreciationRule.CategoryCode).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists","The fixed asset category already exists");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new DepreciationRule
                {

                    CategoryCode = depreciationRule.CategoryCode,
                    AccumulatedAccount = depreciationRule.AccumulatedAccount,
                    FixedAssetGroupID = depreciationRule.FixedAssetGroupID,
                    AccountCode = depreciationRule.AccountCode,
                    DepreciationCode = depreciationRule.DepreciationCode,
                    ClearingAccount = depreciationRule.ClearingAccount
                };

                db.DepretiationRules.Add(entity);
                db.SaveChanges();
                depreciationRule.DepreciationRuleId = entity.DepreciationRuleId;
            }

            return Json(new[] { depreciationRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepretiationRules_Update([DataSourceRequest]DataSourceRequest request, DepreciationRule depreciationRule)
        {
            #region validation

            var exists = db.DepretiationRules.Where(x => x.DepreciationRuleId == depreciationRule.DepreciationRuleId)
                                             .Where(x => x.CategoryCode == depreciationRule.CategoryCode).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists", "The fixed asset category already exists");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new DepreciationRule
                {
                    DepreciationRuleId = depreciationRule.DepreciationRuleId,
                    CategoryCode = depreciationRule.CategoryCode,
                    AccumulatedAccount = depreciationRule.AccumulatedAccount,
                    FixedAssetGroupID = depreciationRule.FixedAssetGroupID,

                    AccountCode = depreciationRule.AccountCode,
                    DepreciationCode = depreciationRule.DepreciationCode,
                    ClearingAccount = depreciationRule.ClearingAccount
                };

                db.DepretiationRules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { depreciationRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepretiationRules_Destroy([DataSourceRequest]DataSourceRequest request, DepreciationRule depreciationRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new DepreciationRule
                {
                    DepreciationRuleId = depreciationRule.DepreciationRuleId,

                    AccumulatedAccount = depreciationRule.AccumulatedAccount,
                    FixedAssetGroupID = depreciationRule.FixedAssetGroupID,
                    CategoryCode = depreciationRule.CategoryCode,
                    AccountCode = depreciationRule.AccountCode,
                    DepreciationCode = depreciationRule.DepreciationCode,
                    ClearingAccount = depreciationRule.ClearingAccount
                };

                db.DepretiationRules.Attach(entity);
                db.DepretiationRules.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { depreciationRule }.ToDataSourceResult(request, ModelState));
        }

        //public JsonResult GetItemCategory()
        //    {
        //    var category = db.ItemCategorys.Where(x => x.IsFixedAsset);
        //    return Json(category.Select(p => new { Name = p.Name + "_" + p.CategoryCode, CategoryCode = p.CategoryCode }),
        //       JsonRequestBehavior.AllowGet);

        //    }
        //public JsonResult GetSubCat(string id)
        //    {
        //    var category = db.ItemCategorys.Where(x => x.IsFixedAsset && x.ParentName == id);
        //    return Json(category.Select(p => new { Name = p.Name + "_" + p.CategoryCode, CategoryCode = p.CategoryCode }),
        //       JsonRequestBehavior.AllowGet);

        //    }

        public JsonResult GetFixedAssetRule()
        {
            var fixedAsset = db.FixedAssetGroups;
            return Json(fixedAsset.Select(p => new { Name = p.Name, FixedAssetGroupID = p.FixedAssetGroupID }),
              JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
