using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
        public class EquipmentInspectionViewModel : Organization
    {


         public string InspectedBy { get; set; }
        public string FindingsOutcome { get; set; }
          public DateTime DateofInspection { get; set; }
          public DateTime ? Date  { get; set; }
          public string EquipmentName { get; set; }     
          public string Name { get; set; }
          public string Description { get; set; }
          public string Code { get; set; }
          public string IsActive { get; set; }
    }
}
