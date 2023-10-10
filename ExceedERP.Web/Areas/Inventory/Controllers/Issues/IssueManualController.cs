using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Issues
{


    public class IssueManualController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly DistributionHelper _distHelper = new DistributionHelper();
        private InventoryTransferHelper _transferHelper=new InventoryTransferHelper();

       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var stores = _db.InventoryStores;

            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();
            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["costCenters"] = selectedcostCenter;

            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["stores"] = stores;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var workitem = _db.ProjectEstimationWorkItem.Where(w => !string.IsNullOrEmpty(w.ItemCode));
            var selectedWorkitem = workitem.Select(
                i => new
                {
                    Name = i.ItemCode + "-" + i.ItemName,
                    Code = i.ItemCode
                }).AsQueryable();
            ViewData["WorkItem"] = selectedWorkitem;
        }

       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Issues_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<Issue> issues = db.Issues.Where(x => x.IsFuel == false).Where(x => mybraches.Contains(x.OrgBranchName));
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<Issue> issues = _db.Issues.Where(x => x.IsFuel == false&&x.IsManual).Where(x => userStores.Contains(x.StoreCode));
            DataSourceResult result = issues.ToDataSourceResult(request, issue => new
            {
                IssueID = issue.IssueID,
                IsManual = issue.IsManual,
                OrgStructureId = issue.OrgStructureId,
                BranchId = issue.BranchId,
                BusinessUnitId = issue.BusinessUnitId,
                ForBranchId = issue.ForBranchId,
                EmployeeId = issue.EmployeeId,
                RecipientId = issue.RecipientId,
                StoreCode = issue.StoreCode,
                StoreRequisitionID = issue.StoreRequisitionID,
                Reference = issue.Reference,
                IssueDate = issue.IssueDate,
                PlateNo = issue.PlateNo,
                Owner = issue.Owner,
                Status = issue.Status,
                Remark = issue.Remark,
                GLPeriodId = issue.GLPeriodId,
                IsVoid = issue.IsVoid,
                VoidBy = issue.VoidBy,
                VoidTime = issue.VoidTime,
                IsOnlineApproved = issue.IsOnlineApproved,
                OnlineApprovedBy = issue.OnlineApprovedBy,
                OnlineApprovedTime = issue.OnlineApprovedTime,
                IsSendForApproval = issue.IsSendForApproval,
                SendForApprovalBy = issue.SendForApprovalBy,
                SendForApprovalTime = issue.SendForApprovalTime,
                IsOnlineTransferred = issue.IsOnlineTransferred,
                IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                OnlineTransferredTime = issue.OnlineTransferredTime,

                DateCreated = issue.DateCreated,
                CreatedBy = issue.CreatedBy,
                LastModified = issue.LastModified,
                ModifiedBy = issue.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Issues_Create([DataSourceRequest]DataSourceRequest request, Issue issue)
        {

            //if (issue.OrgStructureId == 0)
            //    ModelState.AddModelError("workunit","Please choose work unit");

            issue.BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issue.StoreCode);

            if (issue.EmployeeId == 0)
               ModelState.AddModelError("requester","Please choose requester");
            else
            issue.OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(issue.EmployeeId);

            if (!String.IsNullOrEmpty(issue.Reference))
            {
                var exists = _db.Issues.Where(x => x.Reference == issue.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", issue.Reference + " already exists in the system.");
                }
            }
            if (issue.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(issue.GLPeriodId);
                var openPeriod = _db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == issue.GLPeriodId).FirstOrDefault(x => x.StoreCode == issue.StoreCode);

                if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                    ModelState.AddModelError("open", "The selected period is not open for this store.");

                if (inventoryPeriod != null)
                {
                    if (
                        !((issue.IssueDate.Date >= inventoryPeriod.DateFrom) &&
                          (issue.IssueDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Issue Date should be within the period: " + inventoryPeriod.Name + "(From " +
                            inventoryPeriod.DateFrom.ToShortDateString() + " To " +
                            inventoryPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }

            if (issue.IssueDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {
                var entity = new Issue
                {
                    IsManual = true,
                    OrgStructureId = issue.OrgStructureId,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issue.StoreCode),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                    ForBranchId = issue.ForBranchId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    StoreCode = issue.StoreCode,
                    StoreRequisitionID = issue.StoreRequisitionID,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate.Date,
                    PlateNo = issue.PlateNo,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = issue.Remark,
                    GLPeriodId = issue.GLPeriodId,
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
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issue.StoreCode).ToString()
                };

                _db.Issues.Add(entity);
                _db.SaveChanges();
                issue.IssueID = entity.IssueID;
                GetStoreRequisitionItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "manual issue", "Success", User.Identity.Name, $"created issue {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Issues_Update([DataSourceRequest]DataSourceRequest request, Issue issue)
        {
            //if (issue.OrgStructureId == 0)
            //    ModelState.AddModelError("workunit", "Please choose work unit");

            issue.BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issue.StoreCode);

            if (issue.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requester");
            else
                issue.OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(issue.EmployeeId);

            if (!String.IsNullOrEmpty(issue.Reference))
            {
                var exists = _db.Issues.Where(x => x.IssueID != issue.IssueID).Where(x => x.Reference == issue.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", issue.Reference + "  already exists in the system.");
            }

            if (issue.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(issue.GLPeriodId);
                var openPeriod = _db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == issue.GLPeriodId).FirstOrDefault(x => x.StoreCode == issue.StoreCode);

                if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                    ModelState.AddModelError("open", "The selected period is not open for this store.");

                if (inventoryPeriod != null)
                {
                    if (
                        !((issue.IssueDate.Date >= inventoryPeriod.DateFrom) &&
                          (issue.IssueDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Issue Date should be within the period: " + inventoryPeriod.Name + "(From " +
                            inventoryPeriod.DateFrom.ToShortDateString() + " To " +
                            inventoryPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }
            if (issue.IssueDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {
                var entity = new Issue
                {
                    IssueID = issue.IssueID,
                    IsManual = issue.IsManual,
                    OrgStructureId = issue.OrgStructureId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    BranchId = issue.BranchId,
                    BusinessUnitId = issue.BusinessUnitId,
                    ForBranchId = issue.ForBranchId,
                    StoreCode = issue.StoreCode,
                    StoreRequisitionID = issue.StoreRequisitionID,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate,
                    PlateNo = issue.PlateNo,
                    Status = issue.Status,
                    Remark = issue.Remark,
                    GLPeriodId = issue.GLPeriodId,
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
                    OrgBranchName = issue.BranchId.ToString()
                };

                _db.Issues.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "manual issue", "Success", User.Identity.Name, $"updated issue {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Issues_Destroy([DataSourceRequest]DataSourceRequest request, Issue issue)
        {
            if (ModelState.IsValid)
            {
                var entity = new Issue
                {
                    IsManual = issue.IsManual,
                    OrgStructureId = issue.OrgStructureId,
                    EmployeeId = issue.EmployeeId,
                    RecipientId = issue.RecipientId,
                    BranchId = issue.BranchId,
                    BusinessUnitId = issue.BusinessUnitId,
                    ForBranchId = issue.ForBranchId,
                    IssueID = issue.IssueID,
                    StoreCode = issue.StoreCode,
                    StoreRequisitionID = 0,
                    Reference = issue.Reference,
                    IssueDate = issue.IssueDate,
                    PlateNo = issue.PlateNo,
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

                _db.Issues.Attach(entity);
                _db.Issues.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted issue number " + issue.IssueID + " " + issue.Reference);
                #endregion
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }
        public FileResult DownloadTemplate()
        {

            string physicalPath = "~/Content/InventoryTemplate/IssueTemplate.xlsx";
            return File(physicalPath, "application/force-download", Path.GetFileName(physicalPath));
        }



        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.Issues.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                _transferHelper = new InventoryTransferHelper();
                string response = _transferHelper.Issue(myid, description, User?.Identity?.Name ?? "");
                if (response != "SUCCESS")
                {
                    var formattedData = new
                    {
                        Message = response,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Successfuly Transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


            }
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record cannot be transfered before validation.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult GenerateDistribution(int id)
        {

            var record = _db.Issues.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
               _distHelper.UpdateIssueDistribution(record);
                var formattedData1 = new
                {
                    Message = "Complete.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            var formattedData = new
            {
                Message = "Record not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.Issues.Find(id);
            var items = _db.IssueItems.Where(x => x.IssueID == id).ToList();
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && items.Any())
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.Issues.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue manual", "Success", User.Identity.Name, $"send for approval issue manual {id} ", Modules.Inventory);
                #endregion

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

            else if (!items.Any())
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

        [AuthorizeRoles(InventoryConstants.PropertyVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.Issues.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.IssueItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.Issues.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                //_db.SaveChanges();
                #region update stock based on the issue

                string storecode = record.StoreCode;

                foreach (IssueItem i in record.IssueItems)
                {
                    string itemCode = i.ItemCode;

                    InventoryItem stockItem = _db.InventoryItems.Find(itemCode);
                    int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);
                    #region stock
                    var branchItem = _db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId).FirstOrDefault(x => x.ItemCode == i.ItemCode);
                    var storeItem = _db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == storecode && x.ItemCode == itemCode);

                    if (storeItem != null)
                    {
                        storeItem.Balance = storeItem.Balance + i.IssuedQuantity;
                        _db.Entry(storeItem).State = EntityState.Modified;
                       // _db.SaveChanges();
                        #region update bin card

                        #region bin card line

                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeItem,
                            ReferenceDoc = "Void Issue " + record.IssueID,
                            Date = record.IssueDate,
                            ReceivedQTY = i.IssuedQuantity,
                            ReceivedUPrice = i.UnitPrice,
                            ReceivedTPrice = i.Total,
                            BalanceQTY = storeItem.Balance,
                            BalanceUPrice = i.UnitPrice,
                            BalanceTPrice = i.UnitPrice * storeItem.Balance,
                            //new    
                            StoreCode = record.StoreCode,
                            GLPeriodId = record.GLPeriodId,
                            TransactionType = BinCardTransactionTypes.Issue,
                            StatusType = BinCardTransactionStatus.Void,
                            TransactionOrder = BinCardTransactionOrder.VoidIssue
                        };
                        _db.BinCardLines.Add(binCardLine);
                        //_db.SaveChanges();

                        #endregion

                        #endregion

                    }
                    if (branchItem != null)
                    {
                        branchItem.Balance = branchItem.Balance + i.IssuedQuantity;
                        _db.Entry(branchItem).State = EntityState.Modified;
                        //_db.SaveChanges();

                    }
                    if (stockItem != null)
                    {
                        stockItem.Balance = stockItem.Balance + i.IssuedQuantity;
                        _db.Entry(stockItem).State = EntityState.Modified;
                        //_db.SaveChanges();
                    }



                    #endregion

                }

                _db.SaveChanges();
                #endregion

                //copy
                CopyRecord(id);


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue manual", "Success", User.Identity.Name, $"void issue {id} ", Modules.Inventory);
                #endregion

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

            if (record.IssueItems.Count == 0)
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
            var record = _db.Issues.Find(id);
            if (record != null)
            {
                #region header
                var entity = new Issue
                {
                    OrgStructureId = record.OrgStructureId,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                    StoreCode = record.StoreCode,
                    StoreRequisitionID = record.StoreRequisitionID,
                    Reference = record.Reference,
                    IssueDate = record.IssueDate,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = record.Remark,
                    GLPeriodId = record.GLPeriodId,
                    IsManual = true,
                    EmployeeId = record.EmployeeId,
                    RecipientId = record.RecipientId,
                    ForBranchId = record.ForBranchId,
                    PlateNo = record.PlateNo,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    OrgBranchName = record.OrgBranchName
                };

                _db.Issues.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var i in record.IssueItems)
                {
                    #region lines
                    var line = new IssueItem
                    {
                        IssueID = entity.IssueID,
                        ItemCategoryCode = i.ItemCategoryCode,
                        ItemCode = i.ItemCode,
                        RequestedQuantity = i.RequestedQuantity,
                        ApprovedQuantity = i.ApprovedQuantity,
                        IssuedQuantity = i.IssuedQuantity,
                        UnitPrice = i.UnitPrice,
                        Total = i.Total,
                        OrgBranchName = i.OrgBranchName,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                    };

                    _db.IssueItems.Add(line);
                    _db.SaveChanges();

                    #endregion

                }

            }

        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public ActionResult Issue_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            #region Import

            DataSet ds = new DataSet();
            Core.Domain.Inventory.Issue issuee = _db.Issues.FirstOrDefault();
            List<Issue> issues = new List<Issue>();
            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/") + DateTime.Now.Year.ToString() +
                                          DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                          DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                          DateTime.Now.Second.ToString() + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                   ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                }

            }
            List<IssueImport> issueImports = new List<IssueImport>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    IssueImport import = new IssueImport
                    {
                        RequistionNo = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        IssueNo = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        BranchId = int.Parse(ds.Tables[0].Rows[j].ItemArray[2].ToString()),
                        OrgStructureId = int.Parse(ds.Tables[0].Rows[j].ItemArray[3].ToString()),
                        IssueDate = DateTime.Parse(ds.Tables[0].Rows[j].ItemArray[4].ToString()),
                        DocumentRef = ds.Tables[0].Rows[j].ItemArray[5].ToString(),

                        ItemCategoryCode = ds.Tables[0].Rows[j].ItemArray[6].ToString(),
                        ItemCode = ds.Tables[0].Rows[j].ItemArray[7].ToString(),
                        RequestedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[8].ToString()),
                        ApprovedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[9].ToString()),
                        IssuedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[10].ToString())
                    };

                    issueImports.Add(import);
                }
            }
            var imports = issueImports.GroupBy(x => x.IssueNo).Select(x => x.FirstOrDefault());
            foreach (var issueImport in issueImports)
            {
                var inventoryItem = _db.InventoryItems.Find(issueImport.ItemCode);
                if (inventoryItem == null)
                {
                    ModelState.AddModelError("unknown", "Unknown Item " + issueImport.ItemCode);
                    ViewBag.Message = "Unknown Item " + issueImport.ItemCode;
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var issueImport in imports)
                {

                    int reqNo = int.Parse(issueImport.RequistionNo);
                    StoreRequisition storeRequisition = _db.StoreRequisitions.Find(reqNo);
                    Issue issue = new Issue
                    {
                        BranchId = issueImport.BranchId,
                        OrgStructureId = issueImport.OrgStructureId,
                        StoreCode = storeRequisition.StoreCode,
                        IssueDate = issueImport.IssueDate,
                        StoreRequisitionID = reqNo,
                        Reference = issueImport.DocumentRef,
                        Status = Enum.GetName(typeof(OperationStatuses), 5)
                    };

                    List<IssueItem> issueItems = new List<IssueItem>();
                    foreach (var importItem in issueImports.Where(x => x.IssueNo == issueImport.IssueNo))
                    {
                        InventoryItem inventoryItem = _db.InventoryItems.Find(importItem.ItemCode);
                        StoreRequisitionItem requestedItem = storeRequisition.StoreRequisitionItem.FirstOrDefault(x => x.ItemCode == importItem.ItemCode);

                        IssueItem issueItem;
                        if (inventoryItem.Type == ItemTypes.FixedAsset)
                        {

                        }
                        else if (inventoryItem.Type == ItemTypes.StockCommonItem)
                        {
                            issueItem = new IssueItem
                            {
                                UnitPrice = inventoryItem.AvgPrice,
                                IssueID = issue.IssueID,
                                ItemCode = requestedItem.ItemCode,
                                IssuedQuantity = requestedItem.IssuedQuantity,
                                Total = inventoryItem.AvgPrice * requestedItem.RequestedQuantity
                            };

                            issueItems.Add(issueItem);

                        }

                    }
                    issue.IssueItems = issueItems;
                    issue.CreatedBy = User.Identity.Name;
                    issue.DateCreated = DateTime.Now;
                    _db.Issues.Add(issue);
                    _db.SaveChanges();

            #endregion

                    #region Distribution

                    IssueDistribution distribution;
                    Dictionary<string, string> checkAccount = new Dictionary<string, string>();
                    List<IssueDistribution> distributions = new List<IssueDistribution>();
                    #region debit

                    foreach (var line in issue.IssueItems)
                    {
                        string orgstructure = "";
                        if (issue.StoreRequisitionID != 0)
                        {
                            var sr = _db.StoreRequisitions.Find(issue.StoreRequisitionID);
                            orgstructure = sr.OrgStructureId.ToString();
                        }

                        var item = _db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                        ItemCategory itemCategory = _db.ItemCategorys.Find(item.ItemCategoryCode);

                        string acc = itemCategory.AccountCode;
                        string[] account = acc.Split('-');
                        string newacc = account[0] + "-" + account[1] + "-" + orgstructure + "-" + account[3];


                        if (line.Total != 0 && checkAccount.ContainsKey(acc))
                        {
                            IssueDistribution iDistribution = distributions.FirstOrDefault(x => x.AccountCode == newacc);

                            iDistribution.AccountCode = newacc;
                            iDistribution.Debit += line.Total;
                            iDistribution.Credit = 0;
                            iDistribution.IssueID = line.IssueID;
                            iDistribution.IssueItemIDTrackBack = line.IssueItemID;

                        }
                        else if (line.Total != 0 && !checkAccount.ContainsKey(acc))
                        {
                            distribution = new IssueDistribution();
                            distribution.AccountCode = newacc;
                            distribution.Debit = line.Total;
                            distribution.Credit = 0;
                            distribution.IssueID = line.IssueID;
                            distribution.IssueItemIDTrackBack = line.IssueItemID;
                            distributions.Add(distribution);
                            checkAccount.Add(acc, line.ItemCode);
                        }
                    }
                    foreach (IssueDistribution issueDistribution in distributions)
                    {
                        _db.IssueDistributions.Add(issueDistribution);
                        _db.SaveChanges();
                    }
                    distributions.Clear();
                    checkAccount.Clear();
                    #endregion

                    #region Credit

                    if (issue.IssueItems.Sum(x => x.Total) != 0)
                    {
                        foreach (IssueItem items in issue.IssueItems)
                        {
                            InventoryItem inventory = _db.InventoryItems.Find(items.ItemCode);
                            string accCode = inventory.ItemCategory.IssueAccountCode;

                            if (items.Total != 0 && checkAccount.ContainsKey(accCode))
                            {
                                IssueDistribution iDistribution = distributions.FirstOrDefault(x => x.AccountCode == accCode);

                                iDistribution.AccountCode = accCode;
                                iDistribution.Debit = 0;
                                iDistribution.Credit += items.Total;
                                iDistribution.IssueID = items.IssueID;
                                iDistribution.IssueItemIDTrackBack = items.IssueItemID;

                            }
                            else if (items.Total != 0 && !checkAccount.ContainsKey(accCode))
                            {
                                distribution = new IssueDistribution();
                                distribution.AccountCode = accCode;
                                distribution.Debit = 0;
                                distribution.Credit = items.Total;
                                distribution.IssueID = items.IssueID;
                                distribution.IssueItemIDTrackBack = items.IssueItemID;
                                distributions.Add(distribution);
                                checkAccount.Add(accCode, items.ItemCode);
                            }

                        }
                        foreach (var issueDistribution in distributions)
                        {
                            _db.IssueDistributions.Add(issueDistribution);
                            _db.SaveChanges();
                        }
                        distributions.Clear();
                        checkAccount.Clear();

                    }
                    issues.Add(issue);
                    #endregion

                    #endregion

                }

            }
            return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));

        }

        private void GetStoreRequisitionItems(Issue issue)
        {
            var storeRequisitionItem = _db.StoreRequisitionItems.Where(sr => sr.StoreRequisitionID == issue.StoreRequisitionID).ToList();
            var storerequisition = storeRequisitionItem.FirstOrDefault();
            string storeCode = "";
            if (storerequisition != null)
            {
                storeCode = storerequisition.StoreRequisition.StoreCode;
            }
            foreach (var requestedItem in storeRequisitionItem)
            {
                var inventoryItem = _db.InventoryItems.Find(requestedItem.ItemCode);
                if (inventoryItem != null)
                {
                    #region stock

                    #region get average price

                    decimal avgPrice = 0;
                    avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(requestedItem.ItemCode, storeCode);
                    #endregion
                    var issueItem = new IssueItem
                    {

                        IssueID = issue.IssueID,
                        ItemCategoryCode = requestedItem.ItemCategoryCode,
                        ItemCode = requestedItem.ItemCode,
                        RequestedQuantity = requestedItem.RequestedQuantity,
                        ApprovedQuantity = requestedItem.IssuedQuantity,
                        IssuedQuantity = requestedItem.IssuedQuantity,
                        UnitPrice = avgPrice,
                        Total = avgPrice * requestedItem.IssuedQuantity,


                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };
                    _db.IssueItems.Add(issueItem);
                    _db.SaveChanges();

                    #endregion
                }

            }
            _distHelper.UpdateIssueDistribution(issue);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
