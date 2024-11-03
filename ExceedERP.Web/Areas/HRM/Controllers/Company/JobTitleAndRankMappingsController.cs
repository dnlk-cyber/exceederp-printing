using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleAndRankMappingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();

            ViewData["jobTitles"] = jobTitle;
            var ranks = db.Ranks.ToList();
            ViewData["RankList"] =  ranks;
        }
       
       

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult JobTitleAndRankMappings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobTitleAndRankMapping> jobTitleAndRankMappings = db.JobTitleAndRankMappings.AsQueryable();
            DataSourceResult result = jobTitleAndRankMappings.ToDataSourceResult(request, jobTitleAndRankMapping => new
            {
                JobTitleAndRankMappingId = jobTitleAndRankMapping.JobTitleAndRankMappingId,
                LookupId = jobTitleAndRankMapping.LookupId,
                RankId = jobTitleAndRankMapping.RankId,
                DateCreated = jobTitleAndRankMapping.DateCreated,
                LastModified = jobTitleAndRankMapping.LastModified,
                CreatedBy = jobTitleAndRankMapping.CreatedBy,
                ModifiedBy = jobTitleAndRankMapping.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleAndRankMappings_Create([DataSourceRequest]DataSourceRequest request, JobTitleAndRankMapping jobTitleAndRankMapping)
        {
            ModelState.Clear();
            var exist = db.JobTitleAndRankMappings.Any(x => x.LookupId == jobTitleAndRankMapping.LookupId);
            if (exist)
            {
                ModelState.AddModelError("exist", "JobTitle can't be mapped more than once");
            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(jobTitleAndRankMapping);
                db.JobTitleAndRankMappings.Add(entity);
                db.SaveChanges();
                jobTitleAndRankMapping.JobTitleAndRankMappingId = entity.JobTitleAndRankMappingId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        
                    UserHelper.OperationLog(Request.UserHostAddress, "JobTitle and Rank Mapping  ", "Success", User.Identity.Name, User.Identity.Name + " Add new JobTitle and Rank Mapping to JobTitleAndRankMappings list " + entity.Rank.RankName + "," + entity.Lookup.Description, Modules.HRM);

                        

                #endregion
                    
                
               
            }
            return Json(new[] { jobTitleAndRankMapping }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleAndRankMappings_Update([DataSourceRequest]DataSourceRequest request, JobTitleAndRankMapping jobTitleAndRankMapping)
        {
            ModelState.Clear();

            var exist = db.JobTitleAndRankMappings.Any(x => x.LookupId == jobTitleAndRankMapping.LookupId
                                    && x.JobTitleAndRankMappingId != jobTitleAndRankMapping.JobTitleAndRankMappingId);
            if (exist)
            {
                ModelState.AddModelError("exist", "JobTitle can't be mapped more than once");
            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(jobTitleAndRankMapping);
                db.JobTitleAndRankMappings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);


                UserHelper.OperationLog(Request.UserHostAddress, "JobTitle And Rank Mapping ", "Success", User.Identity.Name, User.Identity.Name + " Updated JobTitle and Rank Mapping  Info of " + entity.Rank.RankName+","+entity.Lookup.Description,Modules.HRM);

                #endregion

            }

            return Json(new[] { jobTitleAndRankMapping }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleAndRankMappings_Destroy([DataSourceRequest]DataSourceRequest request, JobTitleAndRankMapping jobTitleAndRankMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(jobTitleAndRankMapping);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Jobtitleand Rank mapping Global Setting " + jobTitleAndRankMapping.Rank.RankName+","+jobTitleAndRankMapping.Lookup.Description);
                #endregion
                db.JobTitleAndRankMappings.Attach(entity);
                db.JobTitleAndRankMappings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobTitleAndRankMapping }.ToDataSourceResult(request, ModelState));
        }

        private JobTitleAndRankMapping GetMappedObject(JobTitleAndRankMapping jobTitleAndRankMapping)
        {
            var entity = new JobTitleAndRankMapping
            {
                JobTitleAndRankMappingId = jobTitleAndRankMapping.JobTitleAndRankMappingId,
                LookupId = jobTitleAndRankMapping.LookupId,
                RankId = jobTitleAndRankMapping.RankId,
                DateCreated = jobTitleAndRankMapping.DateCreated,
                LastModified = jobTitleAndRankMapping.LastModified,
                CreatedBy = jobTitleAndRankMapping.CreatedBy,
                ModifiedBy = jobTitleAndRankMapping.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
