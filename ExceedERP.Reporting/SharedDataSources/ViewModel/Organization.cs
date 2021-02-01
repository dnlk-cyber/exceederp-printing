using System.Drawing;

namespace ExceedERP.Reporting.SharedDataSources.ViewModel
{
    public class Organization : AddressVm
    {
        public int OrganizationID { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Abbreviation { get; set; }
        public string Date { get; set; }
        public string Note { get; set;}

    }
}