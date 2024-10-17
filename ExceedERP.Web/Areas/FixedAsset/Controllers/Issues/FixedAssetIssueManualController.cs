using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;
using FixedAssetIssue = ExceedERP.Core.Domain.FixedAsset.Issues.FixedAssetIssue;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Issues
{


    public class FixedAssetIssueManualController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var assetCategories = _db.FixedAssetCategories;
            var assetSubCategories = _db.FixedAssetSubCategories;
            var fixedAsset = _db.AssetRegistrations;
            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var selectedAsset = fixedAsset.Select(
              i => new
              {
                  Name = i.Tag + "-" + i.Name,
                  AssetRegistrationId = i.AssetRegistrationId
              }).AsQueryable();

           

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
              i => new
              {
                  Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                  Code = i.EmployeeId
              }).AsQueryable();

            var assetSubCategoryNames = _db.FixedAssetSubCategoryNames;

            ViewData["personEmployees"] = selectedEmployee;

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["FixedAsset"] = selectedAsset;
            ViewData["SubCategoryName"] = assetSubCategoryNames;

            var fixedAssetPeriods = _db.FixedAssetPeriods;
            var selectedfixedAssetPeriods = fixedAssetPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.FixedAssetPeriodId
                }).AsQueryable();
            ViewData["fixedassetPeriods"] = selectedfixedAssetPeriods;
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetIssues_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<FixedAssetIssue> fixedAssetIssues = _db.FixedAssetIssues.Where(x=>x.IsManual).Where(x => userStores.Contains(x.StoreCode));
            
            DataSourceResult result = fixedAssetIssues.ToDataSourceResult(request, fixedAssetIssue => new
            {
                FixedAssetIssueID = fixedAssetIssue.FixedAssetIssueID,
                IsManual = fixedAssetIssue.IsManual,
                OrgStructureId = fixedAssetIssue.OrgStructureId,
                BranchId = fixedAssetIssue.BranchId,
                BusinessUnitId = fixedAssetIssue.BusinessUnitId,
                EmployeeId = fixedAssetIssue.EmployeeId,
                RecipientId = fixedAssetIssue.RecipientId,
                GLPeriodId = fixedAssetIssue.GLPeriodId,

                StoreCode = fixedAssetIssue.StoreCode,
                FixedAssetRequisitionId = fixedAssetIssue.FixedAssetRequisitionId,
                Reference = fixedAssetIssue.Reference,
                IssueDate = fixedAssetIssue.IssueDate,
                GatePass = fixedAssetIssue.GatePass,


                Status = fixedAssetIssue.Status,
                Remark = fixedAssetIssue.Remark,
                IsVoid = fixedAssetIssue.IsVoid,
                VoidBy = fixedAssetIssue.VoidBy,
                VoidTime = fixedAssetIssue.VoidTime,
                IsOnlineApproved = fixedAssetIssue.IsOnlineApproved,
                OnlineApprovedBy = fixedAssetIssue.OnlineApprovedBy,
                OnlineApprovedTime = fixedAssetIssue.OnlineApprovedTime,
                IsSendForApproval = fixedAssetIssue.IsSendForApproval,
                SendForApprovalBy = fixedAssetIssue.SendForApprovalBy,
                SendForApprovalTime = fixedAssetIssue.SendForApprovalTime,
                IsOnlineTransferred = fixedAssetIssue.IsOnlineTransferred,
                IsOnlineTransferredBy = fixedAssetIssue.IsOnlineTransferredBy,
                OnlineTransferredTime = fixedAssetIssue.OnlineTransferredTime,

                DateCreated = fixedAssetIssue.DateCreated,
                CreatedBy = fixedAssetIssue.CreatedBy,
                LastModified = fixedAssetIssue.LastModified,
                ModifiedBy = fixedAssetIssue.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssues_Create([DataSourceRequest]DataSourceRequest request, FixedAssetIssue issue)
        {
            issue.BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issue.StoreCode);

            if (issue.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requester");
            else
            {
                issue.OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(issue.EmployeeId);
                issue.BusinessUnitId = 0;
            }

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetIssue
                {
                    IsManual = true,
                    OrgStructureId = issue.OrgStructureId,
                    BranchId = issue.BranchId,
                    BusinessUnitId = issue.BusinessUnitId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    GLPeriodId = issue.GLPeriodId,

                    StoreCode = issue.StoreCode,
                    GatePass = issue.GatePass,
                    FixedAssetRequisitionId = 0,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = issue.Remark,


                    IsVoid = issue.IsVoid,
                    VoidBy = issue.VoidBy,
                    VoidTime = issue.VoidTime,
                    IsOnlineApproved = issue.IsOnlineApproved,
                    OnlineApprovedBy = issue.OnlineApprovedBy,
                    OnlineApprovedTime = issue.OnlineApprovedTime,
                    IsOnlineTransferred = issue.IsOnlineTransferred,
                    IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                    OnlineTransferredTime = issue.OnlineTransferredTime,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = issue.LastModified,
                    ModifiedBy = issue.ModifiedBy,
                };

                _db.FixedAssetIssues.Add(entity);
                _db.SaveChanges();
                issue.FixedAssetIssueID = entity.FixedAssetIssueID;
                GetStoreRequisitionItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fixed Asset Issue", "Success", User.Identity.Name, $"created asset issue {entity.FixedAssetIssueID} Tag {entity.StoreCode}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssues_Update([DataSourceRequest]DataSourceRequest request, FixedAssetIssue issue)
        {
            issue.BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issue.StoreCode);

            if (issue.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requester");
            else
            {
                issue.OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(issue.EmployeeId);
                issue.BusinessUnitId =0;
            }

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetIssue
                {
                    FixedAssetIssueID = issue.FixedAssetIssueID,
                    IsManual = issue.IsManual,
                    OrgStructureId = issue.OrgStructureId,
                    BranchId = issue.BranchId,
                    BusinessUnitId = issue.BusinessUnitId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    GLPeriodId = issue.GLPeriodId,

                    StoreCode = issue.StoreCode,
                    GatePass = issue.GatePass,
                    FixedAssetRequisitionId = issue.FixedAssetRequisitionId,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate,
                    Status = issue.Status,
                    Remark = issue.Remark,


                    IsVoid = issue.IsVoid,
                    VoidBy = issue.VoidBy,
                    VoidTime = issue.VoidTime,
                    IsOnlineApproved = issue.IsOnlineApproved,
                    OnlineApprovedBy = issue.OnlineApprovedBy,
                    OnlineApprovedTime = issue.OnlineApprovedTime,
                    IsOnlineTransferred = issue.IsOnlineTransferred,
                    IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                    OnlineTransferredTime = issue.OnlineTransferredTime,

                    DateCreated = issue.DateCreated,
                    CreatedBy = issue.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.FixedAssetIssues.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fixed Asset Issue", "Success", User.Identity.Name, $"updated asset issue {entity.FixedAssetIssueID} Tag {entity.StoreCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssues_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetIssue issue)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetIssue
                {
                    FixedAssetIssueID = issue.FixedAssetIssueID,
                    IsManual = issue.IsManual,
                    OrgStructureId = issue.OrgStructureId,
                    BranchId = issue.BranchId,
                    BusinessUnitId = issue.BusinessUnitId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    GLPeriodId = issue.GLPeriodId,
                    StoreCode = issue.StoreCode,
                    GatePass = issue.GatePass,
                    FixedAssetRequisitionId = issue.FixedAssetRequisitionId,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate,
                    Status = issue.Status,
                    Remark = issue.Remark,
                    IsVoid = issue.IsVoid,
                    VoidBy = issue.VoidBy,
                    VoidTime = issue.VoidTime,
                    IsOnlineApproved = issue.IsOnlineApproved,
                    OnlineApprovedBy = issue.OnlineApprovedBy,
                    OnlineApprovedTime = issue.OnlineApprovedTime,
                    IsOnlineTransferred = issue.IsOnlineTransferred,
                    IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                    OnlineTransferredTime = issue.OnlineTransferredTime,

                    DateCreated = issue.DateCreated,
                    CreatedBy = issue.CreatedBy,
                    LastModified = issue.LastModified,
                    ModifiedBy = issue.ModifiedBy,
                };

                _db.FixedAssetIssues.Attach(entity);
                _db.FixedAssetIssues.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fixed Asset Issue", "Success", User.Identity.Name, $"deleted asset issue {issue.FixedAssetIssueID} Tag {issue.StoreCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.FixedAssetIssues.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FixedAssetIssueItems.Count > 0)
            {
                #region validate item selection

                var assets = _db.FixedAssetIssueItems.Where(x => x.FixedAssetIssueID == id).ToList();
                foreach (var asset in assets)
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
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FixedAssetIssues.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
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

            else if (record.FixedAssetIssueItems.Count == 0)
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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.FixedAssetIssues.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.FixedAssetIssueItems.Count > 0)
            {
               //reverse transaction here


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.FixedAssetIssues.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                //copy
                CopyRecord(id);

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.FixedAssetIssueItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        private void CopyRecord(int id)
        {
            var issue = _db.FixedAssetIssues.Find(id);
            if (issue != null)
            {
                #region header
                var entity = new FixedAssetIssue
                {
                    OrgStructureId = issue.OrgStructureId,
                    BranchId = issue.BranchId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    GLPeriodId = issue.GLPeriodId,

                    StoreCode = issue.StoreCode,
                    GatePass = issue.GatePass,
                    FixedAssetRequisitionId = issue.FixedAssetRequisitionId,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = issue.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = issue.OrgBranchName,

                };

                _db.FixedAssetIssues.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var issueItem in issue.FixedAssetIssueItems)
                {
                    #region lines
                    var line = new FixedAssetIssueItem
                    {
                        FixedAssetIssueID = entity.FixedAssetIssueID,
                        FixedAssetCategoryId = issueItem.FixedAssetCategoryId,
                        FixedAssetSubCategoryId = issueItem.FixedAssetSubCategoryId,
                        AssetRegistrationId = issueItem.AssetRegistrationId,
                        SendToUserCard = issueItem.SendToUserCard,

                        Unit = issueItem.Unit,
                        Name = issueItem.Name,
                        Size = issueItem.Size,
                        GrnNo = issueItem.GrnNo,

                        RequestedQuantity = issueItem.RequestedQuantity,
                        ApprovedQuantity = issueItem.ApprovedQuantity,
                        IssuedQuantity = issueItem.IssuedQuantity,
                        UnitPrice = issueItem.UnitPrice,

                        Total = issueItem.Total,


                        IsMachinery = issueItem.IsMachinery,
                        TagNo = issueItem.TagNo,
                        Model = issueItem.Model,
                        SerialNo = issueItem.SerialNo,
                        Condition = issueItem.Condition,
                        PlateNo = issueItem.PlateNo,
                        ChassisNo = issueItem.ChassisNo,
                        EngineNo = issueItem.EngineNo,
                        OrgBranchName = issueItem.OrgBranchName,
                        DateCreated = issueItem.DateCreated,
                        LastModified = issueItem.LastModified,
                        CreatedBy = issueItem.CreatedBy,
                        ModifiedBy = issueItem.ModifiedBy
                    };

                    _db.FixedAssetIssueItems.Add(line);
                    _db.SaveChanges();

                    #endregion

                }

            }

        }
        /// <summary>
        /// Gets FixedAssetRequisitionItems and populates FixedAssetIssueItem
        /// </summary>
        /// <param name="issue"></param>
        private void GetStoreRequisitionItems(FixedAssetIssue issue)
        {
            var fixedAssetRequisitionItems = _db.FixedAssetRequisitionItems
                .Where(sr => sr.FixedAssetRequisitionId == issue.FixedAssetRequisitionId)
                .ToList();

            var fixedAssetRequisitionItem = fixedAssetRequisitionItems.FirstOrDefault();

            string storeCode = "";

            if (fixedAssetRequisitionItem != null)
            {
                storeCode = fixedAssetRequisitionItem.FixedAssetRequisition.StoreCode;
            }

            foreach (var requestedItem in fixedAssetRequisitionItems)
            {

                #region fixed asset
                for (int i = 0; i < requestedItem.IssuedQuantity; i++)
                {
                    var issueItem = new FixedAssetIssueItem
                    {
                        FixedAssetIssueID = issue.FixedAssetIssueID,
                        FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                        FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                        ItemName=requestedItem.Name,
                        Size=requestedItem.Size,
                        SendToUserCard = true,//added
                        ChassisNo = requestedItem.ChasisNo,
                        Model = requestedItem.ModelNo,
                        SerialNo = requestedItem.SerialNo,
                        RequestedQuantity = 1,
                        ApprovedQuantity = 1,
                        IssuedQuantity = 1,
                        UnitPrice = requestedItem.Price,
                        Total = requestedItem.Price,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today,
                        Unit = requestedItem.UoM //added
                        
                    };

                    _db.FixedAssetIssueItems.Add(issueItem);
                    _db.SaveChanges();
                }
                #endregion

            }


        }
        public FileResult DownloadTemplate()
        {

            string physicalPath = "~/Content/InventoryTemplate/IssueTemplate.xlsx";
            return File(physicalPath, "application/force-download", Path.GetFileName(physicalPath));
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
