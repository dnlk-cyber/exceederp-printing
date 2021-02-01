using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{
    
    public class MonthlyDepreciationValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MonthlyDepreciationValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<MonthlyDepreciationValidation> monthlyDepreciationValidations = _db.MonthlyDepreciationValidations.Where(ii => ii.MonthlyDepreciationId == id);
            DataSourceResult result = monthlyDepreciationValidations.ToDataSourceResult(request, monthlyDepreciationValidation => new
            {
                MonthlyDepreciationId = monthlyDepreciationValidation.MonthlyDepreciationId,
                Status = monthlyDepreciationValidation.Status,
                Remark = monthlyDepreciationValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyDepreciationValidations_Create([DataSourceRequest] DataSourceRequest request, MonthlyDepreciationValidation monthlyDepreciationValidation, int id)
        {

            var record = _db.MonthlyDepreciations.Find(id);

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
                    if (monthlyDepreciationValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.MonthlyDepreciationId = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();



                    }
                    if (monthlyDepreciationValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   

                    var entity = new MonthlyDepreciationValidation
                    {
                        MonthlyDepreciationId = id,
                        Status = monthlyDepreciationValidation.Status,
                        Remark = monthlyDepreciationValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = monthlyDepreciationValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = monthlyDepreciationValidation.ModifiedBy
                    };

                    _db.MonthlyDepreciationValidations.Add(entity);
                    _db.SaveChanges();
                    monthlyDepreciationValidation.MonthlyDepreciationId = entity.MonthlyDepreciationId;
                }
            }

            return Json(new[] { monthlyDepreciationValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyDepreciationValidations_Update([DataSourceRequest]DataSourceRequest request, MonthlyDepreciationValidation monthlyDepreciationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyDepreciationValidation
                {
                    MonthlyDepreciationId = monthlyDepreciationValidation.MonthlyDepreciationId,
                    Status = monthlyDepreciationValidation.Status,
                    Remark = monthlyDepreciationValidation.Remark,
                };

                _db.MonthlyDepreciationValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { monthlyDepreciationValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyDepreciationValidations_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyDepreciationValidation monthlyDepreciationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyDepreciationValidation
                {
                    MonthlyDepreciationId = monthlyDepreciationValidation.MonthlyDepreciationId,
                    Status = monthlyDepreciationValidation.Status,
                    Remark = monthlyDepreciationValidation.Remark,
                };

                _db.MonthlyDepreciationValidations.Attach(entity);
                _db.MonthlyDepreciationValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { monthlyDepreciationValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateUserCard(FixedAssetIssueItem issueItem)
        {

            var asset = _db.AssetRegistrations.Find(issueItem.AssetRegistrationId);
            var employee = _db.Person_Employee.Find(issueItem.FixedAssetIssue.EmployeeId);
            #region Update UserCard
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
