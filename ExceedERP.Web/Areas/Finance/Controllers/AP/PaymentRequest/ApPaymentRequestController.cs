using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PaymentRequest
{
    public class ApPaymentRequestController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
      

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var employeName = _db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            var tax = _db.GLTaxRates.ToList();
            ViewData["GLTaxRate"] = tax;

            var branch = _db.Branch.ToList();
            ViewData["Branch"] = branch;

            var structure = _db.MainCompanyStructures.ToList();
            ViewData["OrgStructure"] = structure;

            var items = _db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }

        private JsonResult GetPeriod()
        {
            var periodList = _db.GLFiscalYears;
            ViewData["PeriodList"] = periodList?.ToList() ?? new List<GlFiscalYear>();

            return Json((periodList ?? throw new InvalidOperationException()).Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
      

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]
        public ActionResult PaymentRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);


            IQueryable<Core.Domain.Finance.AP.PaymentRequest.PaymentRequest> paymentRequests = _db.PaymentRequests
                .Where(x=>mybraches.Contains(x.BranchId))
                .Where(x => x.IsDepartmentOnlineApproved);

            if (FinanceConfigurationHelper.IsPRBudgetControlEnabled())
            {
                paymentRequests = paymentRequests.Where(x => x.IsBudgetApproved);
            }

            if (FinanceConfigurationHelper.IsPaymentRequestAuthorizationEnabled())
            {
                paymentRequests = paymentRequests.Where(x => x.IsAuthorized);
            }

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                paymentRequests = paymentRequests.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

            DataSourceResult result = paymentRequests.ToDataSourceResult(request, paymentRequest => new
            {
                PaymentRequestId = paymentRequest.PaymentRequestId,
                EmployeeId = paymentRequest.EmployeeId,
                OrgStuructureId = paymentRequest.OrgStuructureId,
                BranchId = paymentRequest.BranchId,
                BusinessUnitId = paymentRequest.BusinessUnitId,
                CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                SupplierId = paymentRequest.SupplierId,
                IsNewSupplier = paymentRequest.IsNewSupplier,
                Supplier = paymentRequest.Supplier,
                Purchaser = paymentRequest.Purchaser,
                ReasonOfPayment = paymentRequest.ReasonOfPayment,
                RequestDate = paymentRequest.RequestDate,
                RequestDateAm = paymentRequest.RequestDateAm,
                InvoiceNo = paymentRequest.InvoiceNo,
                PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                PONo = paymentRequest.PONo,
                GRN = paymentRequest.GRN,
                
                Status =paymentRequest.Status,
                Remark =paymentRequest.Remark,
                IsVoid =paymentRequest.IsVoid,
                VoidBy =paymentRequest.VoidBy,
                VoidTime =paymentRequest.VoidTime,
                IsOnlineApproved =paymentRequest.IsOnlineApproved,
                OnlineApprovedBy =paymentRequest.OnlineApprovedBy,
                OnlineApprovedTime =paymentRequest.OnlineApprovedTime,
                IsSendForApproval =paymentRequest.IsSendForApproval,
                SendForApprovalBy =paymentRequest.SendForApprovalBy,
                SendForApprovalTime =paymentRequest.SendForApprovalTime,
                IsOnlineTransferred =paymentRequest.IsOnlineTransferred,
                IsOnlineTransferredBy =paymentRequest.IsOnlineTransferredBy,
                OnlineTransferredTime =paymentRequest.OnlineTransferredTime,

                IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,


                IsBudgetApproved = paymentRequest.IsBudgetApproved,
                BudgetApprovedBy = paymentRequest.BudgetApprovedBy,
                BudgetApprovedTime = paymentRequest.BudgetApprovedTime,

                IsAuthorized = paymentRequest.IsAuthorized,
                AuthorizedBy = paymentRequest.AuthorizedBy,
                AuthorizedTime = paymentRequest.AuthorizedTime,

                ForwardedFrom = paymentRequest.ForwardedFrom,
                ForwardedTo = paymentRequest.ForwardedTo,
                IsFinalApproval = paymentRequest.IsFinalApproval,
                CurrentApprovalStep = paymentRequest.CurrentApprovalStep,

                
                DateCreated =paymentRequest.DateCreated,
                CreatedBy =paymentRequest.CreatedBy,
                LastModified =paymentRequest.LastModified,
                ModifiedBy =paymentRequest.ModifiedBy,

                OrgBranchName = paymentRequest.OrgBranchName,

            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Finance.AP.PaymentRequest.PaymentRequest paymentRequest)
        {
            #region data

            if (paymentRequest.EmployeeId != 0)
            {
                var placemnt = _db.Placements.Where(x => x.EmployeeId == paymentRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
                if (placemnt != null)
                {
                    paymentRequest.BranchId = placemnt.BranchId;
                    paymentRequest.BusinessUnitId = placemnt.BusinessUnitId;
                    paymentRequest.OrgStuructureId = placemnt.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            if (!String.IsNullOrWhiteSpace(paymentRequest.SupplierId))
            {
                var supplier =
                    _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == paymentRequest.SupplierId);
                if (supplier != null) paymentRequest.Supplier = supplier.Name;
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Finance.AP.PaymentRequest.PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    BranchId = paymentRequest.BranchId,
                    BusinessUnitId = paymentRequest.BusinessUnitId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
                    CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                    SupplierId = paymentRequest.SupplierId,
                    IsNewSupplier = paymentRequest.IsNewSupplier,
                    Supplier = paymentRequest.Supplier,
                    Purchaser = paymentRequest.Purchaser,
                    ReasonOfPayment = paymentRequest.ReasonOfPayment,
                    RequestDateAm = paymentRequest.RequestDateAm,
                    InvoiceNo = paymentRequest.InvoiceNo,
                    RequestDate = paymentRequest.RequestDate,
                    PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                    PONo = paymentRequest.PONo,
                    GRN = paymentRequest.GRN,
                    
                    Remark = paymentRequest.Remark,
                    IsVoid = paymentRequest.IsVoid,
                    VoidBy = paymentRequest.VoidBy,
                    VoidTime = paymentRequest.VoidTime,
                    IsOnlineApproved = paymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = paymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = paymentRequest.OnlineApprovedTime,
                    IsSendForApproval = paymentRequest.IsSendForApproval,
                    SendForApprovalBy = paymentRequest.SendForApprovalBy,
                    SendForApprovalTime = paymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = paymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = paymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = paymentRequest.OnlineTransferredTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                    IsBudgetApproved = paymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = paymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = paymentRequest.BudgetApprovedTime,

                    ForwardedFrom = paymentRequest.ForwardedFrom,
                    ForwardedTo = paymentRequest.ForwardedTo,
                    IsFinalApproval = paymentRequest.IsFinalApproval,
                    CurrentApprovalStep = paymentRequest.CurrentApprovalStep,


                    IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,

                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = paymentRequest.LastModified,
                    ModifiedBy = paymentRequest.ModifiedBy,
                };

                _db.PaymentRequests.Add(entity);
                _db.SaveChanges();
                paymentRequest.PaymentRequestId = entity.PaymentRequestId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP payment request", "Success", User.Identity.Name, $"created payment request {entity.PaymentRequestId} ", Modules.Finance);
                #endregion


            }



            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Finance.AP.PaymentRequest.PaymentRequest paymentRequest)
        {


            #region data

            if (paymentRequest.EmployeeId != 0)
            {
                var placemnt = _db.Placements.Where(x => x.EmployeeId == paymentRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
                if (placemnt != null)
                {
                    paymentRequest.BranchId = placemnt.BranchId;
                    paymentRequest.BusinessUnitId = placemnt.BusinessUnitId;
                    paymentRequest.OrgStuructureId = placemnt.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            if (!String.IsNullOrWhiteSpace(paymentRequest.SupplierId))
            {
                var supplier =
                    _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == paymentRequest.SupplierId);
                if (supplier != null) paymentRequest.Supplier = supplier.Name;
            }

            #endregion

            if (ModelState.IsValid)
            {
              
                var entity = new Core.Domain.Finance.AP.PaymentRequest.PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    OrgStuructureId=paymentRequest.OrgStuructureId,
                    BranchId = paymentRequest.BranchId,
                    BusinessUnitId = paymentRequest.BusinessUnitId,
                    CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                    PaymentRequestId = paymentRequest.PaymentRequestId,
                    SupplierId = paymentRequest.SupplierId,
                    IsNewSupplier = paymentRequest.IsNewSupplier,
                    Supplier = paymentRequest.Supplier,
                    Purchaser = paymentRequest.Purchaser,
                    ReasonOfPayment = paymentRequest.ReasonOfPayment,
                    RequestDateAm = paymentRequest.RequestDateAm,
                    InvoiceNo = paymentRequest.InvoiceNo,
                    RequestDate = paymentRequest.RequestDate,
                    PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                    PONo = paymentRequest.PONo,
                    GRN = paymentRequest.GRN,
                   
                    Remark = paymentRequest.Remark,
                    IsVoid = paymentRequest.IsVoid,
                    VoidBy = paymentRequest.VoidBy,
                    VoidTime = paymentRequest.VoidTime,
                    IsOnlineApproved = paymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = paymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = paymentRequest.OnlineApprovedTime,
                    IsSendForApproval = paymentRequest.IsSendForApproval,
                    SendForApprovalBy = paymentRequest.SendForApprovalBy,
                    SendForApprovalTime = paymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = paymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = paymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = paymentRequest.OnlineTransferredTime,

                    IsBudgetApproved = paymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = paymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = paymentRequest.BudgetApprovedTime,

                    ForwardedFrom = paymentRequest.ForwardedFrom,
                    ForwardedTo = paymentRequest.ForwardedTo,
                    IsFinalApproval = paymentRequest.IsFinalApproval,
                    CurrentApprovalStep = paymentRequest.CurrentApprovalStep,


                    IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,

                    OrgBranchName = paymentRequest.OrgBranchName,

                    DateCreated = paymentRequest.DateCreated,
                    CreatedBy = paymentRequest.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.PaymentRequests.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP payment request", "Success", User.Identity.Name, $"updated payment request {entity.PaymentRequestId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Finance.AP.PaymentRequest.PaymentRequest paymentRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var splitDate = paymentRequest.RequestDateAm.Split('/');
                DateTime requestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                var entity = new Core.Domain.Finance.AP.PaymentRequest.PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
                    BranchId = paymentRequest.BranchId,
                    BusinessUnitId = paymentRequest.BusinessUnitId,
                    CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                    PaymentRequestId = paymentRequest.PaymentRequestId,
                    SupplierId = paymentRequest.SupplierId,
                    IsNewSupplier = paymentRequest.IsNewSupplier,
                    Supplier = paymentRequest.Supplier,
                    Purchaser = paymentRequest.Purchaser,
                    ReasonOfPayment = paymentRequest.ReasonOfPayment,
                    RequestDateAm = paymentRequest.RequestDateAm,
                    InvoiceNo = paymentRequest.InvoiceNo,
                    RequestDate = requestDate,
                    PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                    PONo = paymentRequest.PONo,
                    GRN = paymentRequest.GRN,
                    Remark = paymentRequest.Remark
                };
               
                _db.PaymentRequests.Attach(entity);
                _db.PaymentRequests.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment request number " + paymentRequest.PaymentRequestId + " " + paymentRequest.Remark, Modules.Finance);
                #endregion
            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.PaymentRequests.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {               

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.PaymentRequests.Attach(record);
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

            else if (record.PaymentRequestLines.Count == 0)
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

       

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.PaymentRequests.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                #region validate invoice

                var payments = _db.ApDirectPayments
                    .Where(x => x.SourceId == "PAYMENT-REQUEST_"+record.PaymentRequestId.ToString())
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The payment request {record.PaymentRequestId} is already applied in payment \n hence cannot be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.PaymentRequests.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payment request", "Success", User.Identity.Name, $"void payment request {id} ", Modules.Finance);
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
