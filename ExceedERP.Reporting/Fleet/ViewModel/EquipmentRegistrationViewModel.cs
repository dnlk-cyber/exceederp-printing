using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    class EquipmentRegistrationViewModel
    {
        public Guid EquipmentTypeID { get; set; }
        public Guid EquipmentNameID { get; set; }
        public string EquipmentName { get; set; }
        public string SubType { get; set; }
        public string Type { get; set; }
        public string UnitNum { get; set; }
        public string ManufacturerYear { get; set; }
        public string Condition { get; set; }
        public string EngineModel { get; set; }
        public string OperatorName { get; set; }
    }
}
