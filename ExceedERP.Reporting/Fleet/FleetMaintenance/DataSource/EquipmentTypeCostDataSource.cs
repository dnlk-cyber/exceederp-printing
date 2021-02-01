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

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{ 

  [DataObject]
    public class EquipmentTypeCostDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();

        IJobOrderDetailService JobOrderDetailService = new JobOrderDetailService();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetEquipmentType()
        {

            var result = db.EquipmentIdentity.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.EquipmentCategoryName,
                Value = e.EquipmentIdentityID.ToString()

            }
            ).ToList();

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentTypeCostViewModel> GetTypeCostInfo(DateTime StartingDate, DateTime EndingDate,string Category, string Type , string EquipmentName ,string PlateNo,string Model , string ChasisNo, string Serial , string Location)
        {
            List<EquipmentTypeCostViewModel> FleetList = new List<EquipmentTypeCostViewModel>();
            EquipmentTypeCostViewModel JobOrderModel;
              GLPeriod Periods = new GLPeriod();
            FleetCostHelper fleetCostHelper = new FleetCostHelper();




            List<JobOrder> Jobs = db.JobOrder.Where(x => x.DateIssued >= StartingDate && x.DateIssued <= EndingDate).ToList();
            var InventorySparePartMapping = db.InventorySparePartMapping.ToList();

            if (!string.IsNullOrEmpty(Type))
            {
                Jobs = Jobs.Where(x => x.EquipmentTypeId.ToString() == Type).ToList();
            }
            foreach (var job in Jobs)
            {



                decimal TotalTireCost = 0;
                decimal TotalSpareCost = 0;
                decimal TotalOilLubricantCost = 0;
                decimal TotalOtherCost = 0;
                decimal TotalLabourCost = 0;
                 decimal TotalSalvageCost = 0;
 


                TotalLabourCost = fleetCostHelper.GetSingleJobLabourCost(job.JobOrderID);
                TotalSpareCost = fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping,SpareType.Spare);
                TotalTireCost = fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Tyre);
                TotalSalvageCost = fleetCostHelper.GetSalvageSpareCost(job.JobOrderID);
                TotalOilLubricantCost = fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Oil_Lubricant);
                TotalOtherCost = fleetCostHelper.GetJobOrderSpareCost(job, InventorySparePartMapping, SpareType.Other);

                JobOrderModel = new EquipmentTypeCostViewModel();
 
                JobOrderModel.EquipmentType = job.EquipmentType;
                JobOrderModel.EquipmentName = job.Fleets.EquipmentName;
                JobOrderModel.PlateNo = job.Fleets.PlateNumber;
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
