using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Receives
{


    public class FixedAssetReceiveValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult Index()
        {
            return View();
        }
       
        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult FixedAssetReceiveValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetReceiveValidation> fixedAssetReceiveValidations = db.FixedAssetReceiveValidations.Where(gi => gi.FixedAssetReceiveId == id);
            DataSourceResult result = fixedAssetReceiveValidations.ToDataSourceResult(request, fixedAssetReceiveValidation => new
            {
                FixedAssetReceiveValidationID = fixedAssetReceiveValidation.FixedAssetReceiveValidationID,
                Status = fixedAssetReceiveValidation.Status,
                Remark = fixedAssetReceiveValidation.Remark,
                OrgBranchName = fixedAssetReceiveValidation.OrgBranchName,
                Approver = fixedAssetReceiveValidation.Approver,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveValidations_Create([DataSourceRequest] DataSourceRequest request, FixedAssetReceiveValidation fixedAssetReceiveValidation, int id)
        {
            FixedAssetReceive record = db.FixedAssetReceives.Find(id);
            #region Checking the grn

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (record.FixedAssetReceiveItem.Count == 0)
                {
                    string message = "The Record does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
                {
                    if (fixedAssetReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (var i in record.FixedAssetReceiveItem)
                        {
                            if (i.FixedAssetCategoryID==0)
                            {
                                string message = "Please set Fixed Asset category for all Items.";
                                ModelState.AddModelError(String.Empty, message);
                            }
                            if (i.FixedAssetSubCategoryID==0)
                            {
                                string message = "Please set Fixed Asset sub category for all Items.";
                                ModelState.AddModelError(String.Empty, message);
                            }
                        }
                    }
                   
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "GRN record not found");
            }

            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (fixedAssetReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.FixedAssetReceiveId = id;
                        #region update 

                        foreach (var i in record.FixedAssetReceiveItem)
                        {
                            TransferToAssetRegister(i);
                        }
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                      
                        #endregion

                    }
                    if (fixedAssetReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                   

                    var entity = new FixedAssetReceiveValidation()
                    {
                        FixedAssetReceiveId = id,
                        Status = fixedAssetReceiveValidation.Status,
                        Remark = fixedAssetReceiveValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = fixedAssetReceiveValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = fixedAssetReceiveValidation.ModifiedBy
                    };

                    db.FixedAssetReceiveValidations.Add(entity);
                    db.SaveChanges();
                    fixedAssetReceiveValidation.FixedAssetReceiveValidationID = entity.FixedAssetReceiveValidationID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveValidation", "Success", User.Identity.Name, $"created  FixedAssetReceiveValidation {entity.FixedAssetReceiveValidationID} Tag {entity.FixedAssetReceiveId}", Modules.FixedAsset);
                    #endregion

                }

            }

            return Json(new[] { fixedAssetReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveValidation fixedAssetReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveValidation
                {
                    FixedAssetReceiveValidationID = fixedAssetReceiveValidation.FixedAssetReceiveValidationID,
                    Status = fixedAssetReceiveValidation.Status,
                    Remark = fixedAssetReceiveValidation.Remark,
                    Approver = User.Identity.Name,
                    OrgBranchName = fixedAssetReceiveValidation.OrgBranchName,
                };

                db.FixedAssetReceiveValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveValidation", "Success", User.Identity.Name, $"updated  FixedAssetReceiveValidation {entity.FixedAssetReceiveValidationID} Tag {entity.FixedAssetReceiveId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveValidation fixedAssetReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveValidation()
                {
                    FixedAssetReceiveValidationID = fixedAssetReceiveValidation.FixedAssetReceiveValidationID,
                    Status = fixedAssetReceiveValidation.Status,
                    Remark = fixedAssetReceiveValidation.Remark,
                    OrgBranchName = fixedAssetReceiveValidation.OrgBranchName,
                };

                db.FixedAssetReceiveValidations.Attach(entity);
                db.FixedAssetReceiveValidations.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Deleted", "Success", User.Identity.Name, $"Delete  FixedAssetReceiveValidation {fixedAssetReceiveValidation.FixedAssetReceiveValidationID} Tag {fixedAssetReceiveValidation.FixedAssetReceiveId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        private void TransferToAssetRegister(FixedAssetReceiveItem itemReceive)
        {
            var assetReceive = db.FixedAssetReceives.Find(itemReceive.FixedAssetReceiveId);
            var assetRegistration = new AssetRegistration
            {
                FixedAssetCategoryID = itemReceive.FixedAssetCategoryID,
                FixedAssetSubCategoryID = itemReceive.FixedAssetSubCategoryID,
                IsGroup= itemReceive.FixedAssetSubCategory.IsGroup,
                Name = itemReceive.Name,
                Unit = itemReceive.Unit,
                Quanity = itemReceive.Quantity,
                BookValue = itemReceive.UnitPrice,
                OriginalValue = itemReceive.UnitPrice,
                PurchasePrice = itemReceive.UnitPrice,
                PurchaseDate = assetReceive?.ReceiveDate,
                GRNDate = assetReceive?.ReceiveDate,
                IsMachinery = itemReceive.IsMachinery,
                FixedAssetConditionId = itemReceive.FixedAssetConditionId, 
                GRN = assetReceive?.FixedAssetReceiveId.ToString(),
                Tag = itemReceive.TagNo,
                SerialNo = itemReceive.SerialNo,
                ModelNo = itemReceive.Model,
                Size = itemReceive.Size,
                ChassisNo = itemReceive.ChassisNo,
                MotorNo = itemReceive.EngineNo,
                Supplier = assetReceive?.SupplierName,
                Status = RegistrationStatus.Active,
                StoreCode = assetReceive?.StoreCode,
                BranchId = (int)assetReceive?.BranchId
            };

            #region generate tag

            string tagPrefix = "";
            var tagSetting = db.TagCodeSettings.OrderBy(x => x.Index).ToList();

            if (tagSetting.Any())
            {
                foreach (var tag in tagSetting)
                {
                    switch (tag.Type)
                    {
                        case TagAbbreviationType.Company:
                            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                            if (organization != null) tagPrefix = tagPrefix + "-" + organization.Abbreviation;
                            break;

                        case TagAbbreviationType.BranchOrSector:
                            var branch = db.Branch.FirstOrDefault(x => x.BranchId == itemReceive.FixedAssetReceive.BranchId);
                            if (branch != null) tagPrefix = tagPrefix + "-" + branch.Abbreviation;
                            break;
                        case TagAbbreviationType.Category:
                            tagPrefix = tagPrefix + "-" + itemReceive.FixedAssetCategory.Abbreviation;
                            break;
                        case TagAbbreviationType.SubCategory:
                            tagPrefix = tagPrefix + "-" + itemReceive.FixedAssetSubCategory.SubCategoryCode;
                            break;
                        default:
                            break;
                    }
                }
                //trim first -
                tagPrefix = tagPrefix.Remove(0, 1);
                var indexNumber = db.IndexingCounts.FirstOrDefault(x => x.IndexCode == tagPrefix);
                if (indexNumber == null)
                {
                    var newindexNumber = new IndexingCount
                    {
                        IndexCode = tagPrefix,
                        SubCategoryCode = itemReceive.FixedAssetSubCategory.SubCategoryCode,
                        CurrentValue = 1,
                        FixedAssetSubCategory = itemReceive.FixedAssetSubCategory
                    };
                    db.IndexingCounts.Add(newindexNumber);
                    db.SaveChanges();

                    tagPrefix = tagPrefix + "-" + 1.ToString().PadLeft(4, '0');
                }
                else
                {
                    tagPrefix = tagPrefix + "-" + (indexNumber.CurrentValue + 1).ToString().PadLeft(4, '0');

                    indexNumber.CurrentValue = indexNumber.CurrentValue + 1;
                    db.Entry(indexNumber).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            #endregion

            #region get employee and workunit

            string employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name).ToString();
            string orgid = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name).ToString();

            #endregion

            #region update asset

            assetRegistration.Tag =String.IsNullOrEmpty(tagPrefix)? itemReceive.TagNo:tagPrefix;
            assetRegistration.EmployeeId =employeeid ;
            assetRegistration.OrgStructureId =orgid ;
            db.AssetRegistrations.Add(assetRegistration);
            db.SaveChanges();

            itemReceive.TagNo = String.IsNullOrEmpty(tagPrefix) ? itemReceive.TagNo : tagPrefix;
            db.FixedAssetReceiveItems.Attach(itemReceive);
            db.Entry(itemReceive).State = EntityState.Modified;
            db.SaveChanges();

            #endregion

            #region update location tracking

            var tracking = new LocationTracking
            {
                IsCurrent=true,
                LocationType = LocationType.Store,
                StartDate = itemReceive.FixedAssetReceive.ReceiveDate,
                StoreCode = itemReceive.FixedAssetReceive.StoreCode,
                BranchId = itemReceive.FixedAssetReceive.BranchId,
                EmployeeId=employeeid,
                OrgStructureId=orgid,
                AssetRegistration = assetRegistration,
                AssetRegistrationId = assetRegistration.AssetRegistrationId,
                TransactionType = FixedAssetTransactionType.Receive,//changed from string to enum
                TransactionNo = itemReceive.FixedAssetReceiveId.ToString(),
            };
            db.LocationTrackings.Add(tracking);
            db.SaveChanges();

            #endregion

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
