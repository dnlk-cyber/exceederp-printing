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
        public List<AnnualPlanSummaryViewModel> GenerateAnnualPlanSummaryReport(string fiscalYearId, string equipmentTypeId, string grade, string glPeriod)
        {
            List<AnnualPlanSummaryViewModel> summaryList = new List<AnnualPlanSummaryViewModel>();

            if (int.TryParse(fiscalYearId, out int fId))
            {
                var annualPlans = db.FleetAnnualPlans.Where(x => x.GlFiscalYearId == fId).ToList();
                var planDetail = db.FleetAnnualPlanDetails.ToList();
                if (!string.IsNullOrWhiteSpace(equipmentTypeId) && Guid.TryParse(equipmentTypeId, out Guid eTypeId))
                {
                    annualPlans = annualPlans.Where(x => x.EquipmentTypeId == eTypeId).ToList();
                }
                if (!string.IsNullOrWhiteSpace(grade))
                {
                    annualPlans = annualPlans.Where(x => x.EquipmentGrade.ToString() == grade).ToList();
                }
                if (int.TryParse(glPeriod, out int glPeriodId))
                {
                    planDetail = planDetail.Where(x => x.GlPeriodId == glPeriodId).ToList();
                }
                var planDetails = from plan in annualPlans
                                  join detail in planDetail on plan.FleetAnnualPlanId equals detail.FleetAnnualPlanId
                                  select new
                                  {
                                      plan.EquipmentTypeId,
                                      plan.FleetId,
                                      detail.GlPeriodId,
                                      detail.ExpectedEffectiveWorkingHrPerMonth,
                                      detail.ExpectedMonthlyIncome,
                                      Quantity = detail.NumberOfEquipmentInMonth,
                                      Grade = plan.EquipmentGrade
                                  };

                var groupedDetails = planDetails
                    .GroupBy(x => new { x.EquipmentTypeId, x.GlPeriodId, x.Grade })
                    .Select(g => new
                    {
                        g.Key.EquipmentTypeId,
                        g.Key.GlPeriodId,
                        TotalExpectedEffectiveWorkingHour = g.Sum(x => x.ExpectedEffectiveWorkingHrPerMonth),
                        ExpectedMonthlyIncome = g.Sum(x => x.ExpectedMonthlyIncome),
                        Quantity = g.Sum(x => (decimal?)x.Quantity) ?? 0,
                        Grade = g.Key.Grade,
                        
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
                            Grade = detailGroup.Grade.ToString(),
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
        public string Grade { get; set; }
        public string Month { get; set; }
        public string EquipmentTypeName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalExpectedEffectiveWorkingHour { get; set; }
        public decimal ExpectedMonthlyIncome { get; set; }

    }
}
