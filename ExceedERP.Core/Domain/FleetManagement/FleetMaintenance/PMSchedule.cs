using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class PMSchedule : TrackUserOperation
    {
        public int PMScheduleId { get; set; }
          [Required]
          [DisplayName("Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
          [Required]
          [DisplayName("Year")]
        public int GlFiscalYearId { get; set; }
          [Required]
        public string Location { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

       
        public string Type { get; set; }
        [NotMapped]
        public string Year { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public virtual EquipmentType EquipmentType { get; set; }
    }


}
