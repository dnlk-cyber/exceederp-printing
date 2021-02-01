using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class ExternalMaintenanceViewModel :Organization
    {



        public string EquipmentName { get; set; }
        public string NameofCustomer { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string OperatorName { get; set; }
        public string BankSlipNo { get; set; }
        public string BankBranchName { get; set; }
        public string Description { get; set; }
        public enum ApprovedStatus { Approved, Pending }
        public string ApprovedBy { get; set; }
    }
}
