using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel
{
    public class CannibalizationViewModel : Organization
    {
  

        public string PartNeededModel { get; set; }
        public string PartTakenModel { get; set; }

        public string PartNeededType{ get; set; }
        public string PartTakenType { get; set; }

        public string PartNeededSerialNo { get; set; }
        public string PartTakenSerialNo { get; set; }

        public string PartNeededChasisNo { get; set; }
        public string PartTakenChasisNo { get; set; }

        public bool IsOperational { get; set; }
        public bool IsBreakDown { get; set; }
        public bool IsScrap { get; set; }
        public bool IsPoor { get; set; }
        [DisplayName(" Is Operational ")]
        public bool IsFromOperational { get; set; }
        [DisplayName(" Break Down ")]
        public bool IsFromBreakDown { get; set; }
        [DisplayName(" Scrap ")]
        public bool IsFromScrap { get; set; }
        [DisplayName(" Poor ")]
        public bool IsFromPoor { get; set; }
        public bool IsPermanent { get; set; }
        public string Status { get; set; }
        public string ProjectName { get; set; }
        public string TakenBy { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public enum UnitofMeasurementenum { Set, Pcs }
        public string UnitofMeasurement { get; set; }
        public string PartName { get; set; }
        public string PartType { get; set; }
        public int Num { get; set; }
        public string WHPRNo { get; set; }
        public int Quantity { get; set; }

        public string Operator { get; set; }
        public string Inspector { get; set; }
        public string Approver { get; set; }


    }
}
