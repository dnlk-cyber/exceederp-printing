using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLJournalCategoriers : TrackUserSettingOperation
    {
        public int GLJournalCategoriersID { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool Enable { get; set; }
    }
}
