using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.PrintingEstimation
{
    public class PrintingCriteriaCategory : TrackUserSettingOperation
    {
        [Key]
        public int PrintingCriteriaCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
