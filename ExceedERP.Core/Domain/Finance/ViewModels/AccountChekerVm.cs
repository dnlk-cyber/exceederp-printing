using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Finance.ViewModels
{
    public class AccountChekerVm
    {
        public ErrorType Type { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Id { get; set; }
    }
}
