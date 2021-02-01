using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
   public class MachineryCountSummarySheetVm
    {
        public string TagNo { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string ChasisNo { get; set; }
        public string MotorNo { get; set; }
        public string PlateNo { get; set; }
        public string Unit { get; set; }
        public string RrtNo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

    public class OfficeEquipmentCountSummarySheetVm
    {
        public string TagNo { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string Unit { get; set; }
        public string RrtNo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

    public class ScafoldingAndPanelCountSummarySheetVm
    {
        public string Description { get; set; }
        public string Size { get; set; }
        public string Model { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }
        public string RrtNo { get; set; }
        public string SIV { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
