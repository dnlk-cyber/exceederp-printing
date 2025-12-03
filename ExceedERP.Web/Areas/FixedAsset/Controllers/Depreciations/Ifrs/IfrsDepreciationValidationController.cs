using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{

    public class IfrsDepreciationValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult IfrsDepreciationValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IfrsDepreciationValidation> IfrsDepreciationValidations = _db.IfrsDepreciationValidations.Where(ii => ii.IfrsDepreciationId == id);
            DataSourceResult result = IfrsDepreciationValidations.ToDataSourceResult(request, IfrsDepreciationValidation => new
            {
                IfrsDepreciationId = IfrsDepreciationValidation.IfrsDepreciationId,
                Status = IfrsDepreciationValidation.Status,
                Remark = IfrsDepreciationValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsDepreciationValidations_Create([DataSourceRequest] DataSourceRequest request, IfrsDepreciationValidation IfrsDepreciationValidation, int id)
        {

            var record = _db.IfrsDepreciations.Find(id);

            #region validation 

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                
            }
            else
            {
                ModelState.AddModelError("No Found", "Record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (IfrsDepreciationValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IfrsDepreciationId = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();



                    }
                    if (IfrsDepreciationValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   

                    var entity = new IfrsDepreciationValidation
                    {
                        IfrsDepreciationId = id,
                        Status = IfrsDepreciationValidation.Status,
                        Remark = IfrsDepreciationValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = IfrsDepreciationValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = IfrsDepreciationValidation.ModifiedBy
                    };

                    _db.IfrsDepreciationValidations.Add(entity);
                    _db.SaveChanges();
                    IfrsDepreciationValidation.IfrsDepreciationId = entity.IfrsDepreciationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Ifrs Depreciation Validation", "Success", User.Identity.Name, $"created Ifrs Depreciation Validation {entity.IfrsDepreciationValidationId} Tag {entity.IfrsDepreciationId}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { IfrsDepreciationValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsDepreciationValidations_Update([DataSourceRequest]DataSourceRequest request, IfrsDepreciationValidation IfrsDepreciationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsDepreciationValidation
                {
                    IfrsDepreciationId = IfrsDepreciationValidation.IfrsDepreciationId,
                    Status = IfrsDepreciationValidation.Status,
                    Remark = IfrsDepreciationValidation.Remark,
                };

                _db.IfrsDepreciationValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Ifrs Depreciation Validation", "Success", User.Identity.Name, $"updated  Ifrs Depreciation Validation {entity.IfrsDepreciationValidationId} Tag {entity.IfrsDepreciationId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { IfrsDepreciationValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsDepreciationValidations_Destroy([DataSourceRequest]DataSourceRequest request, IfrsDepreciationValidation IfrsDepreciationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsDepreciationValidation
                {
                    IfrsDepreciationId = IfrsDepreciationValidation.IfrsDepreciationId,
                    Status = IfrsDepreciationValidation.Status,
                    Remark = IfrsDepreciationValidation.Remark,
                };

                _db.IfrsDepreciationValidations.Attach(entity);
                _db.IfrsDepreciationValidations.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete  Ifrs Depreciation Validation {IfrsDepreciationValidation.IfrsDepreciationValidationId} Tag {IfrsDepreciationValidation.IfrsDepreciationId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { IfrsDepreciationValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateUserCard(FixedAssetIssueItem issueItem)
        {

            var asset = _db.AssetRegistrations.Find(issueItem.AssetRegistrationId);
            var employee = _db.Person_Employee.Find(issueItem.FixedAssetIssue.EmployeeId);
            #region Update userCard
            UserCardItems userCard = new UserCardItems();
            if (employee != null)
            {
                userCard.EmployeeId = employee.EmployeeId;
                userCard.EmployeeFullName = employee.FirstName + "_" + employee.MiddleName + "_" + employee.LastName;
            }
            if (asset != null)
            {
                userCard.AssetRegistrationId = asset.AssetRegistrationId;
                userCard.Name = asset.Name;
                userCard.Tag = asset.Tag;
                userCard.IssueDate = issueItem.FixedAssetIssue.IssueDate;
                userCard.UnitPrice = asset.BookValue;
                userCard.Quanity = issueItem.IssuedQuantity;
                userCard.SerialNo = issueItem.SerialNo;
                userCard.SIVNo = issueItem.FixedAssetIssue.FixedAssetIssueID.ToString();
            }
            userCard.Active = true;
            userCard.DateCreated = DateTime.Now;
            userCard.CreatedBy = User.Identity.Name;
            _db.UserCardItems.Add(userCard);
            _db.SaveChanges();
            #endregion
            #region Update Location tracking

            if (asset != null)
            {
                var location = new LocationTracking
                {
                    AssetRegistrationId = asset.AssetRegistrationId,
                    LocationType = LocationType.Employee,
                    StartDate = DateTime.Today,
                    BranchId = issueItem.FixedAssetIssue.BranchId,
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name
                };
                _db.LocationTrackings.Add(location);
                _db.SaveChanges();
            }

            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
