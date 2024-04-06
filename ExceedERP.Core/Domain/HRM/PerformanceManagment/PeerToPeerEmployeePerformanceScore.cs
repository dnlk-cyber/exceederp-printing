using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PeerToPeerEmployeePerformanceScore
    {
        [Key]
        public int PeerToPeerEmployeePerformanceScoreId { get; set; }
        public int PeerEmployeeMeasurementId { get; set; }
        public int PerformanceActionId { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
       // public bool IsChecked { get; set; }
        public string Remark { get; set; }
    }
}
