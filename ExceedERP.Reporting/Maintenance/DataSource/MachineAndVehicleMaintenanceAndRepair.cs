using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.Core.Domain.Maintenance;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
   public class MachineAndVehicleMaintenanceAndRepair
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        //public List<MachineAndVehicleMaintenanceAndRepairVM> GetMaintainedMachines()
        //{
        //    List<MachineAndVehicleMaintenanceAndRepairVM> list = new List<MachineAndVehicleMaintenanceAndRepairVM>();

        //    var closedJobs = db.JobOrder
        //        .Where(x => x.Status == "Delivered" && x.IsSecondOnlineApproved == true)
        //        .ToList();

        //    foreach (var job in closedJobs)
        //    {
        //        var closedMachine = new MachineAndVehicleMaintenanceAndRepairVM();
        //        closedMachine.AF = null;

        //        var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == job.Referance);
        //        if (jobRequest == null) continue; // Skip if no matching JobRequest

        //        bool PMorCM = jobRequest.IsPM;
        //        closedMachine.PM = PMorCM ? "1" : null;
        //        closedMachine.CM = PMorCM ? null : "1";

        //         closedMachine.Out = job.SecondOnlineApprovedTime ?? DateTime.MinValue;
        //         closedMachine.In = closedMachine.Out;
        //        closedMachine.PlateNumber = jobRequest.LicensePlate;
        //        closedMachine.EquipmentType = null;

        //        decimal LabourHours = 0;

        //        var serviceCenterJob = db.ServiceCenterJob
        //            .FirstOrDefault(x => x.JobOrderID == job.JobOrderID &&
        //                                 x.IsOnlineApproved == true &&
        //                                 x.IsSecondOnlineApproved == true &&
        //                                 x.IsSentToWorkShop == true &&
        //                                 x.IsSubmittedToServiceCenter == true);

        //        if (serviceCenterJob != null)
        //        {
        //            var workLogs = db.ServiceCrewWorkLogs
        //                .Where(x => x.JobId == serviceCenterJob.ServiceCenterJobId &&
        //                            x.Status == MaintenanceConst.Approved)
        //                .ToList();

        //            LabourHours = workLogs.Sum(x => x.TotalLabourHours);

        //            var storeRequisition = db.StoreRequisitions
        //                .FirstOrDefault(x => x.ServiceJobOrderNo == serviceCenterJob.ServiceCenterJobId.ToString() &&
        //                                     x.IsForMaintenance == true &&
        //                                     x.IsFuel == false &&
        //                                     x.IsOnlineApproved == true);

        //            if (storeRequisition != null)
        //            {
        //                var storeItems = db.StoreRequisitionItems
        //                    .Where(x => x.StoreRequisitionID == storeRequisition.StoreRequisitionID)
        //                    .ToList();

        //                var itemCodes = storeItems.Select(x => x.ItemCode).ToList();
        //                var inventoryItems = db.InventoryItems
        //                    .Where(x => itemCodes.Contains(x.ItemCode))
        //                    .ToDictionary(x => x.ItemCode);

        //                foreach (var item in storeItems)
        //                {
        //                    if (inventoryItems.TryGetValue(item.ItemCode, out var inventoryItem))
        //                    {
        //                        decimal totalCost = inventoryItem.AvgPrice * item.IssuedQuantity;
        //                        var category = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == inventoryItem.ItemCategoryCode);

        //                        closedMachine.Category = category?.Name;
        //                        closedMachine.ItemName = inventoryItem.Name;
        //                        closedMachine.Cost = totalCost;
        //                    }
        //                }
        //            }
        //        }

        //        closedMachine.Repair = LabourHours;
        //        closedMachine.Down = 0;

        //        list.Add(closedMachine); // ✅ Add to list
        //    }

        //    return list;
        //}
        public List<MachineAndVehicleMaintenanceAndRepairVM> GetMaintainedMachines(DateTime From,DateTime To)
        {
            List<MachineAndVehicleMaintenanceAndRepairVM> list = new List<MachineAndVehicleMaintenanceAndRepairVM>();
            List<CategoryAndCode> CategoryItemList = new List<CategoryAndCode>();
            var closedJob = db.JobOrder.AsNoTracking().ToList();
           closedJob = closedJob.Where(x => x.Status == MaintenanceConst.Delivered && x.IsSecondOnlineApproved == true&&x.SecondOnlineApprovedTime>=From&&x.SecondOnlineApprovedTime<=To).ToList();
            if (!closedJob.Any())
            {
                var closedMachine = new MachineAndVehicleMaintenanceAndRepairVM();
                closedMachine.PlateNumber = "";
                closedMachine.DateIn = "";
                list.Add(closedMachine);
                return list;
            }
                
            foreach (var job in closedJob)
            {
                var closedMachine = new MachineAndVehicleMaintenanceAndRepairVM();
                closedMachine.AF = null;
                var FetchRequest=db.JobRequest.FirstOrDefault(x => x.Num == job.Referance);
                if (FetchRequest != null)
                {
                    bool PMorCM = FetchRequest.IsPM;


                    if (PMorCM == true)
                    {
                        closedMachine.PM = "1";
                        closedMachine.CM = null;
                    }
                    else
                    {
                        closedMachine.PM = null;
                        closedMachine.CM = "1";
                    }

                    // closedMachine.Out = job.SecondOnlineApprovedTime.Year.ToString()+"/"+ job.SecondOnlineApprovedTime.Month().ToString()+"/"+ job.SecondOnlineApprovedTime.Day();
                    closedMachine.DateOut = job.SecondOnlineApprovedTime.HasValue
                   ? job.SecondOnlineApprovedTime.Value.Year.ToString() + "/" +
                     job.SecondOnlineApprovedTime.Value.Month.ToString() + "/" +
                     job.SecondOnlineApprovedTime.Value.Day.ToString()
                   : " ";
                    var dateIn = db.JobRequest
                        .FirstOrDefault(x => x.Num == job.Referance)?
                    .DateCreated;
                    closedMachine.DateIn = dateIn.HasValue
                     ? dateIn.Value.Year.ToString() + "/" +
                       dateIn.Value.Month.ToString() + "/" +
                       dateIn.Value.Day.ToString()
                     : " ";
                    // closedMachine.Down =
                    decimal LabourHoure = 0;
                    var plate = db.JobRequest
                        .FirstOrDefault(x => x.Num == job.Referance).LicensePlate;
                    closedMachine.PlateNumber = plate;
                    var fetchFleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == plate);
                    if (fetchFleet != null)
                    {
                        closedMachine.EquipmentType = db.EquipmentType.SingleOrDefault(x => x.EquipmentTypeID == fetchFleet.EquipmentTypeID).EquipmentTypeName;
                        closedMachine.Category = db.EquipmentIdentity.SingleOrDefault(x => x.EquipmentIdentityID == fetchFleet.EquipmentIdentityID).EquipmentCategoryName;

                    }
                    var fetchFromServiceCenter = db.ServiceCenterJob.FirstOrDefault(x => x.JobOrderID == job.JobOrderID && x.IsOnlineApproved == true && x.IsSecondOnlineApproved == true && x.IsSentToWorkShop == true && x.IsSubmittedToServiceCenter == true);
                    if (fetchFromServiceCenter != null)
                    {
                        var fetchFromWorkSheet = db.ServiceCrewWorkLogs.Where(x => x.JobId == fetchFromServiceCenter.ServiceCenterJobId).ToList();

                        foreach (var workSheet in fetchFromWorkSheet)
                        {
                            LabourHoure += workSheet.TotalLabourHours;
                        }
                        var fetchFromStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == fetchFromServiceCenter.ServiceCenterJobId.ToString() && x.IsForMaintenance == true && x.IsFuel == false && x.IsOnlineApproved == true);
                        if (fetchFromStoreRequisition != null)
                        {
                            var fetchFromSpareAndMaterial = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == fetchFromStoreRequisition.StoreRequisitionID);
                            string CategoryName = null;
                            decimal CategoryCost = 0;

                            foreach (var item in fetchFromSpareAndMaterial)
                            {
                                CategoryAndCode CategoryItem = new CategoryAndCode();
                                var getItemCode = db.InventoryItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                                if (getItemCode != null)
                                {
                                    decimal singlePrice = getItemCode.AvgPrice;
                                    decimal issuedQuantity = item.IssuedQuantity;
                                    CategoryName = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == getItemCode.ItemCategoryCode).Name;
                                    CategoryItem.ItemCategry = CategoryName;
                                    CategoryItem.ItemName = getItemCode.Name;
                                    CategoryItem.ItemCost = Math.Round(singlePrice * issuedQuantity, 2);
                                    CategoryItemList.Add(CategoryItem);
                                }
                            }
                            var groupedItems = CategoryItemList.GroupBy(x => x.ItemCategry);
                            foreach (var group in groupedItems)
                            {
                                var fetchName = group.First();
                                closedMachine.ItemName = fetchName.ItemCategry;
                                closedMachine.Cost = group.Sum(x => x.ItemCost);
                            }


                        }

                    }

                    closedMachine.Repair = LabourHoure;
                    closedMachine.Down = 0;

                    list.Add(closedMachine);
                }

            }
            return list;
        }

    }

    public class MachineAndVehicleMaintenanceAndRepairVM
    {
        public string EquipmentType { get; set; }
        public string PlateNumber { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string PM { get; set; }
        public string CM { get; set; }
        public string AF { get; set; }
        public decimal Down { get; set; }
        public decimal Repair { get; set; }
        public string Category { get; set; }
        public string ItemName { get; set; }
        public decimal Cost { get; set; }
    }
    public class CategoryAndCode
    {
        public string ItemCategry { get; set; }
        public string ItemName { get; set; }
        public decimal ItemCost { get; set; }
        public decimal quantity { get; set; }
    }
}
