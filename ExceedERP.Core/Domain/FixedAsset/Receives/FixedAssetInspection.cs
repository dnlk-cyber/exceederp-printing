using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetInspection : TrackUserOperation
    {
        public int FixedAssetInspectionId { get; set; }
        [Display(Name = "Inspection Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InspectionDate { get; set; }
       
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        
        [Display(Name = "Accepted?")]
        public bool Accepted { get; set; }

        public string Remark { get; set; }
        public int FixedAssetReceiveId { get; set; }
        public virtual FixedAssetReceive FixedAssetReceive { get; set; }

    }
}