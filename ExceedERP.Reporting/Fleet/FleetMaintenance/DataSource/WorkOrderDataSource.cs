using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{

    [DataObject]
    public class WorkOrderDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly IJobOrderService _JobOrderService = new JobOrderService();
        private readonly IJobOrderDetailService JobOrderDetailService = new JobOrderDetailService();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkOrderViewModel> GetWorkOrder(DateTime StartingDate, DateTime EndingDate, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
        {
            List<WorkOrderViewModel> WorkOrderList = new List<WorkOrderViewModel>();
            WorkOrderViewModel JobOrderModel;

            FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var InventoryMapping = db.InventorySparePartMapping.ToList();
            var Equipments = db.Fleets.ToList();


             Equipments = Equipments.FilterFleet(Category, Type, EquipmentName, PlateNo, ChasisNo, Serial, string.Empty, Location, Model);


            var JobOrders = db.JobOrder.Where(x => x.DateIssued >= StartingDate && x.DateIssued <= EndingDate).ToList();



            foreach (var Equipment in Equipments)
            {

                var jobs = JobOrders.Where(x => x.FleetsID == Equipment.FleetsID).ToList();

                foreach (var job in jobs)
                {



                    decimal TotalTireCost = 0;
                    decimal TotalSpareCost = 0;
                    decimal TotalBatteryCost = 0;
                    decimal TotalOilLubricantCost = 0;
                    decimal TotalOtherCost = 0;
                    decimal TotalLabourCost = 0;
                    double TotalHr = 0;
                    decimal TotalSalvageCost = 0;

                    var OrderLine = db.JobOrder.Where(x => x.JobOrderID == job.JobOrderID).FirstOrDefault();
                    if (!OrderLine.IsOnlineTransferred)
                    {
                        TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                        TotalHr = Duration.TotalHours;
                    }
                    else if (OrderLine.IsOnlineTransferred)
                    {
                        TimeSpan Duration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                        TotalHr = Duration.TotalHours;
                    }



                    TotalLabourCost = fleetCostHelper.GetSingleJobLabourCost(job);
                    TotalSpareCost = fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Spare);
                    TotalTireCost = fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Tyre);
                    TotalSalvageCost = fleetCostHelper.GetSalvageSpareCost(job.JobOrderID);
                    TotalOilLubricantCost = fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Oil_Lubricant);
                    TotalOtherCost = fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Other);
                    TotalBatteryCost = fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Battery);

                    JobOrderModel = new WorkOrderViewModel
                    {
                        EquipmentName = job.Fleets.UnitNum,
                        PlateNo = job.EquipmentNo,
                        WorkNum = job.Num,
                        Duration = TotalHr/24,
                        TyreCost = TotalTireCost,
                        SpareCost = TotalSpareCost,
                        SalvageCost = TotalSalvageCost,
                        OilCost = TotalOilLubricantCost,
                        BatteryCost = TotalBatteryCost,
                        LabourCost = TotalLabourCost,
                        TotalCost = TotalOtherCost + TotalLabourCost + TotalOilLubricantCost + TotalSalvageCost + TotalSpareCost + TotalTireCost
                    };




                    WorkOrderList.Add(JobOrderModel);




                }



            }


            return WorkOrderList;
        }


        #region Support Method
        #endregion



    }
}
