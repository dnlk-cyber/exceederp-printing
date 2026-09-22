using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Fleet;
using static ExceedERP.Helpers.Fleet.FleetRentHelper;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class RentalPaymentRequestLineController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RentalPaymentRequestLine_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var rentalPaymentRequestLines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id);
            var result = rentalPaymentRequestLines.ToDataSourceResult(request, model => new RentalPaymentRequestLine
            {
                FleetId = model.FleetId,
                Amount = model.Amount,
                SubTotalAmount = model.SubTotalAmount,
                TaxAmount = model.TaxAmount,
                FuelAmount = model.FuelAmount,
                Commission = model.Commission,
                AdvancedPayment = model.AdvancedPayment,
                TotalAmount = model.TotalAmount,
                VAT = model.VAT,
                SparePartCost = model.SparePartCost,
                OilLubCost = model.OilLubCost,
                MaintenanceCost = model.MaintenanceCost,
                FuelCost = model.FuelCost,
                GrossBeforeVat = model.GrossBeforeVat,
                WithholdingAmount = model.WithholdingAmount,
                CostSheetSummaryId = model.CostSheetSummaryId,
                GLTaxRateID = model.GLTaxRateID,
                EquipmentNameId = model.EquipmentNameId,
                EquipmnetTypeId = model.EquipmnetTypeId,
                RentalPaymentRequestId = model.RentalPaymentRequestId,
                TotalCost = model.TotalCost,
                EquipmentCategoryId = model.EquipmentCategoryId,
                RentalPaymentRequestLineId = model.RentalPaymentRequestLineId
            });
            return Json(result);
        }
        public ActionResult RentalPaymentRequestDepartmentValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RentalPaymentRequestDepartmentValidation> rentalPaymentRequestDepartmentValidations = db.RentalPaymentRequestDepartmentValidations.Where(x => x.RentalPaymentRequestId == id);
            DataSourceResult result = rentalPaymentRequestDepartmentValidations.ToDataSourceResult(request, rentalPaymentRequestDepartmentValidation => new
            {
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
            var rentalPaymentRequest = db.RentalPaymentRequests.Find(id);        
            if (ModelState.IsValid)
            {
                var procurementSupplier = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == rentalPaymentRequest.ProcurementSupplierID).Name;
                var paymentSummary = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.TimeSheetIsDGMApproved && x.CustomerName == procurementSupplier && x.BranchId == rentalPaymentRequest.BranchId && x.GlPeriodId == rentalPaymentRequest.GLPeriodId && x.FleetId== rentalPaymentRequestLine.FleetId.ToString());
                var rentalPaymentLine = new RentalPaymentRequestLine();
                rentalPaymentLine.WithholdingAmount = paymentSummary.GrossBeforeVat * rentalPaymentRequest.WithHoldingTax;
                rentalPaymentLine.TotalAmount = paymentSummary.TotalWithVat;
                rentalPaymentLine.AdvancedPayment= paymentSummary.AdvancePayedWithVat;
                rentalPaymentLine.Amount = paymentSummary.NetPayable-rentalPaymentLine.WithholdingAmount;
                rentalPaymentLine.CostSheetSummaryId = paymentSummary.RentedEquipmentMonthlyPaymentSummaryId;
                rentalPaymentLine.EquipmentCategoryId = rentalPaymentRequestLine.EquipmentCategoryId;
                rentalPaymentLine.EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId;
                rentalPaymentLine.EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId;
                rentalPaymentLine.FleetId = rentalPaymentRequestLine.FleetId;
                rentalPaymentLine.RentalPaymentRequestId =id;
                rentalPaymentLine.GrossBeforeVat = paymentSummary.GrossBeforeVat;
                rentalPaymentLine.VAT = paymentSummary.VAT;
                rentalPaymentLine.SparePartCost = paymentSummary.SparePartCost;
                rentalPaymentLine.MaintenanceCost = paymentSummary.MaintenanceCost;
                rentalPaymentLine.OilLubCost = paymentSummary.OilLubCost;
                rentalPaymentLine.FuelCost = paymentSummary.FuelCost;
                rentalPaymentLine.SubTotalAmount = paymentSummary.NetPayable-rentalPaymentLine.WithholdingAmount;
                rentalPaymentLine.TotalCost = paymentSummary.Deductable;
                db.RentalPaymentRequestLines.Add(rentalPaymentLine);
                db.SaveChanges();
                
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request line registration", "Success", User.Identity.Name, $"created rental payment request number {rentalPaymentRequestLine.RentalPaymentRequestLineId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequestLine_Update([DataSourceRequest] DataSourceRequest request, RentalPaymentRequestLine rentalPaymentRequestLine)
        {
            var rentalPaymentRequest = db.RentalPaymentRequests.Find(rentalPaymentRequestLine.RentalPaymentRequestId);
            var procurementSupplier = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == rentalPaymentRequest.ProcurementSupplierID).Name;
            var paymentSummary = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.TimeSheetIsDGMApproved && x.CustomerName == procurementSupplier && x.BranchId == rentalPaymentRequest.BranchId && x.GlPeriodId == rentalPaymentRequest.GLPeriodId && x.FleetId == rentalPaymentRequestLine.FleetId.ToString());
            var rentalPaymentLine = db.RentalPaymentRequestLines.FirstOrDefault(x => x.RentalPaymentRequestLineId == rentalPaymentRequestLine.RentalPaymentRequestLineId);
            rentalPaymentLine.WithholdingAmount = paymentSummary.GrossBeforeVat * rentalPaymentRequest.WithHoldingTax;
            rentalPaymentLine.TotalAmount = paymentSummary.TotalWithVat;
            rentalPaymentLine.AdvancedPayment = paymentSummary.AdvancePayedWithVat;
            rentalPaymentLine.Amount = paymentSummary.NetPayable-rentalPaymentLine.WithholdingAmount;
            rentalPaymentLine.CostSheetSummaryId = paymentSummary.RentedEquipmentMonthlyPaymentSummaryId;
            rentalPaymentLine.EquipmentCategoryId = rentalPaymentRequestLine.EquipmentCategoryId;
            rentalPaymentLine.EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId;
            rentalPaymentLine.EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId;
            rentalPaymentLine.FleetId = rentalPaymentRequestLine.FleetId;
            rentalPaymentLine.GrossBeforeVat = paymentSummary.GrossBeforeVat;
            rentalPaymentLine.VAT = paymentSummary.VAT;
            rentalPaymentLine.SparePartCost = paymentSummary.SparePartCost;
            rentalPaymentLine.MaintenanceCost = paymentSummary.MaintenanceCost;
            rentalPaymentLine.OilLubCost = paymentSummary.OilLubCost;
            rentalPaymentLine.FuelCost = paymentSummary.FuelCost;
            rentalPaymentLine.SubTotalAmount = paymentSummary.NetPayable - rentalPaymentLine.WithholdingAmount;
            rentalPaymentLine.TotalCost = paymentSummary.Deductable;
            db.Entry(rentalPaymentLine).State = EntityState.Modified;
            db.SaveChanges();
            if (ModelState.IsValid)
            {

                GenerateRentalPaymentRequestDetail(rentalPaymentRequestLine.RentalPaymentRequestId, rentalPaymentRequestLine);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request line registration", "Success", User.Identity.Name, $"created rental payment request number {rentalPaymentRequestLine.RentalPaymentRequestLineId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalPaymentRequestLine_Destroy([DataSourceRequest] DataSourceRequest request, RentalPaymentRequestLine rentalPaymentRequestLine)
        {
            if (ModelState.IsValid)
            {
                var entity = db.RentalPaymentRequestLines.FirstOrDefault(x => x.RentalPaymentRequestLineId == rentalPaymentRequestLine.RentalPaymentRequestLineId);
                if (rentalPaymentRequestLine != null)
                {
                    db.RentalPaymentRequestLines.Remove(entity);
                    db.SaveChanges();
                }
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rental inspection request {entity.RentalPaymentRequestLineId }");
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        private RentalValidation CheckNull(int rentId, RentalPaymentRequestLine detail)
        {
            var rentalPaymentRequest = db.RentalPaymentRequests.FirstOrDefault(x => x.RentalPaymentRequestId == rentId);
            var equipmentType = db.EquipmentType.Find(detail.EquipmnetTypeId);
            FleetRentHelper fleetRentHelper = new FleetRentHelper();
            switch (equipmentType.TimeSheetType)
            {
                case TimeSheetType.DryLoading:
                    fleetRentHelper.Dryload(rentalPaymentRequest, detail);
                    break;
                case TimeSheetType.DumpTruck:
                    fleetRentHelper.Dumptruck(rentalPaymentRequest, detail);
                    break;
                case TimeSheetType.LightVehicle:
                    fleetRentHelper.Lightvehicle(rentalPaymentRequest, detail);
                    break;
                case TimeSheetType.Machinery:
                    fleetRentHelper.Machinary(rentalPaymentRequest, detail);
                    break;
                case TimeSheetType.WagonDrill:
                    fleetRentHelper.Wagondrill(rentalPaymentRequest, detail.FleetId);
                    break;

            }
            return RentalValidation.NotFound;
        }
        private void GenerateRentalPaymentRequestDetail(int rentId, RentalPaymentRequestLine detail)
        {
            FleetRentalCostsheetHelper fleetRentalCostsheetHelper = new FleetRentalCostsheetHelper();
            var equipmentType = db.EquipmentType.Find(detail.EquipmnetTypeId);
            switch (equipmentType.TimeSheetType)
            {
                case TimeSheetType.DryLoading:
                    GenerateRentalPayment(detail, fleetRentalCostsheetHelper.DryloadNetAmount(detail, rentId), rentId);
                    break;
                case TimeSheetType.DumpTruck:
                    GenerateRentalPayment(detail, fleetRentalCostsheetHelper.DumptruckNetAmount(detail, rentId), rentId);
                    break;
                case TimeSheetType.LightVehicle:
                    GenerateRentalPayment(detail, fleetRentalCostsheetHelper.LightvehicleNetAmount(detail, rentId), rentId);
                    break;
                case TimeSheetType.Machinery:
                    GenerateRentalPayment(detail, fleetRentalCostsheetHelper.MachinaryNetAmount(detail, rentId), rentId);
                    break;
                case TimeSheetType.WagonDrill:
                    GenerateRentalPayment(detail, fleetRentalCostsheetHelper.WagondrillNetAmount(detail, rentId), rentId);
                    break;
            }
        }

        private void GenerateRentalPayment(RentalPaymentRequestLine detail, decimal netAmount, int rentalPaymentRequestId)
        {
            
            var taxRate = db.GLTaxRates.Find(detail.GLTaxRateID);
            var rentalPaymentLine = db.RentalPaymentRequestLines.FirstOrDefault(x => x.RentalPaymentRequestLineId == detail.RentalPaymentRequestLineId);
            if(rentalPaymentLine != null)
            {
                RentalPaymentRequestLine entityUpdate = new RentalPaymentRequestLine
                {
                    AdvancedPayment = rentalPaymentLine.AdvancedPayment,
                    Amount = netAmount,
                    EquipmentCategoryId = rentalPaymentLine.EquipmentCategoryId,
                    EquipmentNameId = rentalPaymentLine.EquipmentNameId,
                    EquipmnetTypeId = rentalPaymentLine.EquipmnetTypeId,
                    FleetId = rentalPaymentLine.FleetId,
                    GLTaxRateID = rentalPaymentLine.GLTaxRateID,
                    RentalPaymentRequestId = rentalPaymentRequestId,
                    
                    TotalAmount = 0m
                };
                if (taxRate != null)
                    entityUpdate.SubTotalAmount = netAmount * taxRate.CalculatedRate + netAmount;

                if (taxRate == null)
                    entityUpdate.SubTotalAmount = netAmount;


                entityUpdate.TotalAmount = entityUpdate.SubTotalAmount - entityUpdate.AdvancedPayment;
                db.Entry(entityUpdate).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                RentalPaymentRequestLine entity = new RentalPaymentRequestLine
                {
                    AdvancedPayment = detail.AdvancedPayment,
                    Amount = netAmount,
                    EquipmentCategoryId = detail.EquipmentCategoryId,
                    EquipmentNameId = detail.EquipmentNameId,
                    EquipmnetTypeId = detail.EquipmnetTypeId,
                    FleetId = detail.FleetId,
                    GLTaxRateID = detail.GLTaxRateID,
                    RentalPaymentRequestId = rentalPaymentRequestId,
                    TotalAmount = 0m
                };
                if (taxRate != null)
                    entity.SubTotalAmount = netAmount * taxRate.CalculatedRate + netAmount;

                if (taxRate == null)
                    entity.SubTotalAmount = netAmount;
                entity.TotalAmount = entity.SubTotalAmount - entity.AdvancedPayment;

                db.RentalPaymentRequestLines.Add(entity);
                db.SaveChanges();
            }
            
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
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}