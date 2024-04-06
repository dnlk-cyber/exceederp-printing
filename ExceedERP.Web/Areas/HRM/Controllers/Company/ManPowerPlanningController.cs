using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class ManPowerPlanningController :  BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ManPowerPlanning
        public ActionResult Index()
        {
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            ViewData["jobTitles"] = jobTitle;
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
            var branchCostCenter = db.BranchCostCenter.ToList();
            foreach (var b in branchCostCenter)
            {
                var oneWorkUnit = db.MainCompanyStructures.Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (oneWorkUnit != null)
                {
                    workUnits.Add(oneWorkUnit);
                }

            }
            ViewData["workunits"] = workUnits;
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            ViewData["projects"] = types;
            ViewData["businessUnits"] = db.BusinessUnits;
            return View();
        }
    }
}