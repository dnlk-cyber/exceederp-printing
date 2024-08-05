using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class WeeklyCMPMPerformanceSummaryReport
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WeeklyPerformanceDTO> GetWeeklyPerormance(string fiscalYearId, string periodId)
        {
            List<WeeklyPerformanceDTO> list = new List<WeeklyPerformanceDTO>();
            if (int.TryParse(fiscalYearId, out int glFiscalYearId))
            {
                var maintenancePlans = db.MaintenancePlans.Where(x => x.GlFiscalYearId == glFiscalYearId).ToList();
                var planDetails = db.MaintenancePlanDetails.ToList();
                if (int.TryParse(periodId, out int glPeriodId))
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
                        CMQuantity = s.Sum(x => x.CMQuantity),
                        PMQuantity = s.Sum(x => x.PMQuantity)

                    }).ToList();
                foreach (var groupedPlan in groupedPlans)
                {
                    var equipmentType = db.EquipmentType.Find(groupedPlan.EquipmentTypeId);
                    var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == groupedPlan.GlPeriodId)?.Name;
                    if (equipmentType != null && month != null)
                    {
                        list.Add(new WeeklyPerformanceDTO
                        {
                            EquipmentType = equipmentType.EquipmentTypeName,
                            UoM = "",
                            CMPlanCost = groupedPlan.CMCost,
                            PMPlanCost = groupedPlan.PMCost,
                            PlanTotalCost = groupedPlan.CMCost + groupedPlan.PMCost,

                            CMActualCost = 0,
                            PMActualCost = 0,
                            ActualTotalCost = 0,
                           
                        });

                    }
                }
            }
            return list;
        }
        
    }

    public class WeeklyPerformanceDTO
    {
        public string ProjectName { get; set; }
        public string EquipmentType { get; set; }
        public string UoM { get; set; }
        //public string PlateNumber { get; set; }
        //public string OperatorName { get; set; }
        public decimal OutSourcedMaint { get; set; }
        public decimal CMPlanCost { get; set; }
        public decimal PMPlanCost { get; set; }
        public decimal PlanTotalCost { get; set; }
        public decimal CMActualCost { get; set; }
        public decimal PMActualCost { get; set; }
        public decimal ActualTotalCost { get; set; }

        //PM
        public decimal PMSparePartCost { get; set; }
        public decimal PMLubricantCost { get; set; }
        public decimal PMTyreCost { get; set; }
        public decimal PMBattery { get; set; }
        public decimal PMOtherCost { get; set; }
        public decimal PMMShopCost { get; set; }
        public decimal PMLaborCost { get; set; }

        //CM
        public decimal CMSparePartCost { get; set; }
        public decimal CMLubricantCost { get; set; }
        public decimal CMTyreCost { get; set; }
        public decimal CMBattery { get; set; }
        public decimal CMOtherCost { get; set; }
        public decimal CMShopCost { get; set; }
        public decimal CMLaborCost { get; set; }
        

    }
}
