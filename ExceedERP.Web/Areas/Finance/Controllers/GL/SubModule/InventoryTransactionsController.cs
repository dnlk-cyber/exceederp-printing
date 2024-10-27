using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL.SubModule
{
    [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
    public class InventoryTransactionsController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private InventoryTransferHelper _transferHelper;
        private string _response = "SUCCESS";
        
        public ActionResult GoodReceive()
        {
             var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);

            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var branches = _db.Branch;
            var tax = _db.GLTaxRates;
            var stores = _db.InventoryStores;
            var receivetype = _db.ReceiveTypes;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            ViewData["stores"] = stores;
            ViewData["tax"] = tax;
            ViewData["ReeceiveType"] = receivetype;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            return View();
        }
        public ActionResult GoodReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            
            IQueryable<GoodReceive> goodreceives = _db.GoodReceives
                .Where(x => x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if(myBusinessUnit!=0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    goodreceives = goodreceives.Where(x => storesinBusinessUnit.Contains(x.StoreCode));
                }
            }
            DataSourceResult result = goodreceives.ToDataSourceResult(request, goodReceive => new
            {
                GLPeriodId = goodReceive.GLPeriodId,
                ReceiveRef = goodReceive.ReceiveRef,
                BranchId = goodReceive.BranchId,
                ReceiveTypeId = goodReceive.ReceiveTypeId,
                GoodReceiveId = goodReceive.GoodReceiveId,
                TenderLot = goodReceive.TenderLot,
                StoreCode = goodReceive.StoreCode,
                SupplierName = goodReceive.SupplierName,
                SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                Reference = goodReceive.Reference,
                ReceiveDate = goodReceive.ReceiveDate,
                DateCreated = goodReceive.DateCreated,
                CreatedBy = goodReceive.CreatedBy,
                Status = goodReceive.Status,
                Remark = goodReceive.Remark,
                IsVoid = goodReceive.IsVoid,
                VoidBy = goodReceive.VoidBy,
                VoidTime = goodReceive.VoidTime,
                IsOnlineApproved = goodReceive.IsOnlineApproved,
                OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                OnlineApprovedTime = goodReceive.OnlineApprovedTime,

                IsSendForApproval = goodReceive.IsSendForApproval,
                SendForApprovalBy = goodReceive.SendForApprovalBy,
                SendForApprovalTime = goodReceive.SendForApprovalTime,

                IsOnlineTransferred = goodReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = goodReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = goodReceive.OnlineTransferredTime,
            });

            return Json(result);

        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        public JsonResult BatchtransferGrn(string id)
        {
            int did = int.Parse(id);
            var period = _db.GLPeriods.Find(did);

            var untransffered = _db.GoodReceives.Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.StoreCode)).ToList();
                }
            }

            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.GoodReceive(record.GoodReceiveId,"Batch Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Grn transfer ID:"+record.GoodReceiveId+_response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory receive", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Issue()
        {
             var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);
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
            ViewData["stores"] = stores;
            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var suppliers = _db.ProcurementSuppliers;
            var selectedSuppliers = suppliers.Select(
                i => new
                {
                    Name = i.SupplierId + "-" + i.Name,
                    Code = i.SupplierId
                }).AsQueryable();
            ViewData["Suppliers"] = selectedSuppliers;

            return View();
        }
        public ActionResult Issues_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
                
            IQueryable<Issue> issues = _db.Issues
                .Where(x => x.IsFuel == false && x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    issues = issues.Where(x => storesinBusinessUnit.Contains(x.StoreCode));
                }
            }
            DataSourceResult result = issues.ToDataSourceResult(request, issue => new
            {
                GLPeriodId = issue.GLPeriodId,
                IssueID = issue.IssueID,
                OrgStructureId = issue.OrgStructureId,
                BranchId = issue.BranchId,
                StoreCode = issue.StoreCode,
                StoreRequisitionID = issue.StoreRequisitionID,
                Reference = issue.Reference,
                IssueDate = issue.IssueDate,
                SupplierId = issue.SupplierId,
                IsLoan = issue.IsLoan,
                Status = issue.Status,
                Remark = issue.Remark,
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
        public JsonResult BatchtransferIssue(string id)
        {
            int did = int.Parse(id);
            var period = _db.GLPeriods.Find(did);

            var untransffered = _db.Issues.Where(x=>x.IsFuel==false).Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.StoreCode)).ToList();
                }
            }
            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.Issue(record.IssueID, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;//Logger.Debug("Issue transfer ID:" + record.IssueID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory issue", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IssueFuel()
        {
             var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var costCenter = _db.BranchCostCenter;
            var branches = _db.Branch;
            var stores = _db.InventoryStores;
            ViewData["stores"] = stores;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            var suppliers = _db.ProcurementSuppliers;
            var selectedSuppliers = suppliers.Select(
                i => new
                {
                    Name = i.SupplierId + "-" + i.Name,
                    Code = i.SupplierId
                }).AsQueryable();
            ViewData["Suppliers"] = selectedSuppliers;

            return View();
        }
        public ActionResult IssueFuels_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<Issue> issues = _db.Issues
                .Where(x => x.IsFuel && x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    issues = issues.Where(x => storesinBusinessUnit.Contains(x.StoreCode));
                }
            }
            DataSourceResult result = issues.ToDataSourceResult(request, issueFuel => new
            {
                GLPeriodId = issueFuel.GLPeriodId,
                StoreCode = issueFuel.StoreCode,
                IssueID = issueFuel.IssueID,
                StoreRequisitionID = issueFuel.StoreRequisitionID,
                OrgStructureId = issueFuel.OrgStructureId,
                BranchId = issueFuel.BranchId,
                HasReading = issueFuel.HasReading,
                ReadingKM = issueFuel.ReadingKM,
                Owner = issueFuel.Owner,
                PlateNo = issueFuel.PlateNo,
                SupplierAccountCode = issueFuel.SupplierAccountCode,
                Reference = issueFuel.Reference,
                IssueDate = issueFuel.IssueDate,
                SupplierId = issueFuel.SupplierId,
                IsLoan = issueFuel.IsLoan,
                Status = issueFuel.Status,
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

        public JsonResult BatchtransferIssueFuel(string id)
        {
            int did = int.Parse(id);
            var period = _db.GLPeriods.Find(did);

            var untransffered = _db.Issues.Where(x => x.IsFuel).Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.StoreCode)).ToList();
                }
            }
            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.IssueFuel(record.IssueID, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Issue fuel transfer ID:" + record.IssueID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory issue fuel", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemTransfer()
        {
             var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();


            var stores = _db.InventoryStores;
            var suppliers = _db.ProcurementSuppliers;
            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(i => new
            {
                Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                Code = i.EmployeeId
            }).AsQueryable();

            ViewData["suppliers"] = suppliers;
            ViewData["stores"] = stores;
            ViewData["defaultstore"] = stores.First();
            ViewData["personEmployees"] = selectedEmployee;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            return View();
        }

        public ActionResult ItemTransfer_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<ItemTransfer> itemtransfersends = _db.ItemTransfers
                .Where(st => st.IsOnlineApproved && !st.IsVoid && !st.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    itemtransfersends = itemtransfersends.Where(x => storesinBusinessUnit.Contains(x.FromStore));
                }
            }
            DataSourceResult result = itemtransfersends.ToDataSourceResult(request, itemTransferReceive => new
            {
                GLPeriodId = itemTransferReceive.GLPeriodId,
                ItemTransferID = itemTransferReceive.ItemTransferID,
                StoreRequisitionID = itemTransferReceive.StoreRequisitionID,
                DocumentRef = itemTransferReceive.DocumentRef,
                TransferDate = itemTransferReceive.TransferDate,
                FromStore = itemTransferReceive.FromStore,
                ToStore = itemTransferReceive.ToStore,
                DeliveredBy = itemTransferReceive.DeliveredBy,
                PlateNo = itemTransferReceive.PlateNo,
                SupplierName = itemTransferReceive.SupplierName,
                DriverType = itemTransferReceive.DriverType,
                Status = itemTransferReceive.Status,
                Remark = itemTransferReceive.Remark,
                IsVoid = itemTransferReceive.IsVoid,
                VoidBy = itemTransferReceive.VoidBy,
                VoidTime = itemTransferReceive.VoidTime,
                IsOnlineApproved = itemTransferReceive.IsOnlineApproved,
                OnlineApprovedBy = itemTransferReceive.OnlineApprovedBy,
                OnlineApprovedTime = itemTransferReceive.OnlineApprovedTime,
                IsSendForApproval = itemTransferReceive.IsSendForApproval,
                SendForApprovalBy = itemTransferReceive.SendForApprovalBy,
                SendForApprovalTime = itemTransferReceive.SendForApprovalTime,
                IsOnlineTransferred = itemTransferReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = itemTransferReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = itemTransferReceive.OnlineTransferredTime,


                DateCreated = itemTransferReceive.DateCreated,
                LastModified = itemTransferReceive.LastModified,
                CreatedBy = itemTransferReceive.CreatedBy,
                ModifiedBy = itemTransferReceive.ModifiedBy
            });

            return Json(result);
        }
        public JsonResult BatchtransferTransferSend(string id)
        {           

            var untransffered = _db.ItemTransfers.Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.FromStore)).ToList();
                }
            }
            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.TransferSend(record.ItemTransferID, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Item Transfer transfer ID:" + record.ItemTransferID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory transfer send", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ItemTransferReceive()
        {
            var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();


            var stores = _db.InventoryStores;
            var suppliers = _db.ProcurementSuppliers;
            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(i => new
            {
                Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                Code = i.EmployeeId
            }).AsQueryable();

            ViewData["suppliers"] = suppliers;
            ViewData["stores"] = stores;
            ViewData["defaultstore"] = stores.First();
            ViewData["personEmployees"] = selectedEmployee;
            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            return View();
        }
        public ActionResult ItemTransferReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<ItemTransferReceive> itemtransferreceives = _db.ItemTransferReceive
                .Where(st => st.IsOnlineApproved && !st.IsVoid && !st.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    itemtransferreceives = itemtransferreceives.Where(x => storesinBusinessUnit.Contains(x.ToStore));
                }
            }
            DataSourceResult result = itemtransferreceives.ToDataSourceResult(request, itemTransferReceive => new
            {
                GLPeriodId = itemTransferReceive.GLPeriodId,
                ItemTransferReceiveId = itemTransferReceive.ItemTransferReceiveId,
                ItemTransferID = itemTransferReceive.ItemTransferID,
                DocumentRef = itemTransferReceive.DocumentRef,
                ReceiveDate = itemTransferReceive.ReceiveDate,
                FromStore = itemTransferReceive.FromStore,
                ToStore = itemTransferReceive.ToStore,
                DeliveredBy = itemTransferReceive.DeliveredBy,
                PlateNo = itemTransferReceive.PlateNo,
                SupplierName = itemTransferReceive.SupplierName,
                DriverType = itemTransferReceive.DriverType,
                Status = itemTransferReceive.Status,
                Remark = itemTransferReceive.Remark,
                IsVoid = itemTransferReceive.IsVoid,
                VoidBy = itemTransferReceive.VoidBy,
                VoidTime = itemTransferReceive.VoidTime,
                IsOnlineApproved = itemTransferReceive.IsOnlineApproved,
                OnlineApprovedBy = itemTransferReceive.OnlineApprovedBy,
                OnlineApprovedTime = itemTransferReceive.OnlineApprovedTime,
                IsSendForApproval = itemTransferReceive.IsSendForApproval,
                SendForApprovalBy = itemTransferReceive.SendForApprovalBy,
                SendForApprovalTime = itemTransferReceive.SendForApprovalTime,
                IsOnlineTransferred = itemTransferReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = itemTransferReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = itemTransferReceive.OnlineTransferredTime,


                DateCreated = itemTransferReceive.DateCreated,
                LastModified = itemTransferReceive.LastModified,
                CreatedBy = itemTransferReceive.CreatedBy,
                ModifiedBy = itemTransferReceive.ModifiedBy
            });

            return Json(result);
        }
        public JsonResult BatchtransferTransferReceive(string id)
        {
            var untransffered = _db.ItemTransferReceive.Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.ToStore)).ToList();
                }
            }
            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.TransferReceive(record.ItemTransferID, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Item Transfer transfer ID:" + record.ItemTransferID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory transfer receive", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StoreReturn()
        {
             var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;

            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();
            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            return View();
        }
        public ActionResult StoreReturns_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<StoreReturn> storereturns = _db.StoreReturns
                .Where(x=>x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    storereturns = storereturns.Where(x => storesinBusinessUnit.Contains(x.StoreCode));
                }
            }
            DataSourceResult result = storereturns.ToDataSourceResult(request, storeReturn => new
            {
                GLPeriodId = storeReturn.GLPeriodId,
                StoreReturnID = storeReturn.StoreReturnID,
                Reference = storeReturn.Reference,
                StoreCode = storeReturn.StoreCode,
                ReturnDate = storeReturn.ReturnDate,
                Reason = storeReturn.Reason,
                Status = storeReturn.Status,
                Remark = storeReturn.Remark,
                IsVoid = storeReturn.IsVoid,
                VoidBy = storeReturn.VoidBy,
                VoidTime = storeReturn.VoidTime,
                IsOnlineApproved = storeReturn.IsOnlineApproved,
                OnlineApprovedBy = storeReturn.OnlineApprovedBy,
                OnlineApprovedTime = storeReturn.OnlineApprovedTime,
                IsOnlineTransferred = storeReturn.IsOnlineTransferred,
                IsOnlineTransferredBy = storeReturn.IsOnlineTransferredBy,
                OnlineTransferredTime = storeReturn.OnlineTransferredTime,
                IsSendForApproval = storeReturn.IsSendForApproval,
                SendForApprovalBy = storeReturn.SendForApprovalBy,
                SendForApprovalTime = storeReturn.SendForApprovalTime,
                DateCreated = storeReturn.DateCreated,
                CreatedBy = storeReturn.CreatedBy,
            });

            return Json(result);
        }
        public JsonResult BatchtransferStoreReturn(string id)
        {
            var untransffered = _db.StoreReturns.Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.StoreCode)).ToList();
                }
            }
            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.StoreReturn(record.StoreReturnID, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Store ReturnID:" + record.StoreReturnID + _response);
            }

            #region auto job



            #endregion
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory store return", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GoodSalesOrLend()
        {
             var periods = _db.InventoryPeriods.OrderByDescending(x=>x.DateFrom).ToList();
            ViewData["PeriodList"] = periods
                .Select(e => new InventoryPeriod
                {
                    Name = e.Name,
                    InventoryPeriodId = e.InventoryPeriodId
                })
                .OrderBy(e => e.DateTo);
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;

            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();
            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            return View();
        }
        public ActionResult GoodSalesOrLend_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<GoodSellingAndLending> goodsellingandlendings = _db.GoodSellingAndLendings
                .Where(x => x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred)
                .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    goodsellingandlendings = goodsellingandlendings.Where(x => storesinBusinessUnit.Contains(x.StoreCode));
                }
            }
            DataSourceResult result = goodsellingandlendings.ToDataSourceResult(request, goodSellingAndLending => new {
                GoodSellingAndLendingID = goodSellingAndLending.GoodSellingAndLendingID,
                SellOrLend = goodSellingAndLending.SellOrLend,
                StoreCode = goodSellingAndLending.StoreCode,
                Buyer = goodSellingAndLending.Buyer,
                BuyerAddress = goodSellingAndLending.BuyerAddress,
                Date = goodSellingAndLending.Date,
                Status = goodSellingAndLending.Status,
                Remark = goodSellingAndLending.Remark,
                IsVoid = goodSellingAndLending.IsVoid,
                VoidBy = goodSellingAndLending.VoidBy,
                VoidTime = goodSellingAndLending.VoidTime,
                IsOnlineApproved = goodSellingAndLending.IsOnlineApproved,
                OnlineApprovedBy = goodSellingAndLending.OnlineApprovedBy,
                OnlineApprovedTime = goodSellingAndLending.OnlineApprovedTime,
                IsSendForApproval = goodSellingAndLending.IsSendForApproval,
                SendForApprovalBy = goodSellingAndLending.SendForApprovalBy,
                SendForApprovalTime = goodSellingAndLending.SendForApprovalTime,
                IsOnlineTransferred = goodSellingAndLending.IsOnlineTransferred,
                IsOnlineTransferredBy = goodSellingAndLending.IsOnlineTransferredBy,
                OnlineTransferredTime = goodSellingAndLending.OnlineTransferredTime,

            });

            return Json(result);
        }
        public JsonResult BatchtransferGoodSalesOrLend(string id)
        {
            var untransffered = _db.GoodSellingAndLendings.Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                int myBusinessUnit = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                if (myBusinessUnit != 0)
                {
                    List<string> storesinBusinessUnit = InventoryHelper.GetStoreByBusinessUnit(myBusinessUnit);
                    untransffered = untransffered.Where(x => storesinBusinessUnit.Contains(x.StoreCode)).ToList();
                }
            }
            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new InventoryTransferHelper();
                _response = _transferHelper.GoodSelling(record.GoodSellingAndLendingID, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Good selling ID:" + record.GoodSellingAndLendingID + _response);
            }

            #region auto job



            #endregion
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module inventory store return", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
    }
}