using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRTrainingUser)]
    public class TrainingProviderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
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

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        public ActionResult Companies_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MainCompany> companies = db.Companies.Where(c => c.Category == "1");// 1 for training provider company
            DataSourceResult result = companies.ToDataSourceResult(request, mainCompany => new
            {
                MainCompanyId = mainCompany.MainCompanyId,
                TradeName = mainCompany.TradeName,
                BrandName = mainCompany.BrandName,
                BusinessType = mainCompany.BusinessType,
                Category = mainCompany.Category,
                IsActive = mainCompany.IsActive
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
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
                    Category = "1",
                    IsActive = mainCompany.IsActive
                };

                db.Companies.Add(entity);
                db.SaveChanges();
                mainCompany.MainCompanyId = entity.MainCompanyId;

                #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Training Provider ", "Success", User.Identity.Name,  User.Identity.Name+ " add new Training Provider called "+ entity.TradeName + " date "+DateTime.Today.ToShortDateString(),Modules.HRM);
                
                #endregion

            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
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
                    Category = "1",
                    IsActive = mainCompany.IsActive
                };

                db.Companies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Training Provider ", "Success", User.Identity.Name, User.Identity.Name + " update Training Provider info of " +entity.TradeName + " date "+ DateTime.Today.ToShortDateString(),Modules.HRM);
                
                #endregion

            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Destroy([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            var courseTracking = db.CourseTrackings.FirstOrDefault(x => x.MainCompanyId == mainCompany.MainCompanyId);
            var trainingPlan = db.TrainingPlans.FirstOrDefault(x => x.MainCompanyId == mainCompany.MainCompanyId);
            if (courseTracking != null || trainingPlan != null)
            {
                if (courseTracking != null && trainingPlan == null)
                {
                    ModelState.AddModelError("", "you can't delete this provider it is used in Course Tracking");
                }
                if (courseTracking == null && trainingPlan != null)
                {
                    ModelState.AddModelError("", "you can't delete this provider it is used in Training Plan");
                }
                if (courseTracking != null && trainingPlan != null)
                {
                    ModelState.AddModelError("", "you can't delete this provider it is used in Course Tracking and Training Plan");
                }

            }

            if (ModelState.IsValid)
            {
                var entity = new MainCompany
                {
                    MainCompanyId = mainCompany.MainCompanyId,
                    TradeName = mainCompany.TradeName,
                    BrandName = mainCompany.BrandName,
                    BusinessType = mainCompany.BusinessType,
                    Category = "1",
                    IsActive = mainCompany.IsActive
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training provider information of provider" + mainCompany.TradeName);
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
