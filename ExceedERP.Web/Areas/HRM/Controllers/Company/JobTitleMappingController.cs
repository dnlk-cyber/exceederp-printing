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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{

    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleMappingController : BaseController
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
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
            var branchCostCenter = db.BranchCostCenter.ToList();
            foreach (var b in branchCostCenter)
            {
                var w = db.MainCompanyStructures.Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (w != null)
                {
                    var exisit = workUnits.Where(x => x.OrgStructureId == w.OrgStructureId).FirstOrDefault();
                    if (exisit == null) workUnits.Add(w);
                }

            }
            ViewData["workunits"] = workUnits;
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            ViewData["projects"] = types;
        }
        public JsonResult GetWorkUnit()
        {
            db = new ExceedDbContext();
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var branchCostCenter = db.BranchCostCenter.ToList();
            foreach (var b in branchCostCenter)
            {
                var w = db.MainCompanyStructures.Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (w != null)
                {
                    var exisit = workUnits.Where(x => x.OrgStructureId == w.OrgStructureId).FirstOrDefault();
                    if (exisit == null) workUnits.Add(w);
                }

            }

            return Json(workUnits.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetJobTitleList()
        {
            db = new ExceedDbContext();
            var orgs = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();


            return Json(orgs.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetBranch()
        {
            db = new ExceedDbContext();
            var orgs = db.Branch.ToList();


            return Json(orgs.Select(p => new { BranchId = p.BranchId, AmharicName = p.AmharicName }), JsonRequestBehavior.AllowGet);


        }
        private bool CheckMappingValidation(int id)
        {
            ErrorMessage = "";


            #region mappingDetail
            var mappingDetail = db.JobTitleMappingDetail.Where(x => x.JobTitleMappingId == id).FirstOrDefault();
            if (mappingDetail != null)
            {

                ErrorMessage = "You cannot delete this mapping .Please delete mapping detail information first.";
                return false;


            }
            #endregion
            #region mappingMian
            var mappingMian = db.JobTitleMappingMian.Where(x => x.JobTitleMappingId == id).FirstOrDefault();
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
            IQueryable<JobTitleMapping> jobtitlemapping = db.JobTitleMapping;
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
                BranchId = jobTitleMapping.BranchId,
                WorkUnitId = jobTitleMapping.WorkUnitId,
                OrgBranchName = jobTitleMapping.OrgBranchName
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Create([DataSourceRequest]DataSourceRequest request, JobTitleMapping jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var mappings = db.JobTitleMapping.Where(x => x.LookupId == jobTitleMapping.LookupId && x.OrgStructureId == jobTitleMapping.OrgStructureId && x.BranchId == jobTitleMapping.BranchId).FirstOrDefault();
                if (mappings != null)
                {
                    ModelState.AddModelError("Please Check", "This Mapping is already exist");
                }
                else
                {

                    var entity = new JobTitleMapping
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
                        BranchId = jobTitleMapping.BranchId,
                        WorkUnitId = jobTitleMapping.WorkUnitId,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.JobTitleMapping.Add(entity);
                    db.SaveChanges();
                    jobTitleMapping.JobTitleMappingId = entity.JobTitleMappingId;

                    #region operation log

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == entity.BranchId);
                    if (branch != null)
                    {
                        var main = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == entity.OrgStructureId);
                        if (main != null)
                        {
                            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                            UserHelper.OperationLog(Request.UserHostAddress, "Man Power Planing ", "Success", User.Identity.Name, User.Identity.Name + " Create new Manpower Plan for " + branch.Name + " structure name called " + main.Name, Modules.HRM);

                        }

                    }

                    #endregion
                }
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Update([DataSourceRequest]DataSourceRequest request, JobTitleMapping jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMapping
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
                    BranchId = jobTitleMapping.BranchId,
                    WorkUnitId = jobTitleMapping.WorkUnitId,
                    OrgBranchName = jobTitleMapping.OrgBranchName
                };

                db.JobTitleMapping.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var branch = db.Branch.FirstOrDefault(x => x.BranchId == entity.BranchId);
                if (branch != null)
                {
                    var main = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == entity.OrgStructureId);
                    if (main != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                        UserHelper.OperationLog(Request.UserHostAddress, "Man Power Planing ", "Success", User.Identity.Name, User.Identity.Name + " Updated Manpower Plan Info of " + branch.Name + " structure name called " + main.Name, Modules.HRM);

                    }

                }

                #endregion
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Destroy([DataSourceRequest]DataSourceRequest request, JobTitleMapping jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckMappingValidation(jobTitleMapping.JobTitleMappingId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new JobTitleMapping
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
                        BranchId = jobTitleMapping.BranchId,
                        WorkUnitId = jobTitleMapping.WorkUnitId,
                        OrgBranchName = jobTitleMapping.OrgBranchName

                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted job title mapping of structure " + jobTitleMapping.OrgStructureId + " job title" + jobTitleMapping.LookupId + " branch " + jobTitleMapping.BranchId);
                    #endregion
                    db.JobTitleMapping.Attach(entity);
                    db.JobTitleMapping.Remove(entity);
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
            List<MainCompanyStructure> mainCompanyStructureList = new List<MainCompanyStructure>();

            // var orgs = db.MainCompanyStructures.Where(x => x.Parent == WorkUnitId).ToList();
            //var selfOrg = db.MainCompanyStructures.Where(x => x.OrgStructureId == WorkUnitId).ToList();

            var both = db.MainCompanyStructures.Where(x => x.OrgStructureId == WorkUnitId || x.Parent == WorkUnitId).ToList();
            if (both.Any())
            {
                //selfOrg.AddRange(orgs);

                //mainCompanyStructureList.AddRange(orgs);
                //mainCompanyStructureList.AddRange(selfOrg);
                mainCompanyStructureList.AddRange(both);

            }

            return Json(mainCompanyStructureList.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);


        }
        public ActionResult FilterMenuCustomization_WorkUnit()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();

            var branchCostCenter = db.BranchCostCenter.ToList();
            foreach (var b in branchCostCenter)
            {
                var oneWorkUnit = db.MainCompanyStructures.Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (oneWorkUnit != null)
                {
                    workUnits.Add(oneWorkUnit);
                }

            }

            return Json(workUnits.Select(e => e.Name).Distinct(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateJobTitleMapping(int typeId, int branchId)
        {
            var oneBranch = db.JobTitleMappingTemp.Where(x => x.ProjectType == typeId).ToList();
            if (oneBranch.Any())
            {
                foreach (var o in oneBranch)
                {
                    var checkExistance = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == branchId && x.OrgStructureId == o.OrgStructureId && x.LookupId == o.LookupId && x.WorkUnitId == o.WorkUnitId);
                    if (checkExistance == null)
                    {
                        db = new ExceedDbContext();
                        var entity = new JobTitleMapping
                        {
                            OrgStructureId = o.OrgStructureId,
                            LookupId = o.LookupId,
                            Quntity = o.Quntity,
                            Remark = o.Remark,
                            PlanType = o.PlanType,
                            DateCreated = DateTime.Now,
                            LastModified = null,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            BranchId = branchId,
                            WorkUnitId = o.WorkUnitId,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };

                        db.JobTitleMapping.Add(entity);
                        db.SaveChanges();
                        int JobTitleMappingId = entity.JobTitleMappingId;

                        ExceedDbContext dbE = new ExceedDbContext();
                        var mainfun = dbE.JobTitleMappingMainTemp.Where(x => x.JobTitleMappingId == o.JobTitleMappingId).ToList();
                        foreach (var m in mainfun)
                        {
                            var entityy = new JobTitleMappingMain
                            {
                                MainFunction = m.MainFunction,
                                Remark = m.Remark,
                                OrgBranchName = m.OrgBranchName,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null,
                                JobTitleMappingId = JobTitleMappingId
                            };

                            dbE.JobTitleMappingMian.Add(entityy);
                            dbE.SaveChanges();
                            //jobTitleMappingMian.JobTitleMappingMianId = entityy.JobTitleMappingMianId;
                        }
                        ExceedDbContext dbm = new ExceedDbContext();
                        var mappingDetail = dbm.JobTitleMappingDetailTemp.Where(x => x.JobTitleMappingId == o.JobTitleMappingId).ToList();
                        foreach (var d in mappingDetail)
                        {
                            var entityd = new JobTitleMappingDetail
                            {

                                LookupId = d.LookupId,
                                RelevantExp = d.RelevantExp,
                                Experience = d.Experience,
                                SpecialSkill = d.SpecialSkill,
                                PlannedSalary = d.PlannedSalary,
                                Remark = d.Remark,
                                OrgBranchName = d.OrgBranchName,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null,
                                JobTitleMappingId = JobTitleMappingId
                            };

                            dbm.JobTitleMappingDetail.Add(entityd);
                            dbm.SaveChanges();

                        }
                    }
                    else
                    {
                        checkExistance.Quntity = o.Quntity;
                        checkExistance.Remark = o.Remark;
                        checkExistance.PlanType = o.PlanType;
                        checkExistance.LastModified = DateTime.Now;
                        checkExistance.ModifiedBy = User.Identity.Name;

                        db.Entry(checkExistance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var formatedData = new
                {
                    Message = "Manpower Planning successfully generated",
                    Status = "OK"
                };
                return Json(formatedData,JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formatedData = new
                {
                    Message = "Please set template for this project type first!!",
                    Status = "FAIL"
                };
                return Json(formatedData, JsonRequestBehavior.AllowGet);
            }
           
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
