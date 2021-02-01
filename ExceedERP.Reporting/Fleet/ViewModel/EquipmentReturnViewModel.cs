using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{

   public  class EquipmentReturnViewModel    : Organization
    {

        public string Equipment { get; set; }
        public Nullable<DateTime> ReturnDate { get; set; }
         public string TotalTimeElapsed { get; set; }
         public string ReturnedBy { get; set; }
        public string ReturnStatus { get; set; }
        public enum ReasonforReturnenum { WorkisExecuted, InconvenienceforWork, EquipmentUnderMaintenance }
         public string ReasonforReturn { get; set; }
         public string Description { get; set; }
    }
}
