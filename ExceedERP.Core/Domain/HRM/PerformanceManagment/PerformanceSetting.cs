using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceSetting: TrackUserSettingOperation
    {
        public int PerformanceSettingId { get; set; }
        [Display(Name ="Dead Line(Value)")]
        public decimal Value { get; set; }
        [Display(Name = "Peer & Internal Customer Eval. Hiden ")]
        public bool  IsPeerAndInternalCustomerHide { get; set; }
    }
}
