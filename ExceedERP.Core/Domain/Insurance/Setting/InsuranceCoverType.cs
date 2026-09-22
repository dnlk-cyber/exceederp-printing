using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance.Setting
{

    ///// <summary>
    /// Represents the configuration settings for an employee's InsuranceCoverType.
    /// Created by Kibrom Abera, Feb  11, 2026.
    /// </summary>
    public class InsuranceCoverType : TrackUserSettingOperation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the insurance cover type.
        /// </summary>
        public int InsuranceCoverTypeId { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the insurance cover type.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Cover Code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the insurance cover type.
        /// </summary>
        [Required]
        [StringLength(150)]
        [Display(Name = "Cover Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the insurance cover type.
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cover type is active.
        /// </summary>
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets additional remarks related to the insurance cover type.
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }


}
