
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Procurement.Tender
{

    public enum MessageStatus
    {
        Seen,
        Forwarded,
        NotSeen
    }
   public class TenderMessages
    {
       public int TenderMessagesId { get; set; }
       [Display(Name = "Department")]
       public string ReceiverDepartment { get; set;  }
       [Display(Name = "From")]
       public string Sender { get; set;  }
       [Display(Name = "To")]
       public string Receiver { get; set; }
       public string Message { get; set;  }
       [Display(Name = "Document")]
       public string FileName { get; set;  }
       public string PhysicalPath { get; set;  }
       public DateTime SentDate { get; set;  }
       public DateTime ReceiveDate { get; set; }
       public MessageStatus MessageStatus { get; set;  }
       public Tender Tender { get; set;  }
       public int TenderId { get; set;  }
       
   
    }
}

