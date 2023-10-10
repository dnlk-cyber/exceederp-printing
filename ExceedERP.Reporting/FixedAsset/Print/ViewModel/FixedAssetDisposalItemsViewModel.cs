using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.FixedAsset.Print.ViewModel
{
    class FixedAssetDisposalItemsViewModel
    {
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string GrnNo { get; set; }
        public string Size { get; set; }
        public string TagNo { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal BookValue { get; set; }
        public decimal TaxAmount { get; set; }
        
        public decimal Total { get; set; }

        public string BatchNo { get; set; }
        public string Manufacturer { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }

        [Display(Name = "Condition")]
        public string Condition { get; set; }

        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }

        [Display(Name = "Chassis No.")]
        public string ChassisNo { get; set; }

        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }

        [Display(Name = "Engine Model No.")]
        public string EngineModelNo { get; set; }

        [Display(Name = "Electrical Part (Body Part) S.No.")]
        public string EngineElectricalNo { get; set; }

        [Display(Name = "Electrical Part (Body Part) Model No.")]
        public string EngineElectricalModelNo { get; set; }

        public string Remark { get; set; }
    }

    class FixedAssetDisposalViewModel : Organization
    {
        public int DisposalNumber { get; set; }
        public string Customer { get; set; }
        public string ExternalCustomer { get; set; }
        public DateTime Date { get; set; }
        public string DisposalType { get; set; }
        public string Discription { get; set; }

        public string ApproverName { get; set; }
        public string ApprovedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Datecreated { get; set; }
        public string StoreKeeperName { get; set; }

    }
}
