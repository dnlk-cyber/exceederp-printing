using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class MasterStatusViewModel
    {
        public string Status { get; set; }
        public List<SubTypeViewModel> subTypeViewModel { get; set; }
    }
    public class SubTypeViewModel
    {
        public string MachineryAndVehicleType { get; set; }
       

        public List<MachineryAndVehicleDTO> Machineries { get; set; } = new List<MachineryAndVehicleDTO>();

    }
    public class MachineryAndVehicleDTO
    {
        public string EquipmentName { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialPlateNo { get; set; }
        public string Operator { get; set; }
        public string Status { get; set; }
        public string Problem { get; set; }
        public string OwnRental { get; set; }
        public string Project { get; set; }
    }
}
