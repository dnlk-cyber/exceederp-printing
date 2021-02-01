
namespace ExceedERP.Core.Domain.Inventory
{
    public enum IssueFuelVehicleOwner
    {
        Internal,
        External
    }
    public enum OperationStatuses
    {
        New = 0,
        Approved = 1,
        Declined = 2,
        Issued = 3,
        WaitingApproval = 4,
        Editing = 5,
        SenderApproved = 6,
        ReceiverApproved = 7,
        Rejected = 8,
        WaitingSecondApproval = 9,
        Void = 10,
        SenderVoid = 11,
        ReceiverVoid = 12,
        WaitingMiddleApproval = 13,
        SendForPayment = 14,
        Posted=15,
        SendForDepartmentApproval = 16,
    }
}
