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
    public class PlanVsActualReport
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanVsActualVM> GetplanVactual(string EquipmentCategory, string EquipmentType, string GLPeriod, string GLFiscalYear, string MCId)
        {
            List<PlanVsActualVM> list = new List<PlanVsActualVM>();
            if (!(Guid.TryParse(EquipmentType, out Guid EquipmentTypeID)))
            {
                Guid x = EquipmentTypeID;
            }
            if (!(int.TryParse(GLPeriod, out int GLPeriodId)))
            {
                return list;
            }
            if (!(int.TryParse(GLFiscalYear, out int FiscalYear)))
            {
                return list;
            }

            var FetchPlan = db.MaintenancePlans.Where(x => x.GlFiscalYearId == FiscalYear).ToList();
            if (FetchPlan.Any())
            {
                if (!string.IsNullOrEmpty(MCId))
                {
                    FetchPlan = FetchPlan.Where(x => x.MaintenanceCenterId != null && x.MaintenanceCenterId.ToString() == MCId).ToList();
                }
                if (!string.IsNullOrEmpty(EquipmentType))
                {
                    FetchPlan = FetchPlan.Where(x => x.EquipmentTypeId != null && x.EquipmentTypeId == EquipmentTypeID).ToList();
                }
            }

            foreach (var plan in FetchPlan)
            {
                var FetchPeriod = db.MaintenancePlanDetails.FirstOrDefault(x => x.MaintenancePlanId == plan.MaintenancePlanId && x.GlPeriodId.ToString() == GLPeriod);
                if (FetchPeriod != null)
                {
                    var Periods = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == GLPeriodId);
                    List<JobOrder> FetchClosedWorks = new List<JobOrder>();
                    decimal CMlabourCost = 0;
                    decimal CMmaterialCost = 0;
                    decimal CMquantity = 0;
                    decimal CMtotalCost = 0;
                    decimal PMlabourCost = 0;
                    decimal PMmaterialCost = 0;
                    decimal PMquantity = 0;
                    decimal PMtotalCost = 0;

                    if (Periods != null)
                    {
                        FetchClosedWorks = db.JobOrder.Where(x =>
                            x.SecondOnlineApprovedTime.HasValue &&
                            DbFunctions.TruncateTime(x.SecondOnlineApprovedTime).Value >= Periods.DateFrom.Date &&
                            DbFunctions.TruncateTime(x.SecondOnlineApprovedTime).Value <= Periods.DateTo.Date &&
                            x.Status == MaintenanceConst.Delivered
                        ).ToList();

                        if (plan.MaintenanceCenterId != null)
                            FetchClosedWorks = FetchClosedWorks.Where(x => x.MaintenanceCenterId != null && x.MaintenanceCenterId == plan.MaintenanceCenterId).ToList();

                        foreach (var closedJob in FetchClosedWorks)
                        {
                            var FetchFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == closedJob.FleetsID);
                            if (FetchFleet == null || FetchFleet.EquipmentTypeID != plan.EquipmentTypeId)
                                continue;

                            var FetchJobRequest = db.JobRequest.FirstOrDefault(x => x.Num == closedJob.Referance);

                            if (closedJob.IsPM)
                            {
                                PMquantity += 1;
                            }
                            else
                            {
                                CMquantity += 1;
                            }

                            if (FetchFleet != null)
                            {
                                if (!string.IsNullOrEmpty(EquipmentType))
                                {
                                    if (FetchFleet.EquipmentTypeID == EquipmentTypeID)
                                    {
                                        var FetchServiceCenters = db.ServiceCenterJob.Where(x => x.JobOrderID == closedJob.JobOrderID).ToList();
                                        foreach (var JobService in FetchServiceCenters)
                                        {
                                            var fetchServiceType = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == JobService.ServiceCenterJobId);
                                            decimal LabourCost = 0;
                                            if (fetchServiceType != null)
                                            {
                                                LabourCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceType.RepairTypeID)?.StandaredTime ?? 0;
                                            }

                                            var FetchStorereq = db.StoreRequisitions.FirstOrDefault(x => x.IsForMaintenance && x.ServiceJobOrderNo == JobService.ServiceCenterJobId.ToString());
                                            if (closedJob.IsPM)
                                            {
                                                PMlabourCost += LabourCost;
                                            }
                                            else
                                            {
                                                CMlabourCost += LabourCost;
                                            }

                                            if (FetchStorereq != null)
                                            {
                                                var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == FetchStorereq.StoreRequisitionID);
                                                if (fetchIssue != null)
                                                {
                                                    var FetchItems = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                                                    foreach (var FetchItem in FetchItems)
                                                    {
                                                        decimal fetchAveragePrice = FetchItem?.UnitPrice ?? 0;
                                                        if (FetchJobRequest != null && FetchJobRequest.IsPM)
                                                        {
                                                            PMmaterialCost += fetchAveragePrice * FetchItem.IssuedQuantity;
                                                        }
                                                        else
                                                        {
                                                            CMmaterialCost += fetchAveragePrice * FetchItem.IssuedQuantity;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var FetchServiceCenters = db.ServiceCenterJob.Where(x => x.JobOrderID == closedJob.JobOrderID).ToList();
                                    foreach (var JobService in FetchServiceCenters)
                                    {
                                        var fetchServiceType = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == JobService.ServiceCenterJobId);
                                        decimal LabourCost = 0;
                                        if (fetchServiceType != null)
                                        {
                                            LabourCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceType.RepairTypeID)?.StandaredTime ?? 0;
                                        }

                                        var FetchStorereq = db.StoreRequisitions.FirstOrDefault(x => x.IsForMaintenance && x.ServiceJobOrderNo == JobService.ServiceCenterJobId.ToString());
                                        if (closedJob.IsPM)
                                        {
                                            PMlabourCost += LabourCost;
                                        }
                                        else
                                        {
                                            CMlabourCost += LabourCost;
                                        }

                                        if (FetchStorereq != null)
                                        {
                                            var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == FetchStorereq.StoreRequisitionID);
                                            if (fetchIssue != null)
                                            {
                                                var FetchItems = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                                                foreach (var FetchItem in FetchItems)
                                                {
                                                    decimal fetchAveragePrice = FetchItem?.UnitPrice ?? 0;
                                                    if (FetchJobRequest != null && FetchJobRequest.IsPM)
                                                    {
                                                        PMmaterialCost += fetchAveragePrice * FetchItem.IssuedQuantity;
                                                    }
                                                    else
                                                    {
                                                        CMmaterialCost += fetchAveragePrice * FetchItem.IssuedQuantity;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        PMtotalCost = PMlabourCost + PMmaterialCost;
                        CMtotalCost = CMlabourCost + CMmaterialCost;
                    }

                    var PlanVsActual = new PlanVsActualVM
                    {
                        EquipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == plan.EquipmentTypeId)?.EquipmentTypeName ?? "",
                        EquipmentName = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == plan.GlFiscalYearId)?.Name ?? "",
                        CMLabourCost = CMlabourCost.ToString(),
                        Period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == GLPeriod)?.Name ?? "",
                        PMLabourCost = PMlabourCost.ToString(),
                        PMLabourCostPlan = FetchPeriod.PMLaborCost.ToString(),
                        PMMaterialCost = PMmaterialCost.ToString(),
                        PMMaterialCostPlan = FetchPeriod.PMMaterialCost.ToString(),
                        PMQuantity = PMquantity.ToString(),
                        PMQuantityPlan = FetchPeriod.PMQuantity.ToString(),
                        PMTotalCost = PMtotalCost.ToString(),
                        PMTotalCostPlan = (FetchPeriod.PMLaborCost + FetchPeriod.PMMaterialCost).ToString(),
                        CMLabourCostPlan = FetchPeriod.CMLaborCost.ToString(),
                        CMMaterialCost = CMmaterialCost.ToString(),
                        CMMaterialCostPlan = FetchPeriod.CMMaterialCost.ToString(),
                        CMQuantity = CMquantity.ToString(),
                        CMQuantityPlan = FetchPeriod.CMQuantity.ToString(),
                        CMTotalCost = CMtotalCost.ToString(),
                        CMTotalCostPlan = (FetchPeriod.CMLaborCost + FetchPeriod.CMMaterialCost).ToString()
                    };
                    list.Add(PlanVsActual);
                }
            }

            return list;
        }
    }

    public class PlanVsActualVM
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
        public string PMMaterialCost { get; set; }
        public string PMLabourCost { get; set; }
        public string CMMaterialCost { get; set; }
        public string CMLabourCost { get; set; }
        public string CMQuantity { get; set; }
        public string CMTotalCost { get; set; }
        public string PMTotalCost { get; set; }
        public string PMQuantity { get; set; }
       
 

    }
}
