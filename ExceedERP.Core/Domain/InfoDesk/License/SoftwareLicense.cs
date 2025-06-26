using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.InfoDesk.License
{
    public class SoftwareLicense: TrackUserSettingOperation
    {
        public int SoftwareLicenseId { get; set; }
        [Display(Name = "License ID.")]
        public string LicenseId { get; set; }
        [Display(Name = "Software Name.")]
        [Required]
        public string SoftwareName { get; set; }
        [Display(Name = "Version")]
        public string Version { get; set; }
        [Display(Name = "Vendor")]
        public string Vendor { get; set; }
        public SoftwareLicenseType LicenseType { get; set; }
        [Display(Name = "License Key")]
        public string LicenseKey { get; set; }
        [DisplayName("Purchase Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PurchaseDate { get; set; }
        [DisplayName("License Expiry/Renewal Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime LicenseExpiryOrRenewalDate { get; set; }
        [DisplayName("Number of Licenses")]
        public int NumberofLicenses { get; set; }
        [DisplayName("Number of Users")]
        public int NumberofUsers { get; set; }
        [DisplayName("Cost per License")]
        public decimal CostperLicense { get; set; }
        [DisplayName("Total Cost")]
        public decimal TotalCost { get; set; }
        [DisplayName("Department")]
        public int OrgStructureId { get; set; }
        [DisplayName("Assigned Users(Qty)")]
        public int AssignedUsers { get; set; }
        public LicenseStatus LicenseStatus { get; set; }
        [DisplayName("Usage Limitations")]
        public string UsageLimitations { get; set; }
        [DisplayName("Renewal/Expire Notif. Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? RenewalNotificationDate { get; set; }
        [DisplayName(" Is Renewal Notified")]
        public bool IsRenewalNotified { get; set; }
        [DisplayName(" Support Contact")]
        public string SupportContact { get; set; }
        [DisplayName("Comments/Notes")]
        public string CommentsOrNotes { get; set; }



    }

    public class AssignedUser: TrackUserSettingOperation
    {
        public int AssignedUserId { get; set; }
        public int? EmployeeId { get; set; }
        public string  UserName{get;set;}
        public string  Remark{get;set;}
        public int SoftwareLicenseId { get; set; }
        [NotMapped]
        public bool IsNotified { get; set; }
        public virtual SoftwareLicense SoftwareLicense { get; set; }

    }
    public enum SoftwareLicenseType
    {
        Single_User,
        Multi_User,
        Enterprise,
        Subscription,
        Perpetual,
    }
    public enum LicenseStatus
    {
        Active,
        Expired,
        Renewal,
        Pending,
        Retired,
        Others
    }
}
