using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using System.Data.Entity;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DriverAssignmentController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/DriverAssignment/
        public ActionResult Index()
        {
            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });
            ViewData["equipmentName"] = db.Fleets.ToList().Select(s => new {
                Id = s.FleetsID,
                Name = s.PlateNumber
            });
            return View();
        }


        public ActionResult DriverAssignment_Read([DataSourceRequest] DataSourceRequest request)
        {

            var DriverAssignment = db.DriverAssignments.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(e => new

            {
                DriverAssignmentId = e.DriverAssignmentId,
                CreatedBy = e.CreatedBy,
                EquipmentName = e.EquipmentName,
                FromOperatorName = e.FromOperatorName,
                ToOperatorName = e.ToOperatorName,
                LastModified = e.LastModified,
                ModifiedBy = e.ModifiedBy,
                FleetId = e.FleetId,
                DateCreated = e.DateCreated,
                Remark = e.Remark,
                StartingDate = e.StartingDate,
                ToDate = e.ToDate,
                IsOnlineApproved = e.IsOnlineApproved,
                IsSendForApproval = e.IsSendForApproval,
                Status = e.Status

            }
        ).ToList();

            return Json(DriverAssignment.ToDataSourceResult(request));

        }

        public ActionResult DriverAssignmentByFleet_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {

            var DriverAssignment = db.DriverAssignments.Where(x => x.FleetId.ToString() == Id).Select(e => new

            {
                DriverAssignmentId = e.DriverAssignmentId,
                CreatedBy = e.CreatedBy,
                EquipmentName = e.EquipmentName,
                FromOperatorName = e.FromOperatorName,
                ToOperatorName = e.ToOperatorName,
                Odometer = e.Odometer,
                LastModified = e.LastModified,
                ModifiedBy = e.ModifiedBy,
                StartingDate = e.StartingDate,
                ToDate = e.ToDate,
                FleetId = e.FleetId,
                DateCreated = e.DateCreated,
                Remark = e.Remark,
                Status = e.IsOnlineApproved ? MaintenanceConst.Assigned : MaintenanceConst.Pending,
                BranchId = e.BranchId

            }
        ).ToList();

            return Json(DriverAssignment.ToDataSourceResult(request));

        }


        public ActionResult DriverAssignment_Create([DataSourceRequest] DataSourceRequest request, DriverAssignment DriverAssignment)
        {
            ModelState.Remove("DriverAssignmentId");
            ModelState.Remove("FromOperatorName");
            ModelState.Remove("EquipmentName");

            if (DriverAssignment != null)
            {
                Fleets fleet = db.Fleets.Where(x => x.FleetsID == DriverAssignment.FleetId).FirstOrDefault();

                if (fleet != null)
                {
                    var entity = new DriverAssignment
                    {

                        DriverAssignmentId = Guid.NewGuid(),
                        FleetId = DriverAssignment.FleetId,
                        EquipmentName = fleet.PlateNumber + " - " + fleet.EquipmentName,
                        FromOperatorName = fleet.OperatorName,
                        ToOperatorName = DriverAssignment.ToOperatorName,
                        CreatedBy = User.Identity.Name,
                        Status = MaintenanceConst.Pending,
                        StartingDate = DriverAssignment.StartingDate,
                        ToDate = DriverAssignment.ToDate,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
              
                        DateCreated = DateTime.Now,
                        Remark = DriverAssignment.Remark,
                        BranchId = DriverAssignment.BranchId
                    };

                    db = new ExceedDbContext();

                    db.DriverAssignments.Add(entity);
                    db.SaveChanges();
                    DriverAssignment.DriverAssignmentId = entity.DriverAssignmentId;

                    Operator Operator = db.Operator.Where(x => x.FullName == DriverAssignment.ToOperatorName).FirstOrDefault();
                    if (Operator != null)
                    {
                        Operator.IsActive = true;

                        db.Operator.Attach(Operator);
                        db.Entry(Operator).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Operator Assignment", "Success", User.Identity.Name, $" assgined opertor  {Operator.EmployeeId}", Modules.FleetManagement);
                        #endregion
                    }
                }
            }

            return Json(new[] { DriverAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverAssignment_Update([DataSourceRequest] DataSourceRequest request, DriverAssignment DriverAssignment)
        {


            if (DriverAssignment != null)
            {
                DriverAssignment.LastModified = DateTime.Now;
                DriverAssignment.ModifiedBy = User.Identity.Name;
                DriverAssignment.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
                db.DriverAssignments.Attach(DriverAssignment);
                db.Entry(DriverAssignment).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Opertor assignment", "Success", User.Identity.Name, $"updated operator assignment {DriverAssignment.DriverAssignmentId}", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { DriverAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverAssignment_Destroy([DataSourceRequest] DataSourceRequest request, DriverAssignment DriverAssignment)
        {
            var DriverAssignmentObj = db.DriverAssignments.Where(x => x.DriverAssignmentId == DriverAssignment.DriverAssignmentId).FirstOrDefault();
            if (DriverAssignment != null)
            {

                db.DriverAssignments.Attach(DriverAssignmentObj);
                db.DriverAssignments.Remove(DriverAssignmentObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Driver Assignment " + DriverAssignment.DriverAssignmentId);

            }

            return Json(new[] { DriverAssignment }.ToDataSourceResult(request));
        }


        [AllowAnonymous]
        public ActionResult UnAssignedEquipment()
        {

            var FixedAssets = db.Fleets.Where(x => x.Status.ToString() == MaintenanceConst.ACTIVE).FirstOrDefault();

            return Json(FixedAssets, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult UnAssigned()
        {

            var FixedAssets = db.Fleets.Where(x => x.Status.ToString() == MaintenanceConst.ACTIVE).FirstOrDefault();

            return Json(FixedAssets, JsonRequestBehavior.AllowGet);
        }
    }
}