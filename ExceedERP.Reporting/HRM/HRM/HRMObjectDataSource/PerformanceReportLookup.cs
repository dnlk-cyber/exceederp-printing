using System.Collections.Generic;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    /// <summary>Weight/score totals of one evaluation.</summary>
    internal sealed class PerformanceTotal
    {
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
    }

    /// <summary>Set-based lookups shared by the performance report data sources, replacing per-row queries.</summary>
    internal static class PerformanceReportLookup
    {
        private const int ChunkSize = 1000;

        public static Dictionary<int, PerformanceTotal> LoadTotals(ExceedDbContext db, ICollection<int> performanceIds)
        {
            var totals = new Dictionary<int, PerformanceTotal>();
            var ids = performanceIds.Distinct().ToList();

            for (int i = 0; i < ids.Count; i += ChunkSize)
            {
                var chunk = ids.Skip(i).Take(ChunkSize).ToList();

                var rows = (from m in db.EmployeeMeasurement
                            join s in db.EmployeePerformanceScores on m.EmpMeasurementId equals s.EmployeeMeasurementId
                            where chunk.Contains(m.EmployeePerformanceId)
                            group s by m.EmployeePerformanceId into g
                            select new { PerformanceId = g.Key, Weight = g.Sum(x => x.Weight), Score = g.Sum(x => x.Score) })
                           .ToList();

                foreach (var row in rows)
                {
                    totals[row.PerformanceId] = new PerformanceTotal { Weight = row.Weight, Score = row.Score };
                }
            }

            return totals;
        }

        public static PerformanceTotal Sum(Dictionary<int, PerformanceTotal> totals, IEnumerable<int> performanceIds)
        {
            var sum = new PerformanceTotal();
            foreach (var id in performanceIds)
            {
                if (totals.TryGetValue(id, out var t))
                {
                    sum.Weight += t.Weight;
                    sum.Score += t.Score;
                }
            }
            return sum;
        }

        public static List<PerformanceRank> LoadRanks(ExceedDbContext db)
        {
            return db.PerformanceRank.OrderBy(x => x.Minimum).ToList();
        }

        /// <summary>Bonus fraction of salary from the Performance Rank setting (BonusRate is a percentage), pro-rated by days served over 360.</summary>
        public static decimal CalculateBonus(IEnumerable<PerformanceRank> ranks, decimal daysServed, decimal score)
        {
            const decimal minimumDaysForBonus = 180m;
            if (daysServed < minimumDaysForBonus)
            {
                return 0m;
            }

            var rank = ranks.FirstOrDefault(x => x.Minimum <= score && x.Maximum >= score);
            return rank == null ? 0m : (rank.BonusRate / 100m) * (daysServed / 360m);
        }

        /// <summary>Evaluation criteria (measurement definition) ids belonging to any of the given evaluation types.</summary>
        public static HashSet<int> GetCriteriaIds(ExceedDbContext db, params EvaluationType[] evaluationTypes)
        {
            var result = new HashSet<int>();

            foreach (var evaluationType in evaluationTypes.Distinct())
            {
                var type = evaluationType;
                var ids = (from m in db.MeasurementDefinitions
                           join l in db.Lookups on m.LookupId equals l.LookupId
                           where l.EvalutionType == type && l.LookupType == LookUpTypes.EVALUATION_TYPE
                           select m.MeasurementDefinitionId).ToList();

                result.UnionWith(ids);
            }

            return result;
        }
    }
}
