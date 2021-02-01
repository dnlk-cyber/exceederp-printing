using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class InternalRentViewModel :Organization
    {
        public enum EquipmentTypeenum { DoubleCap, Dump, Track, Excavator }
        public enum EquipmentOwnerenum { Company, Rental }
        public string EquipmentType { get; set; }
        public string EquipmentOwner { get; set; }
        public Nullable<DateTime> AssignedDate { get; set; }
        public Nullable<DateTime> StartingDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string AssignedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        public Nullable<int> RequestQuantity { get; set; }
        public Nullable<int> AssignedQuantity { get; set; }
    }
}
