using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
  public  class CRMTransactionValidationStatusViewModel
    {
        public bool Succeed { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
    public class CRMTransactionCounterViewModel
    {
        public int SuccessfulOperations { get; set; }
        public int IgnoredOperations { get; set; }
        public int FailedOperations { get; set; }
    }
}
