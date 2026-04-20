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
    public class WeeklyPMCMPlanVsActualReportSummaryODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WeeklyPlanVsActualVM> GetplanVactual(string GLPeriod, string GLFiscalYear, string MCId, DateTime From, DateTime To, string EquipmentIdentityId, string EquipmentTypeId)
        {
            List<WeeklyPlanVsActualVM> list = new List<WeeklyPlanVsActualVM>();

            if (!int.TryParse(GLPeriod, out int GLPeriodId))
                return list;
            if (!int.TryParse(GLFiscalYear, out int FiscalYear))
                return list;
            var FetchPlan = db.MaintenancePlans.Where(x => x.GlFiscalYearId == FiscalYear).ToList();
            if (MCId != null)
            {
                FetchPlan = FetchPlan.Where(x => x.MaintenanceCenterId.ToString() == MCId).ToList();
            }
            if (!FetchPlan.Any())
                return list;
            //Guid? EquipmentIdentityID = null;
            //    if (Guid.TryParse(EquipmentIdentityId, out Guid parsedGuid))
            //{
            //    EquipmentIdentityID = parsedGuid;
            //}

            if (EquipmentIdentityId != null)
            {
                FetchPlan = FetchPlan
                    .Where(x => x.EquipmentCategoryId.ToString() == EquipmentIdentityId)
                    .ToList();
            }
            if (!FetchPlan.Any())
                return list;
            //Guid? EquipmentTypeID = null;

            //if (Guid.TryParse(EquipmentTypeId, out Guid parsedTypeId))
            //{
            //    EquipmentTypeID = parsedTypeId;
            //}

            if (EquipmentTypeId != null)
            {
                FetchPlan = FetchPlan.Where(x => x.EquipmentTypeId.ToString() == EquipmentTypeId).ToList();
            }
            if (!FetchPlan.Any())
                return list;
            var fetchPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == GLPeriod);
            if (fetchPeriod != null)
            {
                foreach (var plan in FetchPlan)
                {
                    var FetchPeriods = db.MaintenancePlanDetails
                        .FirstOrDefault(x => x.MaintenancePlanId == plan.MaintenancePlanId && x.GlPeriodId == GLPeriodId);

                    if (FetchPeriods == null)
                        continue;
                    var FetchPeriod = FetchPeriods;
                    TimeSpan interval = To.Date - From.Date;
                    TimeSpan MonthInterval = fetchPeriod.DateTo - fetchPeriod.DateFrom;
                    int MInterval = MonthInterval.Days + 1;
                    int DateInterval = interval.Days + 1;
                    int daysInMonth = DateTime.DaysInMonth(From.Year, From.Month);
                    int wkCM = 0; int wkPM = 0; int CMModulo = 0; int PMModulo = 0;
                    TimeSpan Daysdiference1 = From.Date - fetchPeriod.DateFrom;
                    TimeSpan Daysdiference2 = To.Date - fetchPeriod.DateFrom;
                    int xx = Daysdiference1.Days;
                    int yy = Daysdiference2.Days;

                    //if((fetchPeriod.DateFrom.Date==From.Date)&&(To.Date-fetchPeriod.DateFrom))
                    WeeklyPlanVsActualVM WPVAvm = new WeeklyPlanVsActualVM
                    {
                        PMPlan = (FetchPeriod.PMCost / MInterval) * DateInterval,
                        CMPlan = (FetchPeriod.CMCost / MInterval) * DateInterval,
                    };

                    CMModulo = FetchPeriod.CMQuantity % 4;
                    PMModulo = FetchPeriod.CMQuantity % 4;
                    double CMdivision = (double)FetchPeriod.CMQuantity / 4;
                    double PMdivision = (double)FetchPeriod.PMQuantity / 4;
                    if (xx == 0 && yy == 6)
                    {
                        if (CMdivision < 1)
                        {
                            wkCM = 0;
                        }
                        else
                        {
                            wkCM = (int)CMdivision;
                        }
                        if (CMModulo > 0)
                            wkCM = wkCM + 1;

                        if (PMdivision < 1)
                        {
                            wkPM = 0;
                        }
                        else
                        {
                            wkPM = (int)PMdivision;
                        }
                        if (PMModulo > 0)
                            wkPM = wkPM + 1;
                    }
                    else if (xx == 6 && yy == 13)
                    {
                        if (CMdivision < 1)
                        {
                            wkCM = 0;
                        }
                        else
                        {
                            wkCM = (int)CMdivision;
                        }
                        if (CMModulo > 1)
                            wkCM = wkCM + 1;

                        if (PMdivision < 1)
                        {
                            wkPM = 0;
                        }
                        else
                        {
                            wkPM = (int)PMdivision;
                        }
                        if (PMModulo > 1)
                            wkPM = wkPM + 1;
                    }
                    else if (xx == 13 && yy == 20)
                    {
                        if (CMdivision < 1)
                        {
                            wkCM = 0;
                        }
                        else
                        {
                            wkCM = (int)CMdivision;
                        }
                        if (CMModulo > 2)
                            wkCM = wkCM + 1;

                        if (PMdivision < 1)
                        {
                            wkPM = 0;
                        }
                        else
                        {
                            wkPM = (int)PMdivision;
                        }
                        if (PMModulo > 2)
                            wkPM = wkPM + 1;
                    }
                    else if (xx == 20 && (yy == 20 || yy == 21 || yy == 22 || yy == 23))
                    {
                        if (CMdivision < 1)
                        {
                            wkCM = 0;
                        }
                        else
                        {
                            wkCM = (int)CMdivision;
                        }
                        if (CMModulo > 3)
                            wkCM = wkCM + 1;

                        if (PMdivision < 1)
                        {
                            wkPM = 0;
                        }
                        else
                        {
                            wkPM = (int)PMdivision;
                        }
                        if (PMModulo > 3)
                            wkPM = wkPM + 1;
                    }
                    else
                        continue;
                    WPVAvm.PMQuantityPlan = wkPM.ToString();
                    WPVAvm.CMQuantityPlan = wkCM.ToString();
                    WPVAvm.TotalQuantityPlan = wkPM + wkCM;
                    if (plan.EquipmentTypeId != null)
                    { 
                        var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == plan.EquipmentTypeId);
                    WPVAvm.EquipmentType = equipmentType?.EquipmentTypeName ?? "N/A";
                }
                        var FetchJobOrder = db.JobOrder
        .Where(x => x.DateCreated.HasValue &&
                    DbFunctions.TruncateTime(x.DateCreated.Value) > From.Date &&
                    DbFunctions.TruncateTime(x.DateCreated.Value) <= To.Date)
        .ToList();
                         if(plan.MaintenanceCenterId!=null)
                            FetchJobOrder = FetchJobOrder.Where(x => x.MaintenanceCenterId == plan.MaintenanceCenterId).ToList();
                       
                        foreach (var Order in FetchJobOrder)
                        {
                            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == Order.FleetsID);
                            var EquipmentIdentity = fleet?.EquipmentTypeID;
                            WPVAvm.PlateNumber = fleet.PlateNumber;
                            if (EquipmentIdentity != plan.EquipmentTypeId)
                                continue;
                            bool isApproved = Order.IsSecondOnlineApproved
                              && (Order.Status == MaintenanceConst.Closed || Order.Status == MaintenanceConst.Delivered)
                                   && Order.SecondOnlineApprovedTime.HasValue
                                   && Order.SecondOnlineApprovedTime.Value.Date <= To.Date
                                   && !Order.IsVoid;

                            bool isUnderRepair = Order.Status == MaintenanceConst.Open && !Order.IsVoid;
                            if (isApproved || isUnderRepair)
                            {
                                bool isPM = Order.IsPM;
                                if (isPM)
                                {
                                    WPVAvm.PMQuantity += 1;

                                }
                                else
                                {
                                    WPVAvm.CMQuantity += 1;
                                }

                                if (isApproved)
                                {
                                    WPVAvm.TotalQuantity += 1;
                                }
                                if (isUnderRepair)
                                {
                                    WPVAvm.TotalUnderRepair += 1;
                                }

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

                                    var fetchDetails = db.JobOrderServiceCenterDetails.Where(x => x.ServiceCenterJobId == Service.ServiceCenterJobId).ToList();
                                    decimal LabourCost = 0;
                                    foreach (var Detial in fetchDetails)
                                    {
                                        var fetchActivity = db.RepairType.FirstOrDefault(x => x.RepairTypeID == Detial.RepairTypeID);
                                        LabourCost += fetchActivity.TotalAmountCompany;
                                    }
                                    if (isPM)
                                        WPVAvm.PMLabourCost += LabourCost;
                                    else
                                        WPVAvm.CMLabourCost += LabourCost;

                                    var fetchSRequisition = db.StoreRequisitions
                                        .FirstOrDefault(x => x.ServiceJobOrderNo == Service.ServiceCenterJobId.ToString());

                                    if (fetchSRequisition != null)
                                    {
                                        var fetchIssued = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == fetchSRequisition.StoreRequisitionID);
                                        if (fetchIssued != null)
                                        {
                                            var FetchStoreItem = db.IssueItems
                                                .Where(x => x.IssueID == fetchIssued.IssueID).ToList();

                                            foreach (var Item in FetchStoreItem)
                                            {
                                                var FetchCategory = db.ItemCategorys
                                                    .FirstOrDefault(x => x.ItemCategoryCode == Item.ItemCategoryCode);

                                                if (FetchCategory == null)
                                                    continue;

                                                string categoryName = FetchCategory.Name?.ToLower() ?? "";

                                                if (Order.IsExternal)
                                                {
                                                    WPVAvm.CMExternalCost += Item.UnitPrice;
                                                }
                                                else
                                                {
                                                    bool isSparePart = ContainsThreeConsecutiveChars(categoryName, "spare part");
                                                    bool isLubricant = ContainsThreeConsecutiveChars(categoryName, "oil and lubricant");
                                                    bool isBattery = ContainsThreeConsecutiveChars(categoryName, "battery");
                                                    bool isTyre = ContainsThreeConsecutiveChars(categoryName, "tyre tire");

                                                    if (isSparePart)
                                                    {
                                                        if (isPM) WPVAvm.PMSparePartCost += Item.UnitPrice * Item.IssuedQuantity;
                                                        else WPVAvm.CMSparePartCost += Item.UnitPrice * Item.IssuedQuantity;
                                                    }
                                                    else if (isLubricant)
                                                    {
                                                        if (isPM) WPVAvm.PMLubricantCost += Item.UnitPrice * Item.IssuedQuantity;
                                                        else WPVAvm.CMLubricantCost += Item.UnitPrice * Item.IssuedQuantity;
                                                    }
                                                    else if (isBattery)
                                                    {
                                                        if (isPM) WPVAvm.PMBatteryCost += Item.UnitPrice * Item.IssuedQuantity;
                                                        else WPVAvm.CMBatteryCost += Item.UnitPrice * Item.IssuedQuantity;
                                                    }
                                                    else if (isTyre)
                                                    {
                                                        if (isPM) WPVAvm.PMTyreCost += Item.UnitPrice * Item.IssuedQuantity;
                                                        else WPVAvm.CMTyreCost += Item.UnitPrice * Item.IssuedQuantity;
                                                    }
                                                    else
                                                    {
                                                        WPVAvm.CMOtherCost += Item.UnitPrice * Item.IssuedQuantity;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        WPVAvm.MaintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == plan.MaintenanceCenterId)?.Name ?? "";
                        WPVAvm.PMActual = WPVAvm.PMBatteryCost + WPVAvm.PMLabourCost + WPVAvm.PMSparePartCost + WPVAvm.PMMiselenousCost + WPVAvm.PMLubricantCost;
                        WPVAvm.CMActual = WPVAvm.CMTyreCost + WPVAvm.CMLabourCost + WPVAvm.CMSparePartCost + WPVAvm.CMOtherCost + WPVAvm.CMOutsourcedCost;
                        WPVAvm.CMPMTotalCost = WPVAvm.PMActual + WPVAvm.CMActual;
                        WPVAvm.Category = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == plan.EquipmentCategoryId)?.EquipmentCategoryName ?? "";
                        WPVAvm.Type = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == plan.EquipmentTypeId)?.EquipmentTypeName ?? "";
                        WPVAvm.PMPlan = Math.Round(WPVAvm.PMPlan, 2);
                        WPVAvm.CMPlan = Math.Round(WPVAvm.CMPlan, 2);
                        WPVAvm.PMMiselenousCost = Math.Round(WPVAvm.PMMiselenousCost, 2);
                        WPVAvm.CMMiselenousCost = Math.Round(WPVAvm.CMMiselenousCost, 2);
                        WPVAvm.PMLabourCost = Math.Round(WPVAvm.PMLabourCost, 2);
                        WPVAvm.CMLabourCost = Math.Round(WPVAvm.CMLabourCost, 2);
                        WPVAvm.PMSparePartCost = Math.Round(WPVAvm.PMSparePartCost, 2);
                        WPVAvm.CMSparePartCost = Math.Round(WPVAvm.CMSparePartCost, 2);
                        WPVAvm.PMLubricantCost = Math.Round(WPVAvm.PMLubricantCost, 2);
                        WPVAvm.CMLubricantCost = Math.Round(WPVAvm.CMLubricantCost, 2);
                        WPVAvm.PMBatteryCost = Math.Round(WPVAvm.PMBatteryCost, 2);
                        WPVAvm.CMBatteryCost = Math.Round(WPVAvm.CMBatteryCost, 2);
                        WPVAvm.PMTyreCost = Math.Round(WPVAvm.PMTyreCost, 2);
                        WPVAvm.CMTyreCost = Math.Round(WPVAvm.CMTyreCost, 2);
                        WPVAvm.CMOtherCost = Math.Round(WPVAvm.CMOtherCost, 2);
                        WPVAvm.CMExternalCost = Math.Round(WPVAvm.CMExternalCost, 2);
                        WPVAvm.CMOutsourcedCost = Math.Round(WPVAvm.CMOutsourcedCost, 2);
                        WPVAvm.PMActual = Math.Round(WPVAvm.PMActual, 2);
                        WPVAvm.CMActual = Math.Round(WPVAvm.CMActual, 2);
                        WPVAvm.CMPMTotalCost = Math.Round(WPVAvm.CMPMTotalCost, 2);
                        list.Add(WPVAvm);
                    
                }


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

    
}
