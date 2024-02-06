using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
    public class FurnitureJobOrderSummaryCostCenterDetail
    {
        public int FurnitureJobOrderSummaryCostCenterDetailId { get; set; }
        public int FurnitureJobOrderSummaryCostCenterId { get; set; }
        public string CostCenter { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public double DirectLaborTotalHour { get; set; }
        public double Rate { get; set; }
        public double DirectLaborAmount { get; set; }
        public double DirectMachineTotalHour { get; set; }
        public double OverHeadRate { get; set; }
        public double OverHeadAmount { get; set; }
        public double TotalCostOfTheCenter { get; set; }
    }
}
