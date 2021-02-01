using System.Collections.Generic;

namespace ExceedERP.Reporting.Finance.ViewModel.GAAP
{
    public enum NoteType
    {
        Continuing,
        Discontinued,
        Other,
        Operating,
        Investing,
        Financing,
        Asset,
        Liability,
        Capital,
        Revenue,
        Expense
    }
    public class GaapNoteVeiwModel
    {
        public string Description { get; set; }
        public string NoteNumber { get; set; }
        public NoteType Type { get; set; }
        public List<GaapNoteDetailViewModel> NoteDetailsList { get; set; }
    }
   
    public class GaapNoteDetailViewModel
    {
        public string AccountDesc { get; set; }
        public decimal CurrentYearBalance { get; set; }
        public decimal PreviousYearBalance { get; set; }
    }

   
}
