using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
    public class AssetMovementVm
    {
        public LocationType LocationType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }
        public string Store { get; set; }
        public string Employee { get; set; }
        public string Branch { get; set; }
        public string  IsCurrent { get; set; }
    }
}
