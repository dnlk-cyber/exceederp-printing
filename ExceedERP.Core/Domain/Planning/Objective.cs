using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class Objective: TrackUserSettingOperation
    {
        public int ObjectiveId { get; set; }
        [Display(Name = "Work Unit")]
        public int OrgStructureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}
