using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PaymentSettlement
{
    [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser, ProcurementRoles.ProcurementPaymentApprover)]
    public class PaymentSettlementRequestController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
       

      
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

       
        public ActionResult ApPaymentSettlements_Read([DataSourceRequest]DataSourceRequest request,bool isNotTransffered)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<ApPaymentSettlement> ApPaymentSettlements = _db.ApPaymentSettlements
               .Where(x => mybraches.Contains(x.CreatedAtBranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                ApPaymentSettlements = ApPaymentSettlements.Where(x => myBusinessUnits.Contains(x.CreatedAtBusinessUnitId));
            }

            
                DataSourceResult result = ApPaymentSettlements.ToDataSourceResult(request, apPaymentsettlement => new {
                ApPaymentSettlementId = apPaymentsettlement.ApPaymentSettlementId,
                    BranchId = apPaymentsettlement.BranchId,
                    BusinessUnitId = apPaymentsettlement.BusinessUnitId,
                    OrgStuructureId = apPaymentsettlement.OrgStuructureId,
                    CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = apPaymentsettlement.CreatedAtOrgStructureId,
                    SupplierId = apPaymentsettlement.SupplierId,
                CollectedByPurchaser = apPaymentsettlement.CollectedByPurchaser,
                EmployeeId = apPaymentsettlement.EmployeeId,
                Reference = apPaymentsettlement.Reference,
               
                PaymentDate = apPaymentsettlement.PaymentDate,
                
                Remark = apPaymentsettlement.Remark,
                IsSendForApproval = apPaymentsettlement.IsSendForApproval,
                    SendForApprovalBy = apPaymentsettlement.SendForApprovalBy,
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

                    ForwardedFrom = apPaymentsettlement.ForwardedFrom,
                    ForwardedTo = apPaymentsettlement.ForwardedTo,
                    IsFinalApproval = apPaymentsettlement.IsFinalApproval,
                    CurrentApprovalStep = apPaymentsettlement.CurrentApprovalStep,

                    DateCreated = apPaymentsettlement.DateCreated,
                LastModified = apPaymentsettlement.LastModified,
                CreatedBy = apPaymentsettlement.CreatedBy,
                ModifiedBy = apPaymentsettlement.ModifiedBy
            });

            return Json(result);
        }


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
            apPaymentsettlement.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
            if (apPaymentsettlement.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apPaymentsettlement.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apPaymentsettlement.BranchId = placement.BranchId;
                    apPaymentsettlement.BusinessUnitId = placement.BusinessUnitId;
                    apPaymentsettlement.OrgStuructureId = placement.OrgStructureId;
                }
                else
                {
                    apPaymentsettlement.BranchId = apPaymentsettlement.CreatedAtBranchId;
                    apPaymentsettlement.BusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId;
                    apPaymentsettlement.OrgStuructureId = apPaymentsettlement.CreatedAtOrgStructureId;
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlement
                {
                    BranchId = apPaymentsettlement.BranchId,
                    BusinessUnitId = apPaymentsettlement.BusinessUnitId,
                    OrgStuructureId = apPaymentsettlement.OrgStuructureId,
                    CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = apPaymentsettlement.CreatedAtOrgStructureId,
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

                    ForwardedFrom = apPaymentsettlement.ForwardedFrom,
                    ForwardedTo = apPaymentsettlement.ForwardedTo,
                    IsFinalApproval = apPaymentsettlement.IsFinalApproval,
                    CurrentApprovalStep = apPaymentsettlement.CurrentApprovalStep,

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
            if (apPaymentsettlement.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apPaymentsettlement.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apPaymentsettlement.BranchId = placement.BranchId;
                    apPaymentsettlement.BusinessUnitId = placement.BusinessUnitId;
                    apPaymentsettlement.OrgStuructureId = placement.OrgStructureId;
                }
                else
                {
                    apPaymentsettlement.BranchId = apPaymentsettlement.CreatedAtBranchId;
                    apPaymentsettlement.BusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId;
                    apPaymentsettlement.OrgStuructureId = apPaymentsettlement.CreatedAtOrgStructureId;
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
                        OrgStuructureId = apPaymentsettlement.OrgStuructureId,
                        CreatedAtBranchId = apPaymentsettlement.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = apPaymentsettlement.CreatedAtBusinessUnitId,
                        CreatedAtOrgStructureId = apPaymentsettlement.CreatedAtOrgStructureId,
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

                        ForwardedFrom = apPaymentsettlement.ForwardedFrom,
                        ForwardedTo = apPaymentsettlement.ForwardedTo,
                        IsFinalApproval = apPaymentsettlement.IsFinalApproval,
                        CurrentApprovalStep = apPaymentsettlement.CurrentApprovalStep,

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlements_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentSettlement apPaymentSettlements)
        {

            

            if (ModelState.IsValid)
            {
                    var entity = new ApPaymentSettlement
                    {
                        ApPaymentSettlementId = apPaymentSettlements.ApPaymentSettlementId,
                        BranchId = apPaymentSettlements.BranchId,
                        BusinessUnitId = apPaymentSettlements.BusinessUnitId,
                        OrgStuructureId = apPaymentSettlements.OrgStuructureId,
                        CreatedAtBranchId = apPaymentSettlements.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = apPaymentSettlements.CreatedAtBusinessUnitId,
                        CreatedAtOrgStructureId = apPaymentSettlements.CreatedAtOrgStructureId,
                        SupplierId = apPaymentSettlements.SupplierId,
                        CollectedByPurchaser = apPaymentSettlements.CollectedByPurchaser,
                        EmployeeId = apPaymentSettlements.EmployeeId,
                        Reference = apPaymentSettlements.Reference,
                       
                        PaymentDate = apPaymentSettlements.PaymentDate,
                        
                        Remark = apPaymentSettlements.Remark,
                        IsSendForApproval = apPaymentSettlements.IsSendForApproval,
                        IsOnlineTransferred = apPaymentSettlements.IsOnlineTransferred,
                        IsOnlineTransferredBy = apPaymentSettlements.IsOnlineTransferredBy,
                        OnlineTransferredTime = apPaymentSettlements.OnlineTransferredTime,
                        Status = apPaymentSettlements.Status,
                        IsOnlineApproved = apPaymentSettlements.IsOnlineApproved,
                        OnlineApprovedBy = apPaymentSettlements.OnlineApprovedBy,
                        OnlineApprovedTime = apPaymentSettlements.OnlineApprovedTime,
                        IsVoid = apPaymentSettlements.IsVoid,
                        VoidBy = apPaymentSettlements.VoidBy,
                        VoidTime = apPaymentSettlements.VoidTime,

                        ForwardedFrom = apPaymentSettlements.ForwardedFrom,
                        ForwardedTo = apPaymentSettlements.ForwardedTo,
                        IsFinalApproval = apPaymentSettlements.IsFinalApproval,
                        CurrentApprovalStep = apPaymentSettlements.CurrentApprovalStep,

                        DateCreated = apPaymentSettlements.DateCreated,
                        LastModified = apPaymentSettlements.LastModified,
                        CreatedBy = apPaymentSettlements.CreatedBy,
                        ModifiedBy = apPaymentSettlements.ModifiedBy
                    };

                    _db.ApPaymentSettlements.Attach(entity);
                    _db.ApPaymentSettlements.Remove(entity);
                    _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment settlement number " + apPaymentSettlements.ApPaymentSettlementId + " " + apPaymentSettlements.Reference, Modules.Finance);
                #endregion
            }
            return Json(new[] { apPaymentSettlements }.ToDataSourceResult(request, ModelState));
        }
               

        public JsonResult SendforDepartmentApproval(int id = 0)
        {
            var  record = _db.ApPaymentSettlements.Find(id);
            var grns=_db.ApPaymentSettlementGrns.Where(x=>x.ApPaymentSettlementId==id).ToList(); 
            var payments=_db.ApPaymentSettlementLines.Where(x=>x.ApPaymentSettlementId==id).ToList(); 
            string errMsg = "";
        

            if (!grns.Any() || !payments.Any())
            {
                errMsg = "Please add payment and GRN";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is approved already";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }         


            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {
                record.Status = OperationStatuses.SendForDepartmentApproval.ToString();
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

        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
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

            if (record.IsDepartmentOnlineApproved && !record.IsVoid && !record.IsOnlineApproved )
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsDepartmentOnlineApproved = false;
                record.IsDepartmentSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApPaymentSettlements.Attach(record);
                _db.Entry(record).State = EntityState.Modified;

                var approvals = _db.ApPaymentSettlementDepartmentValidations
                  .Where(x => x.ApPaymentSettlementId == id).ToList();

                //remove to allow approval again
                _db.ApPaymentSettlementDepartmentValidations.RemoveRange(approvals);
                _db.SaveChanges();

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
                UserHelper.OperationLog(Request.UserHostAddress, "payment settlement department", "Success", User.Identity.Name, $"void payment settlement {id} ", Modules.Procurement);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
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
            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is approve by Finance.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsDepartmentOnlineApproved)
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
