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
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleMappingMainTempController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult JobTitleMappingMian_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobTitleMappingMainTemp> jobtitlemappingmian = db.JobTitleMappingMainTemp.Where(X => X.JobTitleMappingId == id);
            DataSourceResult result = jobtitlemappingmian.ToDataSourceResult(request, jobTitleMappingMian => new
            {
                JobTitleMappingMianId = jobTitleMappingMian.JobTitleMappingMianId,
                MainFunction = jobTitleMappingMian.MainFunction,
                Remark = jobTitleMappingMian.Remark,
                OrgBranchName = jobTitleMappingMian.OrgBranchName,
                DateCreated = jobTitleMappingMian.DateCreated,
                LastModified = jobTitleMappingMian.LastModified,
                CreatedBy = jobTitleMappingMian.CreatedBy,
                ModifiedBy = jobTitleMappingMian.ModifiedBy,
                JobTitleMappingId = jobTitleMappingMian.JobTitleMappingId
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMappingMian_Create([DataSourceRequest]DataSourceRequest request, JobTitleMappingMainTemp jobTitleMappingMian, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingMainTemp
                {
                    MainFunction = jobTitleMappingMian.MainFunction,
                    Remark = jobTitleMappingMian.Remark,
                    OrgBranchName = jobTitleMappingMian.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = jobTitleMappingMian.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = jobTitleMappingMian.ModifiedBy,
                    JobTitleMappingId = idTwo
                };

                db.JobTitleMappingMainTemp.Add(entity);
                db.SaveChanges();
                jobTitleMappingMian.JobTitleMappingMianId = entity.JobTitleMappingMianId;
            }

            return Json(new[] { jobTitleMappingMian }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles( HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMappingMian_Update([DataSourceRequest]DataSourceRequest request, JobTitleMappingMainTemp jobTitleMappingMian)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingMainTemp
                {
                    JobTitleMappingMianId = jobTitleMappingMian.JobTitleMappingMianId,
                    MainFunction = jobTitleMappingMian.MainFunction,
                    Remark = jobTitleMappingMian.Remark,
                    OrgBranchName = jobTitleMappingMian.OrgBranchName,
                    DateCreated = jobTitleMappingMian.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = jobTitleMappingMian.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    JobTitleMappingId = jobTitleMappingMian.JobTitleMappingId
                };

                db.JobTitleMappingMainTemp.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobTitleMappingMian }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMappingMian_Destroy([DataSourceRequest]DataSourceRequest request, JobTitleMappingMain jobTitleMappingMian)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMappingMainTemp
                {
                    JobTitleMappingMianId = jobTitleMappingMian.JobTitleMappingMianId,
                    MainFunction = jobTitleMappingMian.MainFunction,
                    Remark = jobTitleMappingMian.Remark,
                    OrgBranchName = jobTitleMappingMian.OrgBranchName,
                    DateCreated = jobTitleMappingMian.DateCreated,
                    LastModified = jobTitleMappingMian.LastModified,
                    CreatedBy = jobTitleMappingMian.CreatedBy,
                    ModifiedBy = jobTitleMappingMian.ModifiedBy,
                    JobTitleMappingId = jobTitleMappingMian.JobTitleMappingId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted job title mapping main function of mapping id " + jobTitleMappingMian.JobTitleMappingId);
                #endregion
                db.JobTitleMappingMainTemp.Attach(entity);
                db.JobTitleMappingMainTemp.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobTitleMappingMian }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
