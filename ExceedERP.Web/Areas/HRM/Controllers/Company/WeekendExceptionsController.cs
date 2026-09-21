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
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, 
        HrmConstants.HRAdmin,
        HrmConstants.HRCompanyUser)]
    public class WeekendExceptionsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
      
        [AuthorizeRoles(HrmConstants.HRDepartmentHead, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser, HrmConstants.HREmployeeSelfService)]
        public ActionResult WeekendExceptions_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<WeekendException> weekendExceptions = db.WeekendExceptions.Where(x=>x.WeekendId==id);
            DataSourceResult result = weekendExceptions.ToDataSourceResult(request, weekendException => new
            {
                WeekendExceptionId = weekendException.WeekendExceptionId,
                WeekendId = weekendException.WeekendId,
                EmployeeId = weekendException.EmployeeId,
                OrgStructureId = weekendException.OrgStructureId,
                BranchId = weekendException.BranchId,
                IsFullDay = weekendException.IsFullDay,
                IsHalfDay = weekendException.IsHalfDay,
                Remark = weekendException.Remark,
                OrgBranchName = weekendException.OrgBranchName,
                DateCreated = weekendException.DateCreated,
                LastModified = weekendException.LastModified,
                CreatedBy = weekendException.CreatedBy,
                ModifiedBy = weekendException.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser, HrmConstants.HREmployeeSelfService)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeekendExceptions_Create([DataSourceRequest]DataSourceRequest request, WeekendException weekendException,int id)
        {

            var checkExistence = db.WeekendExceptions.Any(x =>
                x.WeekendId == id &&
                (
                    (weekendException.EmployeeId.HasValue &&
                     x.EmployeeId.HasValue &&
                     x.EmployeeId == weekendException.EmployeeId)

                    ||

                    (weekendException.OrgStructureId.HasValue &&
                     x.OrgStructureId.HasValue &&
                     x.OrgStructureId == weekendException.OrgStructureId)

                    ||

                    (weekendException.BranchId.HasValue &&
                     x.BranchId.HasValue &&
                     x.BranchId == weekendException.BranchId)
                ));

            if (checkExistence)
            {
                ModelState.AddModelError(
                    "",
                    "A weekend exception already exists for the selected employee, branch, or organizational structure. Please select a different value.");

                return Json(new[] { weekendException }.ToDataSourceResult(request, ModelState));
            }

            weekendException.WeekendId = id;

            if (ModelState.IsValid)
            {
                    var entity = GetMappedObject(weekendException);
                    db.WeekendExceptions.Add(entity);
                    db.SaveChanges();
                    weekendException.WeekendExceptionId = entity.WeekendExceptionId;
                    #region notification
                    
                    NotifModels notif = new NotifModels
                    {
                        UserId = User.Identity.Name,
                        Title = "Test Notification",
                        Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has testing the notification",
                        URL = "~/HRM/Weekend"
                    };

                    Notify.NotifyUser(notif);

                #endregion

               
                #region operation log


                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Weekend ", "Success", User.Identity.Name, User.Identity.Name + " Create new Weekend called " + entity.EmployeeId,Modules.HRM);


                #endregion

            }
            return Json(new[] { weekendException }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser, HrmConstants.HREmployeeSelfService)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeekendExceptions_Update([DataSourceRequest]DataSourceRequest request, WeekendException weekendException)
        {
            var checkExistence = db.WeekendExceptions.Any(x =>
                                    x.WeekendId == weekendException.WeekendId 
                                    &&x.WeekendExceptionId != weekendException.WeekendExceptionId &&
                                    (
                                        (weekendException.EmployeeId.HasValue &&
                                         x.EmployeeId.HasValue &&
                                         x.EmployeeId == weekendException.EmployeeId)

                                        ||

                                        (weekendException.OrgStructureId.HasValue &&
                                         x.OrgStructureId.HasValue &&
                                         x.OrgStructureId == weekendException.OrgStructureId)

                                        ||

                                        (weekendException.BranchId.HasValue &&
                                         x.BranchId.HasValue &&
                                         x.BranchId == weekendException.BranchId)
                                    ));

            if (checkExistence)
            {
                ModelState.AddModelError(
                    "", "A weekend exception already exists for the selected employee, branch, or organizational structure. Please select a different value.");

                return Json(new[] { weekendException }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weekendException);
                db.WeekendExceptions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region notification

                string user = User.Identity.Name;
                NotifModels notif = new NotifModels
                {
                    UserId = user,
                    Title = "Test Notification",
                    Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has testing the notification",
                    URL = "/HRM/Weekend"
                };

                Notify.NotifyUser(notif);

                #endregion

                #region operation log


                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);


                UserHelper.OperationLog(Request.UserHostAddress, "Weekend ", "Success", User.Identity.Name, User.Identity.Name + " Update Weekend Info of " + entity.EmployeeId,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { weekendException }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser, HrmConstants.HREmployeeSelfService)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeekendExceptions_Destroy([DataSourceRequest]DataSourceRequest request, WeekendException weekendException)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(weekendException);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Weekend " + weekendException.EmployeeId);
                #endregion
                db.WeekendExceptions.Attach(entity);
                db.WeekendExceptions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weekendException }.ToDataSourceResult(request, ModelState));
        }

        private WeekendException GetMappedObject(WeekendException weekendException)
        {
            var entity = new WeekendException
            {
                WeekendExceptionId = weekendException.WeekendExceptionId,
                WeekendId = weekendException.WeekendId,
                EmployeeId = weekendException.EmployeeId,
                OrgStructureId = weekendException.OrgStructureId,
                BranchId = weekendException.BranchId,
                IsFullDay = weekendException.IsFullDay,
                IsHalfDay = weekendException.IsHalfDay,
                Remark = weekendException.Remark,
                OrgBranchName = weekendException.OrgBranchName,
                DateCreated = weekendException.DateCreated,
                LastModified = weekendException.LastModified,
                CreatedBy = weekendException.CreatedBy,
                ModifiedBy = weekendException.ModifiedBy
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
