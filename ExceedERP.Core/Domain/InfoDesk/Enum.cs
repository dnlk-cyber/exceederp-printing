using System.ComponentModel.DataAnnotations;



using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.InfoDesk
{

    public enum TicketActivityStatus
    {
        Registration,
        Assignment,
        Handling,
        Archive

    }
    public enum InfoDeskFinalizeType
    {
        [Display(Name = "Waiting for Response")]
        Waiting,
        [Display(Name = "Ticket Assigned")]
        Assigned,
        [Display(Name = "Issue Resolved")]
        Resolved,
        [Display(Name = "Ticket Closed")]
        Closed,
        [Display(Name = "Not Solved")]
        NOT_SOLVED,
        [Display(Name = "Waiting for Confirmation")]
        WAITING_FOR_CONFIRMATION,
        [Display(Name = "Pending")]
        Pending

    }

}