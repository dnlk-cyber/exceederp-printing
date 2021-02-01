using ExceedERP.Core.Domain.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.ViewModel
{
            public class CampaignView
    {

        public int CampaignID { get; set; }
        public string Status { get; set; }


        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }


        [Display(Name = "Team")]
        public string TeamId { get; set; }


        [Display(Name = "Cart")]
        public string CartID { get; set; }

        [Display(Name = "Campaign Type")]
        public string CompaignType { get; set; }

        public decimal Budget { get; set; }

        [Display(Name = "Expected Revenue")]
        public decimal ExpectedRevenu { get; set; }
        public DateTime? CampaginDate { get; set; }

        public string Objective { get; set; }

        public string Description { get; set; }


    }
}
