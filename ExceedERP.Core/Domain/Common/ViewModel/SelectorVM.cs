namespace ExceedERP.Core.Domain.Common.ViewModel
{
    public class SelectorVm
    {
        public string Name { get; set; }
        public string Code { get; set; }

    }
    public class COASegmentSelectorVm: SelectorVm
    {
        public int GLSegmentSetupId { get; set; }

    }
}
