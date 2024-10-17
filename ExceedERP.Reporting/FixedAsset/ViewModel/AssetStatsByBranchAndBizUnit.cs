using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
   public class AssetStatsByBranchAndBizUnitVM
    {
        public string BizUnit { get; set; }
        public string Branch { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int Qty { get; set; }
        public bool Highlight { get; set; }
    }
}
