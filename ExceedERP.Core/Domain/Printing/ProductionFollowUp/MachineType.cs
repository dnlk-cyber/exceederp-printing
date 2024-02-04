using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.printing.JobCosting;
//using ExceedERP.Core.Domain.printing.ProductionFollowUp;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class Machine
    {
        public int MachineId { get; set; }
        [Display(Name = "Machine Name")]
        public string MachineName { get; set; }
        [Display(Name = "Category")]
        public Category Category { get; set; }

        [Display(Name = "Sub Category")]
        public Sub_Category SubCategory { get; set; }

        [StringLength(150)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
    //wait
    public class Settings
    {
        public int SettingsId { get; set; }

        [StringLength(150)]
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [StringLength(15)]
        [Required]
        [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }

        [StringLength(15)]
        [Required]
        [Display(Name = "Company Phone")]
        public string CompanyPhone { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Company Email")]
        public string CompanyEmail { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Company Mail")]
        public string CompanyMail { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Company Vat No")]
        public string CompanyVatNo { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Company TIN")]
        public string CompanyTIN { get; set; }

        [Required]
        [Display(Name = "Company Logo")]
        public byte[] CompanyLogo { get; set; }

        [Required]
        [Display(Name = "Company Screen")]
        public byte[] CompanyScreen { get; set; }

        [StringLength(15)]
        [Required]
        [Display(Name = "SMS Sender Status")]
        public string SMSSenderStatus { get; set; }

        [Required]
        [Display(Name = "Edit Grace")]
        public int EditGrace { get; set; }

        [Required]
        [Display(Name = "Fiscal Year Start")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FiscalYearStart { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Dashboard Calendar")]
        public string DashboardCalendar { get; set; }
    }

    //public class CommertialItems
    //{
    //    public int CommertialItemsId { get; set; }
    //    [Display(Name = "Job Type")]
    //    public Job_Type Job_Type { get; set; }
    //    [Display(Name = "Sub Category")]
    //    [StringLength(50)]
    //    public string SubCategory { get; set; }
    //    [StringLength(150)]
    //    public string Description { get; set; }
    //}
}
