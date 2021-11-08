using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class JobTitleAndSalaryController : Controller
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

            return View();
        }
    }
}