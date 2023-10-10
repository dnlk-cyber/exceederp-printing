using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Issues
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]

    public class IssueFuelController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly DistributionHelper _distHelper = new DistributionHelper();
        private InventoryTransferHelper _transferHelper;

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }


        private void PopulateViewData()
        {
            var inventoryPeriods = _db.InventoryPeriods;
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var costCenter = _db.BranchCostCenter;
            var branches = _db.Branch;
            var stores = _db.InventoryStores;
            ViewData["stores"] = stores;

            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var selectedinventoryPeriods = inventoryPeriods.Select(
            i => new
            {
                Name = i.Name,
                GLPeriodId = i.InventoryPeriodId
            }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            ViewData["costCenters"] = selectedcostCenter;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]

        public ActionResult IssueFuels_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<Issue> issuefuels = db.Issues.Where(x => x.IsFuel).Where(x => mybraches.Contains(x.OrgBranchName));

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<Issue> issuefuels = _db.Issues.Where(x => x.IsFuel).Where(x => userStores.Contains(x.StoreCode));
            DataSourceResult result = issuefuels.ToDataSourceResult(request, issueFuel => new
            {
                IsManual = issueFuel.IsManual,
                StoreCode = issueFuel.StoreCode,
                IssueID = issueFuel.IssueID,
                StoreRequisitionID = issueFuel.StoreRequisitionID,
                OrgStructureId = issueFuel.OrgStructureId,
                BranchId = issueFuel.BranchId,
                BusinessUnitId = issueFuel.BusinessUnitId,
                EmployeeId = issueFuel.EmployeeId,
                RecipientId = issueFuel.RecipientId,
                ForBranchId = issueFuel.ForBranchId,
                HasReading = issueFuel.HasReading,
                ReadingKM = issueFuel.ReadingKM,
                Owner = issueFuel.Owner,
                PlateNo = issueFuel.PlateNo,
                SupplierAccountCode = issueFuel.SupplierAccountCode,
                Reference = issueFuel.Reference,
                IssueDate = issueFuel.IssueDate,
                IsFuel = issueFuel.IsFuel,
                Status = issueFuel.Status,
                GLPeriodId = issueFuel.GLPeriodId,
                AssetRegistrationId = issueFuel.AssetRegistrationId,
                Remark = issueFuel.Remark,
                IsVoid = issueFuel.IsVoid,
                VoidBy = issueFuel.VoidBy,
                VoidTime = issueFuel.VoidTime,
                IsOnlineApproved = issueFuel.IsOnlineApproved,
                OnlineApprovedBy = issueFuel.OnlineApprovedBy,
                OnlineApprovedTime = issueFuel.OnlineApprovedTime,
                IsSendForApproval = issueFuel.IsSendForApproval,
                SendForApprovalBy = issueFuel.SendForApprovalBy,
                SendForApprovalTime = issueFuel.SendForApprovalTime,
                IsOnlineTransferred = issueFuel.IsOnlineTransferred,
                IsOnlineTransferredBy = issueFuel.IsOnlineTransferredBy,
                OnlineTransferredTime = issueFuel.OnlineTransferredTime,

                DateCreated = issueFuel.DateCreated,
                CreatedBy = issueFuel.CreatedBy,
                LastModified = issueFuel.LastModified,
                ModifiedBy = issueFuel.ModifiedBy,
            });

            return Json(result);
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuels_Create([DataSourceRequest]DataSourceRequest request, Issue issueFuel)
        {
            if (issueFuel.HasReading && String.IsNullOrEmpty(issueFuel.ReadingKM))
            {
                ModelState.AddModelError("reading", "Please fill Reading KM ");
            }

            if (issueFuel.Owner == IssueFuelVehicleOwner.Internal && String.IsNullOrEmpty(issueFuel.PlateNo))
            {
                ModelState.AddModelError("reading", "Please fill Plate No.");
            }

            if (issueFuel.Owner == IssueFuelVehicleOwner.External && String.IsNullOrEmpty(issueFuel.SupplierAccountCode))
            {
                ModelState.AddModelError("reading", "Please fill Supplier Account Code.");
            }

            var storeRequisition = _db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == issueFuel.StoreRequisitionID);
            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
            {
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
            }
            if (storeRequisition != null)
            {
                issueFuel.StoreCode = storeRequisition.StoreCode;
                issueFuel.OrgStructureId = storeRequisition.OrgStructureId;
                issueFuel.BranchId = storeRequisition.BranchId;
                issueFuel.BusinessUnitId = storeRequisition.BusinessUnitId;
                issueFuel.ForBranchId = storeRequisition.IsForOther ? (int)storeRequisition.ForBranchId : storeRequisition.BranchId;
                issueFuel.PlateNo = storeRequisition.PlateNo;
                issueFuel.EmployeeId = (int)storeRequisition.EmployeeId;
                if (issueFuel.RecipientId == 0)
                    issueFuel.RecipientId = (int)storeRequisition.EmployeeId;
            }

            if (!String.IsNullOrEmpty(issueFuel.Reference))
            {
                var exists = _db.Issues.Where(x => x.Reference == issueFuel.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", issueFuel.Reference + " already exists in the system.");
                }
            }

            if (issueFuel.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(issueFuel.GLPeriodId);
                var openPeriod = _db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == issueFuel.GLPeriodId).FirstOrDefault(x => x.StoreCode == issueFuel.StoreCode);

                if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                    ModelState.AddModelError("open", "The selected period is not open for this store.");


                if (inventoryPeriod != null)
                {
                    if (
                        !((issueFuel.IssueDate.Date >= inventoryPeriod.DateFrom) &&
                          (issueFuel.IssueDate.Date <= inventoryPeriod.DateTo)))
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

            if (issueFuel.IssueDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {
                var entity = new Issue
                {
                    IsManual = false,
                    StoreCode = issueFuel.StoreCode,
                    StoreRequisitionID = issueFuel.StoreRequisitionID,
                    OrgStructureId = issueFuel.OrgStructureId,
                    EmployeeId = issueFuel.EmployeeId,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issueFuel.StoreCode),
                    BusinessUnitId = issueFuel.BusinessUnitId,
                    ForBranchId = issueFuel.ForBranchId,
                    HasReading = issueFuel.HasReading,
                    ReadingKM = issueFuel.ReadingKM,
                    Owner = issueFuel.Owner,
                    PlateNo = issueFuel.PlateNo,
                    SupplierAccountCode = issueFuel.SupplierAccountCode,
                    Reference = issueFuel.Reference,
                    IssueDate = issueFuel.IssueDate.Date,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsFuel = true,
                    Remark = issueFuel.Remark,
                    GLPeriodId = issueFuel.GLPeriodId,
                    AssetRegistrationId = issueFuel.AssetRegistrationId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = issueFuel.LastModified,
                    ModifiedBy = issueFuel.ModifiedBy,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issueFuel.StoreCode).ToString()
                };

                _db.Issues.Add(entity);
                _db.SaveChanges();
                issueFuel.IssueID = entity.IssueID;
                GetStoreRequisitionItems(entity);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel", "Success", User.Identity.Name, $"created issue {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueFuel }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuels_Update([DataSourceRequest]DataSourceRequest request, Issue issueFuel)
        {
            if (issueFuel.HasReading && String.IsNullOrEmpty(issueFuel.ReadingKM))
            {
                ModelState.AddModelError("reading", "Please fill Reading KM ");
            }

            if (issueFuel.Owner == IssueFuelVehicleOwner.Internal && String.IsNullOrEmpty(issueFuel.PlateNo))
            {
                ModelState.AddModelError("reading", "Please fill Plate No.");
            }

            if (issueFuel.Owner == IssueFuelVehicleOwner.External && String.IsNullOrEmpty(issueFuel.SupplierAccountCode))
            {
                ModelState.AddModelError("reading", "Please fill Supplier Account Code.");
            }

            var storeRequisition = _db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == issueFuel.StoreRequisitionID);
            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
            {
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
            }
            if (storeRequisition != null)
            {
                issueFuel.StoreCode = storeRequisition.StoreCode;
                issueFuel.OrgStructureId = storeRequisition.OrgStructureId;
                issueFuel.BranchId = storeRequisition.BranchId;
                issueFuel.BusinessUnitId = storeRequisition.BusinessUnitId;
                issueFuel.ForBranchId = storeRequisition.IsForOther ? (int)storeRequisition.ForBranchId : storeRequisition.BranchId;
                issueFuel.PlateNo = storeRequisition.PlateNo;
                issueFuel.EmployeeId = (int)storeRequisition.EmployeeId;
                if (issueFuel.RecipientId == 0)
                    issueFuel.RecipientId = (int)storeRequisition.EmployeeId;
            }

            if (!String.IsNullOrEmpty(issueFuel.Reference))
            {
                var exists = _db.Issues.Where(x => x.IssueID != issueFuel.IssueID).Where(x => x.Reference == issueFuel.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", issueFuel.Reference + "  already exists in the system.");
                }
            }

            if (issueFuel.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(issueFuel.GLPeriodId);

                var openPeriod = _db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == issueFuel.GLPeriodId).FirstOrDefault(x => x.StoreCode == issueFuel.StoreCode);

                if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                    ModelState.AddModelError("open", "The selected period is not open for this store.");

                if (inventoryPeriod != null)
                {
                    if (
                        !((issueFuel.IssueDate.Date >= inventoryPeriod.DateFrom) &&
                          (issueFuel.IssueDate.Date <= inventoryPeriod.DateTo)))
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
            if (issueFuel.IssueDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {
                var entity = new Issue
                {
                    IsManual = issueFuel.IsManual,
                    StoreCode = issueFuel.StoreCode,
                    IssueID = issueFuel.IssueID,
                    StoreRequisitionID = issueFuel.StoreRequisitionID,
                    OrgStructureId = issueFuel.OrgStructureId,
                    EmployeeId = issueFuel.EmployeeId,
                    RecipientId = issueFuel.RecipientId,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issueFuel.StoreCode),
                    BusinessUnitId = issueFuel.BusinessUnitId,
                    ForBranchId = issueFuel.ForBranchId,
                    HasReading = issueFuel.HasReading,
                    ReadingKM = issueFuel.ReadingKM,
                    Owner = issueFuel.Owner,
                    PlateNo = issueFuel.PlateNo,
                    SupplierAccountCode = issueFuel.SupplierAccountCode,
                    Reference = issueFuel.Reference,
                    IssueDate = issueFuel.IssueDate,
                    IsFuel = issueFuel.IsFuel,
                    Status = issueFuel.Status,
                    Remark = issueFuel.Remark,
                    GLPeriodId = issueFuel.GLPeriodId,
                    AssetRegistrationId = issueFuel.AssetRegistrationId,
                    IsVoid = issueFuel.IsVoid,
                    VoidBy = issueFuel.VoidBy,
                    VoidTime = issueFuel.VoidTime,
                    IsOnlineApproved = issueFuel.IsOnlineApproved,
                    OnlineApprovedBy = issueFuel.OnlineApprovedBy,
                    OnlineApprovedTime = issueFuel.OnlineApprovedTime,
                    IsOnlineTransferred = issueFuel.IsOnlineTransferred,
                    IsOnlineTransferredBy = issueFuel.IsOnlineTransferredBy,
                    OnlineTransferredTime = issueFuel.OnlineTransferredTime,

                    DateCreated = issueFuel.DateCreated,
                    CreatedBy = issueFuel.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(issueFuel.StoreCode).ToString()
                };

                _db.Issues.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel", "Success", User.Identity.Name, $"updated issue {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueFuel }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuels_Destroy([DataSourceRequest]DataSourceRequest request, Issue issueFuel)
        {
            if (ModelState.IsValid)
            {
                var entity = new Issue
                {
                    StoreCode = issueFuel.StoreCode,
                    IssueID = issueFuel.IssueID,
                    StoreRequisitionID = issueFuel.StoreRequisitionID,
                    OrgStructureId = issueFuel.OrgStructureId,
                    BranchId = issueFuel.BranchId,
                    BusinessUnitId = issueFuel.BusinessUnitId,
                    EmployeeId = issueFuel.EmployeeId,
                    RecipientId = issueFuel.RecipientId,
                    ForBranchId = issueFuel.ForBranchId,
                    HasReading = issueFuel.HasReading,
                    ReadingKM = issueFuel.ReadingKM,
                    Owner = issueFuel.Owner,
                    PlateNo = issueFuel.PlateNo,
                    SupplierAccountCode = issueFuel.SupplierAccountCode,
                    Reference = issueFuel.Reference,
                    IssueDate = issueFuel.IssueDate,
                    AssetRegistrationId = issueFuel.AssetRegistrationId,
                    Status = issueFuel.Status,
                    Remark = issueFuel.Remark,
                    IsVoid = issueFuel.IsVoid,
                    VoidBy = issueFuel.VoidBy,
                    VoidTime = issueFuel.VoidTime,
                    IsOnlineApproved = issueFuel.IsOnlineApproved,
                    OnlineApprovedBy = issueFuel.OnlineApprovedBy,
                    OnlineApprovedTime = issueFuel.OnlineApprovedTime,
                    IsOnlineTransferred = issueFuel.IsOnlineTransferred,
                    IsOnlineTransferredBy = issueFuel.IsOnlineTransferredBy,
                    OnlineTransferredTime = issueFuel.OnlineTransferredTime,

                    DateCreated = issueFuel.DateCreated,
                    CreatedBy = issueFuel.CreatedBy,
                    LastModified = issueFuel.LastModified,
                    ModifiedBy = issueFuel.ModifiedBy,
                };

                _db.Issues.Attach(entity);
                _db.Issues.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted fuel issue number " + issueFuel.IssueID + " " + issueFuel.Reference);
                #endregion
            }

            return Json(new[] { issueFuel }.ToDataSourceResult(request, ModelState));
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
                    var pricesetting = _db.AveragePriceSettings.FirstOrDefault();
                    if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                    {
                        avgPrice = inventoryItem.AvgPrice;
                    }
                    else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                    {
                        int storeBranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeCode);
                        var branchPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetBranchItemAveragePrice(storeBranch, inventoryItem.ItemCode);
                        avgPrice = branchPrice;
                    }
                    else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                    {
                        var storeItemAssignment = _db.StoreItemAssignments.Where(x => x.StoreCode == storeCode).FirstOrDefault(x => x.ItemCode == inventoryItem.ItemCode);
                        if (storeItemAssignment != null)
                        {
                            var storePrice = storeItemAssignment.AvgPrice;
                            avgPrice = storePrice;
                        }
                    }
                    #endregion
                    IssueItem issueItem = new IssueItem
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
                        DateCreated = DateTime.Today,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today
                    };
                    _db.IssueItems.Add(issueItem);
                    _db.SaveChanges();

                    #endregion
                }

            }
            _distHelper.UpdateIssueDistribution(issue);
        }


        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.Issues.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                _transferHelper = new InventoryTransferHelper();
                string response = _transferHelper.IssueFuel(myid, description, User?.Identity?.Name ?? "");
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
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel", "Success", User.Identity.Name, $"send for approval issue {id} ", Modules.Inventory);
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
                    if (stockItem != null)
                    {
                        int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);
                        #region stock
                        BranchItemAssignment branchItem = _db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId).FirstOrDefault(x => x.ItemCode == i.ItemCode);

                        if (branchItem != null)
                        {
                            StoreItemAssignment storeItem = _db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == storecode && x.ItemCode == itemCode);

                            if (storeItem != null)
                            {
                                storeItem.Balance = storeItem.Balance + i.IssuedQuantity;
                                branchItem.Balance = branchItem.Balance + i.IssuedQuantity;
                                stockItem.Balance = stockItem.Balance + i.IssuedQuantity;

                                _db.Entry(storeItem).State = EntityState.Modified;
                               // _db.SaveChanges();

                                _db.Entry(branchItem).State = EntityState.Modified;
                               // _db.SaveChanges();

                                _db.Entry(stockItem).State = EntityState.Modified;
                                //_db.SaveChanges();

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


                        }

                        #endregion
                    }

                }
                _db.SaveChanges();

                #endregion

                CopyRecord(id);


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel", "Success", User.Identity.Name, $"void issue {id} ", Modules.Inventory);
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
                    StoreCode = record.StoreCode,
                    StoreRequisitionID = record.StoreRequisitionID,
                    OrgStructureId = record.OrgStructureId,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                    HasReading = record.HasReading,
                    ReadingKM = record.ReadingKM,
                    Owner = record.Owner,
                    PlateNo = record.PlateNo,
                    SupplierAccountCode = record.SupplierAccountCode,
                    Reference = record.Reference,
                    IssueDate = DateTime.Now,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsFuel = true,
                    Remark = record.Remark,
                    GLPeriodId = record.GLPeriodId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = record.LastModified,
                    ModifiedBy = record.ModifiedBy,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode).ToString()
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
                        AdditionalCost = i.AdditionalCost,
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
        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
