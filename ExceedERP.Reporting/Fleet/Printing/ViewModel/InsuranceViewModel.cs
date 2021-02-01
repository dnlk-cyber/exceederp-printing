using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class InsuranceViewModel : Organization
    {
                public string EquipmentType { get; set; }
                public string Model { get; set; }
                public string PlateNo { get; set; }
                public Guid FleetsID { get; set; }

                public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }
                
                public string InsuranceCompany { get; set; }
          
                public decimal PremiumCost { get; set; }
       
                public Nullable<DateTime> DueDate { get; set; }
             
                public Nullable<DateTime> ExpireDate { get; set; }
              
                public string PhoneNo { get; set; }

                 public string Email { get; set; }
                 public string City { get; set; }

                public string Attachment { get; set; }
                public string Description { get; set; }

    }
}
