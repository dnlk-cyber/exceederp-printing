using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
   public class RegisterViewModel
    {
        public string Description { get; set; }
        public decimal? Addition { get; set; }
        public decimal? Desposal { get; set; }
        public decimal? Before { get; set; }
        public decimal? After { get; set; }
        public DateTime? DateBefore { get; set; }
        public DateTime? DateNow { get; set; }
    }
}
