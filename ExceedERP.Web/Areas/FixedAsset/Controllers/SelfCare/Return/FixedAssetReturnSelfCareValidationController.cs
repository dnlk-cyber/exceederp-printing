using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Return
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class FixedAssetReturnSelfCareValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();


        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult FixedAssetReturnDepartmentValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetReturnDepartmentValidation> FixedAssetReturnDepartmentValidations = _db.FixedAssetReturnDepartmentValidations.Where(ii => ii.FixedAssetReturnId == id);
            DataSourceResult result = FixedAssetReturnDepartmentValidations.ToDataSourceResult(request, FixedAssetReturnDepartmentValidation => new
            {
                FixedAssetReturnDepartmentValidationId = FixedAssetReturnDepartmentValidation.FixedAssetReturnDepartmentValidationId,
                Status = FixedAssetReturnDepartmentValidation.Status,
                Remark = FixedAssetReturnDepartmentValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnDepartmentValidations_Create([DataSourceRequest] DataSourceRequest request, FixedAssetReturnDepartmentValidation FixedAssetReturnDepartmentValidation, int id)
        {

            var fixedAssetReturn = _db.FixedAssetReturns.Find(id);

            #region Checking the return

            if (fixedAssetReturn != null)
            {
                if (fixedAssetReturn.IsDepartmentOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    if (FixedAssetReturnDepartmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        //foreach (FixedAssetReturnItem i in record.FixedAssetReturnItems)
                        //{
                           
                        //}
                    }
                   
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (fixedAssetReturn != null)
                {
                    //validate 
                    if (FixedAssetReturnDepartmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        
                        fixedAssetReturn.Status = "Department Approved";
                        fixedAssetReturn.IsDepartmentOnlineApproved = true;
                        fixedAssetReturn.DepartmentOnlineApprovedBy = User.Identity.Name;
                        fixedAssetReturn.DepartmentOnlineApprovedTime = DateTime.Now;
                        _db.Entry(fixedAssetReturn).State = EntityState.Modified;
                        _db.SaveChanges();



                    }
                    if (FixedAssetReturnDepartmentValidation.Status == ApprovalStatuses.Reject)
                    {
                        fixedAssetReturn.IsSendForApproval = false;
                        fixedAssetReturn.Status = "Rejected";
                        _db.Entry(fixedAssetReturn).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   

                    var entity = new FixedAssetReturnDepartmentValidation
                    {
                        FixedAssetReturnId = id,
                        Status = FixedAssetReturnDepartmentValidation.Status,
                        Remark = FixedAssetReturnDepartmentValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = FixedAssetReturnDepartmentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = FixedAssetReturnDepartmentValidation.ModifiedBy
                    };

                    _db.FixedAssetReturnDepartmentValidations.Add(entity);
                    _db.SaveChanges();
                    FixedAssetReturnDepartmentValidation.FixedAssetReturnDepartmentValidationId = entity.FixedAssetReturnDepartmentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturnDepartmentValidation", "Success", User.Identity.Name, $"created FixedAssetReturnDepartmentValidation {entity.FixedAssetReturnDepartmentValidationId} FixedAssetReturnId {entity.FixedAssetReturnId}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { FixedAssetReturnDepartmentValidation }.ToDataSourceResult(request, ModelState));

        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnDepartmentValidations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReturnDepartmentValidation FixedAssetReturnDepartmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnDepartmentValidation
                {
                    FixedAssetReturnDepartmentValidationId = FixedAssetReturnDepartmentValidation.FixedAssetReturnDepartmentValidationId,
                    Status = FixedAssetReturnDepartmentValidation.Status,
                    Remark = FixedAssetReturnDepartmentValidation.Remark,
                };

                _db.FixedAssetReturnDepartmentValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturnDepartmentValidation", "Success", User.Identity.Name, $"update FixedAssetReturnDepartmentValidation {entity.FixedAssetReturnDepartmentValidationId} FixedAssetReturnId {entity.FixedAssetReturnId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { FixedAssetReturnDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnDepartmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReturnDepartmentValidation FixedAssetReturnDepartmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnDepartmentValidation
                {
                    FixedAssetReturnDepartmentValidationId = FixedAssetReturnDepartmentValidation.FixedAssetReturnDepartmentValidationId,
                    Status = FixedAssetReturnDepartmentValidation.Status,
                    Remark = FixedAssetReturnDepartmentValidation.Remark,
                };

                _db.FixedAssetReturnDepartmentValidations.Attach(entity);
                _db.FixedAssetReturnDepartmentValidations.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"delete FixedAssetReturnDepartmentValidation {FixedAssetReturnDepartmentValidation.FixedAssetReturnDepartmentValidationId} FixedAssetReturnId {FixedAssetReturnDepartmentValidation.FixedAssetReturnId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { FixedAssetReturnDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateLocationTrackingAndUserCard(FixedAssetReturnItem fixedAssetReturnItem)
        {
            //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name).ToString();
            //var orgId = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name).ToString();
            //var user = User.Identity.Name;
            
            // Update LocationTracking
            //FixedAssetHelper.UpdateLocationTracking(fixedAssetReturnItem, User.Identity.Name);

            //// Update AssetInfo
            //FixedAssetHelper.UpdateAssetInfo(fixedAssetReturnItem);

            //// Remove from UserCard
            //FixedAssetHelper.RemovePreviousUserCard(fixedAssetReturnItem);
        }

        private void UpdateAssetMovementAndUserCard(FixedAssetReturnItem item)
        {

            var asset = _db.AssetRegistrations.Find(item.AssetRegistrationId);

            #region get employee and workunit

            string employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name).ToString();
            string orgid = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name).ToString();

            #endregion

            #region Update asset info

            if (asset != null)
            {
                asset.StoreCode = item.FixedAssetReturn.StoreCode;
                asset.EmployeeId = employeeid;
                asset.OrgStructureId = orgid;
                _db.Entry(asset).State = EntityState.Modified;
                _db.SaveChanges();
            }

            #endregion

            #region Update Location tracking to store

            if (asset != null)
            {
                var location = new LocationTracking
                {
                    AssetRegistrationId = asset.AssetRegistrationId,
                    StoreCode = item.FixedAssetReturn.StoreCode,
                    EmployeeId=employeeid,
                    OrgStructureId=orgid,
                    LocationType = LocationType.Store,
                    StartDate = DateTime.Today,
                    BranchId = InventoryHelper.GetStoreBranch(item.FixedAssetReturn.StoreCode),
                    TransactionType = FixedAssetTransactionType.Return,
                    TransactionNo = item.FixedAssetReturnId.ToString(),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };
                _db.LocationTrackings.Add(location);
                _db.SaveChanges();

                #region update previous location track as not current

                var userCard = _db.UserCardItems.Where(q => q.AssetRegistrationId == asset.AssetRegistrationId).FirstOrDefault();
                if(userCard!=null)
                {
                    _db.UserCardItems.Attach(userCard);
                    _db.UserCardItems.Remove(userCard);
                }

                var prevs = _db.LocationTrackings.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                    .Where(x => x.LocationTrackingId != location.LocationTrackingId).ToList();

                foreach (var prev in prevs)
                {
                    prev.IsCurrent = false;
                    _db.Entry(prev).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion
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
