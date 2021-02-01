using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{
    public class AssetDisposalController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var tax = db.GLTaxRates;
            var assetCategories = db.FixedAssetCategories;
            var assetSubCategories = db.FixedAssetSubCategories;
            var arCustomerProfiles = db.ARCustomerProfiles;
            var fixedAsset = db.AssetRegistrations;
            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.Branch.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var selectedAsset = fixedAsset.Select(
              i => new
              {
                  Name = i.Tag + "-" + i.Name,
                  AssetRegistrationId = i.AssetRegistrationId
              }).AsQueryable();

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
              i => new
              {
                  Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                  Code = i.EmployeeId
              }).AsQueryable();

           

            ViewData["tax"] = tax;
            ViewData["personEmployees"] = selectedEmployee;
            ViewData["customers"] = arCustomerProfiles;
            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["FixedAsset"] = fixedAsset;
            ViewData["defaultCategory"] = assetCategories.FirstOrDefault();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetDisposals_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssetDisposal> assetdisposals = db.AssetDisposals;
            DataSourceResult result = assetdisposals.ToDataSourceResult(request, assetDisposal => new {
                AssetDisposalId = assetDisposal.AssetDisposalId,
                CustomerId = assetDisposal.CustomerId,
                Date = assetDisposal.Date,
                DisposalType = assetDisposal.DisposalType,
                Description = assetDisposal.Description,
                OrgBranchName = assetDisposal.OrgBranchName,

                Status = assetDisposal.Status,
                Remark = assetDisposal.Remark,
                IsVoid = assetDisposal.IsVoid,
                VoidBy = assetDisposal.VoidBy,
                VoidTime = assetDisposal.VoidTime,
                IsOnlineApproved = assetDisposal.IsOnlineApproved,
                OnlineApprovedBy = assetDisposal.OnlineApprovedBy,
                OnlineApprovedTime = assetDisposal.OnlineApprovedTime,
                IsSendForApproval = assetDisposal.IsSendForApproval,
                SendForApprovalBy = assetDisposal.SendForApprovalBy,
                SendForApprovalTime = assetDisposal.SendForApprovalTime,
                IsOnlineTransferred = assetDisposal.IsOnlineTransferred,
                IsOnlineTransferredBy = assetDisposal.IsOnlineTransferredBy,
                OnlineTransferredTime = assetDisposal.OnlineTransferredTime,

                DateCreated = assetDisposal.DateCreated,
                LastModified = assetDisposal.LastModified,
                CreatedBy = assetDisposal.CreatedBy,
                ModifiedBy = assetDisposal.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetDisposals_Create([DataSourceRequest]DataSourceRequest request, AssetDisposal assetDisposal)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetDisposal
                {
                    CustomerId = assetDisposal.CustomerId,
                    Date = assetDisposal.Date,
                    DisposalType = assetDisposal.DisposalType,
                    Description = assetDisposal.Description,
                    OrgBranchName = assetDisposal.OrgBranchName,

                    Status = assetDisposal.Status,
                    Remark = assetDisposal.Remark,
                    IsVoid = assetDisposal.IsVoid,
                    VoidBy = assetDisposal.VoidBy,
                    VoidTime = assetDisposal.VoidTime,
                    IsOnlineApproved = assetDisposal.IsOnlineApproved,
                    OnlineApprovedBy = assetDisposal.OnlineApprovedBy,
                    OnlineApprovedTime = assetDisposal.OnlineApprovedTime,
                    IsSendForApproval = assetDisposal.IsSendForApproval,
                    SendForApprovalBy = assetDisposal.SendForApprovalBy,
                    SendForApprovalTime = assetDisposal.SendForApprovalTime,
                    IsOnlineTransferred = assetDisposal.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetDisposal.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetDisposal.OnlineTransferredTime,
                    DateCreated = assetDisposal.DateCreated,
                    LastModified = assetDisposal.LastModified,
                    CreatedBy = assetDisposal.CreatedBy,
                    ModifiedBy = assetDisposal.ModifiedBy
                };

                db.AssetDisposals.Add(entity);
                db.SaveChanges();
                assetDisposal.AssetDisposalId = entity.AssetDisposalId;
            }

            return Json(new[] { assetDisposal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetDisposals_Update([DataSourceRequest]DataSourceRequest request, AssetDisposal assetDisposal)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetDisposal
                {
                    AssetDisposalId = assetDisposal.AssetDisposalId,
                    CustomerId = assetDisposal.CustomerId,
                    Date = assetDisposal.Date,
                    DisposalType = assetDisposal.DisposalType,
                    Description = assetDisposal.Description,
                    OrgBranchName = assetDisposal.OrgBranchName,

                    Status = assetDisposal.Status,
                    Remark = assetDisposal.Remark,
                    IsVoid = assetDisposal.IsVoid,
                    VoidBy = assetDisposal.VoidBy,
                    VoidTime = assetDisposal.VoidTime,
                    IsOnlineApproved = assetDisposal.IsOnlineApproved,
                    OnlineApprovedBy = assetDisposal.OnlineApprovedBy,
                    OnlineApprovedTime = assetDisposal.OnlineApprovedTime,
                    IsSendForApproval = assetDisposal.IsSendForApproval,
                    SendForApprovalBy = assetDisposal.SendForApprovalBy,
                    SendForApprovalTime = assetDisposal.SendForApprovalTime,
                    IsOnlineTransferred = assetDisposal.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetDisposal.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetDisposal.OnlineTransferredTime,
                    DateCreated = assetDisposal.DateCreated,
                    LastModified = assetDisposal.LastModified,
                    CreatedBy = assetDisposal.CreatedBy,
                    ModifiedBy = assetDisposal.ModifiedBy
                };

                db.AssetDisposals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assetDisposal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetDisposals_Destroy([DataSourceRequest]DataSourceRequest request, AssetDisposal assetDisposal)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetDisposal
                {
                    AssetDisposalId = assetDisposal.AssetDisposalId,
                    CustomerId = assetDisposal.CustomerId,
                    Date = assetDisposal.Date,
                    DisposalType = assetDisposal.DisposalType,
                    Description = assetDisposal.Description,
                    OrgBranchName = assetDisposal.OrgBranchName,

                    Status = assetDisposal.Status,
                    Remark = assetDisposal.Remark,
                    IsVoid = assetDisposal.IsVoid,
                    VoidBy = assetDisposal.VoidBy,
                    VoidTime = assetDisposal.VoidTime,
                    IsOnlineApproved = assetDisposal.IsOnlineApproved,
                    OnlineApprovedBy = assetDisposal.OnlineApprovedBy,
                    OnlineApprovedTime = assetDisposal.OnlineApprovedTime,
                    IsSendForApproval = assetDisposal.IsSendForApproval,
                    SendForApprovalBy = assetDisposal.SendForApprovalBy,
                    SendForApprovalTime = assetDisposal.SendForApprovalTime,
                    IsOnlineTransferred = assetDisposal.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetDisposal.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetDisposal.OnlineTransferredTime,
                    DateCreated = assetDisposal.DateCreated,
                    LastModified = assetDisposal.LastModified,
                    CreatedBy = assetDisposal.CreatedBy,
                    ModifiedBy = assetDisposal.ModifiedBy
                };

                db.AssetDisposals.Attach(entity);
                db.AssetDisposals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assetDisposal }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.AssetDisposals.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.DisposalItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.AssetDisposals.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.DisposalItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
