﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class OwningOperatingCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OwningandOperatingCosts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OwningandOperatingCost> owningandoperatingcosts = db.OwningandOperatingCosts;
            DataSourceResult result = owningandoperatingcosts.ToDataSourceResult(request, owningandOperatingCost => new
            {
                OwningandOperatingCostId = owningandOperatingCost.OwningandOperatingCostId,
                Model = owningandOperatingCost.Fleet.UnitNum + " - " + owningandOperatingCost.Fleet.PlateNumber,
                Manufacture = owningandOperatingCost.Fleet.Manufacturer,
                OwnerShipPeriod = owningandOperatingCost.OwnerShipPeriod,
                EstimatedUsage = owningandOperatingCost.EstimatedUsage,
                OwnerShipUsage = owningandOperatingCost.OwnerShipUsage,
                DeliveryPrice = owningandOperatingCost.DeliveryPrice,
                TyreReplacementCost = owningandOperatingCost.TyreReplacementCost,
                TotalOperatingWeight = owningandOperatingCost.TotalOperatingWeight,
                ResdualScrapValue = owningandOperatingCost.ResdualScrapValue,
                YerlyPremiuem = owningandOperatingCost.YerlyPremiuem,
                NetValue = owningandOperatingCost.NetValue,
                FuelConsumption = owningandOperatingCost.FuelConsumption,
                OperatorDriverMonthlySalary = owningandOperatingCost.OperatorDriverMonthlySalary,
                Depreciation = owningandOperatingCost.Depreciation,
                Interest = owningandOperatingCost.Interest,
                Insurance = owningandOperatingCost.Insurance,
                PropertyTax = owningandOperatingCost.PropertyTax,
                NetTotal = owningandOperatingCost.NetTotal,
                Fuel = owningandOperatingCost.Fuel,
                Value = owningandOperatingCost.Value,
                LubOil = owningandOperatingCost.LubOil,
                ServiceParts = owningandOperatingCost.ServiceParts,
                Tire = owningandOperatingCost.Tire,
                MaintenanceRepairCost = owningandOperatingCost.MaintenanceRepairCost,
                SpecialWearItemsCost = owningandOperatingCost.SpecialWearItemsCost,
                UnderCarrageCost = owningandOperatingCost.UnderCarrageCost,
                OperatorCost = owningandOperatingCost.OperatorCost,
                TotalNetCost = owningandOperatingCost.TotalNetCost,
                Profit = owningandOperatingCost.Profit,
                OverHead = owningandOperatingCost.OverHead,
                RentalRateWithFuel = owningandOperatingCost.RentalRateWithFuel,
                RentalRateWithOutFuel = owningandOperatingCost.RentalRateWithOutFuel,
                TotalOperatingCost = owningandOperatingCost.TotalOperatingCost,
                Status = owningandOperatingCost.Status,
                Remark = owningandOperatingCost.Remark,
                IsVoid = owningandOperatingCost.IsVoid,
                VoidBy = owningandOperatingCost.VoidBy,
                VoidTime = owningandOperatingCost.VoidTime,
                IsOnlineApproved = owningandOperatingCost.IsOnlineApproved,
                OnlineApprovedBy = owningandOperatingCost.OnlineApprovedBy,
                OnlineApprovedTime = owningandOperatingCost.OnlineApprovedTime,
                IsOnlineTransferred = owningandOperatingCost.IsOnlineTransferred,
                IsOnlineTransferredBy = owningandOperatingCost.IsOnlineTransferredBy,
                OnlineTransferredTime = owningandOperatingCost.OnlineTransferredTime,
                IsSendForApproval = owningandOperatingCost.IsSendForApproval,
                OrgBranchName = owningandOperatingCost.OrgBranchName,
                SendForApprovalBy = owningandOperatingCost.SendForApprovalBy,
                SendForApprovalTime = owningandOperatingCost.SendForApprovalTime, 
                FleetsID =  owningandOperatingCost.FleetsID,
                DateCreated = owningandOperatingCost.DateCreated,
                LastModified = owningandOperatingCost.LastModified,
                CreatedBy = owningandOperatingCost.CreatedBy,
                ModifiedBy = owningandOperatingCost.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OwningandOperatingCosts_Create([DataSourceRequest]DataSourceRequest request, OwningandOperatingCost owningandOperatingCost)
        {

            var IsFleetExist = db.OwningandOperatingCosts.Where(x => x.FleetsID == owningandOperatingCost.FleetsID).ToList();
            if(IsFleetExist.Any())
            {

                ModelState.Clear();
                ModelState.AddModelError("FleetExist", "Equipment Owning Cost Already Setted ");
                return Json(new[] { owningandOperatingCost }.ToDataSourceResult(request, ModelState));
            }
            var CostConstants = db.OwningandOperatingCostConstants.ToList();

            var Resdual_Scrap = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Resdual_Scrap).FirstOrDefault();
            var Interest_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Interest_Rate).FirstOrDefault();
            var Insurance_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Insurance_Rate).FirstOrDefault();
            var Property_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Property_Rate).FirstOrDefault();
            var Fuel_UnitPrice = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Fuel_UnitPrice).FirstOrDefault();
            var Lubricant_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Lubricant_Rate).FirstOrDefault();
            var SparePart_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.SparePart_Rate).FirstOrDefault();

            var Tyre_Life = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Tyre_Life).FirstOrDefault();
            var Maintenace_Repair_Depreciation = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Maintenace_Repair_Depreciation).FirstOrDefault();
            var Repair_Factor = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Repair_Factor).FirstOrDefault();
            var SpecialWearItemRate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.SpecialWearItemRate).FirstOrDefault();
            var Operator_Days = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Operator_Days).FirstOrDefault();
            var Operator_Hours = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Operator_Hours).FirstOrDefault();
            var Numerator = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Numerator).FirstOrDefault();
            var Denomator = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Denomator).FirstOrDefault();
            var Under_Carrage = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Under_Carrage_Life).FirstOrDefault();

            var entity = new OwningandOperatingCost
                {
                    OwningandOperatingCostId = owningandOperatingCost.OwningandOperatingCostId,
                    FleetsID = owningandOperatingCost.FleetsID,
                    Model = owningandOperatingCost.Model,
                    Manufacture = owningandOperatingCost.Manufacture,
                    OwnerShipPeriod = owningandOperatingCost.OwnerShipPeriod,
                    EstimatedUsage = owningandOperatingCost.EstimatedUsage,
                    DeliveryPrice = owningandOperatingCost.DeliveryPrice,
                    TyreReplacementCost = owningandOperatingCost.TyreReplacementCost,
                    TotalOperatingWeight = owningandOperatingCost.TotalOperatingWeight,
                    YerlyPremiuem = owningandOperatingCost.YerlyPremiuem,
                    NetValue = owningandOperatingCost.NetValue,
                    FuelConsumption = owningandOperatingCost.FuelConsumption,
                    OperatorDriverMonthlySalary = owningandOperatingCost.OperatorDriverMonthlySalary,
                    Depreciation = owningandOperatingCost.Depreciation,
                    Insurance = owningandOperatingCost.Insurance,
                    PropertyTax = owningandOperatingCost.PropertyTax,
                    NetTotal = owningandOperatingCost.NetTotal,
                    Fuel = owningandOperatingCost.Fuel,
                    LubOil = owningandOperatingCost.LubOil,
                    ServiceParts = owningandOperatingCost.ServiceParts,
                    Tire = owningandOperatingCost.Tire,
                    MaintenanceRepairCost = owningandOperatingCost.MaintenanceRepairCost,
                    SpecialWearItemsCost = owningandOperatingCost.SpecialWearItemsCost,
                    UnderCarrageCost = owningandOperatingCost.UnderCarrageCost,
                    OperatorCost = owningandOperatingCost.OperatorCost,
                    TotalNetCost = owningandOperatingCost.TotalNetCost,
                    Profit = owningandOperatingCost.Profit,
                    OverHead = owningandOperatingCost.OverHead,
                    RentalRateWithFuel = owningandOperatingCost.RentalRateWithFuel,
                    RentalRateWithOutFuel = owningandOperatingCost.RentalRateWithOutFuel,
                    Status = owningandOperatingCost.Status,
                    Remark = owningandOperatingCost.Remark,
                    IsVoid = owningandOperatingCost.IsVoid,
                    VoidBy = owningandOperatingCost.VoidBy,
                    VoidTime = owningandOperatingCost.VoidTime,
                    IsOnlineApproved = owningandOperatingCost.IsOnlineApproved,
                    OnlineApprovedBy = owningandOperatingCost.OnlineApprovedBy,
                    OnlineApprovedTime = owningandOperatingCost.OnlineApprovedTime,
                    IsOnlineTransferred = owningandOperatingCost.IsOnlineTransferred,
                    IsOnlineTransferredBy = owningandOperatingCost.IsOnlineTransferredBy,
                    OnlineTransferredTime = owningandOperatingCost.OnlineTransferredTime,
                    IsSendForApproval = owningandOperatingCost.IsSendForApproval,
                    OrgBranchName = owningandOperatingCost.OrgBranchName,
                    SendForApprovalBy = owningandOperatingCost.SendForApprovalBy,
                    SendForApprovalTime = owningandOperatingCost.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = owningandOperatingCost.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = owningandOperatingCost.ModifiedBy
                };
            entity.OwnerShipUsage = owningandOperatingCost.OwnerShipPeriod * owningandOperatingCost.EstimatedUsage;
            entity.ResdualScrapValue = owningandOperatingCost.TotalOperatingWeight * DecimalNullDefaultValue(Resdual_Scrap);
            entity.Value = entity.DeliveryPrice - entity.TyreReplacementCost - entity.ResdualScrapValue;
            entity.Depreciation = entity.NetValue / entity.OwnerShipUsage.DivideByZeroHelper();

            entity.Interest = (((entity.OwnerShipPeriod + DecimalNullDefaultValue(Numerator)) / DecimalNullDefaultValue(Denomator).DivideByZeroHelper() * entity.OwnerShipPeriod) * entity.DeliveryPrice * DecimalNullDefaultValue(Interest_Rate)) / entity.EstimatedUsage.DivideByZeroHelper();
            entity.Insurance = (((entity.OwnerShipPeriod + DecimalNullDefaultValue(Numerator)) / DecimalNullDefaultValue(Denomator).DivideByZeroHelper() * entity.OwnerShipPeriod) * entity.DeliveryPrice * DecimalNullDefaultValue(Insurance_Rate)) / entity.EstimatedUsage.DivideByZeroHelper();
            entity.PropertyTax = (((entity.OwnerShipPeriod + DecimalNullDefaultValue(Numerator)) / DecimalNullDefaultValue(Denomator).DivideByZeroHelper() * entity.OwnerShipPeriod) * entity.DeliveryPrice * DecimalNullDefaultValue(Property_Rate)) / entity.EstimatedUsage.DivideByZeroHelper();
            entity.TotalNetCost = entity.Depreciation + entity.Insurance + entity.PropertyTax + entity.Interest;

            entity.Fuel = entity.FuelConsumption * DecimalNullDefaultValue(Fuel_UnitPrice);
            entity.LubOil = entity.Fuel * DecimalNullDefaultValue(Lubricant_Rate);
            entity.ServiceParts = entity.LubOil * DecimalNullDefaultValue(SparePart_Rate);

            entity.Tire = entity.TyreReplacementCost * DecimalNullDefaultValue(Tyre_Life);
            entity.MaintenanceRepairCost = entity.Depreciation * DecimalNullDefaultValue(Repair_Factor) / DecimalNullDefaultValue(Maintenace_Repair_Depreciation).DivideByZeroHelper();

            entity.OperatorCost = entity.OperatorDriverMonthlySalary / (DecimalNullDefaultValue(Operator_Days) * DecimalNullDefaultValue(Operator_Hours)).DivideByZeroHelper();
            entity.UnderCarrageCost = (entity.Value/10) * DecimalNullDefaultValue(Under_Carrage);
            entity.SpecialWearItemsCost = entity.MaintenanceRepairCost * DecimalNullDefaultValue(SpecialWearItemRate);
            entity.NetTotal = entity.SpecialWearItemsCost + entity.OperatorCost + entity.MaintenanceRepairCost + entity.UnderCarrageCost + entity.Fuel + entity.ServiceParts + entity.LubOil + entity.Tire;
            entity.TotalOperatingCost = entity.NetTotal + entity.TotalNetCost;
            entity.RentalRateWithFuel = entity.TotalOperatingCost + entity.Profit + entity.OverHead;
            entity.RentalRateWithOutFuel = entity.RentalRateWithFuel - entity.Fuel;

            db.OwningandOperatingCosts.Add(entity);
            db.SaveChanges();

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "Owning operating cost registration", "Success", User.Identity.Name, $"created owning operating cost {entity.OwningandOperatingCostId} ", Modules.FixedAsset);
            #endregion

            owningandOperatingCost.OwningandOperatingCostId = entity.OwningandOperatingCostId;
            IFleetService _FleetService = new FleetService();

            var Fleet = _FleetService.GetById(entity.FleetsID);
            Fleet.RentalRate = entity.RentalRateWithFuel;
            Fleet.Condition = entity.Remark;
            _FleetService.Update(Fleet);

            IFleetHistoryService _FleetHistoryService = new FleetHistoryService() ;
            _FleetHistoryService.Insert(new FleetHistory { FleetsID = entity.FleetsID, FleetHistoryID = Guid.NewGuid(), RentalRate = entity.RentalRateWithFuel, FuelStandard = entity.FuelConsumption.ToString(), DateCreated = DateTime.Now, CreatedBy = User.Identity.Name });

            return Json(new[] { owningandOperatingCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OwningandOperatingCosts_Update([DataSourceRequest]DataSourceRequest request, OwningandOperatingCost owningandOperatingCost)
        {
            var IsFleetExist = db.OwningandOperatingCosts.Where(x => x.FleetsID == owningandOperatingCost.FleetsID && x.OwningandOperatingCostId != owningandOperatingCost.OwningandOperatingCostId).ToList();
            if (IsFleetExist.Any())
            {

                ModelState.Clear();
                ModelState.AddModelError("FleetExist", "Equipment Owning Cost Already Setted ");
                return Json(new[] { owningandOperatingCost }.ToDataSourceResult(request, ModelState));
            }
            var CostConstants = db.OwningandOperatingCostConstants.ToList();

            var Resdual_Scrap = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Resdual_Scrap).FirstOrDefault();
            var Interest_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Interest_Rate).FirstOrDefault();
            var Insurance_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Insurance_Rate).FirstOrDefault();
            var Property_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Property_Rate).FirstOrDefault();
            var Fuel_UnitPrice = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Fuel_UnitPrice).FirstOrDefault();
            var Lubricant_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Lubricant_Rate).FirstOrDefault();
            var SparePart_Rate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.SparePart_Rate).FirstOrDefault();

            var Tyre_Life = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Tyre_Life).FirstOrDefault();
            var Maintenace_Repair_Depreciation = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Maintenace_Repair_Depreciation).FirstOrDefault();
            var Repair_Factor = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Repair_Factor).FirstOrDefault();
            var SpecialWearItemRate = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.SpecialWearItemRate).FirstOrDefault();
            var Operator_Days = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Operator_Days).FirstOrDefault();
            var Operator_Hours = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Operator_Hours).FirstOrDefault();
            var Numerator = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Numerator).FirstOrDefault();
            var Denomator = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Denomator).FirstOrDefault();
            var Under_Carrage = CostConstants.Where(x => x.ConstantType == Core.Domain.FleetManagement.OwningandOperatingCostConstantType.Under_Carrage_Life).FirstOrDefault();

            var entity = new OwningandOperatingCost
            {
                OwningandOperatingCostId = owningandOperatingCost.OwningandOperatingCostId,
                FleetsID = owningandOperatingCost.FleetsID,
                Model = owningandOperatingCost.Model,
                Manufacture = owningandOperatingCost.Manufacture,
                OwnerShipPeriod = owningandOperatingCost.OwnerShipPeriod,
                EstimatedUsage = owningandOperatingCost.EstimatedUsage,
                DeliveryPrice = owningandOperatingCost.DeliveryPrice,
                TyreReplacementCost = owningandOperatingCost.TyreReplacementCost,
                TotalOperatingWeight = owningandOperatingCost.TotalOperatingWeight,
                YerlyPremiuem = owningandOperatingCost.YerlyPremiuem,
                NetValue = owningandOperatingCost.NetValue,
                FuelConsumption = owningandOperatingCost.FuelConsumption,
                OperatorDriverMonthlySalary = owningandOperatingCost.OperatorDriverMonthlySalary,
                Depreciation = owningandOperatingCost.Depreciation,
                Insurance = owningandOperatingCost.Insurance,
                PropertyTax = owningandOperatingCost.PropertyTax,
                NetTotal = owningandOperatingCost.NetTotal,
                Fuel = owningandOperatingCost.Fuel,
                LubOil = owningandOperatingCost.LubOil,
                ServiceParts = owningandOperatingCost.ServiceParts,
                Tire = owningandOperatingCost.Tire,
                MaintenanceRepairCost = owningandOperatingCost.MaintenanceRepairCost,
                SpecialWearItemsCost = owningandOperatingCost.SpecialWearItemsCost,
                UnderCarrageCost = owningandOperatingCost.UnderCarrageCost,
                OperatorCost = owningandOperatingCost.OperatorCost,
                TotalNetCost = owningandOperatingCost.TotalNetCost,
                Profit = owningandOperatingCost.Profit,
                OverHead = owningandOperatingCost.OverHead,
                RentalRateWithFuel = owningandOperatingCost.RentalRateWithFuel,
                RentalRateWithOutFuel = owningandOperatingCost.RentalRateWithOutFuel,
                Status = owningandOperatingCost.Status,
                Remark = owningandOperatingCost.Remark,
                IsVoid = owningandOperatingCost.IsVoid,
                VoidBy = owningandOperatingCost.VoidBy,
                VoidTime = owningandOperatingCost.VoidTime,
                IsOnlineApproved = owningandOperatingCost.IsOnlineApproved,
                OnlineApprovedBy = owningandOperatingCost.OnlineApprovedBy,
                OnlineApprovedTime = owningandOperatingCost.OnlineApprovedTime,
                IsOnlineTransferred = owningandOperatingCost.IsOnlineTransferred,
                IsOnlineTransferredBy = owningandOperatingCost.IsOnlineTransferredBy,
                OnlineTransferredTime = owningandOperatingCost.OnlineTransferredTime,
                IsSendForApproval = owningandOperatingCost.IsSendForApproval,
                OrgBranchName = owningandOperatingCost.OrgBranchName,
                SendForApprovalBy = owningandOperatingCost.SendForApprovalBy,
                SendForApprovalTime = owningandOperatingCost.SendForApprovalTime,
                DateCreated = owningandOperatingCost.DateCreated,
                LastModified = DateTime.Now,
                CreatedBy = owningandOperatingCost.CreatedBy,
                ModifiedBy = User.Identity.Name
            };
            entity.OwnerShipUsage = owningandOperatingCost.OwnerShipPeriod * owningandOperatingCost.EstimatedUsage;
            entity.ResdualScrapValue = owningandOperatingCost.TotalOperatingWeight * DecimalNullDefaultValue(Resdual_Scrap);
            entity.Value = entity.DeliveryPrice - entity.TyreReplacementCost - entity.ResdualScrapValue;
            entity.Depreciation = entity.NetValue / entity.OwnerShipUsage.DivideByZeroHelper();

            entity.Interest = (((entity.OwnerShipPeriod + DecimalNullDefaultValue(Numerator)) / DecimalNullDefaultValue(Denomator).DivideByZeroHelper() * entity.OwnerShipPeriod) * entity.DeliveryPrice * DecimalNullDefaultValue(Interest_Rate)) / entity.EstimatedUsage.DivideByZeroHelper();
            entity.Insurance = (((entity.OwnerShipPeriod + DecimalNullDefaultValue(Numerator)) / DecimalNullDefaultValue(Denomator).DivideByZeroHelper() * entity.OwnerShipPeriod) * entity.DeliveryPrice * DecimalNullDefaultValue(Insurance_Rate)) / entity.EstimatedUsage.DivideByZeroHelper();
            entity.PropertyTax = (((entity.OwnerShipPeriod + DecimalNullDefaultValue(Numerator)) / DecimalNullDefaultValue(Denomator).DivideByZeroHelper() * entity.OwnerShipPeriod) * entity.DeliveryPrice * DecimalNullDefaultValue(Property_Rate)) / entity.EstimatedUsage.DivideByZeroHelper();
            entity.TotalNetCost = entity.Depreciation + entity.Insurance + entity.PropertyTax + entity.Interest;

            entity.Fuel = entity.FuelConsumption * DecimalNullDefaultValue(Fuel_UnitPrice);
            entity.LubOil = entity.Fuel * DecimalNullDefaultValue(Lubricant_Rate);
            entity.ServiceParts = entity.LubOil * DecimalNullDefaultValue(SparePart_Rate);

            entity.Tire = entity.TyreReplacementCost * DecimalNullDefaultValue(Tyre_Life);
            entity.MaintenanceRepairCost = entity.Depreciation * DecimalNullDefaultValue(Repair_Factor) / DecimalNullDefaultValue(Maintenace_Repair_Depreciation).DivideByZeroHelper();

            entity.OperatorCost = entity.OperatorDriverMonthlySalary / (DecimalNullDefaultValue(Operator_Days) * DecimalNullDefaultValue(Operator_Hours)).DivideByZeroHelper();
            entity.UnderCarrageCost = (entity.Value / 10) * DecimalNullDefaultValue(Under_Carrage);
            entity.SpecialWearItemsCost = entity.MaintenanceRepairCost * DecimalNullDefaultValue(SpecialWearItemRate);
            entity.NetTotal = entity.SpecialWearItemsCost + entity.OperatorCost + entity.MaintenanceRepairCost + entity.UnderCarrageCost + entity.Fuel + entity.ServiceParts + entity.LubOil + entity.Tire;
            entity.TotalOperatingCost = entity.NetTotal + entity.TotalNetCost;
            entity.RentalRateWithFuel = entity.TotalOperatingCost + entity.Profit + entity.OverHead;
            entity.RentalRateWithOutFuel = entity.RentalRateWithFuel - entity.Fuel;

         
         

            db.OwningandOperatingCosts.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "Owning and operating costs registration", "Success", User.Identity.Name, $"updated owning and operating costs {entity.OwningandOperatingCostId}", Modules.FixedAsset);
            #endregion

            IFleetService _FleetService = new FleetService();

            var Fleet = _FleetService.GetById(entity.FleetsID);
            Fleet.RentalRate = entity.RentalRateWithFuel;
            Fleet.Condition = entity.Remark;
            _FleetService.Update(Fleet);

            return Json(new[] { owningandOperatingCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OwningandOperatingCosts_Destroy([DataSourceRequest]DataSourceRequest request, OwningandOperatingCost owningandOperatingCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new OwningandOperatingCost
                {
                    OwningandOperatingCostId = owningandOperatingCost.OwningandOperatingCostId,
                    Model = owningandOperatingCost.Model,
                    Manufacture = owningandOperatingCost.Manufacture,
                    OwnerShipPeriod = owningandOperatingCost.OwnerShipPeriod,
                    EstimatedUsage = owningandOperatingCost.EstimatedUsage,
                    OwnerShipUsage = owningandOperatingCost.OwnerShipUsage,
                    DeliveryPrice = owningandOperatingCost.DeliveryPrice,
                    TyreReplacementCost = owningandOperatingCost.TyreReplacementCost,
                    TotalOperatingWeight = owningandOperatingCost.TotalOperatingWeight,
                    ResdualScrapValue = owningandOperatingCost.ResdualScrapValue,
                    YerlyPremiuem = owningandOperatingCost.YerlyPremiuem,
                    NetValue = owningandOperatingCost.NetValue,
                    FuelConsumption = owningandOperatingCost.FuelConsumption,
                    OperatorDriverMonthlySalary = owningandOperatingCost.OperatorDriverMonthlySalary,
                    Depreciation = owningandOperatingCost.Depreciation,
                    Insurance = owningandOperatingCost.Insurance,
                    PropertyTax = owningandOperatingCost.PropertyTax,
                    NetTotal = owningandOperatingCost.NetTotal,
                    Fuel = owningandOperatingCost.Fuel,
                    LubOil = owningandOperatingCost.LubOil,
                    ServiceParts = owningandOperatingCost.ServiceParts,
                    Tire = owningandOperatingCost.Tire,
                    MaintenanceRepairCost = owningandOperatingCost.MaintenanceRepairCost,
                    SpecialWearItemsCost = owningandOperatingCost.SpecialWearItemsCost,
                    UnderCarrageCost = owningandOperatingCost.UnderCarrageCost,
                    OperatorCost = owningandOperatingCost.OperatorCost,
                    TotalNetCost = owningandOperatingCost.TotalNetCost,
                    Profit = owningandOperatingCost.Profit,
                    OverHead = owningandOperatingCost.OverHead,
                    RentalRateWithFuel = owningandOperatingCost.RentalRateWithFuel,
                    RentalRateWithOutFuel = owningandOperatingCost.RentalRateWithOutFuel,
                    Status = owningandOperatingCost.Status,
                    Remark = owningandOperatingCost.Remark,
                    IsVoid = owningandOperatingCost.IsVoid,
                    VoidBy = owningandOperatingCost.VoidBy,
                    VoidTime = owningandOperatingCost.VoidTime,
                    IsOnlineApproved = owningandOperatingCost.IsOnlineApproved,
                    OnlineApprovedBy = owningandOperatingCost.OnlineApprovedBy,
                    OnlineApprovedTime = owningandOperatingCost.OnlineApprovedTime,
                    IsOnlineTransferred = owningandOperatingCost.IsOnlineTransferred,
                    IsOnlineTransferredBy = owningandOperatingCost.IsOnlineTransferredBy,
                    OnlineTransferredTime = owningandOperatingCost.OnlineTransferredTime,
                    IsSendForApproval = owningandOperatingCost.IsSendForApproval,
                    OrgBranchName = owningandOperatingCost.OrgBranchName,
                    SendForApprovalBy = owningandOperatingCost.SendForApprovalBy,
                    SendForApprovalTime = owningandOperatingCost.SendForApprovalTime,
                    DateCreated = owningandOperatingCost.DateCreated,
                    LastModified = owningandOperatingCost.LastModified,
                    CreatedBy = owningandOperatingCost.CreatedBy,
                    ModifiedBy = owningandOperatingCost.ModifiedBy
                };

                db.OwningandOperatingCosts.Attach(entity);
                db.OwningandOperatingCosts.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted owning and operating costs registration number {entity.OwningandOperatingCostId}");
                #endregion
            }

            return Json(new[] { owningandOperatingCost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public static decimal DecimalNullDefaultValue(OwningandOperatingCostConstant Constant)
        {
            if (Constant == null)
            {
                return  decimal.Zero;
            }
            return Constant.Value;
        }
    }
}
