using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ProjectJobTitleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        private const string Project_Admin = "Project_Admin";

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
        private string GetTitleName(int id)
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

        public ActionResult ProjectJobTitles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectJobTitle> ProjectJobTitles = db.ProjectJobTitles;
            DataSourceResult result = ProjectJobTitles.ToDataSourceResult(request, ProjectJobTitle => new
            {
                ProjectJobTitleId = ProjectJobTitle.ProjectJobTitleId,
                Salary = ProjectJobTitle.Salary,
                //RankId =  ProjectJobTitle.RankId,
                //StepId =  ProjectJobTitle.StepId,
                JobTitle = ProjectJobTitle.JobTitle,
                LookupId = ProjectJobTitle.LookupId,
                Remark = ProjectJobTitle.Remark,

                OrgBranchName = ProjectJobTitle.OrgBranchName,
                DateCreated = ProjectJobTitle.DateCreated,
                LastModified = ProjectJobTitle.LastModified,
                CreatedBy = ProjectJobTitle.CreatedBy,
                ModifiedBy = ProjectJobTitle.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectJobTitles_Create([DataSourceRequest]DataSourceRequest request, ProjectJobTitle ProjectJobTitle)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var salsries = db.ProjectJobTitles.Where(x => x.StepId ==  ProjectJobTitle.StepId && x.RankId ==  ProjectJobTitle.RankId && x.LookupId ==  ProjectJobTitle.LookupId).FirstOrDefault();
                var salsries = db.ProjectJobTitles.Where(x => x.JobTitle == ProjectJobTitle.JobTitle && x.LookupId == ProjectJobTitle.LookupId).FirstOrDefault();
                if (salsries != null)
                {
                    ModelState.AddModelError("", "This salary structure is already exist!!");
                }
                else
                {
                    var entity = new ProjectJobTitle
                    {
                        Salary = ProjectJobTitle.Salary,
                        //RankId =  ProjectJobTitle.RankId,
                        //StepId =  ProjectJobTitle.StepId,
                        JobTitle = ProjectJobTitle.JobTitle,
                        LookupId = ProjectJobTitle.LookupId,
                        Remark = ProjectJobTitle.Remark,

                        OrgBranchName = ProjectJobTitle.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ProjectJobTitle.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ProjectJobTitle.ModifiedBy
                    };

                    db.ProjectJobTitles.Add(entity);
                    db.SaveChanges();
                    ProjectJobTitle.ProjectJobTitleId = entity.ProjectJobTitleId;
                }
            }

            return Json(new[] { ProjectJobTitle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectJobTitles_Update([DataSourceRequest]DataSourceRequest request, ProjectJobTitle ProjectJobTitle)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ProjectJobTitle
                {
                    ProjectJobTitleId = ProjectJobTitle.ProjectJobTitleId,
                    Salary = ProjectJobTitle.Salary,
                    //RankId =  ProjectJobTitle.RankId,
                    //StepId =  ProjectJobTitle.StepId,
                    JobTitle = ProjectJobTitle.JobTitle,
                    LookupId = ProjectJobTitle.LookupId,
                    Remark = ProjectJobTitle.Remark,

                    OrgBranchName = ProjectJobTitle.OrgBranchName,
                    DateCreated = ProjectJobTitle.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = ProjectJobTitle.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectJobTitles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ProjectJobTitle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectJobTitles_Destroy([DataSourceRequest]DataSourceRequest request, ProjectJobTitle ProjectJobTitle)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ProjectJobTitle
                {
                    ProjectJobTitleId = ProjectJobTitle.ProjectJobTitleId,

                    Salary = ProjectJobTitle.Salary,
                    //RankId =  ProjectJobTitle.RankId,
                    //StepId =  ProjectJobTitle.StepId,
                    JobTitle = ProjectJobTitle.JobTitle,
                    LookupId = ProjectJobTitle.LookupId,
                    Remark = ProjectJobTitle.Remark,

                    OrgBranchName = ProjectJobTitle.OrgBranchName,
                    DateCreated = ProjectJobTitle.DateCreated,
                    LastModified = ProjectJobTitle.LastModified,
                    CreatedBy = ProjectJobTitle.CreatedBy,
                    ModifiedBy = ProjectJobTitle.ModifiedBy
                };
                //#region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted salary structure information of rank " +  ProjectJobTitle.RankId + " , step" +  ProjectJobTitle.StepId + " and " +  ProjectJobTitle.LookupId);
                //#endregion
                db.ProjectJobTitles.Attach(entity);
                db.ProjectJobTitles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProjectJobTitle }.ToDataSourceResult(request, ModelState));
        }


        //public decimal  GetSalary(int jId,int? rId, int? sId)
        public decimal GetSalary(int jId)

        {
            var currentJob = new SalaryJobTitle();
            //if (rId != null && sId != null)
            //{
            //    currentJob = db.SalariesJobTitles.Where(x => x.LookupId == jId && x.RankId == rId && x.StepId == sId).FirstOrDefault();
            //}
            currentJob = db.SalariesJobTitles.Where(x => x.LookupId == jId && x.Lookup.LookupType == LookUpTypes.JOB_TITLE).FirstOrDefault();
            if (currentJob != null)
            {
                return currentJob.Salary;
            }
            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
