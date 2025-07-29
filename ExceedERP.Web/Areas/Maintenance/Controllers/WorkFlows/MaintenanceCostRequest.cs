using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;
using System.ComponentModel;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [DataObject]
    public class MaintenanceCostODS
    {

        
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaintenanceCostDTO> GetMaintenanceCost(string PlateNumber)
        {
            List<MaintenanceCostDTO> list = new List<MaintenanceCostDTO>();
            var FetchServiceCenterJob = db.ServiceCenterJob.Where(x => x.LicensePlate == PlateNumber).ToList();
            foreach(var serviceCenterJob in FetchServiceCenterJob)
            {
               var FetchedServiceCrew = db.ServiceCrewWorkLogs.FirstOrDefault(x=>x.JobId==serviceCenterJob.ServiceCenterJobId);
                  MaintenanceCostDTO MaintenanceCost = new MaintenanceCostDTO();
                    var FetchParent = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == FetchedServiceCrew.JobId);
                    MaintenanceCost.LabCost = FetchedServiceCrew.TotalLabourCost;
                    if (FetchParent != null)
                    {
                        MaintenanceCost.EquipmentType = FetchParent.EquipmentName;
                        MaintenanceCost.ServiceGroup = FetchedServiceCrew.ServiceGroup;
                        //var serviceName = db.JobOrderDetail
                        // .Select(s => new
                        // {
                        //     Name = s.RepairType.Name + "-" + s.JobCauses.Name
                        // })
                        // .FirstOrDefault();
                    var servicename = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == serviceCenterJob.ServiceCenterJobId).ServiceName;
                    //var serviceGroup = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == serviceCenterJob.ServiceCenterJobId).ServiceName;
                    MaintenanceCost.Service = servicename; // Handle null case
                        MaintenanceCost.WorkShop = serviceCenterJob.MaintenanceCenter;
                        var FetchSparePart = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == FetchParent.ServiceCenterJobId.ToString());
                        if (FetchSparePart != null)
                        {
                            var FetchItems = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == FetchSparePart.StoreRequisitionID);
                            if (FetchItems.Any())
                            {
                                foreach (var Item in FetchItems)
                                {
                                    var FetchCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == Item.ItemCategoryCode);
                                    if (FetchCategory != null)
                                    {
                                        string categoryName = FetchCategory.Name.ToLower(); // Convert to lowercase for case-insensitive comparison

                                        bool isSparePart = ContainsThreeConsecutiveChars(categoryName, "spare part");
                                        bool isLubricant = ContainsThreeConsecutiveChars(categoryName, "oil and lubricant");

                                        if (isSparePart)
                                        {
                                            MaintenanceCost.SparePartCost += Item.CurrentAPrice;
                                        }
                                        else if (isLubricant)
                                        {
                                            MaintenanceCost.LubricantCost += Item.CurrentAPrice;
                                        }
                                        else
                                        {
                                            MaintenanceCost.OtherCost += Item.CurrentAPrice;
                                        }
                                    }

                                    // Function to check if at least three consecutive characters match

                                }

                            }
                        }
                    
                    MaintenanceCost.TotalCost = MaintenanceCost.LubricantCost + MaintenanceCost.SparePartCost + MaintenanceCost.OtherCost + MaintenanceCost.LabCost;
                    list.Add(MaintenanceCost);
                }
            }

            return list ;
        }
        private bool ContainsThreeConsecutiveChars(string input, string target)
        {
            for (int i = 0; i < target.Length - 2; i++)
            {
                string subStr = target.Substring(i, 3); // Get 3 consecutive characters from target
                if (input.Contains(subStr))
                {
                    return true;
                }
            }
            return false;
        }


    }
    public class MaintenanceCostDTO
    {
        public string EquipmentType { get; set; }
        public string WorkCategory { get; set; }
        public ServiceGroup ServiceGroup { get; set; }
        public string Service { get; set; }
        public string WorkShop { get; set; }
        public decimal LabCost { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal TotalCost { get; set; }

    }
}
