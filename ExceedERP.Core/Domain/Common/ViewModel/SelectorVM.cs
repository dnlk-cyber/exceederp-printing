using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Common.ViewModel
{
    public class ForeginKeyAmharicSelectorVm
    {
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }
    }
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

    public class ForeginKeySelectorGuidVm
    {
        public string Name { get; set; }
        public System.Guid Id { get; set; }

    }

    public class ForeginKeyStringSelectorVm
    {
        public string Name { get; set; }
        public string Id { get; set; }

    }

    public class COASegmentSelectorVm : SelectorVm
    {
        public int GLSegmentSetupId { get; set; }

    }
    public class ForeginKeySelectorFleetSecondVm
    {

        public System.Guid FleetsID { get; set; }
        public string PlateNumber { get; set; }
    }
    public class ForeginKeySelectorFleetVm
    {
        public string Name { get; set; }
        public System.Guid Id { get; set; }
        public string OrgBranchName { get; set; }
    }
}
