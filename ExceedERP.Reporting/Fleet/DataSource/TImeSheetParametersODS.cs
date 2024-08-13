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
    public class TImeSheetParametersODS
    {
        ExceedDbContext db = new ExceedDbContext();



        /// <summary>
        /// MachineryTimeSheet
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleetMachinery()
        {


            var list = from machineryWorkingTimeSheet in db.MachineryWorkingTimeSheets
                       join fleet in db.Fleets on machineryWorkingTimeSheet.FleetsID equals fleet.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.SerialNo + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID).Name
                       };
            return list.ToList();
            //var list = db.Fleets.Select(s => new FleetVM
            //{ 
            //    FleetsID = s.FleetsID.ToString(),
            //    PlateNumber = s.PlateNumber
            //});
            //return list.ToList();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleetLightVehicle()
        {


            var list = from lightVehicleTimeSheets in db.LightVehicleTimeSheets
                       join fleet in db.Fleets on lightVehicleTimeSheets.FleetsID equals fleet.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID).Name
                       };
            return list.ToList();
           
        }


        /// <summary>
        /// DryLoadingTimeSheet
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleetDryLoading()
        {

            var list = from fleet in db.Fleets
                       join dryLoading in db.DryLoadingTimeSheets
                       on fleet.FleetsID equals dryLoading.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID).Name
                       };

            return list.ToList();

            //var list = db.Fleets.Select(s => new FleetVM
            //{
            //    FleetsID = s.FleetsID.ToString(),
            //    PlateNumber = s.PlateNumber
            //});
            //return list.ToList();
        }


        /// <summary>
        /// DumpTruckTimeSheet
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleetDumpTruck()
        {


            var list = from dumpTruck in db.DumpTruckTimeSheets
                       join fleet in db.Fleets on dumpTruck.FleetsID equals fleet.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID).Name
                       };
            return list.ToList();
            //var list = db.Fleets.Select(s => new FleetVM
            //{
            //    FleetsID = s.FleetsID.ToString(),
            //    PlateNumber = s.PlateNumber
            //});
            //return list.ToList();
        }


        /// <summary>
        /// WagonDrillTimeSheet
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleetWagonDrill()
        {
            var list = from wagonDrill in db.WagonDrillTimesheets
                       join fleet in db.Fleets on wagonDrill.FleetsID equals fleet.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.SerialNo + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID).Name
                       };
            return list.ToList();
            //var list = db.Fleets.Select(s => new FleetVM
            //{
            //    FleetsID = s.FleetsID.ToString(),
            //    PlateNumber = s.PlateNumber
            //});
            //return list.ToList();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentCategory()
         {
            var list = db.EquipmentIdentity.Select(s => new GenericVm
            {
                Text = s.EquipmentCategoryName,
                Value = s.EquipmentIdentityID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentSubCategory(string categoryId)
        {
            Guid.TryParse(categoryId, out Guid id);
            var list = db.EquipmentType.Where(x => x.EquipmentIdentityID == id).Select(s => new GenericVm
            {
                Text = s.EquipmentTypeName,
                Value = s.EquipmentTypeID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetWeeks()
        {
            return new List<GenericVm>
            {
                new GenericVm {Value = "1", Text = "Week One"},
                new GenericVm {Value = "2", Text = "Week Two"},
                new GenericVm {Value = "3", Text = "Week Three"},
                new GenericVm {Value = "4", Text = "Week Four"},
            };
        }
        public class GenericVm
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}
