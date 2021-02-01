using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARSetupTransactionClass : TrackUserSettingOperation
    {
        public int ARSetupTransactionClassID { get; set; }
        [Required]
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }
        public string Description { get; set; }
    }
}
