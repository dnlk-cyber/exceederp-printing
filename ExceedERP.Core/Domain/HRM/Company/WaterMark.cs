using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ExceedERP.Core.Domain.HRM.Company
{
   public class WaterMark
    {
        public int WaterMarkId { get; set; }
        public bool IsNeened { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string WaterMarkFileName { get; set; }
    }
}
