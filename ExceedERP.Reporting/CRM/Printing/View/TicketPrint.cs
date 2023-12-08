namespace ExceedERP.Reporting.CRM.Printing.View
{
    /// <summary>
    /// Summary description for TicketPrint.
    /// </summary>
    public partial class TicketPrint : Telerik.Reporting.Report
    {
        public TicketPrint()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.pageHeaderSection1.Report.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}