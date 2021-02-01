 using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class Variety : TrackUserOperation
    {

        public int VarietyID { get; set; }
       [Required]
        [DisplayName("Variety Name")]
        public string VarietyName { get; set; }
        [Required]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        public string Class { get; set; }





    }
  
}
