using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class PaymentRequestController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        HRMInputs _inputs = new HRMInputs();
        public static bool IsForOther { get; set; }
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
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }
        public ActionResult SendForApproval()
        {
            #region validation 

            var requestItems = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == HRMInputs.RequestId).ToList();
            if (!requestItems.Any())
            {
                ModelState.AddModelError("noitem", "Please add at least one line item under your request.");
                return null;
            }

            var status =_db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            if (status.Any())
            {
                foreach (var s in status)
                {
                    var messages = "";
                    if (s.Status == RequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        return null;
                    }

                }
            }

                #endregion

            _db = new ExceedDbContext();
            var requestesStatus = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == HRMInputs.RequestId &&
                    x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
            _db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                _db.RequestesStatus.Add(entityy);
                _db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                    Remark = requestesStatus.Remark
                };

                _db.RequestesStatus.Attach(entity);
                _db.RequestesStatus.Remove(entity);
                _db.SaveChanges();
                _db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                _db.RequestesStatus.Add(entityy);
                _db.SaveChanges();
            }

          
            return RedirectToAction("Index");
        }
        public ActionResult PaymentRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            IQueryable<PaymentRequest> paymentRequests = _db.PaymentRequests;
                                               // .Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = paymentRequests.ToDataSourceResult(request, paymentRequest => new
            {
                EmployeeId = paymentRequest.EmployeeId,
                OrgStuructureId = paymentRequest.OrgStuructureId,
                BranchId = paymentRequest.BranchId,
                PaymentRequestId = paymentRequest.PaymentRequestId,
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

                IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,

                IsOnlineApproved =paymentRequest.IsOnlineApproved,
                OnlineApprovedBy =paymentRequest.OnlineApprovedBy,
                OnlineApprovedTime =paymentRequest.OnlineApprovedTime,
                IsSendForApproval =paymentRequest.IsSendForApproval,
                SendForApprovalBy =paymentRequest.SendForApprovalBy,
                SendForApprovalTime =paymentRequest.SendForApprovalTime,
                IsOnlineTransferred =paymentRequest.IsOnlineTransferred,
                IsOnlineTransferredBy =paymentRequest.IsOnlineTransferredBy,
                OnlineTransferredTime =paymentRequest.OnlineTransferredTime,
                OrgBranchName = paymentRequest.OrgBranchName,

                DateCreated =paymentRequest.DateCreated,
                CreatedBy =paymentRequest.CreatedBy,
                LastModified =paymentRequest.LastModified,
                ModifiedBy =paymentRequest.ModifiedBy,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Create([DataSourceRequest]DataSourceRequest request, PaymentRequest paymentRequest)
        {
           // ModelState.Clear();

            #region data

            if (!String.IsNullOrWhiteSpace(paymentRequest.SupplierId))
            {
                var supplier =
                    _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == paymentRequest.SupplierId);
                if (supplier != null) paymentRequest.Supplier = supplier.Name;
            }

            #endregion

            if (ModelState.IsValid)
            {
                //var splitDate = paymentRequest.RequestDateAm.Split('/');
               // DateTime requestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                //var employee = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                //var branch = UserInformationHelper.GetEmployeeBranchId(User.Identity.Name);
                //var workunit = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name);

                var entity = new PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    BranchId = paymentRequest.BranchId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
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

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = paymentRequest.LastModified,
                    ModifiedBy = paymentRequest.ModifiedBy,
                };

                _db.PaymentRequests.Add(entity);
                _db.SaveChanges();
                paymentRequest.PaymentRequestId = entity.PaymentRequestId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request", "Success", User.Identity.Name, $"created payment request {entity.PaymentRequestId} ", Modules.Finance);
                #endregion
            }



            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Update([DataSourceRequest]DataSourceRequest request, PaymentRequest paymentRequest)
        {
            ModelState.Clear();

            #region data

            if (!String.IsNullOrWhiteSpace(paymentRequest.SupplierId))
            {
                var supplier =
                    _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == paymentRequest.SupplierId);
                if (supplier != null) paymentRequest.Supplier = supplier.Name;
            }

            #endregion
            if (ModelState.IsValid)
            {
                //var splitDate = paymentRequest.RequestDateAm.Split('/');
               // DateTime requestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                var entity = new PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    OrgStuructureId=paymentRequest.OrgStuructureId,
                    BranchId = paymentRequest.BranchId,
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
                    OrgBranchName = paymentRequest.OrgBranchName,

                    DateCreated = paymentRequest.DateCreated,
                    CreatedBy = paymentRequest.CreatedBy,
                    LastModified =DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.PaymentRequests.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request", "Success", User.Identity.Name, $"updated payment request {entity.PaymentRequestId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Destroy([DataSourceRequest]DataSourceRequest request, PaymentRequest paymentRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var splitDate = paymentRequest.RequestDateAm.Split('/');
                DateTime requestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                var entity = new PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
                    BranchId = paymentRequest.BranchId,
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
                #region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment request of employee code  " + paymentRequest.EmployeeCode);
                #endregion
                _db.PaymentRequests.Attach(entity);
                _db.PaymentRequests.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }
       
        public JsonResult SendforDepartmentApproval(int id = 0)
        {
            var record = _db.PaymentRequests.Find(id);
            string errMsg = "";
           
            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is already approved";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for department approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var lines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id);
            if (!lines.Any())
            {
                errMsg = "Please add line items.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && !record.IsOnlineApproved && !record.IsSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentOnlineApprovedBy = User.Identity.Name;
                record.DepartmentOnlineApprovedTime = DateTime.Now;
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
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
