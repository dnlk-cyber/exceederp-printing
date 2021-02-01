using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLJournalSources : TrackUserOperation
    {
        public int GLJournalSourcesID { get; set; }
        [Required]
        [Display(Name = "Sources")]
        public string Sources { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Journal Refernces")]
        public bool JournalRefernces { get; set; }
        [Display(Name = " Import Journal Approval")]
        public bool JournalApproval { get; set; }
        [Display(Name = "Freez Journal")]
        public bool FreezJournal { get; set; }
    }
}
