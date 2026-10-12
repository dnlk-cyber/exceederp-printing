using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class ExternalSalariesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult ExternalSalaries_Read([DataSourceRequest] DataSourceRequest request, int sourceEmpId)
        {
            var externalSalaries = db.ExternalSalaries.Where(x => x.EmployeeId == sourceEmpId);
            var result = externalSalaries.ToDataSourceResult(request, salary => new
            {
                salary.ExternalSalaryId,
                salary.EmployeeId,
                salary.Salary,
                salary.DateCreated,
                salary.LastModified,
                salary.CreatedBy,
                salary.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalSalaries_Create([DataSourceRequest] DataSourceRequest request, ExternalSalary externalSalary, int sourceEmpId)
        {
            ModelState.Clear();
            if (externalSalary == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to create external salary because the submitted data is empty.");
                return Json(new[] { new ExternalSalary { EmployeeId = sourceEmpId } }.ToDataSourceResult(request, ModelState));
            }

            externalSalary.EmployeeId = sourceEmpId;
            TryValidateModel(externalSalary);
            if (ModelState.IsValid)
            {
                externalSalary.DateCreated = DateTime.Now;
                externalSalary.CreatedBy = User.Identity.Name;

                db.ExternalSalaries.Add(externalSalary);
                db.SaveChanges();
            }

            return Json(new[] { externalSalary }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalSalaries_Update([DataSourceRequest] DataSourceRequest request, ExternalSalary salary)
        {
            ModelState.Clear();
            if (salary == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to update external salary because the submitted data is empty.");
                return Json(new[] { new ExternalSalary() }.ToDataSourceResult(request, ModelState));
            }

            TryValidateModel(salary);

            if (ModelState.IsValid)
            {
                salary.LastModified = DateTime.Now;
                salary.ModifiedBy = User.Identity.Name;

                db.ExternalSalaries.Attach(salary);
                db.Entry(salary).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { salary }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalSalaries_Destroy([DataSourceRequest] DataSourceRequest request, ExternalSalary externalSalary)
        {
            ModelState.Clear();
            if (externalSalary == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete external salary because the submitted data is empty.");
                return Json(new[] { new ExternalSalary() }.ToDataSourceResult(request, ModelState));
            }

            TryValidateModel(externalSalary);

            if (ModelState.IsValid)
            {
                db.ExternalSalaries.Attach(externalSalary);
                db.ExternalSalaries.Remove(externalSalary);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted External Salary for Employee ID {externalSalary.EmployeeId}");
            }

            return Json(new[] { externalSalary }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
