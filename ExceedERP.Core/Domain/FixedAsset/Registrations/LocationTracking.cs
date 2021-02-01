using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
    public enum LocationType
    {
        NotSet,
        Employee,
        Store,
        Office
    }
    public class LocationTracking : TrackUserOperation
    {
        public int LocationTrackingId { get; set; }

        [Display(Name = "AssetRegistration")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }
        [Required]
        [Display(Name = "Type")]
        public LocationType LocationType { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Return Date")]
        public DateTime? EndDateDate { get; set; }
        public string Description { get; set; }
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }
        [Display(Name = "Branch Or Project")]
        public int BranchId { get; set; }
        [DisplayName("Work Unit")]
        public string OrgStructureId { get; set; }
        [DisplayName("Transaction Type")]
        public string TransactionType { get; set; }
        [DisplayName("Transaction No")]
        public string TransactionNo { get; set; }
        public bool IsCurrent { get; set; }
    }
}
