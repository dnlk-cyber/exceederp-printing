using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
    public class CompanyLeaveWorkflowSettingController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompanyLeaveWorkflowSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<CompanyLeaveWorkflowSetting> settings = db.CompanyLeaveWorkflowSettings;

            var result = settings.ToDataSourceResult(request, s => new
            {
                CompanyLeaveWorkflowSettingId = s.CompanyLeaveWorkflowSettingId,
                UseProjectManagerApproval = s.UseProjectManagerApproval,
                Remark = s.Remark,

                DateCreated = s.DateCreated,
                LastModified = s.LastModified,
                CreatedBy = s.CreatedBy,
                ModifiedBy = s.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyLeaveWorkflowSettings_Create( [DataSourceRequest] DataSourceRequest request, CompanyLeaveWorkflowSetting setting)
        {
            ModelState.Clear();

            var existing = db.CompanyLeaveWorkflowSettings.FirstOrDefault();

            if (existing != null)
            {
                ModelState.AddModelError("", "Leave workflow setting already exists.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                var entity = new CompanyLeaveWorkflowSetting
                {
                    UseProjectManagerApproval = setting.UseProjectManagerApproval,
                    Remark = setting.Remark,

                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = User.Identity.Name
                };

                db.CompanyLeaveWorkflowSettings.Add(entity);
                db.SaveChanges();

                setting.CompanyLeaveWorkflowSettingId = entity.CompanyLeaveWorkflowSettingId;

                #region operation log

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Company Leave Workflow Setting",
                    "Success",
                    User.Identity.Name,
                    "Company Leave Workflow Setting Created By " + User.Identity.Name,
                    Modules.HRM);

                #endregion
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyLeaveWorkflowSettings_Update( [DataSourceRequest] DataSourceRequest request, CompanyLeaveWorkflowSetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.CompanyLeaveWorkflowSettings
                    .FirstOrDefault(x => x.CompanyLeaveWorkflowSettingId ==
                                         setting.CompanyLeaveWorkflowSettingId);

                if (entity != null)
                {
                    entity.UseProjectManagerApproval = setting.UseProjectManagerApproval;
                    entity.Remark = setting.Remark;

                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "Company Leave Workflow Setting",
                        "Success",
                        User.Identity.Name,
                        "Company Leave Workflow Setting Updated By " + User.Identity.Name,
                        Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyLeaveWorkflowSettings_Destroy([DataSourceRequest] DataSourceRequest request, CompanyLeaveWorkflowSetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.CompanyLeaveWorkflowSettings
                    .FirstOrDefault(x => x.CompanyLeaveWorkflowSettingId ==
                                         setting.CompanyLeaveWorkflowSettingId);

                if (entity != null)
                {
                    #region delete log

                    UserHelper.DeleteLog(
                        Request.UserHostAddress,
                        "Delete",
                        "Success",
                        User.Identity.Name,
                        "Deleted Company Leave Workflow Setting");

                    #endregion

                    db.CompanyLeaveWorkflowSettings.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}