using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class EquipmentDistributionVewModel : Organization
    {
        public string Type { get; set; }
        public Nullable<DateTime> StartingDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string FromBranchName { get; set; }
        public string OperatorName { get; set; }
        public string FromStructureName { get; set; }
        public string RequestingBranchName { get; set; }
        public string RequestingStructureName { get; set; }
        public string Referance { get; set; }
        public int? RequestNumber { get; set; }
        public Nullable<int> DistrubtionNumber { get; set; }
        public Nullable<int> RequestQuantity { get; set; }
        public Nullable<int> AssignedQuantity { get; set; }
        public string CategoryRef { get; set; }
        public string EquipmentStatus { get; set; }
        public string RequestDesc { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        public DateTime CheckedDate { get; set; }
        public string demandApprovalBy { get; set; }
        public DateTime demandApprovalDate { get; set; }

        public DateTime DistributionApprovalDate { get; set; }
        public string RequestBy { get; set; }
        public string EmployeeName { get; set; }
        public bool IsAssigned { get; set; }
        public string ReasonForTravel { get; set; }
        public string NumberHourPerKm { get; set; }
        public string Destination { get; set; }
        public string Remark { get; set; }
        public string NumberofPersonOrTon { get; set; }
        public string EquipmentName { get; set; }
        public string PlateNumber { get; set; }
        public string EquipmentType { get; set; }
        public string AccountNumber { get; set; }
        public string EquipmentCategory{get;set;}
        public string ManufacturingYear { get; set; }
        public string ApprovedBy { get; set; }
        public string CheckedBy { get; set; }
        public string RequestedBy { get; set; }
        public decimal StartOdometer { get; set; }
        public decimal EndOdometer { get; set; }
        public decimal NetOdometer { get; set; }

        public string PreparedBy { get; set; }
        //Additional Info

        [Display(Name = "Km Traveled Per Liter")]
        public float KmTravelledPerLitre { get; set; }
        [Display(Name = "Km Traveled Total")]
        public float KmTravelledTotal { get; set; }
        [Display(Name = "Km Intersite")]
        public float KmInterSite { get; set; }
        [Display(Name = "Km Grand Total")]
        public float KmGrandTotal { get; set; }

        [Display(Name = "Req Fuel In Liter")]
        public float ReqFuelInLiter { get; set; }

        [Display(Name = "Total In Litre")]
        public float TotalInLitre { get; set; }
        [Display(Name = "From Deppot")]
        public float FromDeppot { get; set; }
        [Display(Name = "In Cash")]
        public decimal InCash { get; set; }
        [Display(Name = "From Project")]
        public float FromProject { get; set; }
        [Display(Name = "Oil Req For Engine")]
        public float OilReqForEngine { get; set; }
        [Display(Name = "Oil Req For Other")]
        public float OilReqForOther { get; set; }
        [Display(Name = "Grease Req")]
        public float GreaseReq { get; set; }
        [Display(Name = "Total")]
        public decimal TotalBirr { get; set; }
        [Display(Name = "Total Perdium")]
        public decimal TotalPerdium { get; set; }
        //Added
        public Image CreatedBySignature { get; set; }
        public Image ApproveddBySignature { get; set; }
        public Image CheckedBySignature { get; set; }
    }

}
