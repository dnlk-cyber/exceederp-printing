using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
    public class SalaryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Company_Admin = "HR_Company_Admin";
        private const string HR_Company_User = "HR_Company_User";
        private const string HR_Admin = "HR_Admin";
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            var ranks = db.Ranks;
            var steps = db.Steps;
            ViewData["RankList"] = ranks != null ? ranks.ToList() : new List<Rank>();
            ViewData["StepList"] = steps != null ? steps.ToList() : new List<Step>();
            return View();
        }
           [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
        public ActionResult Salaries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Salary> salaries = db.Salaries;
            DataSourceResult result = salaries.ToDataSourceResult(request, salary => new {
                SalaryStructureId = salary.SalaryStructureId,
                RankId = salary.RankId,
                StepId = salary.StepId,
                SalaryAmount = salary.SalaryAmount,
                Remark = salary.Remark
            });

            return Json(result);
        }
           [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Salaries_Create([DataSourceRequest]DataSourceRequest request, Salary salary)
        {
            if (ModelState.IsValid)
            {
                var salsries = db.Salaries.Where(x => x.StepId == salary.StepId && x.RankId == salary.RankId).FirstOrDefault();
                if (salsries != null)
                {
                    ModelState.AddModelError("", "This salary structure is already exist!!");
                }
                else
                {
                    var entity = new Salary
                    {
                        RankId = salary.RankId,
                        StepId = salary.StepId,
                        SalaryAmount = salary.SalaryAmount,
                        Remark = salary.Remark
                    };

                    db.Salaries.Add(entity);
                    db.SaveChanges();
                    salary.SalaryStructureId = entity.SalaryStructureId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll salary strcucture", "Success", User.Identity.Name, $"added  {entity.SalaryStructureId} rank {entity.Rank.RankName} step {entity.Step.StepName} amount {entity.SalaryAmount}", Modules.Payroll);
                    #endregion
                }
            }
            return Json(new[] { salary }.ToDataSourceResult(request, ModelState));
        }
           [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Salaries_Update([DataSourceRequest]DataSourceRequest request, Salary salary)
        {
            if (ModelState.IsValid)
            {
                var entity = new Salary
                {
                    SalaryStructureId = salary.SalaryStructureId,
                    RankId = salary.RankId,
                    StepId = salary.StepId,
                    SalaryAmount = salary.SalaryAmount,
                    Remark = salary.Remark
                };

                db.Salaries.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll salary strcucture", "Success", User.Identity.Name, $"updated  {entity.SalaryStructureId} rank {entity.Rank.RankName} step {entity.Step.StepName} amount {entity.SalaryAmount}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { salary }.ToDataSourceResult(request, ModelState));
        }
           [Authorize(Roles = HR_Company_Admin + "," + "," + HR_Admin + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Salaries_Destroy([DataSourceRequest]DataSourceRequest request, Salary salary)
        {
            if (ModelState.IsValid)
            {
                var entity = new Salary
                {
                    SalaryStructureId = salary.SalaryStructureId,
                    RankId = salary.RankId,
                    StepId = salary.StepId,
                    SalaryAmount = salary.SalaryAmount,
                    Remark = salary.Remark
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted salary structure information of rank and step " + salary.RankId+" ,"+ salary .StepId,Modules.Payroll);
                #endregion
                db.Salaries.Attach(entity);
                db.Salaries.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { salary }.ToDataSourceResult(request, ModelState));
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
