using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class FleetAnnualPlanSummeryOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AnnualPlanSummaryViewModel> GenerateAnnualPlanSummaryReport(string fiscalYearId, string equipmentTypeId)
        {
            List<AnnualPlanSummaryViewModel> summaryList = new List<AnnualPlanSummaryViewModel>();

            if (int.TryParse(fiscalYearId, out int fId))
            {
                var annualPlans = db.FleetAnnualPlans.Where(x => x.GlFiscalYearId == fId);

                if (!string.IsNullOrWhiteSpace(equipmentTypeId) && Guid.TryParse(equipmentTypeId, out Guid eTypeId))
                {
                    annualPlans = annualPlans.Where(x => x.EquipmentTypeId == eTypeId);
                }                
                var planDetails = from plan in annualPlans
                                  join detail in db.FleetAnnualPlanDetails on plan.FleetAnnualPlanId equals detail.FleetAnnualPlanId
                                  select new
                                  {
                                      plan.EquipmentTypeId,
                                      plan.FleetId,
                                      detail.GlPeriodId,
                                      detail.ExpectedEffectiveWorkingHrPerMonth,
                                      detail.ExpectedMonthlyIncome,
                                      Quantity = detail.NumberOfEquipmentInMonth
                                  };

                var groupedDetails = planDetails
                    .GroupBy(x => new { x.EquipmentTypeId, x.GlPeriodId })
                    .Select(g => new
                    {
                        g.Key.EquipmentTypeId,
                        g.Key.GlPeriodId,
                        TotalExpectedEffectiveWorkingHour = g.Sum(x => x.ExpectedEffectiveWorkingHrPerMonth),
                        ExpectedMonthlyIncome = g.Sum(x => x.ExpectedMonthlyIncome),
                        Quantity = g.Sum(x => (decimal?)x.Quantity) ?? 0
                        //Quantity = g.Select(x => x.FleetId).Distinct().Count(),
                    })
                    .ToList();

                foreach (var detailGroup in groupedDetails)
                {
                    var equipmentType = db.EquipmentType.Find(detailGroup.EquipmentTypeId);
                    var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == detailGroup.GlPeriodId)?.Name;

                    if (equipmentType != null && month != null)
                    {
                        summaryList.Add(new AnnualPlanSummaryViewModel
                        {
                            EquipmentTypeName = equipmentType.EquipmentTypeName,
                            Month = month,
                            TotalExpectedEffectiveWorkingHour = detailGroup.TotalExpectedEffectiveWorkingHour,
                            ExpectedMonthlyIncome = detailGroup.ExpectedMonthlyIncome,
                            Quantity = (int)detailGroup.Quantity,

                        });
                    }
                }
            }

            return summaryList;
        }
    }

    public class AnnualPlanSummaryViewModel
    {
        public string Month { get; set; }
        public string EquipmentTypeName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalExpectedEffectiveWorkingHour { get; set; }
        public decimal ExpectedMonthlyIncome { get; set; }

    }
}
