using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{

    [AuthorizeRoles(FleetManagementRoles.ServiceUser, FleetManagementRoles.MaintenanceServiceApprover)]
    public class ServiceCrewWorkLogController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ServiceCrewWorkLog_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var ServiceCrewWorkLog = db.ServiceCrewWorkLogs.Where(x => x.JobId.ToString() == Id).Select(
                 s => new
                              {
                                  ServiceCrewWorkLogId = s.ServiceCrewWorkLogId,
                                  JobOrderDetailId = s.JobOrderDetailId,
                                  MecanicId = s.MecanicId,
                                  WorkingHoursAfternoonFrom = s.WorkingHoursAfternoonFrom,
                                  WorkingHoursAfternoonTo = s.WorkingHoursAfternoonTo,
                                  WorkingHoursMorningFrom = s.WorkingHoursMorningFrom,
                                  WorkingHoursMorningto = s.WorkingHoursMorningto,
                                  CreatedBy = s.CreatedBy,
                                  DateCreated = s.DateCreated,
                                  Status = s.Status

                              }
                ).ToList();

            return Json(ServiceCrewWorkLog.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrewWorkLog_Create([DataSourceRequest] DataSourceRequest request, ServiceCrewWorkLog model, string Id)
        {
            ModelState.Remove("ServiceCrewWorkLogId");
            ModelState.Remove("JobId");
            int Now = DateTime.Now.DayOfYear;

            List<ServiceCrewWorkLog> logs = db.ServiceCrewWorkLogs.Where(f => f.MecanicId == model.MecanicId && f.Date == Now && f.JobId.ToString() == Id && f.JobOrderDetailId == model.JobOrderDetailId).ToList();
            if (logs.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  Already Logged Today Work Sheet! ");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                db = new ExceedDbContext();

                var Level = new ServiceCrewWorkLog
                {
                    ServiceCrewWorkLogId = Guid.NewGuid(),
                    JobId = Guid.Parse(Id),
                    MecanicId = model.MecanicId,
                    JobOrderDetailId = model.JobOrderDetailId,
                    WorkingHoursAfternoonFrom = model.WorkingHoursAfternoonFrom,
                    WorkingHoursAfternoonTo = model.WorkingHoursAfternoonTo,
                    WorkingHoursMorningFrom = model.WorkingHoursMorningFrom,
                    WorkingHoursMorningto = model.WorkingHoursMorningto,
                    Date = DateTime.Now.DayOfYear,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    Status = MaintenanceConst.Pending

                };
                db.ServiceCrewWorkLogs.Add(Level);
                db.SaveChanges();
                model.ServiceCrewWorkLogId = Level.ServiceCrewWorkLogId;
            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrewWorkLog_Update([DataSourceRequest] DataSourceRequest request, ServiceCrewWorkLog ServiceCrewWorkLog)
        {
            int Now = DateTime.Now.DayOfYear;
            List<ServiceCrewWorkLog> logs = db.ServiceCrewWorkLogs.Where(f => f.MecanicId == ServiceCrewWorkLog.MecanicId && f.Date == Now && f.JobId == ServiceCrewWorkLog.JobId && f.ServiceCrewWorkLogId != ServiceCrewWorkLog.ServiceCrewWorkLogId ).ToList();
            if (logs.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "   Already Logged Today Work Sheet! ");
                return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                db = new ExceedDbContext();
                if (ServiceCrewWorkLog != null)
                {
                    ServiceCrewWorkLog.ModifiedBy = User.Identity.Name;
                    ServiceCrewWorkLog.LastModified = DateTime.Now;
                    ServiceCrewWorkLog.Date = DateTime.Now.DayOfYear;

                    db.ServiceCrewWorkLogs.Attach(ServiceCrewWorkLog);
                    db.Entry(ServiceCrewWorkLog).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrewWorkLog_Destory([DataSourceRequest] DataSourceRequest request, ServiceCrewWorkLog ServiceCrewWorkLog)
        {

            db.ServiceCrewWorkLogs.Attach(ServiceCrewWorkLog);
            db.ServiceCrewWorkLogs.Remove(ServiceCrewWorkLog);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted ServiceCrewWorkLog number " + ServiceCrewWorkLog.ServiceCrewWorkLogId);

            return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrewWorkLog_Approve([DataSourceRequest]DataSourceRequest request, ServiceCrewWorkLog ServiceCrewWorkLog)
        {
            try
            {
                if (ServiceCrewWorkLog.Status == MaintenanceConst.Approved)
                {
                    var entity = db.ServiceCrewWorkLogs.Where(x => x.ServiceCrewWorkLogId == ServiceCrewWorkLog.ServiceCrewWorkLogId).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.Status = MaintenanceConst.Approved;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.ServiceCrewWorkLogs.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (ServiceCrewWorkLog.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.ServiceCrewWorkLogs.Where(x => x.ServiceCrewWorkLogId == ServiceCrewWorkLog.ServiceCrewWorkLogId).FirstOrDefault();
                    entity.IsSendForApproval = false;

                    db.ServiceCrewWorkLogs.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }


            }
            catch (Exception e)
            {

            }
            return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request));
        }




    }
}