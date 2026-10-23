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
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Common.HRM;
using OperationStatuses = ExceedERP.Core.Domain.Inventory.OperationStatuses;

namespace ExceedERP.Web.Areas.Budget.Controllers.RentalPayment
{
    public class RentalPaymentRequestBudgetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["BusinessUnit"] = db.BusinessUnits.Select(s => new RequestBusinessUnitVM
            {
                Value = s.BusinessUnitId,
                Text = s.Name
            }).ToList();
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new RequestIdentityVM
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();
            ViewData["Type"] = db.EquipmentType.Select(s => new RequestTypeVM
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new RequestNameVM
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new RequestFleetsVM
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).ToList();

            ViewData["Employee"] = db.Person_Employee
                                        .Select(x => new RequestEmployeeVM
                                        {
                                            EmployeeId= x.EmployeeId,
                                            FullName=x.EmployeeCode+"-"+x.FirstNameEnglish+" "+x.MiddleNameEnglish +" "+x.LastNameEnglish
                                        }).ToList();
            ViewData["Branch"] = db.Branch.Select(x => new RequestBranchVM{
                BranchId=x.BranchId,
                Name=x.Name }).ToList();
            ViewData["Department"] = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, x.Name }).ToList();
            //ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new RequestProcurementVM{
                ProcurementSupplierID=x.ProcurementSupplierID,
                Name=x.Name }).ToList();
            ViewData["GLPeriod"] = db.GLPeriods.Select(x => new RequestGlPeriodVM{
                GLPeriodId=x.GLPeriodId,
                Name=x.Name }).ToList();
            ViewData["GlFiscalYear"] = db.GLFiscalYears.Select(x => new RequestFiscalYearVM {
                GLFiscalYearId =x.GlFiscalYearId,
                Name=x.Name }).ToList();

            
            ViewData["Taxes"] = db.GLTaxRates.Select(s => new
            {
                Value = s.GLTaxRateID,
                Text = s.TaxName 
            }).ToList();
            return View();
        }
      
        public ActionResult RentalPaymentRequests_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalPaymentRequests = db.RentalPaymentRequests.Where(x=>x.IsBudgetApproved==false&&x.IsDepartmentOnlineApproved==true&&x.IsVoid==false);
            DataSourceResult result = rentalPaymentRequests.ToDataSourceResult(request, rentalPaymentRequest => new RentalPaymentRequest
            {
                RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                EmployeeId = rentalPaymentRequest.EmployeeId,
                BranchId = rentalPaymentRequest.BranchId,
                ReasonofPayment = rentalPaymentRequest.ReasonofPayment,
                RequestDate = rentalPaymentRequest.RequestDate,
                Refference = rentalPaymentRequest.Refference,
                Remark = rentalPaymentRequest.Remark,
                GLPeriodId = rentalPaymentRequest.GLPeriodId,
                GLFiscalYearId = rentalPaymentRequest.GLFiscalYearId,
                DurationFrom = rentalPaymentRequest.DurationFrom,
                DurationTo = rentalPaymentRequest.DurationTo,
                OrgStructureId = rentalPaymentRequest.OrgStructureId,
                ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID,
                Status = rentalPaymentRequest.Status,
                IsBudgetApproved = rentalPaymentRequest.IsBudgetApproved,
                BudgetApprovedBy = rentalPaymentRequest.BudgetApprovedBy,
                BudgetApprovedTime = rentalPaymentRequest.BudgetApprovedTime,
                IsDepartmentOnlineApproved = rentalPaymentRequest.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = rentalPaymentRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = rentalPaymentRequest.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = rentalPaymentRequest.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = rentalPaymentRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = rentalPaymentRequest.DepartmentSendForApprovalTime,
                BusinessUnitId = rentalPaymentRequest.BusinessUnitId,
                ForwardedFrom = rentalPaymentRequest.ForwardedFrom,
                ForwardedTo = rentalPaymentRequest.ForwardedTo,
                IsFinalApproval = rentalPaymentRequest.IsFinalApproval,
                CurrentApprovalStep = rentalPaymentRequest.CurrentApprovalStep,
                IsOnlineApproved = rentalPaymentRequest.IsOnlineApproved,
                OnlineApprovedBy = rentalPaymentRequest.OnlineApprovedBy,
                OnlineApprovedTime = rentalPaymentRequest.OnlineApprovedTime,
                IsSendForApproval = rentalPaymentRequest.IsSendForApproval,
                SendForApprovalBy = rentalPaymentRequest.SendForApprovalBy,
                SendForApprovalTime = rentalPaymentRequest.SendForApprovalTime,
                IsOnlineTransferred = rentalPaymentRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = rentalPaymentRequest.IsOnlineTransferredBy,
                OnlineTransferredTime = rentalPaymentRequest.OnlineTransferredTime,
                OrgBranchName = rentalPaymentRequest.OrgBranchName,
                DateCreated = rentalPaymentRequest.DateCreated,
                CreatedBy = rentalPaymentRequest.CreatedBy,
                LastModified = rentalPaymentRequest.LastModified,
                ModifiedBy = rentalPaymentRequest.ModifiedBy,
                IsFinanceSendForApproval = rentalPaymentRequest.IsFinanceSendForApproval,
                FinanceSendForApprovalBy = rentalPaymentRequest.FinanceSendForApprovalBy,
                FinanceSendForApprovalTime = rentalPaymentRequest.FinanceSendForApprovalTime,
                IsFinanceApproved = rentalPaymentRequest.IsFinanceApproved,
                FinanceApprovedBy = rentalPaymentRequest.FinanceApprovedBy,
                FinanceApprovedTime = rentalPaymentRequest.FinanceApprovedTime,
                AmountDue = rentalPaymentRequest.AmountDue,
                TotalAmount = rentalPaymentRequest.TotalAmount,
                IsVoid = rentalPaymentRequest.IsVoid,
                Paid = rentalPaymentRequest.Paid,
                VoidBy = rentalPaymentRequest.VoidBy,
                VoidTime = rentalPaymentRequest.VoidTime,
                
            });
            return Json(result);
        }
        public ActionResult RentalPaymentRequests_Create([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
         {
            var durations = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == rentalPaymentRequest.GLPeriodId);
            //var result = CheckNull(rentalPaymentRequest);
            //if (result == RentalValidation.RentFromExternaNotFound)
            //{
            //    ModelState.AddModelError(string.Empty, "No equipment was found under the supplier name in rent from external");
            //}
            //if (result == RentalValidation.FleetNotFound)
            //{
            //    ModelState.AddModelError(string.Empty, "no fleet was found under rent from external fleet registration");
            //}
            //if (result == RentalValidation.DateInvalid)
            //{
            //    ModelState.AddModelError(string.Empty, $"No Timesheet was found under the period {durations.DateFrom} to {durations.DateTo}");
            //}

            if (ModelState.IsValid)

            {
              
                var entity = new RentalPaymentRequest
                {
                    RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                    EmployeeId = rentalPaymentRequest.EmployeeId,
                    BranchId = rentalPaymentRequest.BranchId,
                    ReasonofPayment = rentalPaymentRequest.ReasonofPayment,
                    RequestDate = rentalPaymentRequest.RequestDate,
                    Refference = rentalPaymentRequest.Refference,
                    Remark = rentalPaymentRequest.Remark,
                    GLPeriodId = rentalPaymentRequest.GLPeriodId,
                    GLFiscalYearId = rentalPaymentRequest.GLFiscalYearId,
                    DurationFrom = durations.DateFrom,
                    DurationTo = durations.DateTo,
                    OrgStructureId = rentalPaymentRequest.OrgStructureId,
                    ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID,
                    IsOnlineApproved = rentalPaymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = rentalPaymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = rentalPaymentRequest.OnlineApprovedTime,
                    IsSendForApproval = rentalPaymentRequest.IsSendForApproval,
                    SendForApprovalBy = rentalPaymentRequest.SendForApprovalBy,
                    SendForApprovalTime = rentalPaymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = rentalPaymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalPaymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalPaymentRequest.OnlineTransferredTime,
                    BusinessUnitId = rentalPaymentRequest.BusinessUnitId,
                    IsBudgetApproved = rentalPaymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = rentalPaymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = rentalPaymentRequest.BudgetApprovedTime,
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = rentalPaymentRequest.IsFinalApproval,
                    CurrentApprovalStep = rentalPaymentRequest.CurrentApprovalStep,
                    IsDepartmentOnlineApproved = rentalPaymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = rentalPaymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = rentalPaymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = rentalPaymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = rentalPaymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = rentalPaymentRequest.DepartmentSendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = rentalPaymentRequest.LastModified,
                    ModifiedBy = rentalPaymentRequest.ModifiedBy,
                    IsFinanceSendForApproval = rentalPaymentRequest.IsFinanceSendForApproval,
                    FinanceSendForApprovalBy = rentalPaymentRequest.FinanceSendForApprovalBy,
                    FinanceSendForApprovalTime = rentalPaymentRequest.FinanceSendForApprovalTime,
                    IsFinanceApproved = rentalPaymentRequest.IsFinanceApproved,
                    FinanceApprovedBy = rentalPaymentRequest.FinanceApprovedBy,
                    FinanceApprovedTime = rentalPaymentRequest.FinanceApprovedTime,
                    
                };     

                db.RentalPaymentRequests.Add(entity);
                db.SaveChanges();
                rentalPaymentRequest.RentalPaymentRequestId = entity.RentalPaymentRequestId;

               // GenerateRentalPaymentRequestDetail(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request registration", "Success", User.Identity.Name, $"created rental payment request number {entity.RentalPaymentRequestId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequest }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequests_Update([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
        {
            
            if (ModelState.IsValid)
            {
                var durations = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == rentalPaymentRequest.GLPeriodId);
                if(durations != null)
                {
                    rentalPaymentRequest.DurationFrom = durations.DateFrom;
                    rentalPaymentRequest.DurationTo = durations.DateTo;
                    rentalPaymentRequest.LastModified = DateTime.Today;
                    rentalPaymentRequest.ModifiedBy = User.Identity.Name;
                    db.RentalPaymentRequests.Attach(rentalPaymentRequest);
                    db.Entry(rentalPaymentRequest).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request update", "Success", User.Identity.Name, $"updated rental payment request {rentalPaymentRequest.RentalPaymentRequestId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalPaymentRequests_Destroy([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest model)
        {

            var rentalPaymentRequest = db.RentalPaymentRequests.Find(model.RentalPaymentRequestId);
            var detail = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == rentalPaymentRequest.RentalPaymentRequestId);
            if (detail.Any())
            {
                db.RentalPaymentRequestLines.RemoveRange(detail);
                db.SaveChanges();
            }

            db.RentalPaymentRequests.Remove(rentalPaymentRequest);
            db.SaveChanges();
            #region delete log
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rental inspection request {model.RentalPaymentRequestId }");
            #endregion

            return Json(new[] { model }.ToDataSourceResult(request));
        }
        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.RentalPaymentRequests.Find(id);
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
            var lines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id);
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
            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                //#region approval workflow
                //bool isNewRecord = true;
                //var validations = record.RentalPaymentRequestDepartmentValidation.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                //if (validations.Any())
                //    isNewRecord = false;
                //decimal docamount = record.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestLineId == id).Sum(x => x.TotalAmount);
                //int docid = record.RentalPaymentRequestId;
                ////DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApPaymentRequestSelfCare, isNewRecord);
                //#endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public RentalValidation CheckNull(RentalPaymentRequest rentalPaymentRequest)
        {
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
                if(machinery.Any() || dumpTruck.Any() || dryLoading.Any() || wagonDrill.Any() || lightVehicle.Any())
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
        public void GenerateRentalPaymentRequestDetail(RentalPaymentRequest rentalPaymentRequest)
        {
            var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.ProcurementSupplierID == rentalPaymentRequest.ProcurementSupplierID);
            if (rentFromExternal != null)
            {
                var fleet = db.Fleets.FirstOrDefault(x => x.RentId == rentFromExternal.RentFromExternalID);

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}