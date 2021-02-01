using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleMappingTempController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            ViewData["jobTitles"] = jobTitle;
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            ViewData["projects"] = types;
            var branch = db.Branch;
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
        }
        public JsonResult GetProjectType()
        {
            db = new ExceedDbContext();
            var orgs = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();


            return Json(orgs.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        private bool CheckMappingValidation(int id)
        {
            ErrorMessage = "";


            #region mappingDetail
            var mappingDetail = db.JobTitleMappingDetailTemp.Where(x => x.JobTitleMappingId == id).FirstOrDefault();
            if (mappingDetail != null)
            {

                ErrorMessage = "You cannot delete this mapping .Please delete mapping detail information first.";
                return false;


            }
            #endregion
            #region mappingMian
            var mappingMian = db.JobTitleMappingMainTemp.Where(x => x.JobTitleMappingId == id).FirstOrDefault();
            if (mappingMian != null)
            {

                ErrorMessage = "You cannot delete this mapping .Please delete main Function information first.";
                return false;


            }
            #endregion
            return true;
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult JobTitleMapping_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobTitleMappingTemp> jobtitlemapping = db.JobTitleMappingTemp;
            DataSourceResult result = jobtitlemapping.ToDataSourceResult(request, jobTitleMapping => new
            {
                JobTitleMappingId = jobTitleMapping.JobTitleMappingId,
                OrgStructureId = jobTitleMapping.OrgStructureId,
                LookupId = jobTitleMapping.LookupId,
                //MainFunction = jobTitleMapping.MainFunction,
                Quntity = jobTitleMapping.Quntity,
                Remark = jobTitleMapping.Remark,
                PlanType = jobTitleMapping.PlanType,
                DateCreated = jobTitleMapping.DateCreated,
                LastModified = jobTitleMapping.LastModified,
                CreatedBy = jobTitleMapping.CreatedBy,
                ModifiedBy = jobTitleMapping.ModifiedBy,
                ProjectType = jobTitleMapping.ProjectType,
                WorkUnitId = jobTitleMapping.WorkUnitId,
                OrgBranchName = jobTitleMapping.OrgBranchName
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Create([DataSourceRequest]DataSourceRequest request, JobTitleMappingTemp jobTitleMapping)
        {
            if (ModelState.IsValid)
            {
                var mappings = db.JobTitleMappingTemp.Where(x => x.LookupId == jobTitleMapping.LookupId && x.OrgStructureId == jobTitleMapping.OrgStructureId && jobTitleMapping.ProjectType == x.ProjectType).FirstOrDefault();
                if (mappings != null)
                {
                    ModelState.AddModelError("Please Check", "This mapping template is already exist");
                }
                else
                {

                    var entity = new JobTitleMappingTemp
                    {
                        OrgStructureId = jobTitleMapping.OrgStructureId,
                        LookupId = jobTitleMapping.LookupId,

                        Quntity = jobTitleMapping.Quntity,
                        Remark = jobTitleMapping.Remark,
                        PlanType = jobTitleMapping.PlanType,
                        DateCreated = DateTime.Now,
                        LastModified = jobTitleMapping.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = jobTitleMapping.ModifiedBy,
                        ProjectType = jobTitleMapping.ProjectType,
                        WorkUnitId = jobTitleMapping.WorkUnitId,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.JobTitleMappingTemp.Add(entity);
                    db.SaveChanges();
                    jobTitleMapping.JobTitleMappingId = entity.JobTitleMappingId;

                    #region operation log

                    var main = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == entity.OrgStructureId);
                    if (main != null)
                    {
                        var pro = db.Lookups.FirstOrDefault(x => x.LookupId == entity.ProjectType && x.LookupType == LookUpTypes.PROJECT_TYPE);
                        if (pro != null)
                        {
                            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                            
                                UserHelper.OperationLog(Request.UserHostAddress, "Man Power Planing Template ", "Success", User.Identity.Name, User.Identity.Name + " Create new Manpower Plan Template for " + pro.Description + " for structure name called " + main.Name,Modules.HRM);
                            
                        }
                    }

                    #endregion
                }
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Update([DataSourceRequest]DataSourceRequest request, JobTitleMappingTemp jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingTemp
                {
                    JobTitleMappingId = jobTitleMapping.JobTitleMappingId,
                    OrgStructureId = jobTitleMapping.OrgStructureId,
                    LookupId = jobTitleMapping.LookupId,
                    //MainFunction = jobTitleMapping.MainFunction,
                    Quntity = jobTitleMapping.Quntity,
                    Remark = jobTitleMapping.Remark,
                    PlanType = jobTitleMapping.PlanType,
                    DateCreated = jobTitleMapping.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = jobTitleMapping.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ProjectType = jobTitleMapping.ProjectType,
                    WorkUnitId = jobTitleMapping.WorkUnitId,
                    OrgBranchName = jobTitleMapping.OrgBranchName
                };

                db.JobTitleMappingTemp.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var main = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == entity.OrgStructureId);
                if (main != null)
                {
                    var pro = db.Lookups.FirstOrDefault(x=>x.LookupId == entity.ProjectType && x.LookupType == LookUpTypes.PROJECT_TYPE);
                    if(pro != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                      
                            UserHelper.OperationLog(Request.UserHostAddress, "Man Power Planing Template ", "Success", User.Identity.Name, User.Identity.Name + " Updated Manpower Plan Template of " + pro.Description + " structure name called " + main.Name,Modules.HRM);

                    }
                    
                }

                #endregion
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Destroy([DataSourceRequest]DataSourceRequest request, JobTitleMappingTemp jobTitleMapping)
        {
            if (ModelState.IsValid)
            {
                if (!CheckMappingValidation(jobTitleMapping.JobTitleMappingId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new JobTitleMappingTemp
                    {
                        JobTitleMappingId = jobTitleMapping.JobTitleMappingId,
                        OrgStructureId = jobTitleMapping.OrgStructureId,
                        LookupId = jobTitleMapping.LookupId,
                        //MainFunction = jobTitleMapping.MainFunction,
                        Quntity = jobTitleMapping.Quntity,
                        Remark = jobTitleMapping.Remark,
                        PlanType = jobTitleMapping.PlanType,
                        DateCreated = jobTitleMapping.DateCreated,
                        LastModified = jobTitleMapping.LastModified,
                        CreatedBy = jobTitleMapping.CreatedBy,
                        ModifiedBy = jobTitleMapping.ModifiedBy,
                        ProjectType = jobTitleMapping.ProjectType,
                        WorkUnitId = jobTitleMapping.WorkUnitId,
                        OrgBranchName = jobTitleMapping.OrgBranchName

                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted job title mapping of structure " + jobTitleMapping.MainCompanyStructure.Name + " job title" + jobTitleMapping.LookupId + " Project type " + jobTitleMapping.ProjectType);
                    #endregion
                    db.JobTitleMappingTemp.Attach(entity);
                    db.JobTitleMappingTemp.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Job title mapping");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Job title mapping");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public JsonResult GetJobTitle(int OrgStructureId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (lookup != null)
                    {
                        titleList.Add(lookup);
                    }

                }
            }

            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetStructure(int WorkUnitId)
        {
            db = new ExceedDbContext();
            var orgs = db.MainCompanyStructures.Where(x => x.Parent == WorkUnitId).ToList();
            var selfOrg = db.MainCompanyStructures.Where(x => x.OrgStructureId == WorkUnitId).ToList();
            if (orgs.Any())
            {
                selfOrg.AddRange(orgs);
                //     orgs = db.MainCompanyStructures.Where(x => x.OrgStructureId == WorkUnitId).ToList();
            }

            return Json(selfOrg.Select(p => new { OrgStructureId = p.OrgStructureId, Name = p.Name }), JsonRequestBehavior.AllowGet);


        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
