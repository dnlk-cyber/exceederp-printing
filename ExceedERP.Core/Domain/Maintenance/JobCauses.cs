using System;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class JobCauses : TrackUserSettingOperation
    {
            public Guid JobCausesId { get; set; }
            public Guid OrderId { get; set; }
                [Required]
            public string Name { get; set; }
            public string Description  { get; set; }
           public bool ShowOnReception { get; set; }
           public int FromReceptionCount { get; set; }
    }
}
