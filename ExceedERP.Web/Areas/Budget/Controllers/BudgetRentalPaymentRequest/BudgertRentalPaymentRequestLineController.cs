using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Budget.Controllers.BudgetRentalPaymentRequest
{
    public class BudgertRentalPaymentRequestLineController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();      
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RentalPaymentRequestLine_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var equipmentNameforPlateNumber = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id).FirstOrDefault();
            var fleetPlateNumber = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameforPlateNumber.EquipmentNameId).Select(x=>x.PlateNumber).FirstOrDefault();
            var rentalPaymentRequestLines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id).ToList();
            DataSourceResult result = rentalPaymentRequestLines.ToDataSourceResult(request, rentalPaymentRequestLine => new
            {
                RentalPaymentRequestLineId = rentalPaymentRequestLine.RentalPaymentRequestLineId,
                RentalPaymentRequestId = rentalPaymentRequestLine.RentalPaymentRequestId,
                EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId,
                EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId,
                Amount = rentalPaymentRequestLine.Amount,
                plateNumber = fleetPlateNumber,
                TaxAmount = rentalPaymentRequestLine.TaxAmount,
                SubTotalAmount = rentalPaymentRequestLine.SubTotalAmount,
                FuelAmount = rentalPaymentRequestLine.FuelAmount,
                Commission = rentalPaymentRequestLine.Commission,
                AdvancedPayment = rentalPaymentRequestLine.AdvancedPayment,
                TotalAmount = rentalPaymentRequestLine.TotalAmount,
            });
            return Json(result);
        }
        public ActionResult RentalPaymentRequestDepartmentValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RentalPaymentRequestDepartmentValidation> rentalPaymentRequestDepartmentValidations = db.RentalPaymentRequestDepartmentValidations.Where(x => x.RentalPaymentRequestId == id);
            DataSourceResult result = rentalPaymentRequestDepartmentValidations.ToDataSourceResult(request, rentalPaymentRequestDepartmentValidation => new {
                RentalPaymentRequestDepartmentValidationId = rentalPaymentRequestDepartmentValidation.RentalPaymentRequestDepartmentValidationId,
                PaymentRequestId = id,
                Status = rentalPaymentRequestDepartmentValidation.Status,
                Remark = rentalPaymentRequestDepartmentValidation.Remark,
                OrgBranchName = rentalPaymentRequestDepartmentValidation.OrgBranchName,
                DateCreated = rentalPaymentRequestDepartmentValidation.DateCreated,
                LastModified = rentalPaymentRequestDepartmentValidation.LastModified,
                CreatedBy = rentalPaymentRequestDepartmentValidation.CreatedBy,
                ModifiedBy = rentalPaymentRequestDepartmentValidation.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult RentalPaymentRequestLine_Create([DataSourceRequest] DataSourceRequest request, RentalPaymentRequestLine rentalPaymentRequestLine, int id)
        {
            var tax = db.GLTaxRates.Find(rentalPaymentRequestLine.GLTaxRateID);
            if(tax == null)
            {
                ModelState.AddModelError(string.Empty, "No Tax Rate was Found");
                return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
            }
            rentalPaymentRequestLine.TaxAmount = rentalPaymentRequestLine.Amount * tax.CalculatedRate;
            rentalPaymentRequestLine.SubTotalAmount = rentalPaymentRequestLine.Amount + rentalPaymentRequestLine.TaxAmount;
            rentalPaymentRequestLine.TotalAmount = rentalPaymentRequestLine.SubTotalAmount + rentalPaymentRequestLine.FuelAmount + rentalPaymentRequestLine.Commission + rentalPaymentRequestLine.AdvancedPayment;
            var rentalPaymentRequest = db.RentalPaymentRequests.Where(x => x.RentalPaymentRequestId == id).FirstOrDefault();
            if (ModelState.IsValid)
            {              
                var entity = new RentalPaymentRequestLine
                {
                    RentalPaymentRequestLineId = rentalPaymentRequestLine.RentalPaymentRequestLineId,
                    RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                    EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId,
                    EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId,
                    Amount = rentalPaymentRequestLine.Amount,
                    TaxAmount = rentalPaymentRequestLine.TaxAmount,
                    SubTotalAmount = rentalPaymentRequestLine.SubTotalAmount,
                    FuelAmount = rentalPaymentRequestLine.FuelAmount,
                    Commission = rentalPaymentRequestLine.Commission,
                    AdvancedPayment = rentalPaymentRequestLine.AdvancedPayment,
                    TotalAmount = rentalPaymentRequestLine.TotalAmount
                };
                db.RentalPaymentRequestLines.Add(entity);
                db.SaveChanges();
                rentalPaymentRequestLine.RentalPaymentRequestLineId = entity.RentalPaymentRequestLineId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request line registration", "Success", User.Identity.Name, $"created rental payment request number {entity.RentalPaymentRequestLineId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequestDepartmentValidations_Create([DataSourceRequest]DataSourceRequest request, RentalPaymentRequestDepartmentValidation rentalPaymentRequestDepartmentValidation, int id)
        {
            var record = db.RentalPaymentRequests.Find(id);
            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved", "cannot manipulate this record,it is already approved");

            var lines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = db.RentalPaymentRequestDepartmentValidations
                    .Where(x => x.RentalPaymentRequestId == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (rentalPaymentRequestDepartmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsDepartmentOnlineApproved = true;
                            record.DepartmentOnlineApprovedBy = User.Identity.Name;
                            record.DepartmentOnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                            //#region approval notification

                            //NotifModels notif = new NotifModels
                            //{
                            //    UserId = record.ForwardedFrom,
                            //    URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.PaymentRequestId),
                            //    Message =
                            //        $"Payment request document {record.PaymentRequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            //};

                            //Notify.NotifyUser(notif);

                            //#endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsDepartmentOnlineApproved = true;
                            record.IsVoid = false;
                            record.DepartmentOnlineApprovedBy = User.Identity.Name;
                            record.DepartmentOnlineApprovedTime = DateTime.Now;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            //#region approval notification

                            //NotifModels notif = new NotifModels
                            //{
                            //    UserId = record.ForwardedFrom,
                            //    URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.PaymentRequestId),
                            //    Message =
                            //        $"Payment request document {record.PaymentRequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            //};

                            //Notify.NotifyUser(notif);

                            //#endregion

                            //#region forward to next approver

                            //decimal docamount = record.PaymentRequestLines.Where(x => x.PaymentRequestId == record.PaymentRequestId).Sum(x => x.TotalAmount);
                            //DocApprovalWorkflowHelper.ForwardToApprover(record.PaymentRequestId, docamount, "",
                            //    record.ForwardedTo, ApprovalDocumentType.ApPaymentRequestSelfCare, false,(int)record.CurrentApprovalStep);

                            //#endregion
                        }
                    }

                    if (rentalPaymentRequestDepartmentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new RentalPaymentRequestDepartmentValidation
                    {
                        RentalPaymentRequestId = id,
                        Status = rentalPaymentRequestDepartmentValidation.Status,
                        Remark = rentalPaymentRequestDepartmentValidation.Remark,
                        OrgBranchName = rentalPaymentRequestDepartmentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = rentalPaymentRequestDepartmentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = rentalPaymentRequestDepartmentValidation.ModifiedBy
                    };

                    db.RentalPaymentRequestDepartmentValidations.Add(entity);
                    db.SaveChanges();
                    rentalPaymentRequestDepartmentValidation.RentalPaymentRequestDepartmentValidationId = entity.RentalPaymentRequestDepartmentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request department approval", "Success", User.Identity.Name, $"approved request {entity.RentalPaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { rentalPaymentRequestDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequestLine_Update([DataSourceRequest] DataSourceRequest request, RentalPaymentRequestLine rentalPaymentRequestLine)
        {
          if (ModelState.IsValid)
            {
                var tax = db.GLTaxRates.Find(rentalPaymentRequestLine.GLTaxRateID);
                rentalPaymentRequestLine.TaxAmount = rentalPaymentRequestLine.Amount * tax.CalculatedRate;
                rentalPaymentRequestLine.SubTotalAmount = rentalPaymentRequestLine.Amount + rentalPaymentRequestLine.TaxAmount;
                rentalPaymentRequestLine.TotalAmount = rentalPaymentRequestLine.SubTotalAmount + rentalPaymentRequestLine.FuelAmount + 
                rentalPaymentRequestLine.Commission + rentalPaymentRequestLine.AdvancedPayment;
                var entity = new RentalPaymentRequestLine
                {
                    RentalPaymentRequestLineId = rentalPaymentRequestLine.RentalPaymentRequestLineId,
                    RentalPaymentRequestId = rentalPaymentRequestLine.RentalPaymentRequestId,
                    EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId,
                    EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId,
                    Amount = rentalPaymentRequestLine.Amount,
                    TaxAmount = rentalPaymentRequestLine.TaxAmount,
                    SubTotalAmount = rentalPaymentRequestLine.SubTotalAmount,
                    FuelAmount = rentalPaymentRequestLine.FuelAmount,
                    Commission = rentalPaymentRequestLine.Commission,
                    AdvancedPayment = rentalPaymentRequestLine.AdvancedPayment,
                    TotalAmount = rentalPaymentRequestLine.TotalAmount
                };
                db.RentalPaymentRequestLines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request line update", "Success", User.Identity.Name, $"updated rental payment request line {entity.RentalPaymentRequestLineId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalPaymentRequestLine_Destroy([DataSourceRequest] DataSourceRequest request, RentalPaymentRequestLine rentalPaymentRequestLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalPaymentRequestLine
                {
                    RentalPaymentRequestLineId = rentalPaymentRequestLine.RentalPaymentRequestLineId,
                    RentalPaymentRequestId = rentalPaymentRequestLine.RentalPaymentRequestId,
                    EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId,
                    EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId,
                    Amount = rentalPaymentRequestLine.Amount,
                    TaxAmount = rentalPaymentRequestLine.TaxAmount,
                    SubTotalAmount = rentalPaymentRequestLine.SubTotalAmount,
                    FuelAmount = rentalPaymentRequestLine.FuelAmount,
                    Commission = rentalPaymentRequestLine.Commission,
                    AdvancedPayment = rentalPaymentRequestLine.AdvancedPayment,
                    TotalAmount = rentalPaymentRequestLine.TotalAmount
                };
                db.RentalPaymentRequestLines.Attach(entity);
                db.RentalPaymentRequestLines.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rental inspection request {entity.RentalPaymentRequestLineId }");
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}