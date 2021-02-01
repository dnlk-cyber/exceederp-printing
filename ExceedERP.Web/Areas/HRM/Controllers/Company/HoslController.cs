using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    //[Authorize(Roles = HR_Admin)]
    [AuthorizeRoles( HrmConstants.HRAdmin)]
    public class HospitalController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var BusinessTypes = Enum.GetValues(typeof(BusinessType))
                .Cast<BusinessType>()

                .ToList();
            var businessList = BusinessTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["BusinessTypes"] = businessList;
        }

        public ActionResult Companies_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MainCompany> companies = db.Companies.Where(c => c.Category == "2" && c.IsActive);// 1 for training provider company
            DataSourceResult result = companies.ToDataSourceResult(request, mainCompany => new
            {
                MainCompanyId = mainCompany.MainCompanyId,
                TradeName = mainCompany.TradeName,
                BrandName = mainCompany.BrandName,
                BusinessType = mainCompany.BusinessType,
                Category = mainCompany.Category,
                IsActive = mainCompany.IsActive
            });

            return Json(result);//,JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Create([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            if (ModelState.IsValid)
            {
                var entity = new MainCompany
                {
                    TradeName = mainCompany.TradeName,
                    BrandName = mainCompany.BrandName,
                    BusinessType = mainCompany.BusinessType,
                    Category = "2",
                    IsActive = mainCompany.IsActive
                };

                db.Companies.Add(entity);
                db.SaveChanges();
                mainCompany.MainCompanyId = entity.MainCompanyId;
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));//,JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Update([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            if (ModelState.IsValid)
            {
                var entity = new MainCompany
                {
                    MainCompanyId = mainCompany.MainCompanyId,
                    TradeName = mainCompany.TradeName,
                    BrandName = mainCompany.BrandName,
                    BusinessType = mainCompany.BusinessType,
                    Category = "2",
                    IsActive = mainCompany.IsActive
                };

                db.Companies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Destroy([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            if (ModelState.IsValid)
            {
                var entity = new MainCompany
                {
                    MainCompanyId = mainCompany.MainCompanyId,
                    TradeName = mainCompany.TradeName,
                    BrandName = mainCompany.BrandName,
                    BusinessType = mainCompany.BusinessType,
                    Category = "2",
                    IsActive = mainCompany.IsActive
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Hospital name" + mainCompany.TradeName + " " + mainCompany.BrandName);
                #endregion
                db.Companies.Attach(entity);
                db.Companies.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
