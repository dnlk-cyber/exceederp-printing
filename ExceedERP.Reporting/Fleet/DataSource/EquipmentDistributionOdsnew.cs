using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    class EquipmentDistributionOdsnew
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentDistributionViewModel> GetDistribution(string category, string type,
            string name, string plateNo, DateTime? from, DateTime? to)
        {
            List<EquipmentDistributionViewModel> list = new List<EquipmentDistributionViewModel>();

            var assignedEquipments = db.AssignedEquipment.ToList();
            var requestedEquipments = db.RequestedEquipments.ToList();
            var vehicleDistributions = db.VehicleDistribution.ToList();

            if(Guid.TryParse(category, out Guid catId))
            {
                requestedEquipments = requestedEquipments.Where(x => x.EquipmentIdentityID == catId).ToList();
            }

            if (Guid.TryParse(type, out Guid typeId))
            {
                requestedEquipments = requestedEquipments.Where(x => x.EquipmentTypeID == typeId).ToList();
            }

            if (Guid.TryParse(name, out Guid nameId))
            {
                requestedEquipments = requestedEquipments.Where(x => x.EquipmentNameId == nameId).ToList();
            }

            if (from.HasValue)
            {
                requestedEquipments = requestedEquipments.Where(x => x.StartDate.Value.Date >= from.Value.Date).ToList();
            }
            if (to.HasValue)
            {
                requestedEquipments = requestedEquipments.Where(x => x.EndDate.Value.Date <= to.Value.Date).ToList();
            }

            if(Guid.TryParse(plateNo, out Guid fleetId))
            {
                assignedEquipments = assignedEquipments.Where(x => x.FleetId == fleetId).ToList();
            }

            //if(int.TryParse(branchId, out int bId))
            //{
            //    var branch = db.Branch.FirstOrDefault(x => x.BranchId == bId);
            //    if (branch == null)
            //    {
            //        vehicleDistributions = vehicleDistributions.Where(x => x.FromBranchName == branch.Name).ToList();
            //    }                
            //}
            
            foreach (var item in assignedEquipments) 
            {
                var requestedEquipment = requestedEquipments.FirstOrDefault(x => x.VehicleDemandId == item.VehicleDemandId);
                var assignedEquip = db.AssignedEquipmentAdditionalInfos.FirstOrDefault(x => x.AssignedEquipmentId == item.AssignedEquipmentID);
                if (requestedEquipment != null)
                {
                    var vehicleDistribution = vehicleDistributions.FirstOrDefault(x => x.RequestID == requestedEquipment.RequestNumber);
                    if(vehicleDistribution != null)
                    {
                        var employee = db.Person_Employee?.Find(vehicleDistribution.EmployeeId);
                        var fleet = db.Fleets.Find(item.FleetId);
                        var equipmentCategory = db.EquipmentIdentity.Find(requestedEquipment.EquipmentIdentityID);
                        var equipmentType = db.EquipmentType.Find(requestedEquipment.EquipmentTypeID);
                        var equipName = db.EquipmentName.Find(requestedEquipment.EquipmentNameId);
                        list.Add(new EquipmentDistributionViewModel
                        {
                            Branch = vehicleDistribution.FromBranchName,
                            Department = vehicleDistribution.FromStructureName,
                            Destination = vehicleDistribution.Destination,
                            EmployeeName = employee?.FirstName + employee?.MiddleName + employee?.LastName,
                            EndDate = requestedEquipment.EndDate,
                            StartDate = requestedEquipment.StartDate,
                            EquipmentCategory = equipmentCategory?.EquipmentCategoryName,
                            EquipmentType = equipmentType?.EquipmentTypeName,
                            EquipmentName = equipName?.Name,
                            PlateNo = fleet?.PlateNumber,
                            EndOdoMeter = item.EndOdometer,
                            StartOdoMeter = item.StartOdometr,
                            NetOdoMeter = item.NetOdometer,
                            FromDeppot = assignedEquip?.FromDeppot ?? 0,
                            FromProject = assignedEquip?.FromProject ?? 0,
                            FuelRate = assignedEquip?.FuelRate ?? 0,
                            GreaseReq = assignedEquip?.GreaseReq ?? 0,
                            InCash = assignedEquip?.InCash ?? 0,
                            InCashLiter = assignedEquip?.InCashLiter ?? 0,
                            KmGrandTotal = assignedEquip?.KmGrandTotal ?? 0,
                            KmIntersite = assignedEquip?.KmGrandTotal ?? 0,
                            KmTraveledPerKm = assignedEquip?.KmGrandTotal ?? 0,
                            KmTraveledTotal = assignedEquip?.KmGrandTotal ?? 0,
                            OilReqForEngine = assignedEquip?.OilReqForEngine ?? 0,
                            OilReqForOther = assignedEquip?.OilReqForOther ?? 0,
                            OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == item.OperatorId)?.FullName,
                            ReqFuelInLiter = assignedEquip?.ReqFuelInLiter ?? 0,
                            Total = assignedEquip?.TotalBirr ?? 0,
                            TotalInLiter = assignedEquip?.TotalInLitre ?? 0,
                            TotalPerdium = assignedEquip?.TotalPerdium ?? 0,
                            
                        });
                    }
                }

            }

            return list;
        }
        
    }

    public class EquipmentDistributionViewModel
    {
        public string RequestNo { get; set; }
        public string EmployeeName { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string ReasonForTravel { get; set; }

        public string EquipmentCategory { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Destination { get; set; }

        public string PlateNo { get; set; }
        public string OperatorName { get; set; }
        public decimal StartOdoMeter { get; set; }
        public decimal EndOdoMeter { get; set; }
        public decimal NetOdoMeter { get; set; }

        public float KmTraveledPerKm { get; set; }
        public float KmTraveledTotal { get; set; }
        public float KmIntersite { get; set; }
        public float KmGrandTotal { get; set; }
        public float ReqFuelInLiter { get; set; }
        public float TotalInLiter { get; set; }
        public float FromDeppot { get; set; }
        public decimal InCashLiter { get; set; }
        public decimal FuelRate { get; set; }
        public decimal InCash { get; set; }
        public float FromProject { get; set; }
        public float OilReqForEngine { get; set; }
        public float OilReqForOther { get; set; }    
        public float GreaseReq { get; set; }    
        public decimal TotalPerdium { get; set; }    
        public decimal Total { get; set; }     
    }
}
