using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class CannibalizationViewModel : Organization
    {

        public string PartNeededFleets{ get; set; }
        public string PartTakenFleets{ get; set; }
        public string Model { get; set; }
        public enum UnitofMeasurementenum { Set, Pcs }
        public string UnitofMeasurement { get; set; }
        public string PartName { get; set; }
        public Nullable<int> Quantity { get; set; }

        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public string TakenBy { get; set; }
        public string Description { get; set; }
    }
}
