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
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser)]
    public class WeekendController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser)]
        public ActionResult Weekend_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Weekend> weekends = db.Weekends;
            DataSourceResult result = weekends.ToDataSourceResult(request, weekend => new
            {
                WeekendId = weekend.WeekendId,
                IsHalfDay = weekend.IsHalfDay,
                Name = weekend.Name,
                Remark = weekend.Remark,
                OrgBranchName = weekend.OrgBranchName,
                DateCreated = weekend.DateCreated,
                LastModified = weekend.LastModified,
                CreatedBy = weekend.CreatedBy,
                ModifiedBy = weekend.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Weekend_Create([DataSourceRequest]DataSourceRequest request, Weekend weekend)
        {
            if (ModelState.IsValid)
            {
                var checkExstance = db.Weekends.Where(x => x.Name == weekend.Name).ToList();
                // var jobName = weekend.Name;
                //var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));

                if (checkExstance.Any())
                {
                    ModelState.AddModelError("", "This weekend is already exist!!");
                }
                else
                {
                    var entity = new Weekend
                    {
                        IsHalfDay = weekend.IsHalfDay,
                        Name = weekend.Name,
                        Remark = weekend.Remark,
                        OrgBranchName = weekend.OrgBranchName,
                        DateCreated = weekend.DateCreated,
                        LastModified = weekend.LastModified,
                        CreatedBy = weekend.CreatedBy,
                        ModifiedBy = weekend.ModifiedBy
                    };

                    db.Weekends.Add(entity);
                    db.SaveChanges();
                    weekend.WeekendId = entity.WeekendId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "Weekend ", "Success", User.Identity.Name, User.Identity.Name + " Create new Weekend called " + entity.Name,Modules.HRM);
                    

                    #endregion
                }
            }

            return Json(new[] { weekend }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Weekend_Update([DataSourceRequest]DataSourceRequest request, Weekend weekend)
        {
            if (ModelState.IsValid)
            {
                var entity = new Weekend
                {
                    WeekendId = weekend.WeekendId,
                    IsHalfDay = weekend.IsHalfDay,
                    Name = weekend.Name,
                    Remark = weekend.Remark,
                    OrgBranchName = weekend.OrgBranchName,
                    DateCreated = weekend.DateCreated,
                    LastModified = weekend.LastModified,
                    CreatedBy = weekend.CreatedBy,
                    ModifiedBy = weekend.ModifiedBy
                };

                db.Weekends.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Weekend ", "Success", User.Identity.Name, User.Identity.Name + " Update Weekend Info of " + entity.Name,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { weekend }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Weekend_Destroy([DataSourceRequest]DataSourceRequest request, Weekend weekend)
        {
            if (ModelState.IsValid)
            {
                var entity = new Weekend
                {
                    WeekendId = weekend.WeekendId,
                    IsHalfDay = weekend.IsHalfDay,
                    Name = weekend.Name,
                    Remark = weekend.Remark,
                    OrgBranchName = weekend.OrgBranchName,
                    DateCreated = weekend.DateCreated,
                    LastModified = weekend.LastModified,
                    CreatedBy = weekend.CreatedBy,
                    ModifiedBy = weekend.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Weekend " + weekend.Name);
                #endregion
                db.Weekends.Attach(entity);
                db.Weekends.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weekend }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
