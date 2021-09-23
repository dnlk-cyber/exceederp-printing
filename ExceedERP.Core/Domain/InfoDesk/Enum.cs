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
        WAITING,
        ASSIGNED,
        RESOLVED,
        CLOSED,
        NOT_SOLVED,
        WAITING_FOR_CONFIRMATION

    }

}