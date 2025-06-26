using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentReturnDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistrbutionViewModel> GetEquipmentReturns(string Branch, string Structure, DateTime startingDate, DateTime endingDate)
        {
            var result = new List<EquipmentDistrbutionViewModel>();

            // Preload organization metadata
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            string amharicName = organization?.AmharicName ?? "";
            string name = organization?.TradeName ?? "";
            string address = "";
            Image logoFileName = null;

            if (organization != null)
            {
                var addressList = db.Addresses
                    .Where(a => a.Reference == organization.MainCompanyId.ToString() && a.ReferenceType.ToLower() == "company")
                    .ToList();

                address = string.Join(", ", addressList.Select(a => $"{a.Category}: {a.Value}"));

                var logo = db.Attachments
                    .FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                         a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    logoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            // Build base query for VehicleDistributions
            var vehicleDistributionsQuery = db.VehicleDistribution
                .Where(x => x.FromBranchName == Branch &&
                            x.DateCreated > startingDate && x.DateCreated < endingDate);

            if (!string.IsNullOrEmpty(Structure))
            {
                vehicleDistributionsQuery = vehicleDistributionsQuery.Where(x => x.FromStructureName == Structure);
            }

            var vehicleDistributions = vehicleDistributionsQuery.ToList();

            // Preload request numbers to avoid repeated DB calls
            var requestIds = vehicleDistributions.Select(x => x.RequestID).Distinct().ToList();

            var allAssignedEquipments = db.AssignedEquipment
                .Where(x => requestIds.Contains(x.RequestNumber) && x.IsAssigned == false)
                .ToList();

            var allRequests = db.RequestedEquipments
                .Where(x => requestIds.Contains(x.RequestNumber))
                .ToDictionary(x => x.RequestNumber);

            foreach (var vehicle in vehicleDistributions)
            {
                var assignedEquipments = allAssignedEquipments
                    .Where(x => x.RequestNumber == vehicle.RequestID)
                    .ToList();

                if (!assignedEquipments.Any())
                    continue;

                var demand = allRequests.ContainsKey(vehicle.RequestID) ? allRequests[vehicle.RequestID] : null;

                string equipmentStatus = demand?.EquipmentStatus ?? "";
                int? requestedQuantity = demand?.Quantity ?? 0;

                foreach (var assigned in assignedEquipments)
                {
                    result.Add(new EquipmentDistrbutionViewModel
                    {
                        AssignedQuantity = assignedEquipments.Count,
                        DistrubtionNumber = vehicle.DistrubtionNumber,
                        EquipmentStatus = equipmentStatus,
                        FromBranchName = vehicle.FromBranchName,
                        FromStructureName = vehicle.FromStructureName,
                        StartingDate = assigned.StartDate,
                        ToDate = assigned.EndDate,
                        Type = Enum.GetName(vehicle.Type.GetType(), vehicle.Type),
                        CategoryRef = "", // You previously had this commented out
                        RequestQuamtity = requestedQuantity,

                        AmharicName = amharicName,
                        Name = name,
                        LogoFileName = logoFileName,
                        Address = address
                    });
                }
            }

            return result;
        }
    }
}

