using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    // Stage 1 of the site vehicle transfer workflow: request -> check -> approve.
    // The requestor only specifies equipment category/type/name; the vehicle is assigned later by the distributor.
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceRequestChecker, FleetManagementRoles.ResourceRequestApprover)]
    public class SitesVehicleTransferSC_Controller : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            ViewData["Branch"] = CacheData.GetBranchCaches().Select(i => new { Name = i.Name, BranchId = i.Id }).AsQueryable();
            ViewData["EquipmentCategory"] = CacheData.GetEquipmentIdentityCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            return View();
        }

        public ActionResult SitesVehicleTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer.Where(x => x.OrgBranchName == defaultBranch);
            // IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer;
            DataSourceResult Result = SVT.ToDataSourceResult(request, s => new
            {
                SitesVehicleTransferID = s.SitesVehicleTransferID,
                RequestNumber = s.RequestNumber,
                RequestDate = s.RequestDate,
                EquipmentIdentityID = s.EquipmentIdentityID,
                EquipmentTypeID = s.EquipmentTypeID,
                EquipmentNameID = s.EquipmentNameID,
                BranchId = s.BranchId,
                DestinationSite = s.DestinationSite,
                StartingDate = s.StartingDate,
                ToDate = s.ToDate,
                TransferReason = s.TransferReason,
                Description = s.Description,
                Status = s.Status,
                Remark = s.Remark,
                RequestedBy = s.RequestedBy,
                IsRequestChecked = s.IsRequestChecked,
                RequestCheckedBy = s.RequestCheckedBy,
                RequestCheckedDate = s.RequestCheckedDate,
                IsRequestApproved = s.IsRequestApproved,
                RequestApprovedBy = s.RequestApprovedBy,
                RequestApprovedDate = s.RequestApprovedDate,
                IsRequestReceived = s.IsRequestReceived,
                PlateNo = s.Fleet != null ? s.Fleet.PlateNumber : null,
                IsOnlineApproved = s.IsOnlineApproved,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                ModifiedBy = s.ModifiedBy,
                LastModified = s.LastModified
            });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Create([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            ModelState.Remove("SitesVehicleTransferID");
            ModelState.Remove("InitialSite");
            ModelState.Remove("RequestNumber");
            if (sitesVehicleTransfer.EquipmentIdentityID == Guid.Empty || sitesVehicleTransfer.EquipmentTypeID == Guid.Empty || sitesVehicleTransfer.EquipmentNameID == Guid.Empty)
            {
                ModelState.AddModelError("EquipmentNameID", "Equipment Category, Type and Name are required");
            }
            if (sitesVehicleTransfer.StartingDate > sitesVehicleTransfer.ToDate)
            {
                ModelState.AddModelError("ToDate", "Start Date is Above End Date");
            }
            if (ModelState.IsValid)
            {
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == sitesVehicleTransfer.BranchId);
                sitesVehicleTransfer.SitesVehicleTransferID = Guid.NewGuid();
                sitesVehicleTransfer.DestinationSite = branch?.Name ?? "";
                sitesVehicleTransfer.RequestDate = sitesVehicleTransfer.RequestDate ?? DateTime.Now;
                sitesVehicleTransfer.RequestedBy = User.Identity.Name;
                sitesVehicleTransfer.Status = SitesVehicleTransferStatus.Draft;
                sitesVehicleTransfer.CreatedBy = User.Identity.Name;
                sitesVehicleTransfer.DateCreated = DateTime.Now;
                sitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                

                try
                {
                    db.SitesVehicleTransfer.Add(sitesVehicleTransfer);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    var sqlEx = ex.InnerException;

                    while (sqlEx.InnerException != null)
                    {
                        sqlEx = sqlEx.InnerException;
                    }

                    throw new Exception(sqlEx.Message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }
                // Num is an identity column, so the request number is derived after the first save
                sitesVehicleTransfer.RequestNumber = "SVT-" + sitesVehicleTransfer.Num.ToString("D5");
                db.Entry(sitesVehicleTransfer).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer request", "Success", User.Identity.Name, $"created site vehicle transfer request {sitesVehicleTransfer.RequestNumber}", Modules.FleetManagement);
            }
            return Json(new[] { sitesVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Update([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            ModelState.Remove("Num");
            ModelState.Remove("InitialSite");
            ModelState.Remove("RequestNumber");
            if (sitesVehicleTransfer.StartingDate > sitesVehicleTransfer.ToDate)
            {
                ModelState.AddModelError("ToDate", "Start Date is Above End Date");
            }
            if (ModelState.IsValid)
            {
                var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == sitesVehicleTransfer.SitesVehicleTransferID);
                if (entity == null || entity.IsRequestChecked)
                {
                    ModelState.AddModelError("", "Request can no longer be edited");
                }
                else
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == sitesVehicleTransfer.BranchId);
                    entity.EquipmentIdentityID = sitesVehicleTransfer.EquipmentIdentityID;
                    entity.EquipmentTypeID = sitesVehicleTransfer.EquipmentTypeID;
                    entity.EquipmentNameID = sitesVehicleTransfer.EquipmentNameID;
                    entity.BranchId = sitesVehicleTransfer.BranchId;
                    entity.DestinationSite = branch?.Name ?? entity.DestinationSite;
                    entity.RequestDate = sitesVehicleTransfer.RequestDate;
                    entity.StartingDate = sitesVehicleTransfer.StartingDate;
                    entity.ToDate = sitesVehicleTransfer.ToDate;
                    entity.TransferReason = sitesVehicleTransfer.TransferReason;
                    entity.Description = sitesVehicleTransfer.Description;
                    entity.ModifiedBy = User.Identity.Name;
                    entity.LastModified = DateTime.Now;
                    if (entity.Status == SitesVehicleTransferStatus.Rejected)
                    {
                        entity.Status = SitesVehicleTransferStatus.Draft;
                        entity.Remark = null;
                    }
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer request", "Success", User.Identity.Name, $"updated site vehicle transfer request {entity.RequestNumber}", Modules.FleetManagement);
                }
            }
            return Json(new[] { sitesVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Destroy([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == sitesVehicleTransfer.SitesVehicleTransferID);
            if (entity != null && !entity.IsRequestChecked)
            {
                db.SitesVehicleTransfer.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete site equipment transfer request " + entity.RequestNumber);
            }
            return Json(new[] { sitesVehicleTransfer }.ToDataSourceResult(request));
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestChecker)]
        [HttpPost]
        public JsonResult CheckRequest(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || entity.IsRequestChecked)
                return Json(new { Message = "Request not found or already checked", Status = "Error" });

            entity.IsRequestChecked = true;
            entity.RequestCheckedBy = User.Identity.Name;
            entity.RequestCheckedDate = DateTime.Now;
            entity.Status = SitesVehicleTransferStatus.RequestChecked;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer request", "Success", User.Identity.Name, $"checked request {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Request Checked", Status = "OK" });
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestApprover)]
        [HttpPost]
        public JsonResult ApproveRequest(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || !entity.IsRequestChecked || entity.IsRequestApproved)
                return Json(new { Message = "Request must be checked before approval", Status = "Error" });

            entity.IsRequestApproved = true;
            entity.RequestApprovedBy = User.Identity.Name;
            entity.RequestApprovedDate = DateTime.Now;
            entity.Status = SitesVehicleTransferStatus.RequestApproved;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer request", "Success", User.Identity.Name, $"approved request {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Request Approved", Status = "OK" });
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestChecker, FleetManagementRoles.ResourceRequestApprover)]
        [HttpPost]
        public JsonResult RejectRequest(Guid id, string remark)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || entity.IsRequestApproved)
                return Json(new { Message = "Approved request cannot be rejected", Status = "Error" });

            entity.IsRequestChecked = false;
            entity.RequestCheckedBy = null;
            entity.RequestCheckedDate = null;
            entity.Status = SitesVehicleTransferStatus.Rejected;
            entity.Remark = remark;
            entity.ModifiedBy = User.Identity.Name;
            entity.LastModified = DateTime.Now;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer request", "Success", User.Identity.Name, $"rejected request {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Request Rejected", Status = "OK" });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
