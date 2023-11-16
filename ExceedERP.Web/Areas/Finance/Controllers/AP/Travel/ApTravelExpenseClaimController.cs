using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelExpenseClaimController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelExpenseClaimApprover, FinanceConstants.AccountPayableTravelExpenseClaim)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

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
            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }
        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelExpenseClaimApprover, FinanceConstants.AccountPayableTravelExpenseClaim)]
        public ActionResult ApTravelExpenseClaims_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<ApTravelExpenseClaim> aptravelexpenseclaims = _db.ApTravelExpenseClaims.Where(x => x.IsDepartmentOnlineApproved);

            if (FinanceConfigurationHelper.IsPaymentBudgetControlEnabled())
            {
                aptravelexpenseclaims = aptravelexpenseclaims.Where(x => x.IsBudgetApproved);
            }

            DataSourceResult result = aptravelexpenseclaims.ToDataSourceResult(request, apTravelExpenseClaim => new {
                ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,              
                BranchId = apTravelExpenseClaim.BranchId,
                BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
                OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                ClaimDate = apTravelExpenseClaim.ClaimDate,
               
                Reference = apTravelExpenseClaim.Reference,
                EmployeeId = apTravelExpenseClaim.EmployeeId,
                IsForOther = apTravelExpenseClaim.IsForOther,

                ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
                ForwardedTo = apTravelExpenseClaim.ForwardedTo,
                IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
                CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

                IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
                BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
                BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

                IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                Status = apTravelExpenseClaim.Status,
                IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                IsVoid = apTravelExpenseClaim.IsVoid,
                VoidBy = apTravelExpenseClaim.VoidBy,
                VoidTime = apTravelExpenseClaim.VoidTime,
                IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                OrgBranchName = apTravelExpenseClaim.OrgBranchName,
                DateCreated = apTravelExpenseClaim.DateCreated,
                LastModified = apTravelExpenseClaim.LastModified,
                CreatedBy = apTravelExpenseClaim.CreatedBy,
                ModifiedBy = apTravelExpenseClaim.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaim)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaims_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            #region owner approval

            bool canOwnerApprove = DocApprovalHelper.CanOwnerApprove(ApprovalDocumentType.ApTravelExpenseClaim);
            if (canOwnerApprove)
                apTravelExpenseClaim.ForwardedTo = User.Identity.Name;

            #endregion
            #region data 
            if (apTravelExpenseClaim.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (apTravelExpenseClaim.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelExpenseClaim.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apTravelExpenseClaim.OrgStructureId = placement.OrgStructureId;
                    apTravelExpenseClaim.BranchId = placement.BranchId;
                    apTravelExpenseClaim.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            apTravelExpenseClaim.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            apTravelExpenseClaim.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            apTravelExpenseClaim.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,                    
                    BranchId = apTravelExpenseClaim.BranchId,
                    BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                    CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate.Date,
                   
                    Reference = apTravelExpenseClaim.Reference,
                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    IsForOther = apTravelExpenseClaim.IsForOther,

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
                    CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

                    IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
                    BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
                    BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

                    IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                    Status = apTravelExpenseClaim.Status,
                    IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                    OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                    IsVoid = apTravelExpenseClaim.IsVoid,
                    VoidBy = apTravelExpenseClaim.VoidBy,
                    VoidTime = apTravelExpenseClaim.VoidTime,
                    IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                    SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                    SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = apTravelExpenseClaim.DateCreated,
                    LastModified = apTravelExpenseClaim.LastModified,
                    CreatedBy = apTravelExpenseClaim.CreatedBy,
                    ModifiedBy = apTravelExpenseClaim.ModifiedBy
                };

                _db.ApTravelExpenseClaims.Add(entity);
                _db.SaveChanges();
                apTravelExpenseClaim.ApTravelExpenseClaimId = entity.ApTravelExpenseClaimId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel expense claim", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaim)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaims_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            #region data 
            if (apTravelExpenseClaim.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (apTravelExpenseClaim.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelExpenseClaim.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apTravelExpenseClaim.OrgStructureId = placement.OrgStructureId;
                    apTravelExpenseClaim.BranchId = placement.BranchId;
                    apTravelExpenseClaim.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,                   
                    BranchId = apTravelExpenseClaim.BranchId,
                    BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                    CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,
                   
                    Reference = apTravelExpenseClaim.Reference,
                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    IsForOther = apTravelExpenseClaim.IsForOther,

                    ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
                    ForwardedTo = apTravelExpenseClaim.ForwardedTo,
                    IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
                    CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

                    IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
                    BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
                    BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

                    IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                    Status = apTravelExpenseClaim.Status,
                    IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                    OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                    IsVoid = apTravelExpenseClaim.IsVoid,
                    VoidBy = apTravelExpenseClaim.VoidBy,
                    VoidTime = apTravelExpenseClaim.VoidTime,
                    IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                    SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                    SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                    OrgBranchName = apTravelExpenseClaim.OrgBranchName,
                    DateCreated = apTravelExpenseClaim.DateCreated,
                    LastModified = apTravelExpenseClaim.LastModified,
                    CreatedBy = apTravelExpenseClaim.CreatedBy,
                    ModifiedBy = apTravelExpenseClaim.ModifiedBy
                };

                _db.ApTravelExpenseClaims.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel expense claim", "Success", User.Identity.Name, $"updated expense claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaim)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaims_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,                    
                    BranchId = apTravelExpenseClaim.BranchId,
                    BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,
                   
                    Reference = apTravelExpenseClaim.Reference,
                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    IsForOther = apTravelExpenseClaim.IsForOther,

                    IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                    Status = apTravelExpenseClaim.Status,
                    IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                    OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                    IsVoid = apTravelExpenseClaim.IsVoid,
                    VoidBy = apTravelExpenseClaim.VoidBy,
                    VoidTime = apTravelExpenseClaim.VoidTime,
                    IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                    SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                    SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                    OrgBranchName = apTravelExpenseClaim.OrgBranchName,
                    DateCreated = apTravelExpenseClaim.DateCreated,
                    LastModified = apTravelExpenseClaim.LastModified,
                    CreatedBy = apTravelExpenseClaim.CreatedBy,
                    ModifiedBy = apTravelExpenseClaim.ModifiedBy
                };

                _db.ApTravelExpenseClaims.Attach(entity);
                _db.ApTravelExpenseClaims.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted expense claim " + apTravelExpenseClaim.ApTravelExpenseClaimId + " " + apTravelExpenseClaim.Reference, Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaim)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ApTravelExpenseClaims.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.Expenses.Count > 0)
            {
                //#region check segment length validation

                //var distributions = _db.ApTravelExpenseDistributions.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                //AccountHelper accountHelper = new AccountHelper();
                //foreach (var dist in distributions)
                //{
                //    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                //    if (!String.IsNullOrEmpty(message))
                //    {
                //        var formattedData1 = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                //    }
                //    #region debit credit validation
                //    if (dist.Debit != 0 && dist.Credit != 0)
                //    {
                //        message = $"{dist.AccountCode} Account has both debit and credit value.";
                //        var formattedData1 = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                //    }

                //    if (dist.Debit == 0 && dist.Credit == 0)
                //    {
                //        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                //        var formattedData1 = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                //    }

                //    if (dist.Debit > 0 && dist.Credit > 0)
                //    {
                //        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                //        var formattedData1 = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                //    }

                //    if (dist.Debit < 0 || dist.Credit < 0)
                //    {
                //        message = $"{dist.AccountCode} Negative values are not allowed.";
                //        var formattedData1 = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                //    }
                //    #endregion

                //}


                //#endregion

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ApTravelExpenseClaims.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region approval workflow
                bool isNewRecord = true;
                var validations = record.Validations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.Payments.Sum(x => x.Amount);
                int docid = record.ApTravelExpenseClaimId;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", User.Identity.Name, ApprovalDocumentType.ApTravelExpenseClaim, isNewRecord,step);
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

            if (record.Payments.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add  payment line items first.",
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

        [AuthorizeRoles(FinanceConstants.AccountPayableTravelExpenseClaim)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ApTravelExpenseClaims.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                    .Where(x => x.Source == JournalSources.AP_TravelExpenseClaim.ToString())
                    .Where(x => x.SourceId == record.ApTravelExpenseClaimId.ToString())
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
                //AccountHelper accountHelper = new AccountHelper();
                //foreach (var dist in record.Distributions)
                //{
                //    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                //    if (!String.IsNullOrEmpty(message))
                //    {
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }

                //    #region debit credit validation
                //    if (dist.Debit != 0 && dist.Credit != 0)
                //    {
                //        message = $"{dist.AccountCode} Account has both debit and credit value.";
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }

                //    if (dist.Debit == 0 && dist.Credit == 0)
                //    {
                //        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }

                //    if (dist.Debit > 0 && dist.Credit > 0)
                //    {
                //        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }

                //    if (dist.Debit < 0 || dist.Credit < 0)
                //    {
                //        message = $"{dist.AccountCode} Negative values are not allowed.";
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }
                //    #endregion

                //}


                #endregion
                #region check if debit and credit are equal
                //decimal debit = 0;
                //decimal credit = 0;
                //var distribution = record.Distributions.ToList();
                //if (distribution.Any())
                //{
                //    debit = distribution.Sum(x => x.Debit);
                //    credit = distribution.Sum(x => x.Credit);
                //}
                //if (debit != credit)
                //{
                //    var formattedData = new
                //    {
                //        Message = "Total debit and total credit is not balanced.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData, JsonRequestBehavior.AllowGet);
                //}
                //if (debit == 0)
                //{
                //    var formattedData = new
                //    {
                //        Message = "Total debit and total credit is zero.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData, JsonRequestBehavior.AllowGet);
                //}
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.ClaimDate.Date)
                    .Where(x => x.DateTo >= record.ClaimDate.Date).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the Claim date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                //if (record.Distributions.Any())
                //{
                //    GlRecordJournal journal = new GlRecordJournal
                //    {
                //        DateCreated = DateTime.Now,
                //        CreatedBy = User.Identity.Name,
                //        JournalName = "AP Expense Claim",
                //        Description = !String.IsNullOrEmpty(description) ? description : "AP Expense Claim",
                //        FiscalYear = glyear,
                //        PeriodName = glperiod,
                //        CategoryName = "AP Expense Claim",
                //        SourcesName = "AP",
                //        JournalNumber = "APEXPCLA." + record.ApTravelExpenseClaimId.ToString(),
                //        SourceId = record.ApTravelExpenseClaimId.ToString(),
                //        Source = JournalSources.AP_TravelExpenseClaim.ToString(),
                //        EffectiveDate = record.ClaimDate.Date,
                //        OrgBranchName = record.OrgBranchName,
                //        BranchId = record.BranchId,
                //        Status = "Approved",
                //        IsOnlineApproved = true,
                //        OnlineApprovedBy = User.Identity.Name,
                //        OnlineApprovedTime = DateTime.Now
                //    };
                //    _db.GLRecordJournals.Add(journal);
                //    _db.SaveChanges();
                //    foreach (var i in record.Distributions)
                //    {
                //        #region debit and credit

                //        var journalDetail = new GLRecordJournalDetail
                //        {
                //            DateCreated = DateTime.Now,
                //            CreatedBy = User.Identity.Name,
                //            GLRecordJournal = journal,
                //            AccountCode = i.AccountCode,
                //            AccountDesc = i.AccountDesc,
                //            Debit = i.Debit,
                //            Credit = i.Credit,
                //            AdditionalDescription = i.AdditionalDescription,
                //        };
                //        _db.GLRecordJournalDetails.Add(journalDetail);
                //        _db.SaveChanges();
                //        #endregion
                //    }

                //    #region update record

                //    record.IsOnlineTransferred = true;
                //    _db.Entry(record).State = EntityState.Modified;
                //    _db.SaveChanges();

                //    #endregion
                //    #region split journal
                //    var journaldetails = _db.GLRecordJournalDetails
                //        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                //    foreach (var detail in journaldetails)
                //    {
                //        AccountHelper helper = new AccountHelper();
                //        helper.SplitJournal(detail.GlRecordJournalDetailId);
                //    }
                //    #endregion

                //    var formattedData = new
                //    {
                //        Message = "Successfully transfered.",
                //        Status = "OK"
                //    };
                //    return Json(formattedData, JsonRequestBehavior.AllowGet);

                //}
                #endregion

            }
            else
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

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ApTravelExpenseClaims.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.Payments.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApTravelExpenseClaims.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "expense claim", "Success", User.Identity.Name, $"void expense claim {id} ", Modules.Finance);
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

            if (record.Payments.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add payment lines and try again.",
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

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
