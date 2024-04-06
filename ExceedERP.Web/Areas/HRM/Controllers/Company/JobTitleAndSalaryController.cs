using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class JobTitleAndSalaryController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/JobTitleAndSalary
        public ActionResult Index()
        {
            ViewData["Descriptions"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).Select(x => x.Description).ToList();
            var ranks = db.Ranks;
            var steps = db.Steps;
            ViewData["RankList"] = ranks != null ? ranks.ToList() : new List<Rank>();
            ViewData["StepList"] = steps != null ? steps.ToList() : new List<Step>();
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            ViewData["jobTitles"] = jobTitle != null ? jobTitle.ToList() : new List<Lookup>();
            //  GetSalary();
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;

            var setting = db.SalarySettings.FirstOrDefault();
            if(setting != null)
            {
                ViewBag.IsJobTitleIncludedInSalary = setting.IsJobtitleIncludedInSalary;
            }
            else
            {
                ViewBag.IsJobTitleIncludedInSalary = false;
            }            

            return View();
        }

        public string SalarySetting(bool isJobTitleIncludedInSalary)
        {
            var setting = db.SalarySettings.FirstOrDefault();
            if(setting != null)
            {
                setting.IsJobtitleIncludedInSalary = isJobTitleIncludedInSalary;

                db.Entry(setting).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                var entity = new SalarySetting
                {
                    IsJobtitleIncludedInSalary = isJobTitleIncludedInSalary
                };

                db.SalarySettings.Add(entity);
                db.SaveChanges();
            }

            return "Salary Successfully Updated";
        }
        public bool GetSalarySetting()
        {
            var setting = db.SalarySettings.FirstOrDefault();
            if(setting != null)
            {
                return setting.IsJobtitleIncludedInSalary;
            }

            return false;
        }
    }
}