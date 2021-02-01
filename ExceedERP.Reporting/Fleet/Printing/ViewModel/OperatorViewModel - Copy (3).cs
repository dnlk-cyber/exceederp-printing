using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class OperatorViewModell : Organization
    {
                public string EquipmentType { get; set; }
                public string Model { get; set; }
                public string PlateNo { get; set; }
                public DateTime? DownDate { get; set; }
                public string ReasonForPending { get; set; }
                public string DownDuration { get; set; }
                public decimal DownCost { get; set; }
    }
}
