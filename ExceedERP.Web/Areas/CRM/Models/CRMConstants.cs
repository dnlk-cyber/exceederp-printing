namespace ExceedERP.Web.Areas.CRM.Models
{
    public class CRMConstants
    {
        public const string PaymentReceived = "PaymentReceived";
        public const string PartialPaymentReceived = "Partial_Payment_Received";
        public const string ProductIssued = "Product_Issued";
        public const string Forwarded = "Forwarded";
        public const string UnForwarded = "UnForwarded";
        public const string Paid = "Paid";
        public const string WaitingPayment = "Waiting_Payment";
        public const string UnPaid = "UnPaid";
        public const string CreditPayment = "Credit_Payment";

        public const string Approved = "Approved";
        public const string Pending = "Pending";

        public const string HoldSalesInvoiceStatus = "Hold";
        public const string PrintedSalesInvoiceStatus = "Printed";

        public const string NoReference = "Manual";  // No_Reference
        public const string Rejected = "Rejected";

        public const string CreditPaymentMethod = "CREDIT";
        public const string CashPaymentMethod = "CASH";

        public const int NewCustomerId = 0;

        public const string WaitingAdminstrative = "Waiting Administrative Approval";

    }
    public class CRMVoucherStatusConstatns
    {
        public const string Editing = "Editing"; // new
        public const string WaitingApproval = "Waiting Approval";
        public const string Rejected = "Rejected";
        public const string Void = "Rejected";
        public const string Approved = "Approved";

        // 
        public const string WaitingFirstApproval = "Waiting CRM Approval";
        public const string WaitingSecondApproval = "Waiting Maintenance Approval";
        public const string WaitingThirdApproval = "Waiting Finance Approval";

        public const string FirstRejected = "CRM Rejected";
        public const string SecondRejected = "Maintenance Rejected";
        public const string ThirdRejected = "Finance Rejected";

        public const string FirstApproved = "CRM Approved";
        public const string SecondApproved = "Maintenance Approved";
        public const string ThirdApproved = "Finance Approved";
    }

    public class TenderFollowupStatus
    {
        public const string TechnicalEvaluation = "Under Technical Evalution";
        public const string LostTechnicalEvaluation = "Lost in Technical Evalution";
        public const string FinancialEvaluation = "Under Financial Evalution";
        public const string LostFinancialEvaluation = "Lost in Financial Evalution";
        public const string LostNonTechnicalEvaluation = "Lost in Non-Technical Evalution";
        public const string NonParticipating = "Non Participating";
        public const string Won = "Won";
        public const string AdvancePayment = "Advance Payment";
        public const string OpenLC = "Open LC";
    }
}