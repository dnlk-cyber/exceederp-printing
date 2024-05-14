using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintWorkOrderType : TrackUserSettingOperation
    {
        [Key]
        public int PrintWorkOrderTypeId { get; set; } 
        public string PrintWorkOrderTypeCode { get; set; } 
        public string Description { get; set; }

    }
}
