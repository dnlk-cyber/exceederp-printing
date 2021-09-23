using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ExceedERP.Core.Domain.CRM
{
    public enum CampaignType
    {
        Email,
        Newsletter,
        Mail,
        Web,
        Radio,
        Television,
        Print,
        Telesales
    }


    public class Campaign : Operations
    {
        public int CampaignID { get; set; }
        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [Required]
        [Display(Name = "Team")]
        public string TeamId { get; set; }

            
        [Display(Name = "Cart")]
        public string CartID { get; set; }
            [Required]
        [Display(Name = "Campaign Type")]
        public string CompaignType { get; set; }

            [Required]
        public decimal Budget { get; set; }
            [Required]

        [Display(Name = " Revenue")]
        public decimal ExpectedRevenu { get; set; }
     
        public string Objective { get; set; }
     
        public string Description { get; set; }

    }
    public class CampaignProduct : Operations
    {
        public int CampaignProductID { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignID { get; set; }

        [Display(Name = "Product")]
        public int ProductID { get; set; }

        public string Feedback { get; set; }

    }

    public class PreCustomer : Operations
    {
        public int PreCustomerID { get; set; }
        [Required]
        public string TradeName { get; set; }
          [Required]
        public string BrandName { get; set; }
          [Required]
        public string BusinessType { get; set; }
          [Required]
        public string Category { get; set; }

        public string Status { get; set; }
        public bool IsActive { get; set; }

    }
    public class PreCustomerView
    {
        public int PreCustomerViewID { get; set; }
        public string TradeName { get; set; }
        public string BrandName { get; set; }
        public string BusinessType { get; set; }
        public string Category { get; set; }

        public string Status { get; set; }
        public bool IsActive { get; set; }
    }


    public class PreCustomerSchedule : Operations
    {
        public int PreCustomerScheduleID { get; set; }
        public string PreCustomerID { get; set; }
        [DataType(DataType.Date)]
        [DisplayName(" Scheduled Date ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
      
        public DateTime ScheduledDate { get; set; }
       
        public string ScheduleStatus { get; set; }

        public string ScheduleOutcome { get; set; }
        public bool IsActive { get; set; }
    }
    public class CustomerHandlingTeam : TrackUserOperation
    {
        [Key]
        public int CustomerHandlingTeamID { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
        public bool IsActive { get; set; }
        public int? ParentID { get; set; }
        public string Remark { get; set; }
    }
    public class MailModel  : TrackUserOperation
    {
        public int MailModelID { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}



