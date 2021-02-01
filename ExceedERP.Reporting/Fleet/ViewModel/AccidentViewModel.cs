using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class AccidentViewModel :Organization
    {
        public Guid FleetsID { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateOfOccurrence { get; set; }
        public string CauseOfAccident { get; set; }
        public string LevelOfDamage { get; set; }
        public enum LevelOfDamageenum { Minor, Major, TotalDamage }
        public string Accountablebody { get; set; }
        public string AccidentLocation { get; set; }
        public string LicenseNo { get; set; }
        public string Policereport { get; set; }
        public string Selectreport { get; set; }
        public string Description { get; set; }
        public decimal CostcoveredbyInsurance { get; set; }
       public decimal CostcoveredbyCompany { get; set; }
          public decimal CostcoveredbyOperator { get; set; }

        public decimal CostcoveredbyOtherBodies { get; set; }
    }
}
