using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class TimeSheetApprovalController : Controller
    {
        ExceedDbContext db =new ExceedDbContext();
        //
        // GET: /FleetManagement/TimeSheetApproval/
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.RentUser, FleetManagementRoles.RentApprover)]
        public ActionResult TimeSheetApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<TimeSheetApproval> fleetApproval = db.TimeSheetApprovals.Where(x => x.TimeSheetId == Id).ToList();
            List<TimeSheet> Category = db.TimeSheet.Where(x => x.TimeSheetID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new TimeSheetApproval()
                {
                    TimeSheetId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new TimeSheetApproval()
            {
                TimeSheetApprovalId = approval.TimeSheetApprovalId,
                TimeSheetId = approval.TimeSheetId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentUser)]

        public ActionResult TimeSheet_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var TimeSheet = db.TimeSheet.Where(x => x.TimeSheetID == Id).FirstOrDefault();
            if (TimeSheet != null)
            {
                TimeSheet.IsSendForApproval = true;
                TimeSheet.Status = MaintenanceConst.WaitingApproval;

                db.TimeSheet.Attach(TimeSheet);
                db.Entry(TimeSheet).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "TimeSheet", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceUser)]

        public ActionResult DistributionTimeSheet_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var TimeSheet = db.TimeSheet.Where(x => x.TimeSheetID == Id).FirstOrDefault();
            if (TimeSheet != null)
            {
                TimeSheet.IsSendForApproval = true;
                TimeSheet.Status = MaintenanceConst.WaitingApproval;

                db.TimeSheet.Attach(TimeSheet);
                db.Entry(TimeSheet).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "DistributionTimeSheet", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Approve([DataSourceRequest]DataSourceRequest request, TimeSheetApproval TimeSheetApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (TimeSheetApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.TimeSheet.Where(x => x.TimeSheetID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;
                    db.TimeSheet.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();


                    Fleets fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetsID).FirstOrDefault();

                    fleet.CurentOdoMeter = entity.KMHrReading;

                    db.Fleets.Attach(fleet);
                    db.Entry(fleet).State = EntityState.Modified;
                    db.SaveChanges();

                }
                if (TimeSheetApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.TimeSheet.Where(x => x.TimeSheetID == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.TimeSheet.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var approval = new TimeSheetApproval()
                {

                    Status = TimeSheetApproval.Status,
                    TimeSheetId = Id,
                    Remark = TimeSheetApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.TimeSheetApprovals.Add(approval);
                db.SaveChanges();
                TimeSheetApproval.TimeSheetApprovalId = approval.TimeSheetApprovalId;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { TimeSheetApproval }.ToDataSourceResult(request));
        }




    }
}