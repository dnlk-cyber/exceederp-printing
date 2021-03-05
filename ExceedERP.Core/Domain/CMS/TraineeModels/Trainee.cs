using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    /// <summary>
    /// Trainee - Either individual Personnel or Company
    /// </summary>
    public class CmsTrainee : TrackUserSettingOperation
    {
        [Key]
        public int CmsTraineeId { get; set; }

        [Display(Name ="Type")]
        public TraineeType Type { get; set; }

        [Display(Name ="Registration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name ="Registration No.")]
        public string RegistrationNumber { get; set; }

        [Display(Name ="Program")]
        public int CMSProgramId { get; set; }

        [Display(Name ="Course")]
        public int CMSCourseId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name ="Full Name")]
        public string    FullName { get; set; }

        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        [Display(Name ="Company Name")]
        public string CompanyName { get; set; }


        public Region Region { get; set; }
        public string Address { get; set; }

        [Display(Name ="City")]
        public int CityId { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Place of Birth")]
        public string PlaceOfBirth { get; set; }

        [Display(Name ="Phone #")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name ="Mobile Phone #")]
        public string MobilePhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Prefered Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PreferedDate { get; set; }

        [Display(Name ="Training Venue")]
        public string TrainingVenue { get; set; }

        public string ProfilePicFileName { get; set; }

        [NotMapped]
        public string BarcodeImageName { get; set; }

        [Display(Name ="Zip Code")]
        public string ZipCode { get; set; }

        [Display(Name ="Participant Requirement")]
        public string Requirement { get; set; }

        /// <summary>
        /// Status of Trainee / Company
        /// For Trainee - New, On Training .... examination result
        /// For Company - New, .... to evaluation result
        /// </summary>
        public string Status { get; set; }


    }
}
