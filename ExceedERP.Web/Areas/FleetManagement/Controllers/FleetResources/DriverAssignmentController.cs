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
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DriverAssignmentController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/DriverAssignment/
       // private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
       i => new
       {
           Text = i.Name,
           Value = i.Id
       }).AsQueryable();
            ViewData["type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();

            ViewData["Branch"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             BranchId = i.Id
         }).AsQueryable();
            ViewData["Operators"] = db.Operator.Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });
            ViewData["OperatorTwo"] = db.Operator.Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });
            ViewData["equipmentName"] = CacheData.GetFleetsSecondCache().Select(s => new {
                Id = s.FleetsID,
                Name = s.PlateNumber
            });
            return View();
        }

        public JsonResult Seed()
        {
            var fleets = db.Fleets.ToList();
            foreach (var item in fleets)
            {
                var driverassigned = db.DriverAssignments.FirstOrDefault(x => x.FleetId == item.FleetsID);
                if (driverassigned != null)
                {
                    driverassigned.EquipmentCategoryId = item?.EquipmentIdentityID ?? Guid.Empty;
                    driverassigned.EquipmentNameId = item?.EquipmentNameID ?? Guid.Empty;
                    driverassigned.EquipmentTypeId = item?.EquipmentTypeID ?? Guid.Empty;
                }
                db.Entry(driverassigned).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DriverAssignment_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get current branch
            string currentBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            // Get filtered records
            var driverAssignmentsQuery = db.DriverAssignments
                .Where(x => x.OrgBranchName == currentBranch);

            // Project to new object
            DataSourceResult Result = driverAssignmentsQuery.ToDataSourceResult(request, e => new DriverAssignment
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
                Status = e.Status,
                BranchId = e.BranchId,
                EquipmentCategoryId = e.EquipmentCategoryId,
                EquipmentNameId = e.EquipmentNameId,
                EquipmentTypeId = e.EquipmentTypeId,
                OperatorId = e.OperatorId,
                SecondOperatorId = e.SecondOperatorId
            });
                

            // Return Kendo DataSourceResult
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DriverAssignmentByFleet_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {

            var DriverAssignment = db.DriverAssignments.Where(x => x.FleetId.ToString() == Id).Select(e => new DriverAssignment

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
                BranchId = e.BranchId,
                OperatorId = e.OperatorId,
                SecondOperatorId = e.SecondOperatorId,
                EquipmentCategoryId = e.EquipmentCategoryId,
                EquipmentTypeId = e.EquipmentTypeId,
                EquipmentNameId = e.EquipmentNameId,
                IsOnlineApproved = e.IsOnlineApproved,
                IsOnlineTransferred = e.IsOnlineTransferred,
                

            }
        ).ToList();

            return Json(DriverAssignment.ToDataSourceResult(request));

        }


        public ActionResult DriverAssignment_Create([DataSourceRequest] DataSourceRequest request, DriverAssignment model)
        {
            ModelState.Remove("DriverAssignmentId");
            ModelState.Remove("FromOperatorName");
            ModelState.Remove("EquipmentName");

            if (model != null)
            {
                Fleets fleet = db.Fleets.Where(x => x.FleetsID == model.FleetId).FirstOrDefault();

                if (fleet != null)
                {
                    var entity = new DriverAssignment
                    {
                        EquipmentCategoryId = model.EquipmentCategoryId,
                        EquipmentNameId = model.EquipmentNameId,
                        EquipmentTypeId = model.EquipmentTypeId,
                        
                        DriverAssignmentId = Guid.NewGuid(),
                        FleetId = model.FleetId,
                        EquipmentName = fleet.PlateNumber + " - " + fleet.EquipmentName,
                        FromOperatorName = fleet.OperatorName,
                        ToOperatorName = db.Operator.Find(model.OperatorId).FullName,
                        CreatedBy = User.Identity.Name,
                        Status = MaintenanceConst.Pending,
                        StartingDate = model.StartingDate,
                        ToDate = model.ToDate,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
              
                        DateCreated = DateTime.Now,
                        Remark = model.Remark,
                        BranchId = model.BranchId,
                        OperatorId = model.OperatorId,
                        SecondOperatorId = model.SecondOperatorId
                    };
                    
                    db.DriverAssignments.Add(entity);
                    db.SaveChanges();
                    model.DriverAssignmentId = entity.DriverAssignmentId;
                    
                    Operator Operator = db.Operator.FirstOrDefault(x => x.OperatorID == entity.OperatorId);
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
                    Operator OperatorTwo = db.Operator.FirstOrDefault(x => x.OperatorID == entity.SecondOperatorId);
                    if (OperatorTwo != null)
                    {
                        OperatorTwo.IsActive = true;

                        db.Operator.Attach(OperatorTwo);
                        db.Entry(OperatorTwo).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Operator Assignment", "Success", User.Identity.Name, $" assgined opertor  {OperatorTwo.EmployeeId}", Modules.FleetManagement);
                        #endregion
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
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


        
        public ActionResult UnAssignedEquipment()
        {

            var FixedAssets = db.Fleets.Where(x => x.Status.ToString() == MaintenanceConst.ACTIVE).FirstOrDefault();

            return Json(FixedAssets, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult UnAssigned()
        {

            var FixedAssets = db.Fleets.Where(x => x.Status.ToString() == MaintenanceConst.ACTIVE).FirstOrDefault();

            return Json(FixedAssets, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}