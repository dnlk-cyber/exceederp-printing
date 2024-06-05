using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{

    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser, MaintenanceRoles.MaintenanceWorkShopUser, MaintenanceRoles.ServiceUser)]
    public class ServiceCrewWorkLogController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

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
                     JobId = s.JobId,
                     JobOrderDetailId = s.JobOrderDetailId,
                     MecanicId = s.MecanicId,
                     StandaredHours = s.StandaredHours,
                     ServiceGroup = s.ServiceGroup,
                     SubCategoryCode = s.SubCategoryCode,
                     WorkShopType = s.WorkShopType,
                     WorkingHoursAfternoonFrom = s.WorkingHoursAfternoonFrom,
                     TotalLabourHours = s.TotalLabourHours,
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
                    WorkShopType = model.WorkShopType,
                    JobOrderDetailId = model.JobOrderDetailId,
                    SubCategoryCode = model.SubCategoryCode,
                    ServiceGroup = model.ServiceGroup,
                    TotalLabourHours = model.TotalLabourHours,
                    StandaredHours = model.StandaredHours,
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

                this.UpdateAssignedMechanicOnWorkLogChange(Level);

            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrewWorkLog_Update([DataSourceRequest] DataSourceRequest request, ServiceCrewWorkLog ServiceCrewWorkLog)
        {
            int Now = DateTime.Now.DayOfYear;
            //List<ServiceCrewWorkLog> logs = db.ServiceCrewWorkLogs.Where(f => f.MecanicId == ServiceCrewWorkLog.MecanicId && f.Date == Now && f.JobId == ServiceCrewWorkLog.JobId && f.ServiceCrewWorkLogId != ServiceCrewWorkLog.ServiceCrewWorkLogId).ToList();
            //if (logs.Any())
            //{
            //    ModelState.Clear();
            //    ModelState.AddModelError("ExistingName", "   Already Logged Today Work Sheet! ");
            //    return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request, ModelState));
            //}
            //else
            //{
                db = new ExceedDbContext();
                if (ServiceCrewWorkLog != null)
                {
                    ServiceCrewWorkLog.ModifiedBy = User.Identity.Name;
                    ServiceCrewWorkLog.LastModified = DateTime.Now;
                    ServiceCrewWorkLog.Date = DateTime.Now.DayOfYear;

                    db.ServiceCrewWorkLogs.Attach(ServiceCrewWorkLog);
                    db.Entry(ServiceCrewWorkLog).State = EntityState.Modified;
                    db.SaveChanges();

                    this.UpdateAssignedMechanicOnWorkLogChange(ServiceCrewWorkLog);

                }
          //  }
            return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrewWorkLog_Destory([DataSourceRequest] DataSourceRequest request, ServiceCrewWorkLog ServiceCrewWorkLog)
        {

            db.ServiceCrewWorkLogs.Attach(ServiceCrewWorkLog);
            db.ServiceCrewWorkLogs.Remove(ServiceCrewWorkLog);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted ServiceCrewWorkLog number " + ServiceCrewWorkLog.ServiceCrewWorkLogId);

            this.UpdateAssignedMechanicOnWorkLogChange(ServiceCrewWorkLog);

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

                    this.UpdateAssignedMechanicOnWorkLogChange(entity);
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
            catch (Exception)
            {

            }
            return Json(new[] { ServiceCrewWorkLog }.ToDataSourceResult(request));
        }


        private void UpdateAssignedMechanicOnWorkLogChange(ServiceCrewWorkLog model)
        {
            var assignedMechanic = db.ServiceCrew.FirstOrDefault(m => m.MechanicCrewID == model.MecanicId && m.ServiceID == model.JobId);
            if (assignedMechanic != null)
            {
                var workLogs = db.ServiceCrewWorkLogs
                    .Where(wl => wl.MecanicId == assignedMechanic.MechanicCrewID && 
                    wl.JobId == assignedMechanic.ServiceID && wl.Status == MaintenanceConst.Approved).ToList();
                if (workLogs.Any())
                {
                    assignedMechanic.TotalLabourHours = workLogs.Sum(wl => wl.TotalLabourHours);
                    assignedMechanic.TotalLabourCost = assignedMechanic.HourlyWage * assignedMechanic.TotalLabourHours;


                }
                else
                {
                    assignedMechanic.TotalLabourHours = 0;
                    assignedMechanic.TotalLabourCost = 0;
                }
                db.ServiceCrew.Attach(assignedMechanic);
                db.Entry(assignedMechanic).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}