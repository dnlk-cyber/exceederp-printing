using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.DataSource.Print;
using ExceedERP.Reporting.Finance.Print;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApPaymentSettlementsController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
       

        [AuthorizeRoles(FinanceConstants.AccountPayableView ,FinanceConstants.AccountPayableApprover ,FinanceConstants.AccountPayableSettlement)]
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            //var paymentMethod = _db.PaymentMethods.ToList();

            //ViewData["methods"] = paymentMethod;

            var tax = _db.GLTaxRates.Where(x => x.ApplyToAp).ToList();

            ViewData["taxes"] = tax;

            var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).Where(x => x.ApplyToAp).ToList();

            ViewData["witholdingType"] = witholdings;

            var suppliers = _db.ProcurementSuppliers;
            var selectedSuppliers = suppliers.Select(
              i => new
              {
                  Name = i.SupplierId +" "+i.Name,
                  Code = i.SupplierId
              }).AsQueryable();

            var employees = _db.Person_Employee;
            var selectedEmployees = employees.Select(
              i => new
              {
                  Name = i.EmployeeCode + " " + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                  Code = i.EmployeeId
              }).AsQueryable();

            ViewData["suppliers"] = selectedSuppliers;
            ViewData["employees"] = selectedEmployees;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView ,FinanceConstants.AccountPayableApprover ,FinanceConstants.AccountPayableSettlement)]
        public ActionResult ApPaymentSettlements_Read([DataSourceRequest]DataSourceRequest request,bool isNotTransffered)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<ApPaymentSettlement> ApPaymentSettlements = _db.ApPaymentSettlements
                .Where(x=>x.IsDepartmentOnlineApproved)
                .Where(x => mybraches.Contains(x.CreatedAtBranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                ApPaymentSettlements = ApPaymentSettlements.Where(x => myBusinessUnits.Contains(x.CreatedAtBusinessUnitId));
            }

            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApPaymentSettlementTranferableList();
                ApPaymentSettlements =
                    ApPaymentSettlements.Where(x => transfferableList.Contains(x.ApPaymentSettlementId));
            }
            DataSourceResult result = ApPaymentSettlements.ToDataSourceResult(request, apPaymentsettlement => new {
                ApPaymentSettlementId = apPaymentsettlement.ApPaymentSettlementId,
                BranchId = apPaymentsettlement.BranchId,
                BusinessUnitId = apPaymentsettlement.BusinessUnitId,
                CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                SupplierId = apPaymentsettlement.SupplierId,
                CollectedByPurchaser = apPaymentsettlement.CollectedByPurchaser,
                EmployeeId = apPaymentsettlement.EmployeeId,
                Reference = apPaymentsettlement.Reference,
               
                PaymentDate = apPaymentsettlement.PaymentDate,
                
                Remark = apPaymentsettlement.Remark,
                IsSendForApproval = apPaymentsettlement.IsSendForApproval,
                IsOnlineTransferred = apPaymentsettlement.IsOnlineTransferred,
                IsOnlineTransferredBy = apPaymentsettlement.IsOnlineTransferredBy,
                OnlineTransferredTime = apPaymentsettlement.OnlineTransferredTime,
                Status = apPaymentsettlement.Status,

                IsOnlineApproved = apPaymentsettlement.IsOnlineApproved,
                OnlineApprovedBy = apPaymentsettlement.OnlineApprovedBy,
                OnlineApprovedTime = apPaymentsettlement.OnlineApprovedTime,
                IsVoid = apPaymentsettlement.IsVoid,
                VoidBy = apPaymentsettlement.VoidBy,
                VoidTime = apPaymentsettlement.VoidTime,

                IsDepartmentOnlineApproved = apPaymentsettlement.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = apPaymentsettlement.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = apPaymentsettlement.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = apPaymentsettlement.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = apPaymentsettlement.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = apPaymentsettlement.DepartmentSendForApprovalTime,

               
                DateCreated = apPaymentsettlement.DateCreated,
                LastModified = apPaymentsettlement.LastModified,
                CreatedBy = apPaymentsettlement.CreatedBy,
                ModifiedBy = apPaymentsettlement.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlements_Create([DataSourceRequest]DataSourceRequest request, ApPaymentSettlement apPaymentsettlement)
        {
            #region  validation
            
            if (!apPaymentsettlement.CollectedByPurchaser)
            {
                if (String.IsNullOrEmpty(apPaymentsettlement.SupplierId))
                {
                    ModelState.AddModelError("supplierid", "Please select supplier.");
                }

                apPaymentsettlement.EmployeeId = 0;
            }

            else
            {
                if (apPaymentsettlement.EmployeeId==null)
                {
                    ModelState.AddModelError("purchaser", "Please select Purchaser.");
                }
                apPaymentsettlement.SupplierId = "";
            }

            #endregion
            #region data
            apPaymentsettlement.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            apPaymentsettlement.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            if (apPaymentsettlement.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apPaymentsettlement.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apPaymentsettlement.BranchId = placement.BranchId;
                    apPaymentsettlement.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    apPaymentsettlement.BranchId = apPaymentsettlement.CreatedAtBranchId;
                    apPaymentsettlement.BusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId;
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlement
                {
                    BranchId = apPaymentsettlement.BranchId,
                    BusinessUnitId = apPaymentsettlement.BusinessUnitId,
                    CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                    SupplierId = apPaymentsettlement.SupplierId,
                    CollectedByPurchaser = apPaymentsettlement.CollectedByPurchaser,
                    EmployeeId = apPaymentsettlement.EmployeeId,
                    Reference = apPaymentsettlement.Reference,
                  
                    PaymentDate = apPaymentsettlement.PaymentDate.Date,
                    
                    Remark = apPaymentsettlement.Remark,
                    IsSendForApproval = apPaymentsettlement.IsSendForApproval,
                    IsOnlineTransferred = apPaymentsettlement.IsOnlineTransferred,
                    IsOnlineTransferredBy = apPaymentsettlement.IsOnlineTransferredBy,
                    OnlineTransferredTime = apPaymentsettlement.OnlineTransferredTime,
                    Status = apPaymentsettlement.Status,
                    IsOnlineApproved = apPaymentsettlement.IsOnlineApproved,
                    OnlineApprovedBy = apPaymentsettlement.OnlineApprovedBy,
                    OnlineApprovedTime = apPaymentsettlement.OnlineApprovedTime,
                    IsVoid = apPaymentsettlement.IsVoid,
                    VoidBy = apPaymentsettlement.VoidBy,
                    VoidTime = apPaymentsettlement.VoidTime,

                    IsDepartmentOnlineApproved = apPaymentsettlement.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = apPaymentsettlement.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = apPaymentsettlement.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = apPaymentsettlement.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = apPaymentsettlement.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = apPaymentsettlement.DepartmentSendForApprovalTime,

                   
                    DateCreated = DateTime.Now,
                    LastModified = apPaymentsettlement.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apPaymentsettlement.ModifiedBy
                };

                _db.ApPaymentSettlements.Add(entity);
                _db.SaveChanges();
                apPaymentsettlement.ApPaymentSettlementId = entity.ApPaymentSettlementId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP payment settlement", "Success", User.Identity.Name, $"created payment settlement {entity.ApPaymentSettlementId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { apPaymentsettlement }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlements_Update([DataSourceRequest]DataSourceRequest request, ApPaymentSettlement apPaymentsettlement)
        {
            #region  validation

            if (!apPaymentsettlement.CollectedByPurchaser)
            {
                if (String.IsNullOrEmpty(apPaymentsettlement.SupplierId))
                {
                    ModelState.AddModelError("supplierid", "Please select supplier.");
                }
                apPaymentsettlement.EmployeeId = 0;
            }

            else
            {
                if (apPaymentsettlement.EmployeeId == null)
                {
                    ModelState.AddModelError("purchaser", "Please select Purchaser.");
                }

                apPaymentsettlement.SupplierId = "";
            }

            #endregion
            #region data
            apPaymentsettlement.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            apPaymentsettlement.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            if (apPaymentsettlement.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apPaymentsettlement.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apPaymentsettlement.BranchId = placement.BranchId;
                    apPaymentsettlement.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    apPaymentsettlement.BranchId = apPaymentsettlement.CreatedAtBranchId;
                    apPaymentsettlement.BusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId;
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new ApPaymentSettlement
                    {
                        ApPaymentSettlementId = apPaymentsettlement.ApPaymentSettlementId,
                        BranchId = apPaymentsettlement.BranchId,
                        BusinessUnitId = apPaymentsettlement.BusinessUnitId,
                        CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                        SupplierId = apPaymentsettlement.SupplierId,
                        CollectedByPurchaser = apPaymentsettlement.CollectedByPurchaser,
                        EmployeeId = apPaymentsettlement.EmployeeId,
                        Reference = apPaymentsettlement.Reference,
                       
                        PaymentDate = apPaymentsettlement.PaymentDate,
                        
                        Remark = apPaymentsettlement.Remark,
                        IsSendForApproval = apPaymentsettlement.IsSendForApproval,
                        IsOnlineTransferred = apPaymentsettlement.IsOnlineTransferred,
                        IsOnlineTransferredBy = apPaymentsettlement.IsOnlineTransferredBy,
                        OnlineTransferredTime = apPaymentsettlement.OnlineTransferredTime,
                        Status = apPaymentsettlement.Status,
                        IsOnlineApproved = apPaymentsettlement.IsOnlineApproved,
                        OnlineApprovedBy = apPaymentsettlement.OnlineApprovedBy,
                        OnlineApprovedTime = apPaymentsettlement.OnlineApprovedTime,
                        IsVoid = apPaymentsettlement.IsVoid,
                        VoidBy = apPaymentsettlement.VoidBy,
                        VoidTime = apPaymentsettlement.VoidTime,

                        IsDepartmentOnlineApproved = apPaymentsettlement.IsDepartmentOnlineApproved,
                        DepartmentOnlineApprovedBy = apPaymentsettlement.DepartmentOnlineApprovedBy,
                        DepartmentOnlineApprovedTime = apPaymentsettlement.DepartmentOnlineApprovedTime,
                        IsDepartmentSendForApproval = apPaymentsettlement.IsDepartmentSendForApproval,
                        DepartmentSendForApprovalBy = apPaymentsettlement.DepartmentSendForApprovalBy,
                        DepartmentSendForApprovalTime = apPaymentsettlement.DepartmentSendForApprovalTime,

                       
                        DateCreated = apPaymentsettlement.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = apPaymentsettlement.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApPaymentSettlements.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP payment settlement", "Success", User.Identity.Name, $"updated payment settlement {entity.ApPaymentSettlementId} ", Modules.Finance);
                #endregion

            }
            return Json(new[] { apPaymentsettlement }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlements_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentSettlement apPaymentsettlement)
        {

            

            if (ModelState.IsValid)
            {
                    var entity = new ApPaymentSettlement
                    {
                        ApPaymentSettlementId = apPaymentsettlement.ApPaymentSettlementId,
                        BranchId = apPaymentsettlement.BranchId,
                        BusinessUnitId = apPaymentsettlement.BusinessUnitId,
                        CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                        SupplierId = apPaymentsettlement.SupplierId,
                        CollectedByPurchaser = apPaymentsettlement.CollectedByPurchaser,
                        EmployeeId = apPaymentsettlement.EmployeeId,
                        Reference = apPaymentsettlement.Reference,
                       
                        PaymentDate = apPaymentsettlement.PaymentDate,
                        
                        Remark = apPaymentsettlement.Remark,
                        IsSendForApproval = apPaymentsettlement.IsSendForApproval,
                        IsOnlineTransferred = apPaymentsettlement.IsOnlineTransferred,
                        IsOnlineTransferredBy = apPaymentsettlement.IsOnlineTransferredBy,
                        OnlineTransferredTime = apPaymentsettlement.OnlineTransferredTime,
                        Status = apPaymentsettlement.Status,
                        IsOnlineApproved = apPaymentsettlement.IsOnlineApproved,
                        OnlineApprovedBy = apPaymentsettlement.OnlineApprovedBy,
                        OnlineApprovedTime = apPaymentsettlement.OnlineApprovedTime,
                        IsVoid = apPaymentsettlement.IsVoid,
                        VoidBy = apPaymentsettlement.VoidBy,
                        VoidTime = apPaymentsettlement.VoidTime,
                       
                        DateCreated = apPaymentsettlement.DateCreated,
                        LastModified = apPaymentsettlement.LastModified,
                        CreatedBy = apPaymentsettlement.CreatedBy,
                        ModifiedBy = apPaymentsettlement.ModifiedBy
                    };

                    _db.ApPaymentSettlements.Attach(entity);
                    _db.ApPaymentSettlements.Remove(entity);
                    _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment settlement number " + apPaymentsettlement.ApPaymentSettlementId + " " + apPaymentsettlement.Reference, Modules.Finance);
                #endregion
            }
            return Json(new[] { apPaymentsettlement }.ToDataSourceResult(request, ModelState));
        }

       

        [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        public JsonResult SendforApproval(int id = 0)
        {
            var  record = _db.ApPaymentSettlements.Find(id);
            string errMsg = "";
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            #region check segment length validation

            var distributions = _db.ApPaymentSettlementDistributions.Where(x => x.ApPaymentSettlementId == id).ToList();
            AccountHelper accountHelper = new AccountHelper();
            foreach (var dist in distributions)
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

                #region debit credit validation
                if (dist.Debit != 0 && dist.Credit != 0)
                {
                    message = $"{dist.AccountCode} Account has both debit and credit value.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit == 0 && dist.Credit == 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit > 0 && dist.Credit > 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit < 0 || dist.Credit < 0)
                {
                    message = $"{dist.AccountCode} Negative values are not allowed.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }


            #endregion

            if (record != null && record.IsOnlineApproved)
            {
                errMsg = "You cannot manipulate this record";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsSendForApproval)
            {
                errMsg = "This record is already sent for approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

          

            try
            {
                if (record != null)
                {
                    sumTotalDisDebit = record.ApPaymentSettlementDistributions.Sum(x => x.Debit);
                    sumTotalDisCredit = record.ApPaymentSettlementDistributions.Sum(x => x.Credit);
                }
            }
            catch (Exception e)
            {
                errMsg = "Please add distribution and try again";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                errMsg = "Total Debit and Total Credit is not equal.Please correct it and try again";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit == 0 || sumTotalDisCredit == 0)
            {
                errMsg = "Total balance should be more than 0.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineApproved && !record.IsSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

            }
            return null;
        }

         [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
         public JsonResult TransferDistribution(string id, string description)
         {
             int myid = int.Parse(id);
             var record = _db.ApPaymentSettlements.Find(myid);
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
                    .Where(x => x.Source == JournalSources.DirectPaymentSettlement.ToString())
                    .Where(x => x.SourceId == record.ApPaymentSettlementId.ToString())
                    .FirstOrDefault();
                if(exist!=null)
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
                foreach (var dist in record.ApPaymentSettlementDistributions)
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

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                 decimal credit = 0;
                 var distribution = record.ApPaymentSettlementDistributions.ToList();
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

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
                    .Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
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
                        Message = "There are no periods defined that contain the payment date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                 if (record.ApPaymentSettlementDistributions.Any())
                 {
                     var journal = new GlRecordJournal
                     {
                         DateCreated = DateTime.Now,
                         CreatedBy = User.Identity.Name,
                         JournalName = "AP Direct Payment Settlement",
                         Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment Settlement",
                         FiscalYear = glyear,
                         PeriodName = glperiod,
                         CategoryName = "AP Direct Payment Settlement",
                         SourcesName = "AP",
                         JournalNumber = "APDPAYSETTLE." + record.ApPaymentSettlementId.ToString(),
                         SourceId = record.ApPaymentSettlementId.ToString(),
                         Source = JournalSources.DirectPaymentSettlement.ToString(),
                         EffectiveDate = record.PaymentDate.Date,
                         BranchId = record.BranchId,
                         Status = "Approved",
                         Remark = record.Reference,
                         PaymentReference = record.Reference,
                         ReferenceDoc=record.Remark,
                         InvoiceReferenceDoc="",
                         IsOnlineApproved = true,
                         OnlineApprovedBy = User.Identity.Name,
                         OnlineApprovedTime = DateTime.Now
                     };
                     _db.GLRecordJournals.Add(journal);
                     _db.SaveChanges();
                     foreach (var i in record.ApPaymentSettlementDistributions)
                     {
                         #region debit and credit

                         var journalDetail = new GLRecordJournalDetail
                         {
                             DateCreated = DateTime.Now,
                             CreatedBy = User.Identity.Name,
                             GLRecordJournal = journal,
                             AccountCode = i.AccountCode,
                             AccountDesc = i.AccountDesc,
                             Debit = i.Debit,
                             Credit = i.Credit,
                             AdditionalDescription = i.AdditionalDescription,
                         };
                         _db.GLRecordJournalDetails.Add(journalDetail);
                         _db.SaveChanges();
                         #endregion
                     }

                     #region update record

                     record.IsOnlineTransferred = true;
                     _db.Entry(record).State = EntityState.Modified;
                     _db.SaveChanges();

                    #endregion

                     #region split journal
                     var journaldetails = _db.GLRecordJournalDetails
                         .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                     foreach (var detail in journaldetails)
                     {
                         AccountHelper helper = new AccountHelper();
                         helper.SplitJournal(detail.GlRecordJournalDetailId);
                     }
                     #endregion

                    var formattedData = new
                     {
                         Message = "Successfully transfered.",
                         Status = "OK"
                     };
                     return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
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
            var record = _db.ApPaymentSettlements.Find(id);
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ApPaymentSettlementLines.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApPaymentSettlements.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region update direct payment
                //get the list from line items
                //int requestid = int.Parse(record.SourceId);
                //var payment = _db.ForeignPurchaseCosts.Find(requestid);
                //if (payment != null)
                //{
                //    payment.Paid = false;
                //    _db.Entry(payment).State = EntityState.Modified;
                //}

                //_db.SaveChanges();
                #endregion
                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payment settlement", "Success", User.Identity.Name, $"void payment settlement {id} ", Modules.Finance);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
           
            if (record.ApPaymentSettlementLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add lines and try again.",
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
