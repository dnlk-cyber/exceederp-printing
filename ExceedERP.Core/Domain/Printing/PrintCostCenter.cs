using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintCostCenter : TrackUserSettingOperation
    {
        [Key]
        public int PrintCostCenterId { get; set; } 
        public string PrintCostCenterCode { get; set; }
        public string PrintCostCenterName { get; set; } 

    }
}
