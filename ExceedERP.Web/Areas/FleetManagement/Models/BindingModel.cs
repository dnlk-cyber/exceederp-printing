using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class BindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MechanicCrewViewModel
    {

        public Guid ServiceCrewID { get; set; }
        [DisplayName("Service")]
        [Required]
        public Guid ServiceID { get; set; }
        public Guid MechanicCrewID { get; set; }
        public string ServiceName { get; set; }
        public string Type { get; set; }

        public bool IsActive { get; set; }

        [DisplayName("Hourly Wage")]
        public decimal HourlyWage { get; set; }


        [NotMapped]
        [DisplayName("Labour Cost")]
        public decimal? LabourCost { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<DateTime> ActivatedDate { get; set; }

        


    }
    public class UploadBindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<HttpPostedFileBase> AttachmentFile { get; set; }
    }
}