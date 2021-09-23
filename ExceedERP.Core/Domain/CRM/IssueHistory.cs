using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class IssueHistory  :  Operations
    {
       public int IssueHistoryId { get; set; }
       public int InvoiceId { get; set; }
       public int Amount { get; set; }
      

    }
}
