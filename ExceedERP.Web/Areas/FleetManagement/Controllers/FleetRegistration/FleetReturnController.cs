﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin, FleetManagementRoles.RegistrationUser)]
    public class FleetReturnController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult FleetReturns_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FleetReturn> FleetReturns = db.FleetReturns.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).AsQueryable();
            DataSourceResult result = FleetReturns.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturns_Create([DataSourceRequest]DataSourceRequest request, FleetReturn FleetReturn)
        {
            var FleetReturns = db.FleetReturns.Where(x => x.FleetsId == FleetReturn.FleetsId).ToList();
            var Fleet = db.Fleets.Where(x => x.FleetsID == FleetReturn.FleetsId).FirstOrDefault();
            if (FleetReturns.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " Equipment Already Exists !");
                return Json(new[] { FleetReturn }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                if (Fleet != null)
                {
                    db = new ExceedDbContext();

                    FleetReturn.AssetNo = Fleet.UnitNum;
                    FleetReturn.ChancesSerialNo = Fleet.ChasisNumber + " / " + Fleet.SerialNo;
                    FleetReturn.Condition = Fleet.Condition;
                    FleetReturn.EngineNo = Fleet.EngineNumber;
                    FleetReturn.EquipmentName = Fleet.EquipmentName;
                    FleetReturn.Manufacturer = Fleet.Manufacturer;
                    FleetReturn.PlateNo = Fleet.PlateNumber;
                    FleetReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    FleetReturn.CreatedBy = User.Identity.Name;
                    FleetReturn.DateCreated = DateTime.Now;

                    db.FleetReturns.Add(FleetReturn);
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fleet return registration", "Success", User.Identity.Name, $"created fleet return {FleetReturn.FleetReturnId}", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { FleetReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturns_Update([DataSourceRequest]DataSourceRequest request, FleetReturn FleetReturn)
        {

            var FleetReturns = db.FleetReturns.Where(x => x.FleetsId == FleetReturn.FleetsId && x.IsOnlineApproved).ToList();
            var Fleet = db.Fleets.Where(x => x.FleetsID == FleetReturn.FleetsId).FirstOrDefault();

            if (FleetReturns.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", "  Equipment Already Exists !");
                return Json(new[] { FleetReturn }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                if (Fleet != null)
                {
                    db = new ExceedDbContext();

                    FleetReturn.AssetNo = Fleet.UnitNum;
                    FleetReturn.ChancesSerialNo = Fleet.ChasisNumber + " / " + Fleet.SerialNo;
                    FleetReturn.Condition = Fleet.Condition;
                    FleetReturn.EngineNo = Fleet.EngineNumber;
                    FleetReturn.EquipmentName = Fleet.EquipmentName;
                    FleetReturn.Manufacturer = Fleet.Manufacturer;
                    FleetReturn.PlateNo = Fleet.PlateNumber;
                    FleetReturn.ModifiedBy = User.Identity.Name;
                    FleetReturn.LastModified = DateTime.Now;
                    FleetReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    db.FleetReturns.Attach(FleetReturn);
                    db.Entry(FleetReturn).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fleet return registration", "Success", User.Identity.Name, $"updated fleet return {FleetReturn.FleetReturnId}", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { FleetReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturn_Destroy([DataSourceRequest]DataSourceRequest request, FleetReturn FleetReturn)
        {

            if (FleetReturn != null)
            {
                db.FleetReturns.Attach(FleetReturn);
                db.FleetReturns.Remove(FleetReturn);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted fleet return {FleetReturn.FleetReturnId }");
                #endregion
            }
            return Json(new[] { FleetReturn }.ToDataSourceResult(request));
        }

        #region Approval




        public ActionResult FleetReturnApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {

            List<FleetReturnApproval> fleetApproval = db.FleetReturnApprovals.Where(x => x.FleetReturnId == Id).ToList();
            List<FleetReturn> Category = db.FleetReturns.Where(x => x.FleetReturnId == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new FleetReturnApproval()
                {
                    FleetReturnId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new FleetReturnApproval()
            {
                FleetReturnApprovalId = approval.FleetReturnApprovalId,
                FleetReturnId = approval.FleetReturnId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }




        public ActionResult FleetReturn_SendForOnlineApproval(int Id)
        {

            var FleetReturn = db.FleetReturns.Where(x => x.FleetReturnId == Id).FirstOrDefault();
            if (FleetReturn != null)
            {
                FleetReturn.IsSendForApproval = true;
                FleetReturn.Status = MaintenanceConst.WaitingApproval;

                db.FleetReturns.Attach(FleetReturn);
                db.Entry(FleetReturn).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "FleetReturn", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RegistrationApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetReturn_Approve([DataSourceRequest]DataSourceRequest request, FleetReturnApproval FleetReturnApproval, int Id)
        {
            try
            {

                if (FleetReturnApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.FleetReturns.Where(x => x.FleetReturnId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.FleetReturns.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    var Fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetsId).FirstOrDefault();
                    if(Fleet != null)
                    {

                        Fleet.IsReturned = true;
                        db.Fleets.Attach(Fleet);
                        db.Entry(Fleet).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (FleetReturnApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.FleetReturns.Where(x => x.FleetReturnId == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.FleetReturns.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new FleetReturnApproval()
                {

                    Status = FleetReturnApproval.Status,
                    FleetReturnId = Id,
                    Remark = FleetReturnApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.FleetReturnApprovals.Add(approval);
                db.SaveChanges();
                FleetReturnApproval.FleetReturnApprovalId = approval.FleetReturnApprovalId;

            }
            catch (Exception e)
            {


            }
            return Json(new[] { FleetReturnApproval }.ToDataSourceResult(request));
        }



        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
