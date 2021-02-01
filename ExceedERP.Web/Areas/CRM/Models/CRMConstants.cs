using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public const string NoReference = "Manual";  // No_Reference
        public const string Rejected = "Rejected";

        public const string CreditPaymentMethod = "CREDIT";
        public const string CashPaymentMethod = "CASH";

        public const int NewCustomerId = 0;


    }
}