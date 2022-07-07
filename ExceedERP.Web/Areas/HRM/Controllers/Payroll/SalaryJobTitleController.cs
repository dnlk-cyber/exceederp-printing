using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
    public class SalaryJobTitleController : BaseController
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
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            ViewData["jobTitles"] = jobTitle != null ? jobTitle.ToList() : new List<Lookup>();
            //  GetSalary();
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            return View();
        }
        public JsonResult GetJobTitle(int OrganizationId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrganizationId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    titleList.Add(lookup);

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        private string GetTitleName(int? id)
        {
            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }
        [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
        public ActionResult SalariesJobTitles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SalaryJobTitle> salariesjobtitles = db.SalariesJobTitles;
            DataSourceResult result = salariesjobtitles.ToDataSourceResult(request, salaryJobTitle => new
            {
                SalJobTiId = salaryJobTitle.SalJobTiId,
                Salary = salaryJobTitle.Salary,
                RankId = salaryJobTitle.RankId,
                StepId = salaryJobTitle.StepId,
                JobTitle = GetTitleName(salaryJobTitle.LookupId),
                LookupId = salaryJobTitle.LookupId,
                Remark = salaryJobTitle.Remark,
                DateCreated = salaryJobTitle.DateCreated,
                LastModified = salaryJobTitle.LastModified,
                CreatedBy = salaryJobTitle.CreatedBy,
                ModifiedBy = salaryJobTitle.ModifiedBy
            });

            return Json(result);
        }
        [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalariesJobTitles_Create([DataSourceRequest]DataSourceRequest request, SalaryJobTitle salaryJobTitle)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var salsries = db.SalariesJobTitles.Where(x => x.StepId == salaryJobTitle.StepId && x.RankId == salaryJobTitle.RankId && x.LookupId == salaryJobTitle.LookupId).FirstOrDefault();
                if (salsries != null)
                {
                    ModelState.AddModelError("", "This salary structure is already exist!!");
                }
                else
                {
                    var entity = new SalaryJobTitle
                    {
                        Salary = salaryJobTitle.Salary,
                        RankId = salaryJobTitle.RankId,
                        StepId = salaryJobTitle.StepId,
                        JobTitle = GetTitleName(salaryJobTitle.LookupId),
                        LookupId = salaryJobTitle.LookupId,
                        Remark = salaryJobTitle.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = salaryJobTitle.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = salaryJobTitle.ModifiedBy
                    };

                    db.SalariesJobTitles.Add(entity);
                    db.SaveChanges();
                    salaryJobTitle.SalJobTiId = entity.SalJobTiId;

                    #region operation log
                    string rankName = "";
                    string stepName = "";
                    string lookUpDescription = "";
                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == entity.RankId);
                    if (rank != null)
                    {
                        rankName = rank.RankName;
                    }
                    var step = db.Steps.FirstOrDefault(x => x.StepId == entity.StepId);
                    if (step != null)
                    {
                        stepName = step.StepName;
                    }
                    var lookup = db.Lookups.FirstOrDefault(x=>x.LookupId == entity.LookupId);
                    if(lookup != null)
                    {
                        lookUpDescription = lookup.Description;
                    }

                    UserHelper.OperationLog(Request.UserHostAddress, "payroll salary job title", "Success", User.Identity.Name, $"added  {entity.SalJobTiId} rank {rank} step {step} job title {lookUpDescription} amount {entity.SalJobTiId}", Modules.Payroll);

                    #endregion
                }
            }

            return Json(new[] { salaryJobTitle }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalariesJobTitles_Update([DataSourceRequest]DataSourceRequest request, SalaryJobTitle salaryJobTitle)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new SalaryJobTitle
                {
                    SalJobTiId = salaryJobTitle.SalJobTiId,
                    Salary = salaryJobTitle.Salary,
                    RankId = salaryJobTitle.RankId,
                    StepId = salaryJobTitle.StepId,
                    JobTitle = GetTitleName(salaryJobTitle.LookupId),
                    LookupId = salaryJobTitle.LookupId,
                    Remark = salaryJobTitle.Remark,
                    DateCreated = salaryJobTitle.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = salaryJobTitle.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.SalariesJobTitles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log

                string rankName = "";
                string stepName = "";
                string lookUpDescription = "";
                var rank = db.Ranks.FirstOrDefault(x => x.RankId == entity.RankId);
                if (rank != null)
                {
                    rankName = rank.RankName;
                }
                var step = db.Steps.FirstOrDefault(x => x.StepId == entity.StepId);
                if (step != null)
                {
                    stepName = step.StepName;
                }
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == entity.LookupId);
                if (lookup != null)
                {
                    lookUpDescription = lookup.Description;
                }

                UserHelper.OperationLog(Request.UserHostAddress, "payroll salary job title", "Success", User.Identity.Name, $"updated  {entity.SalJobTiId} rank {rank} step {step} job title {lookUpDescription} amount {entity.SalJobTiId}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { salaryJobTitle }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Company_Admin + "," + "," + HR_Admin + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalariesJobTitles_Destroy([DataSourceRequest]DataSourceRequest request, SalaryJobTitle salaryJobTitle)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new SalaryJobTitle
                {
                    SalJobTiId = salaryJobTitle.SalJobTiId,

                    Salary = salaryJobTitle.Salary,
                    RankId = salaryJobTitle.RankId,
                    StepId = salaryJobTitle.StepId,
                    JobTitle = GetTitleName(salaryJobTitle.LookupId),
                    LookupId = salaryJobTitle.LookupId,
                    Remark = salaryJobTitle.Remark,
                    DateCreated = salaryJobTitle.DateCreated,
                    LastModified = salaryJobTitle.LastModified,
                    CreatedBy = salaryJobTitle.CreatedBy,
                    ModifiedBy = salaryJobTitle.ModifiedBy
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted salary structure information of rank " + salaryJobTitle.RankId + " , step" + salaryJobTitle.StepId + " and " + salaryJobTitle.LookupId, Modules.Payroll);
                #endregion
                db.SalariesJobTitles.Attach(entity);
                db.SalariesJobTitles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { salaryJobTitle }.ToDataSourceResult(request, ModelState));
        }


        public JsonResult GetSalary()
        {

            var Salaries = db.Salaries.ToList();

            ViewData["Salaries"] = Salaries;
            return Json(Salaries.Select(s => new { SalaryStructureId = s.SalaryStructureId, SalaryAmount = s.SalaryAmount }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
