using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
  public  class RentRequestViewModel :Organization
    {


        public Guid RentRequestID { get; set; }
        public int? RequestNumber { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }

       public Nullable<int> RequestQuantity { get; set; }
       public Nullable<int> AssignedQuantity { get; set; }
     
       public Nullable<double> AssignedDuration { get; set; }
        public string FromBranchName { get; set; }
        public string FromStructureName { get; set; }

        public string RequestingBranchName { get; set; }

        public string RequestingStructureName { get; set; }
        public Nullable<DateTime> RequestedDate { get; set; }
               public Nullable<DateTime> StartingDate { get; set; }

         public Nullable<DateTime> ToDate { get; set; }


         public string Description { get; set; }
        public string EquipmnetStatus { get; set; }
    }
}
