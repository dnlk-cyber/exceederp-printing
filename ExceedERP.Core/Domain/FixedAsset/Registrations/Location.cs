using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
    public class Location : TrackUserOperation
    {
        public int LocationID { get; set; }

        [Display(Name = "Fixed Asset")]
        public int AssetRegistrationId { get; set; }

        public virtual AssetRegistration AssetRegistration { get; set; }

       
        [Display(Name = "Belong To"), Required]
        public string LocationUser { get; set; }


        [Display(Name = "Received Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [Display(Name = "Return Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        public string Description { get; set; }
    }
}
