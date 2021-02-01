using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{




    public class FleetInput 
    {
        public static string Id { get; set; }
    
    }
    public class MaintenanceConst
    {
        public const string ABC = "ABCDEFGHIJKLMNOPQRSTVXYZ";


        public const string Assigned = "Assigned";
        public const string Rent = "Rent";
        public const string Closed = "Closed";
        public const string InspectionDone = "Inspection Done";
        public const string SentForClosing = "Sent For Closing";
        public const string Open = "Open";
        public const string Received = "Received";
        public const string Company = "Company";
        public const string Internal = "INTERNAL";
        public const string EXTERNAL = "EXTERNAL";
        public const string Request = "Pending";
        public const string Rejected = "Rejected";
        public const string R = "Request";

        public const string UnAssigned = "UnAssigned";

        public const string ACTIVE = "Operational";
        public const string ONDUTY = "On Duty";
        public const string OnMaintenance = "Under_Maintenace";

        public const string WaitingTechnical = "Waiting Technical Approval";
        public const string WaitingAdminstrative = "Waiting Administrative Approval";
        public const string Approved = "Approved";
        public const string WaitingApproval= "Waiting Approval";
        public const string Editing = "Editing";

        public const string Forwarded = "Forwarded";

        public const string Day = "Day";
        public const string Week = "Week";
        public const string Month = "Month";
        public const string Year = "Year";
 
        public const string Planned = "Planned";
        public const string UnPlanned = "UnPlanned";
        public const string Male = "Male";
        public const string Female = "Female";

        public const string Pending = "Pending";
        public const string Finished = "Finished";

        public const string SINO = "SINO";
        public const string NISSAN = "NISSAN";
        public const string TOYOTA = "TOYOTA";

        public const string BLACK = "BLACK";
        public const string BLUE = "BLUE";
        public const string RED = "RED";
        public const string WHITE = "WHITE";



        public const string Completed = "Completed";


            //TimeSheet Const
        public const string RentalTimeSheet = "RentalTimeSheet";
        public const string DistributionTimeSheet = "DistributionTimeSheet";
    }
}