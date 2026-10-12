using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeExternalFundingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult EmployeeExternalFundings_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var externalFundings = db.EmployeeExternalFundings.Where(x => x.EmployeeId == id);
            var result = externalFundings.ToDataSourceResult(request, funding => new
            {
                funding.EmployeeExternalFundingId,
                funding.EmployeeId,
                funding.FundingSource,
                funding.ProjectName,
                funding.OutCome,
                funding.LevelOfEffortPercentage,
                funding.DateCreated,
                funding.LastModified,
                funding.CreatedBy,
                funding.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeExternalFundings_Create([DataSourceRequest] DataSourceRequest request, EmployeeExternalFunding funding, int id)
        {
            ModelState.Clear();
            funding.EmployeeId = id;
            if (ModelState.IsValid)
            {
                funding.DateCreated = DateTime.Now;
                funding.CreatedBy = User.Identity.Name;

                db.EmployeeExternalFundings.Add(funding);
                db.SaveChanges();
            }

            return Json(new[] { funding }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeExternalFundings_Update([DataSourceRequest] DataSourceRequest request, EmployeeExternalFunding funding)
        {
            ModelState.Clear();

            
            if (ModelState.IsValid)
            {
                funding.LastModified = DateTime.Now;
                funding.ModifiedBy = User.Identity.Name;

                db.EmployeeExternalFundings.Attach(funding);
                db.Entry(funding).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { funding }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeExternalFundings_Destroy([DataSourceRequest] DataSourceRequest request, EmployeeExternalFunding funding)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                db.EmployeeExternalFundings.Attach(funding);
                db.EmployeeExternalFundings.Remove(funding);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted External Funding for Employee ID {funding.EmployeeId}");
            }

            return Json(new[] { funding }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
