using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class MachinaryAndVehicleMaintenancereportOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MachinaryAndVehicleMaintenanceReportViewModel> GetReport(string fiscalYearId, string periodId)
        {
            List<MachinaryAndVehicleMaintenanceReportViewModel> list = new List<MachinaryAndVehicleMaintenanceReportViewModel>();
            if(int.TryParse(fiscalYearId, out int glFiscalYearId))
            {
                var maintenancePlans = db.MaintenancePlans.Where(x => x.GlFiscalYearId == glFiscalYearId).ToList();
                var planDetails = db.MaintenancePlanDetails.ToList();
                if(int.TryParse(periodId, out int glPeriodId))
                {
                    planDetails = planDetails.Where(x => x.GlPeriodId == glPeriodId).ToList();
                }
                var joinedPlan = from plan in maintenancePlans
                                 join detail in planDetails
                                 on plan.MaintenancePlanId equals detail.MaintenancePlanId
                                 select new
                                 {
                                     plan.GlFiscalYearId,
                                     plan.TotalQuantity,
                                     plan.EquipmentTypeId,
                                     detail.GlPeriodId,
                                     detail.CMQuantity,
                                     detail.CMCost,
                                     detail.PMQuantity,
                                     detail.PMCost,
                                     
                                 };
                var groupedPlans = joinedPlan.GroupBy(g => new { g.EquipmentTypeId, g.GlPeriodId })
                    .Select(s => new
                    {
                        s.Key.EquipmentTypeId,
                        s.Key.GlPeriodId,
                        CMCost = s.Sum(x => (decimal?)x.CMCost) ?? 0,
                        PMCost = s.Sum(x => (decimal?)x.PMCost) ?? 0,
                        CMQuantity = s.Sum(x => (decimal?)x.CMQuantity) ?? 0,
                        PMQuantity = s.Sum(x => (decimal?)x.PMQuantity) ?? 0

                    }).ToList();
                foreach (var groupedPlan in groupedPlans)
                {
                    var equipmentType = db.EquipmentType.Find(groupedPlan.EquipmentTypeId);
                    var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == groupedPlan.GlPeriodId)?.Name;
                    if(equipmentType != null && month != null)
                    {
                        decimal cmCost = GetCMActualCost(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId);
                        decimal pmCost = GetPMActualCost(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId);
                        decimal totalCost = cmCost + pmCost;
                        list.Add(new MachinaryAndVehicleMaintenanceReportViewModel
                        {
                            EquipmentTypeName = equipmentType.EquipmentTypeName,
                            CMAnnualPlanQuantity = groupedPlan.CMQuantity,
                            PMAnnualPlanQuantity = groupedPlan.PMQuantity,
                            CMPlanCost = groupedPlan.CMCost,
                            PMPlanCost = groupedPlan.PMCost,
                            PMActualCount = GetExcutionCount(groupedPlan.EquipmentTypeId, true, groupedPlan.GlPeriodId),
                            CMActualCount = GetExcutionCount(groupedPlan.EquipmentTypeId, false, groupedPlan.GlPeriodId),
                            CMActualCost = cmCost,
                            PMActualCost = pmCost,
                            
                        });
                       
                    }
                }
            }

            return list;
        }

        private int GetExcutionCount(Guid equipmentTypeId, bool isPM, int glPeriodId)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            if (glPeriod == null)
                return 0;
            var fleetIds = db.Fleets
              .Where(x => x.EquipmentTypeID == equipmentTypeId)
              .Select(x => x.FleetsID)
              .ToList();
            if(isPM)
                return db.JobOrder.Where(x => x.IsPM && fleetIds.Contains(x.FleetsID))
                    .Where(x => x.DateCompleted >= glPeriod.DateFrom && x.DateCompleted <= glPeriod.DateTo)
                    .Count();
            return db.JobOrder.Where(x => !x.IsPM && fleetIds.Contains(x.FleetsID)).Count();
        }

        private decimal GetPMActualCost(Guid equipmentTypeId, int glPeriodId)
        {
            decimal cost = 0;
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            if (glPeriod == null)
                return 0;
            var crmPeriod = db.CRMPeriods.FirstOrDefault(x => x.Name == glPeriod.Name);
            if (crmPeriod == null)
                return 0;
            var fleet = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(s => s.FleetsID)
                .ToList();

            // Get the job order numbers for fleets and maintenance tasks
            var jobOrderNumbers = db.JobOrder
                .Where(x => fleet.Contains(x.FleetsID) && x.IsPM)
                .Select(x => x.Num);
                

            // Fetch sales quote vouchers that match the criteria
            var salesQuotes = db.SalesQuoteVouchers
                .Where(x => x.Source == SalesQuoteSource.Maintenance &&
                            x.CRMPeriodId == crmPeriod.CRMPeriodId)
                .AsEnumerable() // Switch to in-memory filtering to use TryParse
                .Where(x => int.TryParse(x.JobOrderNumber, out var jobOrderNumber) && jobOrderNumbers.Contains(jobOrderNumber))
                .ToList();


            // Service Cost + Spare Part Cost
            foreach (var item in salesQuotes)
            {
                var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == item.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();
                var lineItems = db.LineItems.Where(t => t.Voucher == item.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();

                cost += serviceLineItems.Sum(x => (decimal?)x.UnitAmount) ?? 0 + lineItems.Sum(s => (decimal?)s.TotalAmount) ?? 0; 
            }

            // Additional Cost
            var workShop = db.ServiceCenterJob.Where(x => jobOrderNumbers.Contains(x.Num)).ToList();
            foreach (var item in workShop)
            {
                cost += db.AdditionalWorkShopDetails.Where(x => x.ServiceCenterJobId == item.ServiceCenterJobId).Sum(x => (decimal?)x.OtherCosts) ?? 0;
            }
            return cost;
        }
        private decimal GetCMActualCost(Guid equipmentTypeId, int glPeriodId)
        {
            decimal cost = 0;
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            if (glPeriod == null)
                return 0;
            var crmPeriod = db.CRMPeriods.FirstOrDefault(x => x.Name == glPeriod.Name);
            if (crmPeriod == null)
                return 0;
            var fleet = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(s => s.FleetsID)
                .ToList();

            // Get the job order numbers for fleets and maintenance tasks
            var jobOrderNumbers = db.JobOrder
                .Where(x => fleet.Contains(x.FleetsID) && !x.IsPM)
                .Select(x => x.Num);
                

            // Fetch sales quote vouchers that match the criteria
            var salesQuotes = db.SalesQuoteVouchers
                .Where(x => x.Source == SalesQuoteSource.Maintenance &&
                            x.CRMPeriodId == crmPeriod.CRMPeriodId)
                .AsEnumerable() // Switch to in-memory filtering to use TryParse
                .Where(x => int.TryParse(x.JobOrderNumber, out var jobOrderNumber) && jobOrderNumbers.Contains(jobOrderNumber))
                .ToList();


            // Service Cost + Spare Part Cost
            foreach (var item in salesQuotes)
            {
                var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == item.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();
                var lineItems = db.LineItems.Where(t => t.Voucher == item.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();

                cost += serviceLineItems.Sum(x => (decimal?)x.UnitAmount) ?? 0 + lineItems.Sum(s => (decimal?)s.TotalAmount) ?? 0;
            }

            // Additional Cost
            var workShop = db.ServiceCenterJob.Where(x => jobOrderNumbers.Contains(x.Num)).ToList();
            foreach (var item in workShop)
            {
                cost += db.AdditionalWorkShopDetails.Where(x => x.ServiceCenterJobId == item.ServiceCenterJobId).Sum(x => (decimal?)x.OtherCosts) ?? 0;
            }
            return cost;
        }
    }

    public class MachinaryAndVehicleMaintenanceReportViewModel
    {
        public string EquipmentTypeName { get; set; }
        public decimal CMAnnualPlanQuantity { get; set; }
        public decimal PMAnnualPlanQuantity { get; set; }
        public decimal CMPlanCost { get; set; }
        public decimal PMPlanCost { get; set; }
        
        public decimal CMActualCost { get; set; }
        public decimal PMActualCost { get; set; }
        public int PMActualCount { get; set; }
        public int CMActualCount { get; set; } 
        public decimal Ratio { get; set; }
    }
}
