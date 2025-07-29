using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class OutSourcedMaintenanceReportODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OutSourcedMaintenanceReportVM> GetStandardTimeVsActualTime(string MCId,DateTime From,DateTime To)
        {
            List<OutSourcedMaintenanceReportVM> list = new List<OutSourcedMaintenanceReportVM>();
            List<CategoryAndCode> CategoryItemList = new List<CategoryAndCode>();
            if (!(Guid.TryParse(MCId, out Guid MaintenanceCId)))
            {
                return list;
            }
           
            var FetchJobOrders = db.JobOrder.Where(x => x.MaintenanceCenterId == MaintenanceCId && (DbFunctions.TruncateTime(x.OnlineApprovedTime)>=DbFunctions.TruncateTime(From.Date))&&(DbFunctions.TruncateTime(x.OnlineApprovedTime) <= DbFunctions.TruncateTime(To.Date))).ToList();          
                foreach (var order in FetchJobOrders)
                {
                var externalService = new OutSourcedMaintenanceReportVM();
                
                externalService.PlateNumber = order.LicensePlate;
                string xyz = null;
                var FetchChasis=db.Fleets.FirstOrDefault(x => x.FleetsID == order.FleetsID);
                if (FetchChasis != null)
                {
                    externalService.ChasisNumber =FetchChasis.ChasisNumber;
                    var fetchType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == FetchChasis.EquipmentTypeID);
                    if (fetchType != null)
                    {
                        externalService.EquipmentName = fetchType.EquipmentTypeName;
                    }
                    var fetchDetail = db.JobOrderDetail.Where(x => x.JobOrderID == order.JobOrderID).ToList();
                   
                    foreach (var detail in fetchDetail)
                    {
                        var fetchRepairType = db.RepairType.FirstOrDefault(x => x.RepairTypeID == detail.RepairTypeID);
                        if(fetchRepairType!=null)
                        {
                            xyz = xyz + " " + fetchRepairType.Name;
                        }
                    }
             
                }
                externalService.MaintenanceDetail = xyz;
                externalService.DateOut = order.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd");
                externalService.DateIn = order.OnlineApprovedTime?.ToString("yyyy-MM-dd");
                    
                

                var Businessunit = new BusinessUnit();
                var FetchMC = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == MaintenanceCId);
                if(FetchMC!=null)
                {
                    var FetchBU = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == FetchMC.BusinessUnitId);
                    if (FetchBU != null)
                    {
                      externalService.Location = FetchBU.Name;

                    }
                }
              
                externalService.GarageName = db.MaintenanceCenter.FirstOrDefault(x=>x.MaintenanceCenterID.ToString()==MCId)?.Name??"";
                externalService.Remark = "";
                externalService.Status = order.Status;
            //    var fetchFromStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == ServiceJob.ServiceCenterJobId.ToString() && x.IsForMaintenance == true && x.IsFuel == false && x.IsOnlineApproved == true);
            //    if (fetchFromStoreRequisition != null)
            //    {
            //        var fetchFromSpareAndMaterial = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == fetchFromStoreRequisition.StoreRequisitionID);
            //        string CategoryName = null;
            //        decimal CategoryCost = 0;

            //        foreach (var item in fetchFromSpareAndMaterial)
            //        {
            //            CategoryAndCode CategoryItem = new CategoryAndCode();
            //            var getItemCode = db.InventoryItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
            //            if (getItemCode != null)
            //            {
            //                decimal singlePrice = getItemCode.AvgPrice;
            //                decimal issuedQuantity = item.IssuedQuantity;
            //                int IssuedQuantity = issuedQuantity.;
            //                CategoryName = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == getItemCode.ItemCategoryCode).Name;
            //                CategoryItem.ItemCategry = CategoryName;
            //                CategoryItem.ItemName = getItemCode.Name;
            //                CategoryItem.quantity = issuedQuantity;
            //                CategoryItemList.Add(CategoryItem);


            //            }
            //        }

            //        var groupedItems = CategoryItemList.GroupBy(x => x.ItemCategry);
            //        foreach (var group in groupedItems)
            //        {
            //            var fetchName = group.First();
            //            externalService.ComponentName = fetchName.ItemCategry;
            //            externalService.Qty = group.Sum(x => x.quantity);
            //        }


            //    }



            }


            return list;
        }


    }


    public class OutSourcedMaintenanceReportVM
    {

        public string EquipmentName { get; set; }
        public string PlateNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string ComponentName { get; set; }
        public decimal Qty { get; set; }
        public string MaintenanceStatus { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string GarageName { get; set; }
        public string MaintenanceCenter { get; set; }
        public string ContactPhone { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string MaintenanceDetail { get; set; }
        


    }
}
