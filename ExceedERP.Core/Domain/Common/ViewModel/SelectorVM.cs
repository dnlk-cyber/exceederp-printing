using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Common.ViewModel
{
    public class SelectorVm
    {
        public string Name { get; set; }
        public string Code { get; set; }

    }

    public class ForeginKeySelectorVm
    {
        public string Name { get; set; }
        public int Id { get; set; }

    }
   
    public class COASegmentSelectorVm : SelectorVm
    {
        public int GLSegmentSetupId { get; set; }

    }

}
