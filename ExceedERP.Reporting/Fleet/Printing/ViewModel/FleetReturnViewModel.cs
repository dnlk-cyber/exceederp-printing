using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class FleetReturnViewModel : Organization
    {

        public int FleetReturnId { get; set; }
        public int FleetReturnItemId { get; set; }
        public Guid FleetsId { get; set; }

        public string From { get; set; }

        public string DeliveredTo { get; set; }

        public string DebitAccountNo { get; set; }

        public string CreditAccountNo { get; set; }


        public string PlateNo { get; set; }

        public string EquipmentName { get; set; }
        public string Manufacturer { get; set; }

        public string ChancesSerialNo { get; set; }
        public string EngineNo { get; set; }

        public string OwnerCertificateNo { get; set; }

        public string Condition { get; set; }


        public string AssetNo { get; set; }

        public string Causeofreturn { get; set; }

        public string CheckList { get; set; }

        public string IdentifcationNo { get; set; }

        public string CostCode { get; set; }
        public string Description { get; set; }
        public string PartNo { get; set; }
        public string Location { get; set; }
        public string Requested { get; set; }

        public string Recieved { get; set; }
        public string Unit { get; set; }
        public decimal UnitPriceBirr { get; set; }

        public decimal UnitPriceCent { get; set; }
        public decimal TotalPriceCent { get; set; }
        public decimal TotalPriceBirr { get; set; }
    }
}
