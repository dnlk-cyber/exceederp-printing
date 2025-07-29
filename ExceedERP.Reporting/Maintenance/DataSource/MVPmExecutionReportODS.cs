using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;
using System.ComponentModel;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class MVPmExecutionReportODS
    {

        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MVPmExecutionReportVM> GetMVPmExecutionReport(string MCId, DateTime From, DateTime To)
        {
            List<MVPmExecutionReportVM> list = new List<MVPmExecutionReportVM>();

            if (string.IsNullOrEmpty(MCId)) return list;

            var FetchServiceCenterJob = db.ServiceCenterJob
                .Where(x => x.MaintenanceCenterId.ToString() == MCId
                            && x.IsSubmittedToServiceCenter == true
                            && x.IsExternalMaintenanceCenter == false
                            && x.IsSecondOnlineApproved == true)
                .ToList();

            if (FetchServiceCenterJob.Any())
            {
                FetchServiceCenterJob = FetchServiceCenterJob
                    .Where(x => x.SecondOnlineApprovedTime.HasValue
                                && x.SecondOnlineApprovedTime.Value.Date >= From.Date
                                && x.SecondOnlineApprovedTime.Value.Date <= To.Date)
                    .ToList();
            }

            foreach (var serviceCenterJob in FetchServiceCenterJob)
            {
                var FetchOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == serviceCenterJob.JobOrderID);
                if (FetchOrder != null && FetchOrder.IsPM)
                {
                    var FetchedServiceCrew = db.ServiceCrewWorkLogs.FirstOrDefault(x => x.JobId == serviceCenterJob.ServiceCenterJobId);
                    if (FetchedServiceCrew == null) continue;

                    MVPmExecutionReportVM MaintenanceCost = new MVPmExecutionReportVM
                    {
                        PlateNumber = serviceCenterJob.LicensePlate,
                        DateClosed = serviceCenterJob.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd"),
                        LabCost = FetchedServiceCrew.TotalLabourCost
                    };

                    var FetchParent = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == FetchedServiceCrew.JobId);
                    if (FetchParent != null)
                    {
                        var fetchEquipmentType = db.Fleets.FirstOrDefault(x => x.FleetsID == FetchOrder.FleetsID);
                        if(fetchEquipmentType!=null)
                        {
                            var fetchEquipmentTypeName = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fetchEquipmentType.EquipmentTypeID);
                            if(fetchEquipmentTypeName!=null)
                            {
                                MaintenanceCost.EquipmentType = fetchEquipmentTypeName.EquipmentTypeName;
                            }
                        }
                       // MaintenanceCost.EquipmentType = FetchParent.EquipmentName;
                        MaintenanceCost.MaintenanceCenter = serviceCenterJob.MaintenanceCenter;
                        var FetchSparePart = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == FetchParent.ServiceCenterJobId.ToString());
                        if (FetchSparePart != null)
                        {
                            var FetchItems = db.StoreRequisitionItems
                                .Where(x => x.StoreRequisitionID == FetchSparePart.StoreRequisitionID)
                                .ToList();

                            foreach (var Item in FetchItems)
                            {
                                var FetchCategory = db.ItemCategorys
                                    .FirstOrDefault(x => x.ItemCategoryCode == Item.ItemCategoryCode);
                                if (FetchCategory != null)
                                {
                                    string categoryName = FetchCategory.Name?.ToLower() ?? "";

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
                            }
                        }

                        MaintenanceCost.TotalCost = MaintenanceCost.LubricantCost + MaintenanceCost.SparePartCost + MaintenanceCost.OtherCost + MaintenanceCost.LabCost;
                        MaintenanceCost.DatePerformed = FetchParent.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd");
                        list.Add(MaintenanceCost);
                    }
                }
            }

            return list;
        }

        private bool ContainsThreeConsecutiveChars(string input, string target)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(target)) return false;

            for (int i = 0; i < target.Length - 2; i++)
            {
                string subStr = target.Substring(i, 3);
                if (input.Contains(subStr))
                {
                    return true;
                }
            }
            return false;
        }

       
    }
    public class MVPmExecutionReportVM
    {
        public string EquipmentType { get; set; }
        public string Remark { get; set; }
        public string CurrentKMReading { get; set; }
        public string DatePerformed { get; set; }
        public string WorkShop { get; set; }
        public decimal LabCost { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal TotalCost { get; set; }
        public string PlateNumber { get; set; }
        public string DateClosed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string MaintenanceCenter { get; set; }

    }
}
