using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class Appreciation : TrackUserOperation
    {
        public int AppreciationID { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int RegistrationId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AppreciationDate { get; set; }
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }

    }
}
