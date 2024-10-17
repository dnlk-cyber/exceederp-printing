using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Issues
{

    public class FixedAssetIssueValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetIssueValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetIssueValidation> fixedAssetIssueValidations = db.FixedAssetIssueValidations.Where(ii => ii.FixedAssetIssueID == id);
            DataSourceResult result = fixedAssetIssueValidations.ToDataSourceResult(request, issueValidation => new
            {
                FixedAssetIssueValidationID = issueValidation.FixedAssetIssueValidationID,
                Status = issueValidation.Status,
                Remark = issueValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssueValidations_Create([DataSourceRequest] DataSourceRequest request, FixedAssetIssueValidation issueValidation, int id)
        {

            var record = db.FixedAssetIssues.Find(id);
            var fixedAssetIssueItems = db.FixedAssetIssueItems.Where(x => x.FixedAssetIssueID == id).ToList();

            #region validate item selection


            foreach (var asset in fixedAssetIssueItems)
            {
                if (asset.AssetRegistrationId == 0)
                {
                    var formattedData2 = new
                    {
                        Message = "Please select asset to be issued for each item.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
            }

            #endregion

            #region Checking the Issue

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record, it is already approved";
                    ModelState.AddModelError("approved", message);
                }

                {
                    if (issueValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (FixedAssetIssueItem i in record.FixedAssetIssueItems)
                        {
                            //if (!FixedAssetHelper.IsAssetCurrentLocationStore(i.AssetRegistrationId, i.FixedAssetIssue.StoreCode))
                            //{
                            //    string message = "Asset with tag number " + i.TagNo + " does not exist in store " + i.FixedAssetIssue.StoreCode;
                            //    ModelState.AddModelError("doesnotexist", message);

                            //}
                        }
                    }
                    if (issueValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Issue record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (issueValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.FixedAssetIssueID = id;

                        #region update stock based on the issue

                        string storecode = record.StoreCode;

                        foreach (var fixedAssetIssueItem in fixedAssetIssueItems)
                        {
                           
                            FixedAssetHelper.UpdateUserCard(fixedAssetIssueItem, User.Identity.Name);
                            FixedAssetHelper.UpdateLocationTracking(fixedAssetIssueItem, User.Identity.Name);
                            FixedAssetHelper.UpdateAssetInfo(fixedAssetIssueItem, User.Identity.Name);
                          
                        }

                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                        #endregion

                    }

                    if (issueValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }

                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();

                    var entity = new FixedAssetIssueValidation
                    {
                        FixedAssetIssueID = id,
                        Status = issueValidation.Status,
                        Remark = issueValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = issueValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = issueValidation.ModifiedBy
                    };

                    db.FixedAssetIssueValidations.Add(entity);
                    db.SaveChanges();
                    issueValidation.FixedAssetIssueValidationID = entity.FixedAssetIssueValidationID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, " FixedAssetIssueValidation", "Success", User.Identity.Name, $"created  FixedAssetIssueValidation {entity.FixedAssetIssueValidationID} Tag {entity.FixedAssetIssueID}", Modules.FixedAsset);
                    #endregion

                }
            }

            return Json(new[] { issueValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssueValidations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetIssueValidation issueValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new 
                    FixedAssetIssueValidation
                {
                    FixedAssetIssueValidationID = issueValidation.FixedAssetIssueValidationID,
                    Status = issueValidation.Status,
                    Remark = issueValidation.Remark,
                };

                db.FixedAssetIssueValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " FixedAssetIssueValidation", "Success", User.Identity.Name, $"updated  FixedAssetIssueValidation {entity.FixedAssetIssueValidationID} Tag {entity.FixedAssetIssueID}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { issueValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssueValidations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetIssueValidation issueValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetIssueValidation
                {
                    FixedAssetIssueValidationID = issueValidation.FixedAssetIssueValidationID,
                    Status = issueValidation.Status,
                    Remark = issueValidation.Remark,
                };

                db.FixedAssetIssueValidations.Attach(entity);
                db.FixedAssetIssueValidations.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " delete", "Success", User.Identity.Name, $"deleted  FixedAssetIssueValidation {issueValidation.FixedAssetIssueValidationID} Tag {issueValidation.FixedAssetIssueID}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { issueValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateUserCard(FixedAssetIssueItem issueItem)
        {

            var asset = db.AssetRegistrations.Find(issueItem.AssetRegistrationId);

            var issue = db.FixedAssetIssues.Find(issueItem.FixedAssetIssueID);

            UserCardItems userCard = new UserCardItems();

            Person_Employee employee = new Person_Employee();

            string employeeid = "";

            string orgid = "";

            if (issue!=null)
            {
                 employee = db.Person_Employee.Find(issue.RecipientId);

                if (employee != null)
                {
                    userCard.EmployeeId = employee.EmployeeId;
                    userCard.EmployeeFullName = employee.FirstNameEnglish + "_" + employee.MiddleNameEnglish + "_" + employee.LastNameEnglish;

                    #region get employee and workunit

                     employeeid = employee?.EmployeeId.ToString() ?? "";

                     orgid = employee != null ? UserInformationHelper.GetEmployeeDepartmentIdByEmpId(employee.EmployeeId).ToString() : "";

                    #endregion          


                }

            }

            #region Update UserCard
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
            db.UserCardItems.Add(userCard);
            db.SaveChanges();
            #endregion
            
            #region Update asset info

            if (asset != null)
            {
                asset.StoreCode = "";
                asset.SIV = issueItem.FixedAssetIssueID.ToString();
                asset.EmployeeId = employeeid;
                asset.OrgStructureId = orgid;
                db.Entry(asset).State = EntityState.Modified;
                db.SaveChanges();
            }

            #endregion
            #region Update Location tracking

            if (asset != null&&employee!=null)
            {
                var location = new LocationTracking
                {
                    AssetRegistrationId = asset.AssetRegistrationId,
                    LocationType = LocationType.Employee,
                    StartDate = DateTime.Today,
                    BranchId = issueItem.FixedAssetIssue.BranchId,
                    EmployeeId= employeeid,
                    OrgStructureId = orgid,
                    TransactionType = FixedAssetTransactionType.Issue,
                    TransactionNo = issueItem.FixedAssetIssueID.ToString(),
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name
                };
                db.LocationTrackings.Add(location);
                db.SaveChanges();
            }

            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
