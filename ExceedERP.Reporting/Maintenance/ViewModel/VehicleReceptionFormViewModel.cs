using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.ViewModel
{
    public class VehicleReceptionFormViewModel : Organization
    {
        public string CustomerName { get; set; }
        public string PlateNo { get; set; }
        public string VehicleType { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleModel { get; set; }
        public decimal Mileage { get; set; }
        public FuelValue Fuel { get; set; }
        public List<string> TellNo { get; set; }
        public string Tell { get; set; }
        public string FaxNo { get; set; }
        public string Date { get; set; }
        public string CustomersAuthorizedAgent { get; set; }
        public string Reception { get; set; }
        public string TechnicianAssigned { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<VehiclePartDamageLevel> VehiclePartDamageLevels1 { get; set; }
        public virtual ICollection<VehiclePartDamageLevel> VehiclePartDamageLevels2 { get; set; }
        public virtual ICollection<VehiclePart> VehicleParts1 { get; set; }
        public virtual ICollection<VehiclePart> VehicleParts2 { get; set; }
    }
    public class VehiclePartDamageLevel
    {

        public string Code { get; set; }
        public string Name { get; set; }
      

    }
    public class Contact
    {

        public string PhoneNo { get; set; }
      

    }
    public class VehiclePart
    {

        public string Part { get; set; }
        public decimal Quantity { get; set; }
        public string Code { get; set; }
      

    }
}
