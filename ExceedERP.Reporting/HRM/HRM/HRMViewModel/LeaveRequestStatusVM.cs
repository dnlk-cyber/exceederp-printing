using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class LeaveRequestStatusVM
    {
        public string ApplicantName { get; set; }
        public string Department { get; set; }
       
        public List<RequestesStatusVM> LeaveRequestStatus { get; set; }

    }
    public class RequestesStatusVM
    {
        public string Period { get; set; }
        public string Type { get; set; }
        public string ActionType { get; set; }
        public string Day { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal RequestAmount { get; set; }
        public decimal LeaveDays { get; set; }
        public List<RequestesStatusDetailVM> RequestStatusDetail { get; set; }
       
    }
    public class RequestesStatusDetailVM
    {
        public string AproverName { get; set; }
        public string ApproverDepartmentName { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
    }



}