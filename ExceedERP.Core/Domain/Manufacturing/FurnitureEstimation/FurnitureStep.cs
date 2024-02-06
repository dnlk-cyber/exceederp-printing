using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureStep : TrackUserSettingOperation
    {
        public FurnitureStep()
        {
            this.DateCreated = DateTime.UtcNow;
            this.LastModified = DateTime.UtcNow;
        }
        public int FurnitureStepId { get; set; }
        [Required]
        public FurnitureSteps FurnitureSteps { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
