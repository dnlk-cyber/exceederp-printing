using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
    public class FixedAssetHeader : Organization
    {
        public string Year { get; set; }
        public string Branch { get; set; }
    }
    public class FixedAssetReportHeader : Organization
    {
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ProjectOrBranch { get; set; }
    }

    public class FixedAssetCountSheetHeader : Organization
    {
        public string FiscalYear { get; set; }
        public string CountDate { get; set; }
        public string ProjectOrBranch { get; set; }
        public string No { get; set; }
        public string WorkUnit { get; set; }
        public string ResponsibleUser { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string OperatorOrDriverName { get; set; }
    }

    
}
