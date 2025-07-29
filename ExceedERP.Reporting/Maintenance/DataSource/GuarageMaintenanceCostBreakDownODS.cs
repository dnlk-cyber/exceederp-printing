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
    public class GuarageMaintenanceCostBreakDownODS
    {

        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GarageMaintenanceCostBreakDownVM> GetMVPmExecutionReport(string MCId, DateTime From, DateTime To)
        {
            var list = new List<GarageMaintenanceCostBreakDownVM>();

            if (string.IsNullOrEmpty(MCId))
                return list;

            var FetchServiceCenterJob = db.ServiceCenterJob
                .Where(x => x.MaintenanceCenterId.ToString() == MCId
                            && x.IsSubmittedToServiceCenter
                            && !x.IsExternalMaintenanceCenter
                            && x.IsSecondOnlineApproved
                            )
                .ToList();
            FetchServiceCenterJob = FetchServiceCenterJob
                .Where(x =>x.SecondOnlineApprovedTime.HasValue
                            && x.SecondOnlineApprovedTime.Value.Date >= From.Date
                            && x.SecondOnlineApprovedTime.Value.Date <= To.Date)
                .ToList();

            if (!FetchServiceCenterJob.Any())
                return list;

            foreach (var serviceCenterJob in FetchServiceCenterJob)
            {
                var FetchOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == serviceCenterJob.JobOrderID);
                if (FetchOrder == null) continue;

                var FetchedServiceCrew = db.ServiceCrewWorkLogs.FirstOrDefault(x => x.JobId == serviceCenterJob.ServiceCenterJobId);
                var fetchDetails = db.JobOrderServiceCenterDetails.Where(x => x.ServiceCenterJobId == serviceCenterJob.ServiceCenterJobId).ToList();
                decimal LabourCost = 0;
                decimal LabourHour = 0;
                foreach (var Detial in fetchDetails)
                {
                    var fetchActivity = db.RepairType.FirstOrDefault(x => x.RepairTypeID == Detial.RepairTypeID);
                    LabourCost += fetchActivity.TotalAmountCompany;
                    LabourHour += fetchActivity.AutomotiveStandaredHour;
                }
                if (FetchedServiceCrew == null) continue;

                var MaintenanceCost = new GarageMaintenanceCostBreakDownVM
                {
                    PlateNumber = serviceCenterJob.LicensePlate,
                    DateIn = serviceCenterJob.OnlineApprovedTime?.ToString("yyyy-MM-dd"),
                    DateClosed = serviceCenterJob.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd"),
                    LabCost = LabourCost,
                    RepairTime = FetchedServiceCrew.TotalLabourHours,
                    DownTime = db.FollowUp.FirstOrDefault(x => x.ServiceCrewWorkLogId == FetchedServiceCrew.ServiceCrewWorkLogId)?.DurationHr??"",
                    IsPM = FetchOrder.IsPM ? "✔" : " ",
                    IsCM = !FetchOrder.IsPM ? "✔" : " ",
                    Num=FetchOrder.Num
                };

                var FetchAdditional = db.AdditionalWorkShopDetails
                    .Where(x => x.ServiceCenterJobId == serviceCenterJob.ServiceCenterJobId)
                    .ToList();

                foreach (var additional in FetchAdditional)
                {
                    MaintenanceCost.OtherCost += additional?.OtherCosts ?? 0;
                }

                var serviceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == serviceCenterJob.ServiceCenterId);
                MaintenanceCost.ServiceCenter = serviceCenter?.Name ?? "Unknown";

                var FetchParent = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == FetchedServiceCrew.JobId);
                if (FetchParent == null) continue;

                MaintenanceCost.EquipmentType = FetchParent.EquipmentName;
                MaintenanceCost.MaintenanceCenter = serviceCenterJob.MaintenanceCenter;

                var FetchSparePart = db.StoreRequisitions
                    .FirstOrDefault(x => x.ServiceJobOrderNo == FetchParent.ServiceCenterJobId.ToString());

                if (FetchSparePart != null)
                {
                    var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == FetchSparePart.StoreRequisitionID);
                    if (fetchIssue != null)
                    {

                    
                    var FetchItems = db.IssueItems
                        .Where(x => x.IssueID == fetchIssue.IssueID)
                        .ToList();

                    foreach (var item in FetchItems)
                    {
                        if (item == null) continue;

                        var FetchCategory = db.ItemCategorys
                            .FirstOrDefault(x => x.ItemCategoryCode == item.ItemCategoryCode);

                        var categoryName = FetchCategory?.Name?.ToLower() ?? "";

                        bool isSparePart = ContainsThreeConsecutiveChars(categoryName, "spare part");
                        bool isLubricant = ContainsThreeConsecutiveChars(categoryName, "oil and lubricant");
                        bool isTyre = ContainsThreeConsecutiveChars(categoryName, "tire tyre");
                        bool isBattery = ContainsThreeConsecutiveChars(categoryName, "battery");
                        bool isFuel = ContainsThreeConsecutiveChars(categoryName, "fuel");

                        if (isSparePart)
                        {
                            MaintenanceCost.SparePartCost += item.UnitPrice*item.IssuedQuantity;
                        }
                        else if (isTyre)
                        {
                            MaintenanceCost.TyreCost += item.UnitPrice * item.IssuedQuantity;
                        }
                        else if (isFuel)
                        {
                            MaintenanceCost.FuelCost += item.UnitPrice * item.IssuedQuantity;
                        }
                        else if (isBattery)
                        {
                            MaintenanceCost.BatteryCost += item.UnitPrice * item.IssuedQuantity;
                        }
                        else if (isLubricant)
                        {
                            MaintenanceCost.LubricantCost += item.UnitPrice * item.IssuedQuantity;
                        }
                        else
                        {
                            MaintenanceCost.OtherCost += item.UnitPrice * item.IssuedQuantity;
                        }
                    }
                }
            }
                MaintenanceCost.TotalCost = MaintenanceCost.LubricantCost +
                                             MaintenanceCost.SparePartCost +
                                             MaintenanceCost.BatteryCost +
                                             MaintenanceCost.TyreCost +
                                             MaintenanceCost.FuelCost +
                                             MaintenanceCost.OtherCost +
                                             MaintenanceCost.LabCost;

                list.Add(MaintenanceCost);
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
    public class GarageMaintenanceCostBreakDownVM
    {
        public string EquipmentType { get; set; }
        public string Remark { get; set; }
        public string IsPM { get; set; }
        public string IsCM { get; set; }
        public string CurrentKMReading { get; set; }
        public string DatePerformed { get; set; }
        public string WorkShop { get; set; }
        public decimal LabCost { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal LubricantCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal TyreCost { get; set; }
        public decimal BatteryCost { get; set; }
        public decimal FuelCost { get; set; }
        public decimal TotalCost { get; set; }
        public string PlateNumber { get; set; }
        public string DateClosed { get; set; }
        public string DateIn { get; set; }
        public string ServiceCenter { get; set; }
        public string DownTime { get; set; }
        public decimal RepairTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Num { get; set; }
        public string MaintenanceCenter { get; set; }


    }
}
