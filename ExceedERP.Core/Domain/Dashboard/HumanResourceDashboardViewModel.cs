using System.Collections.Generic;

namespace ExceedERP.Core.Domain.Dashboard
{
    public class MySeriesData
    {
        public string Name { get; set; }
        public string Stack { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class HrSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrTrainingData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrAccidentRecordData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrVacancyData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrAssignmentSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrResponsiblitySeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrVacantPostionData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrLetterData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrAbsentData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrTerminationData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class PieSeriesData
    {
        public string category { get; set; }
        public decimal value { get; set; }
        public string Color { get; set; }
    }
}
