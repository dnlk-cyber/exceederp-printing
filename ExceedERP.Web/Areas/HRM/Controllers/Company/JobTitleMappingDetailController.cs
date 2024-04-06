using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleMappingDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;


        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult JobTitleMappingDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobTitleMappingDetail> jobtitlemappingdetail = db.JobTitleMappingDetail.Where(x => x.JobTitleMappingId == id);
            DataSourceResult result = jobtitlemappingdetail.ToDataSourceResult(request, jobTitleMappingDetail => new
            {
                JobTitleMappingDetailId = jobTitleMappingDetail.JobTitleMappingDetailId,

                LookupId = jobTitleMappingDetail.LookupId,
                RelevantExp = jobTitleMappingDetail.RelevantExp,
                Experience = jobTitleMappingDetail.Experience,
                SpecialSkill = jobTitleMappingDetail.SpecialSkill,
                PlannedSalary = jobTitleMappingDetail.PlannedSalary,
                Remark = jobTitleMappingDetail.Remark,
                OrgBranchName = jobTitleMappingDetail.OrgBranchName,
                DateCreated = jobTitleMappingDetail.DateCreated,
                LastModified = jobTitleMappingDetail.LastModified,
                CreatedBy = jobTitleMappingDetail.CreatedBy,
                ModifiedBy = jobTitleMappingDetail.ModifiedBy,
                JobTitleMappingId = jobTitleMappingDetail.JobTitleMappingId
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMappingDetail_Create([DataSourceRequest]DataSourceRequest request, JobTitleMappingDetail jobTitleMappingDetail, int idTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingDetail
                {
                    LookupId = jobTitleMappingDetail.LookupId,
                    RelevantExp = jobTitleMappingDetail.RelevantExp,
                    Experience = jobTitleMappingDetail.Experience,
                    SpecialSkill = jobTitleMappingDetail.SpecialSkill,
                    PlannedSalary = jobTitleMappingDetail.PlannedSalary,
                    Remark = jobTitleMappingDetail.Remark,
                    OrgBranchName = jobTitleMappingDetail.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = jobTitleMappingDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = jobTitleMappingDetail.ModifiedBy,
                    JobTitleMappingId = idTwo
                };

                db.JobTitleMappingDetail.Add(entity);
                db.SaveChanges();
                jobTitleMappingDetail.JobTitleMappingDetailId = entity.JobTitleMappingDetailId;
            }

            return Json(new[] { jobTitleMappingDetail }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMappingDetail_Update([DataSourceRequest]DataSourceRequest request, JobTitleMappingDetail jobTitleMappingDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingDetail
                {
                    JobTitleMappingDetailId = jobTitleMappingDetail.JobTitleMappingDetailId,

                    LookupId = jobTitleMappingDetail.LookupId,
                    RelevantExp = jobTitleMappingDetail.RelevantExp,
                    Experience = jobTitleMappingDetail.Experience,
                    SpecialSkill = jobTitleMappingDetail.SpecialSkill,
                    PlannedSalary = jobTitleMappingDetail.PlannedSalary,
                    Remark = jobTitleMappingDetail.Remark,
                    OrgBranchName = jobTitleMappingDetail.OrgBranchName,
                    DateCreated = jobTitleMappingDetail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = jobTitleMappingDetail.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    JobTitleMappingId = jobTitleMappingDetail.JobTitleMappingId
                };

                db.JobTitleMappingDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobTitleMappingDetail }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMappingDetail_Destroy([DataSourceRequest]DataSourceRequest request, JobTitleMappingDetail jobTitleMappingDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingDetail
                {
                    JobTitleMappingDetailId = jobTitleMappingDetail.JobTitleMappingDetailId,

                    LookupId = jobTitleMappingDetail.LookupId,
                    RelevantExp = jobTitleMappingDetail.RelevantExp,
                    Experience = jobTitleMappingDetail.Experience,
                    SpecialSkill = jobTitleMappingDetail.SpecialSkill,
                    PlannedSalary = jobTitleMappingDetail.PlannedSalary,
                    Remark = jobTitleMappingDetail.Remark,
                    OrgBranchName = jobTitleMappingDetail.OrgBranchName,
                    DateCreated = jobTitleMappingDetail.DateCreated,
                    LastModified = jobTitleMappingDetail.LastModified,
                    CreatedBy = jobTitleMappingDetail.CreatedBy,
                    ModifiedBy = jobTitleMappingDetail.ModifiedBy,
                    JobTitleMappingId = jobTitleMappingDetail.JobTitleMappingId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted job title mapping detail of mapping id " + jobTitleMappingDetail.JobTitleMappingId);
                #endregion
                db.JobTitleMappingDetail.Attach(entity);
                db.JobTitleMappingDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobTitleMappingDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
