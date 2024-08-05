using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class TotalFleetFuelConsumptionOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetFuelViewModel> GetTotalFuel(string equipmentNameId, string plateNo, DateTime? from, DateTime? to)
        {
            // Apply filters directly in the query
            var assignedEquipmentsQuery = db.AssignedEquipment.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(plateNo) && Guid.TryParse(plateNo, out Guid plateNoGuid))
            {
                assignedEquipmentsQuery = assignedEquipmentsQuery.Where(x => x.FleetId == plateNoGuid);
            }

            if (!string.IsNullOrWhiteSpace(equipmentNameId) && Guid.TryParse(equipmentNameId, out Guid equipmentNameIdGuid))
            {
                assignedEquipmentsQuery = assignedEquipmentsQuery.Where(x => x.EquipmentNameID == equipmentNameIdGuid);
            }

            var assignedEquipments = assignedEquipmentsQuery.ToList();
            if (assignedEquipments == null || !assignedEquipments.Any())
            {
                return new List<FleetFuelViewModel>();
            }

            var groupedAssignedEquipments = assignedEquipments.GroupBy(x => x.FleetId).ToList();
            var fleetIds = groupedAssignedEquipments.Select(g => g.Key).ToList();
            var fleets = db.Fleets.Where(f => fleetIds.Contains(f.FleetsID)).ToList();
            var equipmentTypeIds = fleets.Select(f => f.EquipmentTypeID).Distinct().ToList();
            var equipmentTypes = db.EquipmentType.Where(et => equipmentTypeIds.Contains(et.EquipmentTypeID)).ToList();
            var equipmentNameIds = fleets.Select(f => f.EquipmentNameID).Distinct().ToList();
            var equipmentNames = db.EquipmentName.Where(en => equipmentNameIds.Contains(en.EquipmentNameID)).ToList();

            var list = new List<FleetFuelViewModel>();

            foreach (var group in groupedAssignedEquipments)
            {
                var fleet = fleets.FirstOrDefault(f => f.FleetsID == group.Key);
                if (fleet != null)
                {
                    var equipmentType = equipmentTypes.FirstOrDefault(et => et.EquipmentTypeID == fleet.EquipmentTypeID)?.EquipmentTypeName;
                    var equipmentName = equipmentNames.FirstOrDefault(en => en.EquipmentNameID == fleet.EquipmentNameID)?.Name;

                    var totalFuelFromStore = GetFuelFromIssueItem(fleet.PlateNumber, from, to);
                    var totalFuelFromTravelOrder = GetTotalFuelFromTravelOrder(group, from, to);

                    var fuelViewModel = new FleetFuelViewModel
                    {
                        PlateNo = fleet.PlateNumber ?? fleet.SerialNo ?? "UnAssigned",
                        EquipmentTypeName = equipmentType,
                        EquipmentName = equipmentName,
                        TotalFuelFromStore = totalFuelFromStore,
                        TotalFuelFromTravelOrder = totalFuelFromTravelOrder,
                        TotalFuelConsumption = totalFuelFromStore + totalFuelFromTravelOrder
                    };

                    list.Add(fuelViewModel);
                }
            }

            return list;
        }

        private decimal GetFuelFromIssueItem(string plateNo, DateTime? from, DateTime? to)
        {
            var query = db.IssueItems.AsNoTracking().Where(ii => ii.Issue.IsFuel);
            if (query == null)
                return 0;
            if (from.HasValue)
            {
                query = query.Where(ii => ii.DateCreated >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(ii => ii.DateCreated <= to.Value);
            }

            if (!string.IsNullOrWhiteSpace(plateNo))
            {
                query = query.Where(ii => ii.Issue.PlateNo == plateNo);
            }

            return query.Sum(ii => (decimal?)ii.Total) ?? 0;
        }

        private decimal GetTotalFuelFromTravelOrder(IEnumerable<AssignedEquipment> assignedEquipments, DateTime? from, DateTime? to)
        {
            var totalFuelFromTravelOrder = 0m;
            var vehicleDemandIds = assignedEquipments.Select(ae => ae.VehicleDemandId).ToList();
            var requestedEquipments = db.RequestedEquipments.Where(re => vehicleDemandIds.Contains(re.VehicleDemandId)).ToList();

            var requestNumbers = requestedEquipments.Select(re => re.RequestNumber).ToList();
            var vehicleDistributions = db.VehicleDistribution.Where(vd => requestNumbers.Contains(vd.RequestID)).ToList();

            foreach (var vd in vehicleDistributions)
            {
                totalFuelFromTravelOrder += GetFuelSumFromTravelOrder(vd.VehicleDistributionID, from, to);
            }

            return totalFuelFromTravelOrder;
        }

        private decimal GetFuelSumFromTravelOrder(Guid vehicleDistributionId, DateTime? from, DateTime? to)
        {
            var query = db.ApTravelOrders.AsNoTracking().Where(ato => ato.VehicleDistributionID == vehicleDistributionId);
            if (query == null)
                return 0;
            if (from.HasValue)
            {
                query = query.Where(ato => ato.DateCreated >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(ato => ato.DateCreated <= to.Value);
            }

            var travelOrders = query.ToList();
            if (!travelOrders.Any())
            {
                return 0m;
            }

            var travelOrderIds = travelOrders.Select(ato => ato.ApTravelOrderId).ToList();

            var advancePayments = db.ApTravelOrderAdvancePayments
                                   .Where(ap => travelOrderIds.Contains(ap.ApTravelOrderId) && ap.Type == TravelPaymentType.Fuel)
                                   .Sum(ap => (decimal?)ap.Total) ?? 0;

            var expenseClaims = db.ApTravelExpenseClaims
                                  .Where(ec => travelOrderIds.Contains((int)ec.ApTravelOrderId))
                                  .ToList();

            var claimIds = expenseClaims.Select(ec => ec.ApTravelExpenseClaimId).ToList();

            var claimExpenses = db.ApTravelExpenseExpenses
                                   .Where(ce => claimIds.Contains(ce.ApTravelExpenseClaimId) && ce.Type == TravelPaymentType.Fuel)
                                   .Sum(ce => (decimal?)ce.Total) ?? 0;

            return advancePayments + Math.Max(0, claimExpenses - advancePayments);
        }
    }

    public class FleetFuelViewModel
    {
        public string PlateNo { get; set; }
        public string FuelType { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalFuelFromStore { get; set; }
        public decimal TotalFuelFromTravelOrder { get; set; }
        public decimal TotalFuelConsumption { get; set; }
        public string EquipmentTypeName { get; set; }
        public string EquipmentName { get; set; }
    }
}
