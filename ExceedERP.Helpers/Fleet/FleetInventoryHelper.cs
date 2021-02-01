using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Fleet
{
    public class FleetInventoryHelper
    {

        public async void AssignItemToEquipment(Issue model)
        {
            ExceedDbContext db = new ExceedDbContext();

            IFleetService _FleetService = new FleetService();

            if (!string.IsNullOrWhiteSpace(model.PlateNo))
            {
                var FleetInventoryMaps = db.InventorySparePartMapping.ToList();


                var Fleet = _FleetService.GetAll().Where(x => x.PlateNumber == model.PlateNo).FirstOrDefault();

                if (model.IssueItems.Any() && Fleet != null)
                {


                    foreach (var m in model.IssueItems)
                    {

                        var Map = FleetInventoryMaps.Where(x => x.InventoryCategoryId == m.InventoryItem.ItemCategoryCode).FirstOrDefault();
                        if (Map.SpareType == SpareType.Battery)
                        {
                            EquipmentBattery Equipment_Battery = new EquipmentBattery()
                            {
                                EquipmentBatteryId = Guid.NewGuid(),
                                StoreCode = model.StoreCode,
                                SerialNo = m.InventoryItem.PartNo,
                                UnitPrice = m.InventoryItem.TotalPrice,
                                DateCreated = DateTime.Now,
                                Isattached = true,
                                Condition = "New",
                            };
                            db.EquipmentBattery.Add(Equipment_Battery);
                            await db.SaveChangesAsync();
                        }


                        if (Map.SpareType == SpareType.Tyre)
                        {
                            EquipmentTyre Equipment_Tyre = new EquipmentTyre()
                            {
                                EquipmentTyreId = Guid.NewGuid(),

                                StoreCode = model.StoreCode,
                                SerialNo = m.InventoryItem.PartNo,
                                UnitPrice = m.InventoryItem.TotalPrice,
                                OdoMeter = Fleet.CurentOdoMeter,
                                DateCreated = DateTime.Now,
                                Isattached = true,
                                Condition = "New",
                            };
                            db.EquipmentTyres.Add(Equipment_Tyre);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}
