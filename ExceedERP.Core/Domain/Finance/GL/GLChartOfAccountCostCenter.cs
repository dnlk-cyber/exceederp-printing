using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public enum GlCostCenterTypes
    {
        HeadOffice,
        Project
    }
    public class GLChartOfAccountCostCenter : TrackUserOperation
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Values")]
        //[StringLength(4)]
        public string Values { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Parent")]
        public string HasParent { get; set; }
        [Display(Name = "Is Enabled?")]
        public bool Enable { get; set; }
        [Display(Name = "Is Parent?")]
        public bool Parent { get; set; }
        [Required]
        [Display(Name = "Allow Budgeting")]
        public bool AllowBudgeting { get; set; }
        [Display(Name = "Allow Posting")]
        public bool AllowPosting { get; set; }
        public GlCostCenterTypes Type { get; set; }
    }
}
