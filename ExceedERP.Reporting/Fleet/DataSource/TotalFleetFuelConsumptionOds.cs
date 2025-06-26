using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class TotalFleetFuelConsumptionOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetFuelViewModel> GetTotalFuel(string equipmentNameId, string plateNo, DateTime from, DateTime to)
        {
            var assignedEquipmentsQuery = db.AssignedEquipment.AsNoTracking().AsQueryable();
            Guid plateNoGuid = Guid.Empty;
            bool isPlateNoValid = !string.IsNullOrWhiteSpace(plateNo) && Guid.TryParse(plateNo, out plateNoGuid);

            List<FleetFuelViewModel> list = new List<FleetFuelViewModel>();
            //var fetchAllDemandIds = db.VehicleDemand.Where(x => x.SecondOnlineApprovedTime.HasValue && (DbFunctions.TruncateTime(x.SecondOnlineApprovedTime)) >= from.Date && (DbFunctions.TruncateTime(x.SecondOnlineApprovedTime)) <= to.Date).Select(x => x.VehicleDemandId.ToString()).ToList();
            //var fetchAllDemandIds = db.VehicleDemand.Select(x => x.VehicleDemandId.ToString()).ToList();
            //var fetchEquipmentDistributionsIds = db.VehicleDistribution.Where(x => fetchAllDemandIds.Contains(x.RequestID.ToString())).Select(x=>x.RequestID).ToList();
            //var fetchRequestIds = db.RequestedEquipments.Where(x => fetchEquipmentDistributionsIds.Contains(x.RequestNumber)).Select(x=>x.RequestedEquipmentId).ToList();
           // var fetchAssignedEquipmnet = db.AssignedEquipment.Where(x => fetchRequestIds.Contains(x.RequestedEquipmentId)).ToList();
            var fetchAssignedEquipmnet = db.AssignedEquipment.ToList();
            if (plateNo != null)
            {
                fetchAssignedEquipmnet = fetchAssignedEquipmnet.Where(x => x.FleetId.ToString() == plateNo).ToList();
            }
            var GroupedData = fetchAssignedEquipmnet.GroupBy(x => x.FleetId);
            foreach (var group in GroupedData)
            {
                FleetFuelViewModel FuelConsumption = new FleetFuelViewModel();
                foreach (var g in group)
                {
                    var fetchRequest = db.RequestedEquipments.FirstOrDefault(x => x.RequestedEquipmentId == g.RequestedEquipmentId);
                    var FetchDistrubution = db.VehicleDistribution.FirstOrDefault(x => x.RequestID.ToString() == fetchRequest.RequestNumber.ToString() && (DbFunctions.TruncateTime(x.OnlineApprovedTime) >= from.Date && DbFunctions.TruncateTime(x.OnlineApprovedTime) <= to.Date));
                    if (FetchDistrubution != null)
                    {
                        var fetchTravelOrder = db.ApTravelOrders.FirstOrDefault(x => x.VehicleDistributionID == FetchDistrubution.VehicleDistributionID);
                        if (fetchTravelOrder != null)
                        {
                            var fetchApTravelOrderAdvancePayment = db.ApTravelOrderAdvancePayments.FirstOrDefault(x => x.ApTravelOrderId == fetchTravelOrder.ApTravelOrderId && x.Type == TravelPaymentType.Fuel);
                            if (fetchApTravelOrderAdvancePayment != null)
                            {
                                FuelConsumption.TotalFuelFromTravelOrder += fetchApTravelOrderAdvancePayment.Total;
                                FuelConsumption.FuelFromTravelOrder +=fetchApTravelOrderAdvancePayment.Quantity;
                            }
                        }
                    }

                }
                var firstObject = group.First();
                var FetchFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == firstObject.FleetId);
                if (FetchFleet != null)
                {
                    FuelConsumption.PlateNo = FetchFleet.PlateNumber;
                    FuelConsumption.TotalFuelFromStore = GetFuelFromIssueItem(FetchFleet.PlateNumber, from, to);
                    FuelConsumption.FuelFromStore = GetFuelFromIssueItemInLitter(FetchFleet.PlateNumber, from, to);
                    var FetchEquipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == FetchFleet.EquipmentTypeID);
                    if (FetchEquipmentType != null)
                    {
                        FuelConsumption.EquipmentTypeName = FetchEquipmentType.EquipmentTypeName;
                    }
                    var FetchEquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == FetchFleet.EquipmentNameID);
                    if (FetchEquipmentName != null)
                    {
                        FuelConsumption.EquipmentName = FetchEquipmentName.Name;
                    }
                }
                var claim = GetClaim(group, from, to);
                var claimInLitter = GetClaimInLitter(group, from, to);
                FuelConsumption.TotalClaim = claim - FuelConsumption.TotalFuelFromTravelOrder;
                FuelConsumption.FuelFromClaim = Math.Round((claimInLitter - FuelConsumption.FuelFromTravelOrder), 2);
                FuelConsumption.TotalFuelConsumption = FuelConsumption.TotalFuelFromStore + FuelConsumption.TotalFuelFromTravelOrder + FuelConsumption.TotalClaim;
        
                list.Add(FuelConsumption);
            }





            return list;

        }

        //        if (!string.IsNullOrWhiteSpace(equipmentNameId) && Guid.TryParse(equipmentNameId, out Guid equipmentNameIdGuid))
        //        {
        //            assignedEquipmentsQuery = assignedEquipmentsQuery.Where(x => x.EquipmentNameID == equipmentNameIdGuid);
        //        }

        //        var assignedEquipments = assignedEquipmentsQuery.ToList();
        //        if (assignedEquipments == null || !assignedEquipments.Any())
        //        {
        //            return new List<FleetFuelViewModel>();
        //        }

        //        var groupedAssignedEquipments = assignedEquipments.GroupBy(x => x.FleetId).ToList();
        //        var fleetIds = groupedAssignedEquipments.Select(g => g.Key).ToList();
        //        var fleets = db.Fleets.Where(f => fleetIds.Contains(f.FleetsID)).ToList();
        //        var equipmentTypeIds = fleets.Select(f => f.EquipmentTypeID).Distinct().ToList();
        //        var equipmentTypes = db.EquipmentType.Where(et => equipmentTypeIds.Contains(et.EquipmentTypeID)).ToList();
        //        var equipmentNameIds = fleets.Select(f => f.EquipmentNameID).Distinct().ToList();
        //        var equipmentNames = db.EquipmentName.Where(en => equipmentNameIds.Contains(en.EquipmentNameID)).ToList();

        //        var list = new List<FleetFuelViewModel>();

        //        foreach (var group in groupedAssignedEquipments)
        //        {
        //            var fleet = fleets.FirstOrDefault(f => f.FleetsID == group.Key);
        //            if (fleet != null)
        //            {
        //                var equipmentType = equipmentTypes.FirstOrDefault(et => et.EquipmentTypeID == fleet.EquipmentTypeID)?.EquipmentTypeName;
        //                var equipmentName = equipmentNames.FirstOrDefault(en => en.EquipmentNameID == fleet.EquipmentNameID)?.Name;
        //                var totalFuelFromStore = GetFuelFromIssueItem(fleet.PlateNumber, from, to);
        //                var totalFuelFromStoreInLitter = GetFuelFromIssueItemInLitter(fleet.PlateNumber, from, to);
        //                var FetchPrice = db.IssueItems.AsNoTracking().FirstOrDefault(ii => ii.Issue.IsFuel);
        //                var totalFuelFromTravelOrder = GetTotalFuelFromTravelOrder(group, from, to);
        //                 foreach(var g in group)
        //                {

        //                    var fetchDemand = db.VehicleDemand.FirstOrDefault(x => x.VehicleDemandId == g.RequestNumber);

        //                }
        //                var totalFuelFromTravelOrderInLitter = GetTotalFuelFromTravelOrderInLitter(group, from, to);
        //                var claim = GetClaim(group, from, to);
        //                var claimInLitter = GetClaimInLitter(group, from, to);

        //                var fuelViewModel = new FleetFuelViewModel
        //                {
        //                    PlateNo = fleet.PlateNumber ?? fleet.SerialNo ?? "UnAssigned",
        //                    EquipmentTypeName = equipmentType,
        //                    EquipmentName = equipmentName,
        //                    TotalFuelFromStore = totalFuelFromStore,
        //                    TotalFuelFromTravelOrder = totalFuelFromTravelOrder,
        //                    TotalClaim = claim-totalFuelFromTravelOrder,
        //                    FuelFromTravelOrder =Math.Round(totalFuelFromTravelOrderInLitter, 2),
        //                    FuelFromClaim = Math.Round((claimInLitter - totalFuelFromTravelOrderInLitter),2),
        //                    FuelFromStore = Math.Round(totalFuelFromStoreInLitter,2),
        //                    TotalFuelConsumption = totalFuelFromStore + totalFuelFromTravelOrder + claim,
        //                };

        //                list.Add(fuelViewModel);
        //            }
        //        }

        //        return list;
        //    }

        private decimal GetFuelFromIssueItem(string plateNo, DateTime from, DateTime to)
        {
            var query = db.IssueItems.AsNoTracking().Where(ii => ii.Issue.IsFuel);
            if (query == null)
                return 0;
            
                query = query.Where(ii => (DbFunctions.TruncateTime(ii.Issue.IssueDate)) >= from.Date);
            

          
                query = query.Where(ii=>(DbFunctions.TruncateTime(ii.Issue.IssueDate)) <= to.Date);
            

            if (!string.IsNullOrWhiteSpace(plateNo))
            {
                query = query.Where(ii => ii.Issue.PlateNo == plateNo);
            }

            return query.Sum(ii => (decimal?)ii.Total) ?? 0;
        }
        private decimal GetFuelFromIssueItemInLitter(string plateNo, DateTime from, DateTime to)
            {
                var query = db.IssueItems.AsNoTracking().Where(ii => ii.Issue.IsFuel);
                //if (query == null)
                //    return 0;
                   query = query.Where(ii => (DbFunctions.TruncateTime(ii.Issue.IssueDate)) >= from.Date);
                

               
                    query = query.Where(ii => (DbFunctions.TruncateTime(ii.Issue.IssueDate)) <= to.Date);
                

                if (!string.IsNullOrWhiteSpace(plateNo))
                {
                    query = query.Where(ii => ii.Issue.PlateNo == plateNo);
                }

                return query
         .Where(ii => ii.UnitPrice != 0) // Exclude zero values
         .Sum(ii => (decimal?)(ii.Total / ii.UnitPrice)) ?? 0;
            }

        //    private decimal GetTotalFuelFromTravelOrder(IEnumerable<AssignedEquipment> assignedEquipments, DateTime? from, DateTime? to)
        //    {
        //        var totalFuelFromTravelOrder = 0m;
        //        decimal totalFuelFromTravelOrderInLitter = 0;
        //        var vehicleDemandIds = assignedEquipments.Select(ae => ae.VehicleDemandId).ToList();
        //        var requestedEquipments = db.RequestedEquipments.Where(re => vehicleDemandIds.Contains(re.VehicleDemandId)).ToList();
        //        var requestNumbers = requestedEquipments.Select(re => re.RequestNumber).ToList();
        //        var vehicleDistributions = db.VehicleDistribution.Where(vd => requestNumbers.Contains(vd.RequestID)).ToList();
        //        foreach (var vd in vehicleDistributions)
        //        {
        //            totalFuelFromTravelOrder += GetFuelSumFromTravelOrder(vd.VehicleDistributionID, from, to);

        //        }
        //        return totalFuelFromTravelOrder;
        //    }
        //    private decimal GetTotalFuelFromTravelOrderInLitter(IEnumerable<AssignedEquipment> assignedEquipments, DateTime? from, DateTime? to)
        //    {
        //        var totalFuelFromTravelOrder = 0m;
        //        decimal totalFuelFromTravelOrderInLitter = 0;
        //        var vehicleDemandIds = assignedEquipments.Select(ae => ae.VehicleDemandId).ToList();
        //        var requestedEquipments = db.RequestedEquipments.Where(re => vehicleDemandIds.Contains(re.VehicleDemandId)).ToList();
        //        var requestNumbers = requestedEquipments.Select(re => re.RequestNumber).ToList();
        //        var vehicleDistributions = db.VehicleDistribution.Where(vd => requestNumbers.Contains(vd.RequestID)).ToList();
        //        foreach (var vd in vehicleDistributions)
        //        {

        //            totalFuelFromTravelOrderInLitter += GetFuelSumFromTravelOrderInLitter(vd.VehicleDistributionID, from, to);
        //        }

        //        return totalFuelFromTravelOrderInLitter;
        //    }
        private decimal GetClaim(IEnumerable<AssignedEquipment> assignedEquipments, DateTime from, DateTime to)
        {
            var totalClaim = 0m;
            var vehicleDemandIds = assignedEquipments.Select(ae => ae.VehicleDemandId).ToList();
            var requestedEquipments = db.RequestedEquipments.Where(re => vehicleDemandIds.Contains(re.VehicleDemandId)).ToList();

            var requestNumbers = requestedEquipments.Select(re => re.RequestNumber).ToList();
            var vehicleDistributions = db.VehicleDistribution.Where(vd => requestNumbers.Contains(vd.RequestID)).ToList();

            foreach (var vd in vehicleDistributions)
            {
                totalClaim += GetClaimTotal(vd.VehicleDistributionID, from, to);
            }

            return totalClaim;
        }
        private decimal GetClaimInLitter(IEnumerable<AssignedEquipment> assignedEquipments, DateTime from, DateTime to)
        {
            var totalClaim = 0m;
            var vehicleDemandIds = assignedEquipments.Select(ae => ae.VehicleDemandId).ToList();
            var requestedEquipments = db.RequestedEquipments.Where(re => vehicleDemandIds.Contains(re.VehicleDemandId)).ToList();

            var requestNumbers = requestedEquipments.Select(re => re.RequestNumber).ToList();
            var vehicleDistributions = db.VehicleDistribution.Where(vd => requestNumbers.Contains(vd.RequestID)).ToList();

            foreach (var vd in vehicleDistributions)
            {
                totalClaim += GetClaimTotalInLitter(vd.VehicleDistributionID, from, to);
            }

            return totalClaim;
        }

        //    private decimal GetFuelSumFromTravelOrder(Guid vehicleDistributionId, DateTime? from, DateTime? to)
        //    {
        //        var query = db.ApTravelOrders.AsNoTracking().Where(ato => ato.VehicleDistributionID == vehicleDistributionId&& ato.RequestDate >= from.Value&& ato.RequestDate <= to.Value);
        //        var travelOrderIds = query.Select(ato => ato.ApTravelOrderId).ToList();
        //        if (!travelOrderIds.Any())
        //        {
        //            return 0m;
        //        }

        //        var advancePayments = db.ApTravelOrderAdvancePayments
        //                               .Where(ap => travelOrderIds.Contains(ap.ApTravelOrderId) && ap.Type == TravelPaymentType.Fuel)
        //                               .Sum(ap => (decimal?)ap.Total) ?? 0;
        //        return advancePayments;
        //    }
        //    private decimal GetFuelSumFromTravelOrderInLitter(Guid vehicleDistributionId, DateTime? from, DateTime? to)
        //    {
        //        var query = db.ApTravelOrders.AsNoTracking().Where(ato => ato.VehicleDistributionID == vehicleDistributionId && ato.RequestDate >= from.Value && ato.RequestDate <= to.Value);



        //        var travelOrderIds = query.Select(ato => ato.ApTravelOrderId).ToList();
        //        if (!travelOrderIds.Any())
        //        {
        //            return 0m;
        //        }


        //        var advancePayments = db.ApTravelOrderAdvancePayments
        // .Where(ap => travelOrderIds.Contains(ap.ApTravelOrderId) &&
        //              ap.Type == TravelPaymentType.Fuel &&
        //              ap.UnitPrice != 0) 
        // .Sum(ap => (decimal?)(ap.Total / ap.UnitPrice)) ?? 0;
        //        return advancePayments;
        //    }

        private decimal GetClaimTotal(Guid vehicleDistributionId, DateTime from, DateTime to)
        {
            var query = db.ApTravelOrders.AsNoTracking().Where(ato => ato.VehicleDistributionID == vehicleDistributionId && (DbFunctions.TruncateTime(ato.RequestDate)) >= from.Date && (DbFunctions.TruncateTime(ato.RequestDate)) <= to.Date);


            var travelOrderIds = query.Select(ato => ato.ApTravelOrderId).ToList();
            if (!travelOrderIds.Any())
            {
                return 0m;
            }
            var expenseClaims = db.ApTravelExpenseClaims
                                  .Where(ec => travelOrderIds.Contains((int)ec.ApTravelOrderId))
                                  .ToList();

            var claimIds = expenseClaims.Select(ec => ec.ApTravelExpenseClaimId).ToList();

            return db.ApTravelExpenseExpenses
                                   .Where(ce => claimIds.Contains(ce.ApTravelExpenseClaimId) && ce.Type == TravelPaymentType.Fuel)
                                   .Sum(ce => (decimal?)ce.Total) ?? 0;
        }
        private decimal GetClaimTotalInLitter(Guid vehicleDistributionId, DateTime from, DateTime to)
        {
            var query = db.ApTravelOrders.AsNoTracking().Where(ato => ato.VehicleDistributionID == vehicleDistributionId && (DbFunctions.TruncateTime(ato.RequestDate)) >= from.Date && (DbFunctions.TruncateTime(ato.RequestDate)) <= to.Date);


            var travelOrderIds = query.Select(ato => ato.ApTravelOrderId).ToList();
            if (!travelOrderIds.Any())
            {
                return 0m;
            }
            var expenseClaims = db.ApTravelExpenseClaims
                                  .Where(ec => travelOrderIds.Contains((int)ec.ApTravelOrderId))
                                  .ToList();

            var claimIds = expenseClaims.Select(ec => ec.ApTravelExpenseClaimId).ToList();

            return db.ApTravelExpenseExpenses
    .Where(ce => claimIds.Contains(ce.ApTravelExpenseClaimId) && ce.Type == TravelPaymentType.Fuel)
    .Sum(ce => ce.UnitPrice != 0 ? (decimal?)(ce.Total / ce.UnitPrice) : 0) ?? 0;
        }
    }

        public class FleetFuelViewModel
    {
        public string PlateNo { get; set; }
        public string FuelType { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalFuelFromStore { get; set; }
        public decimal TotalFuelFromTravelOrder { get; set; }
        public decimal FuelFromStore { get; set; }
        public decimal FuelFromTravelOrder { get; set; }
        public decimal FuelFromClaim { get; set; }
        public decimal TotalClaim { get; set; }
        public decimal TotalFuelConsumption { get; set; }
        public string EquipmentTypeName { get; set; }
        public string EquipmentName { get; set; }
    }
}
