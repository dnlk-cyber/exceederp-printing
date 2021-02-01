using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser)]
    public class ProjectTypeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser)]
        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Lookup> lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE);
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new
            {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                Remark = lookup.Remark,
                DateCreated = lookup.DateCreated,
                LastModified = lookup.LastModified,
                CreatedBy = lookup.CreatedBy,
                ModifiedBy = lookup.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var checkExstance = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
                var jobName = lookup.Description;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Project type already exists in the database!!");
                }
                else
                {

                    var entity = new Lookup
                    {
                        Code = lookup.Code,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        LookupType = LookUpTypes.PROJECT_TYPE,
                        Description = lookup.Description,
                        IsDefault = lookup.IsDefault,
                        IsActive = true,
                        Remark = lookup.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = lookup.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = lookup.ModifiedBy
                    };

                    db.Lookups.Add(entity);
                    db.SaveChanges();
                    lookup.LookupId = entity.LookupId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Project Type ", "Success", User.Identity.Name, User.Identity.Name + " Create new project type called " + entity.Description,Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.PROJECT_TYPE,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark,
                    DateCreated = lookup.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = lookup.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Lookups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Project Type ", "Success", User.Identity.Name, User.Identity.Name + " Update project type Info called " + entity.Description,Modules.HRM);
                

                #endregion
                // }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var checkExitance = db.PayrollProcessAdditions.Where (x=> x.AdditionId!=0).FirstOrDefault();
                //if (checkExitance != null)
                //{
                //    ModelState.AddModelError("","You cannot delete this addition type. It already processed!!");
                //}
                //else
                //{
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.PROJECT_TYPE,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark,
                    DateCreated = lookup.DateCreated,
                    LastModified = lookup.LastModified,
                    CreatedBy = lookup.CreatedBy,
                    ModifiedBy = lookup.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Addition type information of type " + lookup.Description);
                #endregion
                db.Lookups.Attach(entity);
                db.Lookups.Remove(entity);
                db.SaveChanges();
                //}
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
