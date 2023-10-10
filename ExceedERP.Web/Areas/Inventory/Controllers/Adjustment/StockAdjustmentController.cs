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
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Adjustment
{

    public class StockAdjustmentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

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
            //var stores = _db.InventoryStores;

            //var branches = _db.Branch;
            //var costCenter = _db.BranchCostCenter;
            //var selectedcostCenter = costCenter.Select(
            //  i => new
            //  {
            //      Name = i.MainCompanyStructure.Name,
            //      Code = i.OrgStructureId
            //  }).AsQueryable();
            //var inventoryPeriods = _db.InventoryPeriods;
            //var selectedinventoryPeriods = inventoryPeriods.Select(
            // i => new
            // {
            //     Name = i.Name,
            //     GLPeriodId = i.InventoryPeriodId
            // }).AsQueryable();
            //ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            //ViewData["costCenters"] = selectedcostCenter;

            //ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            //ViewData["stores"] = stores;

            //var employee = _db.Person_Employee;
            //var selectedEmployee = employee.Select(
            //    i => new
            //    {
            //        Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
            //        Code = i.EmployeeId
            //    }).AsQueryable();
            //ViewData["personEmployees"] = selectedEmployee;

            //var workitem = _db.ProjectEstimationWorkItem.Where(w => !string.IsNullOrEmpty(w.ItemCode));
            //var selectedWorkitem = workitem.Select(
            //    i => new
            //    {
            //        Name = i.ItemCode + "-" + i.ItemName,
            //        Code = i.ItemCode
            //    }).AsQueryable();
            //ViewData["WorkItem"] = selectedWorkitem;
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult StockAdjustments_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<StockAdjustment> stockadjustments = _db.StockAdjustments.Where(x => userStores.Contains(x.StoreCode));
            DataSourceResult result = stockadjustments.ToDataSourceResult(request, stockAdjustment => new
            {
                StockAdjustmentID = stockAdjustment.StockAdjustmentID,
                BranchId = stockAdjustment.BranchId,
                BusinessUnitId = stockAdjustment.BusinessUnitId,
                Reference = stockAdjustment.Reference,
                StoreCode = stockAdjustment.StoreCode,
                AdjustmentDate = stockAdjustment.AdjustmentDate,
                Reason = stockAdjustment.Reason,
                Status = stockAdjustment.Status,
                Remark = stockAdjustment.Remark,
                IsVoid = stockAdjustment.IsVoid,
                VoidBy = stockAdjustment.VoidBy,
                VoidTime = stockAdjustment.VoidTime,
                IsOnlineApproved = stockAdjustment.IsOnlineApproved,
                OnlineApprovedBy = stockAdjustment.OnlineApprovedBy,
                OnlineApprovedTime = stockAdjustment.OnlineApprovedTime,
                IsSendForApproval = stockAdjustment.IsSendForApproval,
                SendForApprovalBy = stockAdjustment.SendForApprovalBy,
                SendForApprovalTime = stockAdjustment.SendForApprovalTime,
                IsOnlineTransferred = stockAdjustment.IsOnlineTransferred,
                IsOnlineTransferredBy = stockAdjustment.IsOnlineTransferredBy,
                OnlineTransferredTime = stockAdjustment.OnlineTransferredTime,
                DateCreated = stockAdjustment.DateCreated,
                CreatedBy = stockAdjustment.CreatedBy,
                LastModified = stockAdjustment.LastModified,
                ModifiedBy = stockAdjustment.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustments_Create([DataSourceRequest]DataSourceRequest request, StockAdjustment stockAdjustment)
        {
            if (stockAdjustment.AdjustmentDate.Date > DateTime.Today.Date)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustment
                {
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(stockAdjustment.StoreCode),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                    Reference = stockAdjustment.Reference,
                    StoreCode = stockAdjustment.StoreCode,
                    AdjustmentDate = stockAdjustment.AdjustmentDate.Date,
                    Reason = stockAdjustment.Reason,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = stockAdjustment.Remark,
                    IsVoid = stockAdjustment.IsVoid,
                    VoidBy = stockAdjustment.VoidBy,
                    VoidTime = stockAdjustment.VoidTime,
                    IsOnlineApproved = stockAdjustment.IsOnlineApproved,
                    OnlineApprovedBy = stockAdjustment.OnlineApprovedBy,
                    OnlineApprovedTime = stockAdjustment.OnlineApprovedTime,
                    IsOnlineTransferred = stockAdjustment.IsOnlineTransferred,
                    IsOnlineTransferredBy = stockAdjustment.IsOnlineTransferredBy,
                    OnlineTransferredTime = stockAdjustment.OnlineTransferredTime,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(stockAdjustment.StoreCode).ToString()
                };

                _db.StockAdjustments.Add(entity);
                _db.SaveChanges();
                stockAdjustment.StockAdjustmentID = entity.StockAdjustmentID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment", "Success", User.Identity.Name, $"created stock adjustment id {entity.StockAdjustmentID}  ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { stockAdjustment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustments_Update([DataSourceRequest]DataSourceRequest request, StockAdjustment stockAdjustment)
        {
            if (stockAdjustment.AdjustmentDate.Date > DateTime.Today.Date)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustment
                {
                    StockAdjustmentID = stockAdjustment.StockAdjustmentID,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(stockAdjustment.StoreCode),
                    BusinessUnitId =stockAdjustment.BusinessUnitId,
                    Reference = stockAdjustment.Reference,
                    StoreCode = stockAdjustment.StoreCode,
                    AdjustmentDate = stockAdjustment.AdjustmentDate,
                    Reason = stockAdjustment.Reason,
                    Status = stockAdjustment.Status,
                    Remark = stockAdjustment.Remark,
                    IsVoid = stockAdjustment.IsVoid,
                    VoidBy = stockAdjustment.VoidBy,
                    VoidTime = stockAdjustment.VoidTime,
                    IsOnlineApproved = stockAdjustment.IsOnlineApproved,
                    OnlineApprovedBy = stockAdjustment.OnlineApprovedBy,
                    OnlineApprovedTime = stockAdjustment.OnlineApprovedTime,
                    IsOnlineTransferred = stockAdjustment.IsOnlineTransferred,
                    IsOnlineTransferredBy = stockAdjustment.IsOnlineTransferredBy,
                    OnlineTransferredTime = stockAdjustment.OnlineTransferredTime,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now
                };

                _db.StockAdjustments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment", "Success", User.Identity.Name, $"updated stock adjustment id {entity.StockAdjustmentID}  ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { stockAdjustment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockAdjustments_Destroy([DataSourceRequest]DataSourceRequest request, StockAdjustment stockAdjustment)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockAdjustment
                {
                    StockAdjustmentID = stockAdjustment.StockAdjustmentID,
                    BranchId = stockAdjustment.BranchId,
                    BusinessUnitId =stockAdjustment.BusinessUnitId,
                    Reference = stockAdjustment.Reference,
                    StoreCode = stockAdjustment.StoreCode,
                    AdjustmentDate = stockAdjustment.AdjustmentDate,
                    Reason = stockAdjustment.Reason,
                    Status = stockAdjustment.Status,
                    Remark = stockAdjustment.Remark,
                    IsVoid = stockAdjustment.IsVoid,
                    VoidBy = stockAdjustment.VoidBy,
                    VoidTime = stockAdjustment.VoidTime,
                    IsOnlineApproved = stockAdjustment.IsOnlineApproved,
                    OnlineApprovedBy = stockAdjustment.OnlineApprovedBy,
                    OnlineApprovedTime = stockAdjustment.OnlineApprovedTime,
                    IsOnlineTransferred = stockAdjustment.IsOnlineTransferred,
                    IsOnlineTransferredBy = stockAdjustment.IsOnlineTransferredBy,
                    OnlineTransferredTime = stockAdjustment.OnlineTransferredTime,
                };

                _db.StockAdjustments.Attach(entity);
                _db.StockAdjustments.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted stock adjustment number " + stockAdjustment.StockAdjustmentID + " " + stockAdjustment.Remark);
                #endregion
            }

            return Json(new[] { stockAdjustment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult TransferDistribution(string id)
        {
            int myid = int.Parse(id);
            StockAdjustment record = _db.StockAdjustments.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                   .Where(x => x.Source == JournalSources.Stock_Adjustment.ToString())
                   .Where(x => x.SourceId == record.StockAdjustmentID.ToString())
                   .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.StockAdjustmentDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.StockAdjustmentDistribution.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region send to GL
                string glperiod = "";
                string glyear = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.AdjustmentDate).Where(x => x.DateTo >= record.AdjustmentDate).ToList();
                if (period.Any())
                {
                    if (period.Count == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name;
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod + " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    else if (period.Count > 1)
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the transaction date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are no periods defined that contain the transaction date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);

                    }
                }

                #endregion
                if (record.StockAdjustmentDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "Adjustment",
                        Description = "Adjustment",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Adjustment",
                        SourcesName = "Adjustment",
                        JournalNumber = "Adjustment." + record.StockAdjustmentID.ToString(),
                        SourceId = record.StockAdjustmentID.ToString(),
                        Source = JournalSources.Stock_Adjustment.ToString(),
                        EffectiveDate = record.AdjustmentDate,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName=record.OrgBranchName
                    };

                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (StockAdjustmentDistribution i in record.StockAdjustmentDistribution)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AdditionalDescription = i.AdditionalDescription,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = User.Identity.Name;
                    record.OnlineTransferredTime = DateTime.Now;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully transferred.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There is no distribution to be transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

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

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.StockAdjustments.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.StockAdjustmentItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.StockAdjustments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "stock adjustment", "Success", User.Identity.Name, $"send for approval stock adjustment id {id}  ", Modules.Inventory);
                #endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
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

            if (record.StockAdjustmentItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
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
            _db.Dispose();
            base.Dispose(disposing);
        }
        private void PopulateCategories()
        {
            var dataContext = new ExceedDbContext();
            var items = dataContext.InventoryItems;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["defaultCategory"] = items.FirstOrDefault();
        }
    }
}
