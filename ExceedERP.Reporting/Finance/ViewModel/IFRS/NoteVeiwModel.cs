using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.IFRS
{
    public enum Type
    {
        Continuing,
        Discontinued,
        Other,
        Operating,
        Investing,
        Financing,
        Asset,
        Liability
    }
    public class NoteVeiwModel
    {
        public string Description { get; set; }
        public string NoteNumber { get; set; }
        public Type Type { get; set; }
        public List<NoteDetailViewModel> NoteDetailsList { get; set; }
    }
   
    public class NoteDetailViewModel
    {
        public string AccountDesc { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
    }

   
}
