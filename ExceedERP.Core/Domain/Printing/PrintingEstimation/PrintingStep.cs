using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.PrintingEstimation
{
    public class PrintingStep : TrackUserSettingOperation
    {
        public PrintingStep()
        {
            this.DateCreated = DateTime.UtcNow;
            this.LastModified = DateTime.UtcNow;
        }
        public int PrintingStepId { get; set; }
        [Required]
        public PrintingSteps PrintingSteps { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
