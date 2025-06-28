using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
namespace ExceedERP.Core.Domain.CRM.TenderManagement
{
  public  class TenderWinnerValidation:TrackUserSettingOperation
    {
        [Key]
        public int TenderWinnerValidationId { get; set; }
        public int SalesLotId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
