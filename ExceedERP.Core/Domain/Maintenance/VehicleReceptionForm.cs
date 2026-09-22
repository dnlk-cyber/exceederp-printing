using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class VehicleReceptionForm : Operations
    {
        public int VehicleReceptionFormId { get; set; }
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }
        [Display(Name = "Reception")]
        public int RecipientId { get; set; }
        public string CustomerName { get; set; }
        public Guid? VehicleCategoryId { get; set; }
        [Display(Name = "Inspector Assigned")]
        public int ? TechnicianAssignedId { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }
        public string ReferenceNumber { get; set; }
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }
        [Display(Name = "Vehicle Model")]
        public string VehicleModel { get; set; }
        [Display(Name = "Mileage")]
        public decimal Mileage { get; set; }
        [Display(Name = "Fuel")]
        public FuelValue Fuel { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        public Source Source { get; set; }
        public bool FromRequest { get; set; }
        public string DriverTransferId { get; set; }
        public string JobRequestId { get; set; }
        public string SitesVehicleTransferID { get; set; }
        public bool IssentToBackToJobRequest { get; set; }
       
    }

    public class TyreSerialInfo:TrackUserSettingOperation
    {
        public int TyreSerialInfoId { get; set; }
        public int VehicleReceptionFormId { get; set; }
        public string TireSize { get; set; }
        public string TireModel { get; set; }
        public string TireName { get; set; }
        public string NumberOfTire { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }
    }

    public class AccesseryInfo:TrackUserSettingOperation
    {
        public int AccesseryInfoId { get; set; }
        public int VehricleReceptionFormId { get; set; }
        public string AccName { get; set; }
        public string AccModel { get; set; }
        public string Description { get; set; }
        public int NumberOfAcc { get; set; }
    }
    public enum Source { External,Internal,ExternalCustomer}
    public enum FuelValue
    {
        [Display(Name = "E-empty")]
        [Description("E-empty")]
        E_empty,
        [Display(Name = "Red light")]
        [Description("Red light")]

        Red_light,
        [Display(Name = "1/2")]
        [Description("1/2")]
        One_over_two,
        [Display(Name = "B/n 1/2 and 3/4")]
        [Description("B/n 1/2 and 3/4")]
        Bn_1_over_2_and_3_over_4,
        [Display(Name = "3/4")]
        [Description("3/4")]
        Three_over_four,
        [Display(Name = "B/n 3/4 and full")]
        [Description("B/n 3/4 and full")]
        Bn_3_over_4_and_full,
        Full,


    }
    public class VehicleReceptionApproval : TrackUserOperation
    {
        public int VehicleReceptionApprovalId { get; set; }
        public int VehicleReceptionFormId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class VehicleReceptionDocument
    {
        public int VehicleReceptionDocumentId { get; set; }
        public int VehicleReceptionFormId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }

    }
}
