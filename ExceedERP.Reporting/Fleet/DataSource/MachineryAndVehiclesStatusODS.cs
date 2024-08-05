using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class MachineryAndVehiclesStatusODS
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SubTypeViewModel> GetOwnMachinery(string type, string status, string categoryId, string subCategory)
        {
            List<SubTypeViewModel> list = new List<SubTypeViewModel>();
            bool.TryParse(type, out bool typeBool);
            #region Short Method
            //var equipmentTypes = db.EquipmentType.ToList();
            //foreach (var eqType in equipmentTypes)
            //{
            //    var record = new MachineryTypeViewmodel
            //    {
            //        MachineryAndVehicleType = eqType.EquipmentTypeName
            //    };
            //    List<Fleets> fleets = db.Fleets.Where(x => !x.IsExternal && x.EquipmentTypeID == eqType.EquipmentTypeID).ToList();


            //    foreach (var fleet in fleets)
            //    {
            //        MachineryAndVehicleDTO machineryRecord = new MachineryAndVehicleDTO
            //        {
            //            Model = fleet.Model,
            //            Make = fleet.Manufacturer,
            //            SerialPlateNo = fleet.SerialNo ?? fleet.PlateNumber,
            //            Operator = fleet.OperatorName,
            //            Status = fleet.Status,
            //            OwnRental = fleet.IsExternal.ToString(),
            //        };
            //        record.Machineries.Add(machineryRecord);
            //    }
            //    list.Add(record);
            //}
            #endregion
            
            var fleetList = db.Fleets.ToList();
            var equipmentTypeList = db.EquipmentType.ToList();
            if (type != null)
            {
                fleetList = fleetList.Where(x => x.IsExternal == typeBool).ToList();
            }
            if(status != null)
            {
                fleetList = fleetList.Where(x => x.Status.ToLower() == status.ToLower()).ToList();
            }
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                Guid.TryParse(categoryId, out Guid id);
                fleetList = fleetList.Where(x => x.EquipmentIdentityID == id).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(subCategory))
            {
                Guid.TryParse(subCategory, out Guid equipmentTypeId);
                fleetList = fleetList.Where(x => x.EquipmentTypeID == equipmentTypeId).ToList();
            }
            var typeByFleet = equipmentTypeList
                .GroupJoin(fleetList,
                            eT => eT.EquipmentTypeID,
                            ft => ft.EquipmentTypeID,
                            (equipmentType, fleets) => new
                            {
                                MachineryAndVehicleType = equipmentType,
                                Machineries = fleets
                            }).ToList();

            foreach (var item in typeByFleet)
            {
                if (item.Machineries.Any())
                {
                    var recordM = new SubTypeViewModel
                    {
                        MachineryAndVehicleType = item.MachineryAndVehicleType.EquipmentTypeName,
                       
                    };
                    
                    foreach (var fleet in item.Machineries)
                    {
                        
                        var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                        MachineryAndVehicleDTO machineryRecord = new MachineryAndVehicleDTO
                        {
                            EquipmentName = equipmentName?.Name,
                            Model = fleet.Model,
                            Make = fleet.Manufacturer,
                            SerialPlateNo = fleet?.SerialNo ?? fleet.PlateNumber,
                            Operator = fleet.OperatorName,
                            Status = fleet.Status,
                            Project = fleet.Location,
                            
                            //OwnRental = fleet.IsExternal.ToString(),
                        };
                        recordM.Machineries.Add(machineryRecord);
                    }

                    list.Add(recordM);
                }

            }


            return list;
        }


        
    }
}
