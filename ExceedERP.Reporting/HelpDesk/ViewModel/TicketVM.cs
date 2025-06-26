using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.ViewModel
{
   public class TicketVM
    {
        public string RequestDate { get; set; }
        public string Branch { get; set; }
        public string BusinessUnit { get; set; }
        public string RequesterDepartment { get; set; }
        public string Requester { get; set; }
        public string HandledBy { get; set; }
        public string ApprovedBy { get; set; }
        public int RequestNumber { get; set; }
       public List<RequestDetailVM> RequestDetails { get; set; }
       public List<ResponseDetailVM> ResponseDetails { get; set; }



    }

    public class RequestDetailVM
    {
        public string Date { get; set; }
        public string Customr { get; set; }
        public string TicketType { get; set; }
        public string Group { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
       

    }

    public class ResponseDetailVM
    {
        public string AssignedUser { get; set; }
        public string Problem { get; set; }
        public string SolutionGiven { get; set; }
        public string  IsExernal { get; set; }
        public string Remark { get; set; }
       

    }
}
