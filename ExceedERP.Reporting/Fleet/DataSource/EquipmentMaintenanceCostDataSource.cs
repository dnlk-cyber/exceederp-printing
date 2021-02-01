using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentMaintenanceCostDataSource
    {


        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public  List<MaintenaceCostViewModel> GetMaintenanceCost(DateTime StartingDate, DateTime EndingDate, string EquipmentType, string Project, string EquipmentCategory, string EquipmentName, string status, string PlateNo, string Model, string ChasisNo, string serialNo)
        {
            FleetCostHelper FleetCostHelper = new FleetCostHelper();
            FleetReportHelper FleetReportHelper = new FleetReportHelper();
            List<MaintenaceCostViewModel> MaintenaceCostList = new List<MaintenaceCostViewModel>();
            var Issues = FleetReportHelper.GetInventoryIssue(x=>x.DateCreated >= StartingDate && x.DateCreated <= EndingDate);
            var InventorySparePartMappings = db.InventorySparePartMapping.ToList();
            List<EquipmentIdentity> EquipmentTypes = db.EquipmentIdentity.ToList();
 
            GLPeriod Periods = new GLPeriod();
            EquipmentIdentity Type = new EquipmentIdentity();
            List<Fleets> Equipments = db.Fleets.ToList();




             #region for ShardedDataSource
            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            #endregion

            Equipments = Equipments.FilterFleet(EquipmentCategory, EquipmentType,EquipmentName, PlateNo, ChasisNo, serialNo, string.Empty, Project, Model);


         var   JobOrders = db.JobOrder.Where(x => x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();

 

                foreach (var Equipment in Equipments)
                {
                  
                    var Orders = JobOrders.Where(x => x.FleetsID == Equipment.FleetsID || x.EquipmentNo == Equipment.PlateNumber).ToList();
                    var Jobrequests = db.JobRequest.Where(x => x.FleetsID == Equipment.FleetsID && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();
                   foreach (var Jobrequest in Jobrequests)
                {
                    var order = db.JobOrder.Where(x => x.Referance == Jobrequest.Num).FirstOrDefault();
                       if(order!= null)
                       {
                           Orders.Add(order);
                       }
                   } 
                    
                    var MaintenaceCostViewModel = new MaintenaceCostViewModel();
                  



                            MaintenaceCostViewModel.BatteryRepairCost =   FleetCostHelper.GetEquipmentSpareCost(Orders, InventorySparePartMappings,Core.Domain.FleetManagement.SpareType.Battery);
                            MaintenaceCostViewModel.OilRepairCost =   FleetCostHelper.GetEquipmentSpareCost(Orders, InventorySparePartMappings, Core.Domain.FleetManagement.SpareType.Oil_Lubricant);
                            MaintenaceCostViewModel.OtherRepairCost =   FleetCostHelper.GetEquipmentSpareCost(Orders, InventorySparePartMappings, Core.Domain.FleetManagement.SpareType.Other);
                            MaintenaceCostViewModel.SpareRepairCost =   FleetCostHelper.GetEquipmentSpareCost(Orders, InventorySparePartMappings, Core.Domain.FleetManagement.SpareType.Spare);
                            MaintenaceCostViewModel.TyreRepairCost =   FleetCostHelper.GetEquipmentSpareCost(Orders,  InventorySparePartMappings, Core.Domain.FleetManagement.SpareType.Tyre);
                             MaintenaceCostViewModel.LabourCost = FleetCostHelper.GetLabourCost(Orders);

                            MaintenaceCostViewModel.EquipmentCategory = Equipment.EquipmentIdentity.EquipmentCategoryName;
                            MaintenaceCostViewModel.EquipmentType = Equipment.EquipmentType.EquipmentTypeName;
                            MaintenaceCostViewModel.AssetCode = Equipment.UnitNum;
                            MaintenaceCostViewModel.PlateNo = Equipment.PlateNumber;
                             MaintenaceCostViewModel.AmharicName = AmharicName;
                             MaintenaceCostViewModel.Name = Name;
                             MaintenaceCostViewModel.LogoFileName = LogoFileName;
                             MaintenaceCostViewModel.Address = Address;

                                MaintenaceCostList.Add(MaintenaceCostViewModel);
                        }
            




 

            if (!MaintenaceCostList.Any())
            {
                var MaintenaceCostViewModel = new MaintenaceCostViewModel();
                MaintenaceCostViewModel.AmharicName = AmharicName;
                MaintenaceCostViewModel.Name = Name;
                MaintenaceCostViewModel.LogoFileName = LogoFileName;
                MaintenaceCostViewModel.Address = Address;

                MaintenaceCostList.Add(MaintenaceCostViewModel);
            }

            return MaintenaceCostList;

        }

    }
}
