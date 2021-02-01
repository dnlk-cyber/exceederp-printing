using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{

    [DataObject]
    public class SummaryDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        IJobOrderDetailService JobOrderDetailService = new JobOrderDetailService();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SummaryViewModel> GetSummaryInfo(DateTime StartingDate, DateTime EndingDate, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
          {
              List<SummaryViewModel> FleetList = new List<SummaryViewModel>();
              SummaryViewModel JobOrderModel;
            

              decimal TotalEquipment = 0;
              decimal TotalTireCost = 0;
              decimal TotalSpareCost = 0;
              decimal TotalOilLubricantCost = 0;
              decimal TotalOtherCost = 0;
              decimal TotalBatteryCost = 0;
              decimal TotalLabourCost = 0;
              decimal TotalNoCrew = 0;
              decimal TotalHr = 0;
              decimal TotalSalvageCost = 0;
              decimal TotalExternalCost = 0;
              int TotalDuration = 0;

              FleetCostHelper fleetCostHelper = new FleetCostHelper();




              var InventorySparePartMapping = db.InventorySparePartMapping.ToList();

              List<JobOrder> jobs = db.JobOrder.Where(x => x.DateIssued >= StartingDate && x.DateIssued <= EndingDate && x.IsExternal == false).ToList();

              foreach (var job in jobs)
              {

                  TotalDuration = 0;
                  if (FleetGenericHelper.FilterSingleFleet(job.FleetsID, Category, Type, PlateNo, ChasisNo, Serial, string.Empty, Location, Model))
                  {
                      var OrderLine = job;
                      if (!OrderLine.IsOnlineTransferred)
                      {
                          TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                          TotalDuration = (int)Duration.TotalHours;
                      }
                      else if (OrderLine.IsOnlineTransferred)
                      {
                          TimeSpan Duration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                          TotalDuration = (int)Duration.TotalHours;
                      }

                      TotalLabourCost += fleetCostHelper.GetSingleJobLabourCost(job.JobOrderID);
                      TotalSpareCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Spare);
                      TotalTireCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Tyre);
                      TotalSalvageCost += fleetCostHelper.GetSalvageSpareCost(job.JobOrderID);
                      TotalOilLubricantCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Oil_Lubricant);
                      TotalOtherCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Other);
                      TotalBatteryCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Battery);
                      TotalExternalCost+=  fleetCostHelper.GetComponentCost(job.JobOrderID);
                      TotalEquipment = TotalEquipment + 1;
                      TotalHr = TotalHr + TotalDuration;






                    

                  }
              }
              JobOrderModel = new SummaryViewModel();


              JobOrderModel.Type = "INTERNAL";
              JobOrderModel.TotalEquipment = TotalEquipment;
              JobOrderModel.TotalHr = TotalHr;
              JobOrderModel.TotalTireCost = TotalTireCost;
              JobOrderModel.TotalNoCrew = TotalNoCrew;
              JobOrderModel.TotalSpareCost = TotalSpareCost;
              JobOrderModel.TotalSalvageCost = TotalSalvageCost;
              JobOrderModel.TotalOilLubricantCost = TotalOilLubricantCost;
              JobOrderModel.TotalLabourCost = TotalLabourCost;
              JobOrderModel.TotalOtherCost = TotalOtherCost;
              JobOrderModel.TotalBatteryCost = TotalBatteryCost;
              JobOrderModel.ExternalCost = TotalExternalCost;
              JobOrderModel.TotalCost = TotalOtherCost + TotalLabourCost + TotalOilLubricantCost + TotalSalvageCost + TotalSpareCost + TotalTireCost;


              FleetList.Add(JobOrderModel);

                  TotalEquipment = 0;
                  TotalTireCost = 0;
                  TotalSpareCost = 0;
                  TotalOilLubricantCost = 0;
                  TotalOtherCost = 0;
                  TotalLabourCost = 0;
                  TotalNoCrew = 0;
                  TotalHr = 0;
                  TotalSalvageCost = 0;
                  TotalDuration = 0;
              TotalExternalCost = 0;

                  jobs = db.JobOrder.Where(x => x.DateIssued >= StartingDate && x.DateIssued <= EndingDate && x.IsExternal == true).ToList();

                  foreach (var job in jobs)
                  {
                      if (FleetGenericHelper.FilterSingleFleet(job.FleetsID, Category, Type, PlateNo, ChasisNo, Serial, string.Empty, Location, Model))
                      {

                          TotalDuration = 0;


                          var OrderLine = job;
                          if (!OrderLine.IsOnlineTransferred)
                          {
                              TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                              TotalDuration = (int)Duration.TotalHours;
                          }
                          else if (OrderLine.IsOnlineTransferred)
                          {
                              TimeSpan Duration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                              TotalDuration = (int)Duration.TotalHours;
                          }




                          TotalEquipment = TotalEquipment + 1;
                          TotalHr = TotalHr + TotalDuration;
                          TotalLabourCost += fleetCostHelper.GetSingleJobLabourCost(job.JobOrderID);
                          TotalSpareCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Spare);
                          TotalTireCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Tyre);
                          TotalSalvageCost += fleetCostHelper.GetSalvageSpareCost(job.JobOrderID);
                          TotalOilLubricantCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Oil_Lubricant);
                          TotalOtherCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Other);
                          TotalBatteryCost += fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Battery);
                        TotalExternalCost+=  fleetCostHelper.GetComponentCost(job.JobOrderID);
              






                       

                      }
                  }
                  JobOrderModel = new SummaryViewModel();

                  JobOrderModel.Type = "EXTERNAL";
                  JobOrderModel.TotalEquipment = TotalEquipment;
                  JobOrderModel.TotalHr = TotalHr;
                  JobOrderModel.TotalTireCost = TotalTireCost;
                  JobOrderModel.TotalNoCrew = TotalNoCrew;
                  JobOrderModel.TotalSpareCost = TotalSpareCost;
                  JobOrderModel.TotalSalvageCost = TotalSalvageCost;
                  JobOrderModel.TotalOilLubricantCost = TotalOilLubricantCost;
                  JobOrderModel.TotalLabourCost = TotalLabourCost;
                  JobOrderModel.TotalOtherCost = TotalOtherCost;
                  JobOrderModel.TotalBatteryCost = TotalBatteryCost;
                  JobOrderModel.ExternalCost = TotalExternalCost;
                  JobOrderModel.TotalCost = TotalOtherCost + TotalLabourCost + TotalOilLubricantCost + TotalSalvageCost + TotalSpareCost + TotalTireCost;


                  FleetList.Add(JobOrderModel);

                  return FleetList;
              }





    }
}
