using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
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

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{

    [DataObject]
    public class EquipmentTypeCostSummaryDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        IJobOrderDetailService JobOrderDetailService = new JobOrderDetailService();
   
    

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentTypeCostViewModel> GetTypeCostInfo(DateTime StartingDate, DateTime EndingDate, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
        {
            List<EquipmentTypeCostViewModel> FleetList = new List<EquipmentTypeCostViewModel>();
            EquipmentTypeCostViewModel JobOrderModel;
            GLPeriod Periods = new GLPeriod();
            FleetCostHelper fleetCostHelper = new FleetCostHelper();

            var InventoryMapping = db.InventorySparePartMapping.ToList();

            var Fleets = db.Fleets.ToList();

            List<JobOrder> Jobs = db.JobOrder.Where(x => x.DateIssued >= StartingDate && x.DateIssued <= EndingDate).ToList();

            IEquipmentTypeService _EquipmentTypeService = new EquipmentTypeService();

            var EquipmentTypes = _EquipmentTypeService.GetAll();
             foreach (var EquipmentType in EquipmentTypes)
            {
                 
                decimal TotalTireCost = 0;
                decimal TotalSpareCost = 0;
                decimal TotalOilLubricantCost = 0;
                decimal TotalOtherCost = 0;
                decimal TotalLabourCost = 0;
                decimal TotalSalvageCost = 0;

               

                    foreach (var job in Jobs.Where(x => x.EquipmentTypeId == EquipmentType.EquipmentIdentityID).ToList())
                    {


                        TotalLabourCost = TotalLabourCost + fleetCostHelper.GetSingleJobLabourCost(job);
                        TotalSpareCost = TotalSpareCost + fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Spare); ;
                        TotalTireCost = TotalTireCost + fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Tyre); ;
                        TotalSalvageCost = TotalSalvageCost + fleetCostHelper.GetSalvageSpareCost(job.JobOrderID);
                        TotalOilLubricantCost = TotalOilLubricantCost + fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Oil_Lubricant); ;
                        TotalOtherCost = TotalOtherCost + fleetCostHelper.GetJobOrderSpareCost(job, InventoryMapping, SpareType.Other);
                    }
                

                JobOrderModel = new EquipmentTypeCostViewModel();

                JobOrderModel.EquipmentType = EquipmentType.EquipmentTypeName;
                JobOrderModel.TyreCost = TotalTireCost;
                JobOrderModel.SpareCost = TotalSpareCost;
                JobOrderModel.SalvageCost = TotalSalvageCost;
                JobOrderModel.OilCost = TotalOilLubricantCost;
                JobOrderModel.LabourCost = TotalLabourCost;
                JobOrderModel.TotalCost = TotalOtherCost + TotalLabourCost + TotalOilLubricantCost + TotalSalvageCost + TotalSpareCost + TotalTireCost;

                FleetList.Add(JobOrderModel);




            }



            return FleetList;
        }
  


    }
}
