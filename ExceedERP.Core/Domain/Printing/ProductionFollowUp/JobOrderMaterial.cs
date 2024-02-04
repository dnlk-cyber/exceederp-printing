using ExceedERP.Core.Domain.printing.PrintingEstimation.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
   public class JobOrderMaterial
    {
        public int JobOrderMaterialId { get; set; }
        public int JobId { get; set; }
        public string Name { get; set; }
        public JobOrderMaterialType Type { get; set; }
        public double Gram { get; set; }
        public double Size { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        [NotMapped]
        public double BeforeVat { get; set; }
        [NotMapped]
        public double TotalPrice { get; set; }
        public string SIVNo { get; set; }
        public string Remark { get; set; }
    }
}
