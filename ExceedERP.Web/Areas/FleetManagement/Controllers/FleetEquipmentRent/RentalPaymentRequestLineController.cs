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
                GLTaxRateID = model.GLTaxRateID,
                EquipmentNameId = model.EquipmentNameId,
                EquipmnetTypeId = model.EquipmnetTypeId,
                RentalPaymentRequestId = model.RentalPaymentRequestId,
                TotalCost = model.TotalCost
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
            var rentalPaymentRequest = db.RentalPaymentRequests.Find(id);
            var durations = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == rentalPaymentRequest.GLPeriodId);
            var result = CheckNull(id);
            if (result == RentalValidation.RentFromExternaNotFound)
            {
                ModelState.AddModelError(string.Empty, "No equipment was found under the supplier name in rent from external");
            }
            if (result == RentalValidation.FleetNotFound)
            {
                ModelState.AddModelError(string.Empty, "no fleet was found under rent from external fleet registration");
            }
            if (result == RentalValidation.DateInvalid)
            {
                ModelState.AddModelError(string.Empty, $"No Timesheet was found between the period {durations.DateFrom.Date} to {durations.DateTo.Date}");
            }
            ModelState.Remove("EquipmnetTypeId");
            ModelState.Remove("EquipmentNameId");
            ModelState.Remove("FleetId");
            var tax = db.GLTaxRates.Find(rentalPaymentRequestLine.GLTaxRateID);
            if (tax == null)
            {
                ModelState.AddModelError(string.Empty, "No Tax Rate was Found");
                return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
            }
            rentalPaymentRequestLine.TaxAmount = rentalPaymentRequestLine.Amount * tax.CalculatedRate;
            //rentalPaymentRequestLine.SubTotalAmount = rentalPaymentRequestLine.Amount + rentalPaymentRequestLine.TaxAmount;
            //rentalPaymentRequestLine.TotalAmount = rentalPaymentRequestLine.SubTotalAmount + rentalPaymentRequestLine.FuelAmount + rentalPaymentRequestLine.Commission + rentalPaymentRequestLine.AdvancedPayment;
            
            if (ModelState.IsValid)
            {
                
                GenerateRentalPaymentRequestDetail(id, rentalPaymentRequestLine.FleetId);
                #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request line registration", "Success", User.Identity.Name, $"created rental payment request number {rentalPaymentRequestLine.RentalPaymentRequestLineId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequestLine }.ToDataSourceResult(request, ModelState));
        }
        private RentalValidation CheckNull(int id)
        {
            var rentalPaymentRequest = db.RentalPaymentRequests.FirstOrDefault(x => x.RentalPaymentRequestId == id);
            var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.ProcurementSupplierID == rentalPaymentRequest.ProcurementSupplierID);
            if (rentFromExternal == null) return RentalValidation.RentFromExternaNotFound;

            var fleet = db.Fleets.FirstOrDefault(x => x.RentId == rentFromExternal.RentFromExternalID);
            if (fleet == null) return RentalValidation.FleetNotFound;

            var machinery = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
            var dumpTruck = db.DumpTruckTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
            var dryLoading = db.DryLoadingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
            var wagonDrill = db.WagonDrillTimesheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
            var lightVehicle = db.LightVehicleTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();


            if (machinery.Any() || dumpTruck.Any() || dryLoading.Any() || wagonDrill.Any() || lightVehicle.Any())
            {
                machinery = machinery
                         .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();
                dumpTruck = dumpTruck
                             .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                dryLoading = dryLoading
                            .Where(x => x.TripStartDate.Date >= rentalPaymentRequest.DurationFrom.Date && x.TripStartDate.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                wagonDrill = wagonDrill
                           .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();
                lightVehicle = lightVehicle
                            .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();
                if (machinery.Any() || dumpTruck.Any() || dryLoading.Any() || wagonDrill.Any() || lightVehicle.Any())
                {
                    return RentalValidation.valid;
                }
                else
                {
                    return RentalValidation.DateInvalid;
                }
            }

            return RentalValidation.NotFound;
        }
        public enum RentalValidation
        {
            valid,
            RentFromExternaNotFound,
            FleetNotFound,
            DateInvalid,
            NotFound
        }
        public void GenerateRentalPaymentRequestDetail(int id, Guid fleetId)
        {
            var rentalPaymentRequest = db.RentalPaymentRequests.FirstOrDefault(X => X.RentalPaymentRequestId == id);
            var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.ProcurementSupplierID == rentalPaymentRequest.ProcurementSupplierID);
            if (rentFromExternal != null)
            {
                var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);

                var machinery = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                var dumpTruck = db.DumpTruckTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                var dryLoading = db.DryLoadingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                var wagonDrill = db.WagonDrillTimesheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                var lightVehicle = db.LightVehicleTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();

                if (machinery.Any())
                {
                    machinery = machinery.Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                    decimal totalAmountBeforeTax = 0;

                    decimal subTotal = 0;
                    decimal fuelCost = 0;
                    decimal commission = 0; // service charge
                    decimal totalCost = 0;

                    foreach (var machineryTimeSheet in machinery)
                    {

                        var operationNetworkinHour = Math.Round((decimal)((machineryTimeSheet.MorningTimeEnd - machineryTimeSheet.MorningTimeStart).TotalHours + (machineryTimeSheet.AfternoonTimeEnd - machineryTimeSheet.AfternoonTimeStart).TotalHours + (machineryTimeSheet.NightTimeEnd - machineryTimeSheet.NightTimeStart).TotalHours), 2);
                        var operationRevenue = operationNetworkinHour * machineryTimeSheet.RentalRate;
                        var idelRevenue = machineryTimeSheet.IdleHours * machineryTimeSheet.IdealRentalRate;
                        totalAmountBeforeTax += idelRevenue + operationRevenue;
                        //subTotal += totalAmountBeforeTax * tax + totalAmountBeforeTax;
                        fuelCost += machineryTimeSheet.FuelConsumption * machineryTimeSheet.FuelRate;
                        commission += machineryTimeSheet.Salary;
                        totalCost = commission + fuelCost;
                        //totalAmount = amount - (fuel + commission);
                    }

                    var rentalPaymentRequestDetail = new RentalPaymentRequestLine
                    {
                        FleetId = fleet.FleetsID,
                        EquipmentNameId = fleet.EquipmentNameID,
                        EquipmnetTypeId = fleet.EquipmentTypeID,
                        Amount = totalAmountBeforeTax,
                        //SubTotalAmount = subTotal,                        
                        FuelAmount = fuelCost,
                        Commission = commission,
                        AdvancedPayment = 0,
                        TotalCost = totalCost,
                        //TotalAmount = subTotal - totalCost,
                        RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId
                    };

                    db.RentalPaymentRequestLines.Add(rentalPaymentRequestDetail);
                    db.SaveChanges();
                }
                else if (dumpTruck.Any())
                {
                    dumpTruck = dumpTruck
                        .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                    decimal totalAmountBeforeTax = 0;

                    decimal subTotal = 0;
                    decimal fuelCost = 0;
                    decimal commission = 0; // service charge
                    decimal totalCost = 0;

                    foreach (var item in dumpTruck)
                    {
                        fuelCost += item.FuelRate * item.FuelConsumption;
                        commission += item.Salary;
                        var capacity = item.Capacity;
                        var actualDistance = item.ActualDistance;
                        var noOfTrips = item.NumberOfTrip;
                        var rentalRate = item.RentalRate;
                        var idleAmount = item.IdealRentalRate * item.IdleHour;
                        var operationAmount = (capacity * actualDistance * noOfTrips * rentalRate);

                        totalAmountBeforeTax += operationAmount + idleAmount;
                        //subTotal += totalAmountBeforeTax * tax + totalAmountBeforeTax;

                    }
                    var rentalPaymentRequestDetail = new RentalPaymentRequestLine
                    {
                        FleetId = fleet.FleetsID,
                        EquipmentNameId = fleet.EquipmentNameID,
                        EquipmnetTypeId = fleet.EquipmentTypeID,
                        Amount = totalAmountBeforeTax,
                        //SubTotalAmount = subTotal,

                        FuelAmount = fuelCost,
                        Commission = commission,
                        AdvancedPayment = 0,
                        TotalCost = totalCost,
                        TotalAmount = subTotal - totalCost,
                        RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId
                    };

                    db.RentalPaymentRequestLines.Add(rentalPaymentRequestDetail);
                    db.SaveChanges();
                }
                else if (dryLoading.Any())
                {
                    dryLoading = dryLoading
                        .Where(x => x.TripStartDate.Date >= rentalPaymentRequest.DurationFrom.Date && x.TripStartDate.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                    decimal totalAmountBeforeTax = 0;

                    decimal subTotal = 0;
                    decimal fuelCost = 0;
                    decimal commission = 0; // service charge
                    decimal totalCost = 0;

                    foreach (var item in dryLoading)
                    {
                        fuelCost += item.FuelRate * item.FuelInLiter;
                        commission += item.Salary;

                        totalAmountBeforeTax += item.NoOfTripe * (item.Asphalt * item.AsphaltRate + item.Gravel * item.GravelRate);
                        //subTotal += totalAmountBeforeTax * tax + totalAmountBeforeTax;
                        totalCost += fuelCost + commission;
                    }
                    var rentalPaymentRequestDetail = new RentalPaymentRequestLine
                    {
                        FleetId = fleet.FleetsID,
                        EquipmentNameId = fleet.EquipmentNameID,
                        EquipmnetTypeId = fleet.EquipmentTypeID,
                        Amount = totalAmountBeforeTax,
                        //SubTotalAmount = subTotal,

                        FuelAmount = fuelCost,
                        Commission = commission,
                        AdvancedPayment = 0,
                        TotalCost = totalCost,
                        TotalAmount = subTotal - totalCost,
                        RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId
                    };

                    db.RentalPaymentRequestLines.Add(rentalPaymentRequestDetail);
                    db.SaveChanges();
                }
                else if (wagonDrill.Any())
                {
                    wagonDrill = wagonDrill
                        .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                    decimal totalAmountBeforeTax = 0;

                    decimal subTotal = 0;
                    decimal fuelCost = 0;
                    decimal commission = 0; // service charge
                    decimal totalCost = 0;

                    foreach (var item in wagonDrill)
                    {
                        fuelCost += item.FuelRate * item.FuelInLiter;
                        commission += item.ServiceCharge;

                        totalAmountBeforeTax += item.TotalAmountBeforeVAT;
                        //subTotal += totalAmountBeforeTax * tax + totalAmountBeforeTax;
                        totalCost += fuelCost + commission;
                    }
                    var rentalPaymentRequestDetail = new RentalPaymentRequestLine
                    {
                        FleetId = fleet.FleetsID,
                        EquipmentNameId = fleet.EquipmentNameID,
                        EquipmnetTypeId = fleet.EquipmentTypeID,
                        Amount = totalAmountBeforeTax,
                        SubTotalAmount = subTotal,

                        FuelAmount = fuelCost,
                        Commission = commission,
                        AdvancedPayment = 0,
                        TotalCost = totalCost,
                        TotalAmount = subTotal - totalCost,
                        RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId
                    };

                    db.RentalPaymentRequestLines.Add(rentalPaymentRequestDetail);
                    db.SaveChanges();
                }
                else if (lightVehicle.Any())
                {
                    lightVehicle = lightVehicle
                        .Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date && x.Date.Date <= rentalPaymentRequest.DurationTo.Date).ToList();

                    decimal totalAmountBeforeTax = 0;

                    decimal subTotal = 0;
                    decimal fuelCost = 0;
                    decimal commission = 0; // service charge
                    decimal totalCost = 0;

                    foreach (var item in lightVehicle)
                    {
                        var operationNetworkinHour = Math.Round((decimal)((item.MorningTimeEnd - item.MorningTimeStart).TotalHours + (item.AfternoonTimeEnd - item.AfternoonTimeStart).TotalHours + (item.NightTimeEnd - item.NightTimeStart).TotalHours), 2);
                        var operationRevenue = operationNetworkinHour * item.RentalRate;
                        var idelRevenue = item.IdleHours * item.IdealRentalRate;
                        totalAmountBeforeTax += idelRevenue + operationRevenue;
                        //subTotal += totalAmountBeforeTax * tax + totalAmountBeforeTax;
                        fuelCost += item.FuelConsumption * item.FuelRate;
                        commission += item.Salary;
                        totalCost = commission + fuelCost;
                        //totalAmount = amount - (fuel + commission);
                    }
                    var rentalPaymentRequestDetail = new RentalPaymentRequestLine
                    {
                        FleetId = fleet.FleetsID,
                        EquipmentNameId = fleet.EquipmentNameID,
                        EquipmnetTypeId = fleet.EquipmentTypeID,
                        Amount = totalAmountBeforeTax,
                        //SubTotalAmount = subTotal,

                        FuelAmount = fuelCost,
                        Commission = commission,
                        AdvancedPayment = 0,
                        TotalCost = totalCost,
                        TotalAmount = subTotal - totalCost,
                        RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId
                    };

                    db.RentalPaymentRequestLines.Add(rentalPaymentRequestDetail);
                    db.SaveChanges();
                }
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequestLine_Update([DataSourceRequest] DataSourceRequest request, RentalPaymentRequestLine rentalPaymentRequestLine)
        {
            var glTaxRate = db.GLTaxRates.Find(rentalPaymentRequestLine.GLTaxRateID);
            if (glTaxRate == null) ModelState.AddModelError(string.Empty, "Tax Rate not found");
            if (ModelState.IsValid)
            {
                
                rentalPaymentRequestLine.SubTotalAmount = rentalPaymentRequestLine.Amount * glTaxRate.CalculatedRate + rentalPaymentRequestLine.Amount;
                rentalPaymentRequestLine.TotalCost = (rentalPaymentRequestLine.Commission + rentalPaymentRequestLine.FuelAmount + rentalPaymentRequestLine.AdvancedPayment);
                rentalPaymentRequestLine.TotalAmount = rentalPaymentRequestLine.SubTotalAmount - rentalPaymentRequestLine.TotalCost;
                rentalPaymentRequestLine.TaxAmount = glTaxRate.CalculatedRate;
                db.RentalPaymentRequestLines.Attach(rentalPaymentRequestLine);
                db.Entry(rentalPaymentRequestLine).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request line update", "Success", User.Identity.Name, $"updated rental payment request line {rentalPaymentRequestLine.RentalPaymentRequestLineId} ", Modules.FleetManagement);
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