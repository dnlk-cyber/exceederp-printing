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
    public class WeeklyPMCMPerformanceReportODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WeeklyPMCMPerformanceReportVM> GetplanVactual(string GLPeriod, string GLFiscalYear, DateTime From, DateTime To)
        {
            List<WeeklyPMCMPerformanceReportVM> list = new List<WeeklyPMCMPerformanceReportVM>();

            if (!int.TryParse(GLPeriod, out int GLPeriodId))
                return list;

            if (!int.TryParse(GLFiscalYear, out int FiscalYear))
                return list;

            var FetchPlan = db.MaintenancePlans.Where(x => x.GlFiscalYearId == FiscalYear).ToList();

            if (!FetchPlan.Any())
                return list;

            foreach (var plan in FetchPlan)
            {
                var FetchPeriod = db.MaintenancePlanDetails
                    .FirstOrDefault(x => x.MaintenancePlanId == plan.MaintenancePlanId && x.GlPeriodId == GLPeriodId);

                if (FetchPeriod == null)
                    continue;

                WeeklyPMCMPerformanceReportVM WPVAvm = new WeeklyPMCMPerformanceReportVM
                {
                    PMPlan = FetchPeriod.PMCost / 4,
                    CMPlan = FetchPeriod.CMCost / 4
                };

                var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == plan.EquipmentTypeId);
                WPVAvm.EquipmentType = equipmentType?.EquipmentTypeName ?? "N/A";

                var FetchJobOrder = db.JobOrder
                    .Where(x => x.DateCreated.HasValue &&
                                x.DateCreated.HasValue&&x.DateCreated.Value.Date > From.Date &&
                                x.DateCreated.HasValue && x.DateCreated.Value.Date <= To.Date)
                    .ToList();

                foreach (var Order in FetchJobOrder)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == Order.FleetsID);
                    var EquipmentIdentity = fleet?.EquipmentTypeID;

                    if (EquipmentIdentity != plan.EquipmentTypeId)
                        continue;

                    bool isApproved = Order.IsSecondOnlineApproved &&Order.SecondOnlineApprovedTime.HasValue&&Order.SecondOnlineApprovedTime.Value.Date <= To.Date && !Order.IsVoid;
                    bool isUnderRepair = !Order.IsSecondOnlineApproved && Order.IsOnlineApproved && !Order.IsVoid;

                    if (isApproved || isUnderRepair)
                    {
                        bool isPM = Order.IsPM;
                        if (isPM)
                        {
                            WPVAvm.PMQuantity += 1;
                            if (isUnderRepair) WPVAvm.TotalUnderRepair += 1;
                        }
                        else
                        {
                            WPVAvm.CMQuantity += 1;
                        }

                        WPVAvm.TotalQuantity += 1;

                        var FetchServices = db.ServiceCenterJob.Where(x => x.JobOrderID == Order.JobOrderID).ToList();
                        foreach (var Service in FetchServices)
                        {
                            var MiselenousCostObject = db.AdditionalWorkShopDetails
                                .Where(x => x.ServiceCenterJobId == Service.ServiceCenterJobId).ToList();

                            foreach (var AdditionalCost in MiselenousCostObject)
                            {
                                if (isPM)
                                    WPVAvm.PMMiselenousCost += AdditionalCost.OtherCosts;
                                else
                                    WPVAvm.CMMiselenousCost += AdditionalCost.OtherCosts;
                            }

                            var LabourCostObject = db.ServiceCrew
                                .FirstOrDefault(x => x.ServiceID == Service.ServiceCenterJobId);
                            decimal LabourCost = LabourCostObject?.TotalLabourCost ?? 0;

                            if (isPM)
                                WPVAvm.PMLabourCost += LabourCost;
                            else
                                WPVAvm.CMLabourCost += LabourCost;

                            var fetchSRequisition = db.StoreRequisitions
                                .FirstOrDefault(x => x.ServiceJobOrderNo == Service.ServiceCenterJobId.ToString());

                            if (fetchSRequisition != null)
                            {
                                var FetchStoreItem = db.StoreRequisitionItems
                                    .Where(x => x.StoreRequisitionID == fetchSRequisition.StoreRequisitionID).ToList();

                                foreach (var Item in FetchStoreItem)
                                {
                                    var FetchCategory = db.ItemCategorys
                                        .FirstOrDefault(x => x.ItemCategoryCode == Item.ItemCategoryCode);

                                    if (FetchCategory == null)
                                        continue;

                                    string categoryName = FetchCategory.Name?.ToLower() ?? "";

                                    if (Order.IsExternal)
                                    {
                                        WPVAvm.CMExternalCost += Item.CurrentAPrice;
                                    }
                                    else
                                    {
                                        bool isSparePart = ContainsThreeConsecutiveChars(categoryName, "spare part");
                                        bool isLubricant = ContainsThreeConsecutiveChars(categoryName, "oil and lubricant");
                                        bool isBattery = ContainsThreeConsecutiveChars(categoryName, "battery");
                                        bool isTyre = ContainsThreeConsecutiveChars(categoryName, "tyre tire");

                                        if (isSparePart)
                                        {
                                            if (isPM) WPVAvm.PMSparePartCost += Item.CurrentAPrice;
                                            else WPVAvm.CMSparePartCost += Item.CurrentAPrice;
                                        }
                                        else if (isLubricant)
                                        {
                                            if (isPM) WPVAvm.PMLubricantCost += Item.CurrentAPrice;
                                            else WPVAvm.CMLubricantCost += Item.CurrentAPrice;
                                        }
                                        else if (isBattery)
                                        {
                                            if (isPM) WPVAvm.PMBatteryCost += Item.CurrentAPrice;
                                            else WPVAvm.CMBatteryCost += Item.CurrentAPrice;
                                        }
                                        else if (isTyre)
                                        {
                                            if (isPM) WPVAvm.PMTyreCost += Item.CurrentAPrice;
                                            else WPVAvm.CMTyreCost += Item.CurrentAPrice;
                                        }
                                        else
                                        {
                                            WPVAvm.CMOtherCost += Item.CurrentAPrice;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                WPVAvm.PMActual = WPVAvm.PMBatteryCost + WPVAvm.PMLabourCost + WPVAvm.PMSparePartCost + WPVAvm.PMMiselenousCost + WPVAvm.PMLubricantCost;
                WPVAvm.CMActual = WPVAvm.CMTyreCost + WPVAvm.CMLabourCost + WPVAvm.CMSparePartCost + WPVAvm.CMOtherCost + WPVAvm.CMOutsourcedCost;
                WPVAvm.TotalCost = WPVAvm.PMActual + WPVAvm.CMActual;
                list.Add(WPVAvm);
            }

            return list;
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

    public class WeeklyPMCMPerformanceReportVM
    {
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public string Period { get; set; }
        public string PMMaterialCostPlan { get; set; }
        public string PMLabourCostPlan { get; set; }
        public string CMMaterialCostPlan { get; set; }
        public string CMLabourCostPlan { get; set; }
        public string CMQuantityPlan { get; set; }
        public string CMTotalCostPlan { get; set; }
        public string PMTotalCostPlan { get; set; }
        public string PMQuantityPlan { get; set; }
        public decimal PMMaterialCost { get; set; }
        public decimal PMLabourCost { get; set; }
        public int TotalQuantity { get; set; }
        public decimal PMSparePartCost { get; set; }
        public decimal PMLubricantCost { get; set; }
        public decimal PMBatteryCost { get; set; }
        public decimal PMPlan { get; set; }
        public decimal PMActual { get; set; }
        public decimal CMPlan { get; set; }
        public decimal CMActual { get; set; }
        public decimal CMBatteryCost { get; set; }
        public decimal PMTyreCost { get; set; }
        public decimal PMMiselenousCost { get; set; }
        public decimal CMMiselenousCost { get; set; }
        public decimal CMExternalCost { get; set; }
        public decimal PMExternalCost { get; set; }
        public decimal CMMaterialCost { get; set; }
        public decimal CMSparePartCost { get; set; }
        public decimal CMLubricantCost { get; set; }
        public decimal CMOtherCost { get; set; }
        public decimal CMOutsourcedCost { get; set; }
        public decimal CMTyreCost { get; set; }
        public decimal CMMShopCost { get; set; }
        public decimal CMLabourCost { get; set; }
        public decimal TotalCost { get; set; }
        public int CMQuantity { get; set; }
        public decimal CMTotalCost { get; set; }
        public int TotalUnderRepair { get; set; }
        public string PMTotalCost { get; set; }
        public string PMQuantity { get; set; }
       
 

    }
}
