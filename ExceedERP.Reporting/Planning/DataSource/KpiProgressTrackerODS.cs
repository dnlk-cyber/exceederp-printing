using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Planning.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class KpiProgressTrackerODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<KpiProgressTrackerVM> GetKpiProgressTrackerData(string kpi, string subKpi)
        {

            var kpiProgressTrackers = new List<KpiProgressTrackerVM>();
            var progressTrackers = db.KpiProgressTrackers.ToList();

            if (!string.IsNullOrEmpty(kpi))
            {

                int.TryParse(kpi, out int kpiId);
                progressTrackers = progressTrackers.Where(s => s.GlobalKPIId == kpiId).ToList();

            }
            if (!string.IsNullOrEmpty(subKpi))
            {

                int.TryParse(subKpi, out int subKpiId);
                var trackerIds = db.SubKpiProgressTrackers.Where(x => x.GlobalSubKPIId == subKpiId).Select(x => x.KpiProgressTrackerId);
                progressTrackers = progressTrackers.Where(x =>trackerIds.Contains(x.KpiProgressTrackerId)).ToList();

            }
            int i = 1;
            foreach (var progressTracker in progressTrackers)
            {
                var model = new KpiProgressTrackerVM();
                var globalKpI = db.GlobalKPIs.Find(progressTracker.GlobalKPIId);
                if (globalKpI != null)
                {
                    model.KPI = globalKpI.KPIName;
                }
                model.Index = i.ToString();
                model.Weight = progressTracker.Weight;
                model.Evaluation = progressTracker.Evaluation;
                model.Remark = progressTracker.Remark;
                int j = 1;
                model.SubKpiProgresses = new List<SubKpiProgressTrackerVM>();
                var subKpis = db.SubKpiProgressTrackers.Where(x=>x.KpiProgressTrackerId == progressTracker.KpiProgressTrackerId).ToList();
                if (!string.IsNullOrEmpty(subKpi))
                {

                    int.TryParse(subKpi, out int subKpiId);
                     subKpis = subKpis.Where(x => x.GlobalSubKPIId == subKpiId).ToList();

                }
                var subKpidistictss = subKpis.DistinctByField(x => x.GlobalSubKPIId).ToList();
                foreach (var subkpi in subKpidistictss)
                {
                    var subkpiprogesstracker = new SubKpiProgressTrackerVM();
                    var subGlobalKpI = db.GlobalSubKPIs.Find(subkpi.GlobalSubKPIId);
                    if (subGlobalKpI != null)
                    {
                        subkpiprogesstracker.SubKPI = subGlobalKpI.SubKPIName;
                    }
                    subkpiprogesstracker.Index = $"{i}.{j}";

                    subkpiprogesstracker.IndicatorsProgresses = new List<IndicatorsProgressTrackerVM>();
                    var indicators = subKpis.Where(x => x.GlobalSubKPIId == subkpi.GlobalSubKPIId).ToList();
                    foreach(var indicator in indicators)
                    {
                        var IndicatorProgess = new IndicatorsProgressTrackerVM()
                        {
                            Indicators=indicator.Indicators,
                            Saf = indicator.Saf,
                            Weight = indicator.Weight,
                            WeightDisplay = FormatWeight(indicator.Weight),
                            AnnualPlan=indicator.AnnualPlan,
                            Plan=indicator.UpTo2ndQuarterPlan,
                            Performance=indicator.UpTo2ndQuarterPerformance,
                            Percent=indicator.UpTo2ndQuarterPercent,
                            Evaluation=indicator.Evaluation,
                            Remark=indicator.Remark,

                        };

                        subkpiprogesstracker.IndicatorsProgresses.Add(IndicatorProgess);
                    }
                    subkpiprogesstracker.Weight = subkpiprogesstracker.IndicatorsProgresses.Any() ? subkpiprogesstracker.IndicatorsProgresses.Sum(x => x.Weight) : 0;
                    subkpiprogesstracker.WeightDisplay = FormatWeight(subkpiprogesstracker.Weight);
                    subkpiprogesstracker.Evaluation = subkpiprogesstracker.IndicatorsProgresses.Any() ? subkpiprogesstracker.IndicatorsProgresses.Sum(x => x.Evaluation) : 0;
                    model.SubKpiProgresses.Add(subkpiprogesstracker);
                    j++;
                }
                if (model.SubKpiProgresses.Any())
                {
                    model.Weight = model.SubKpiProgresses.Sum(x => x.Weight);
                }
                model.WeightDisplay = FormatWeight(model.Weight);
                model.Evaluation = model.SubKpiProgresses.Any() ? model.SubKpiProgresses.Sum(x => x.Evaluation) : 0;
                kpiProgressTrackers.Add(model);
                i++;
            }


            return kpiProgressTrackers;

        }

        private static string FormatWeight(decimal? weight)
        {
            return weight.HasValue ? weight.Value.ToString("0.##") : "-";
        }


    }

}
