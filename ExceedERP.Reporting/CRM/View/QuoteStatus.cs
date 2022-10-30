namespace ExceedERP.Reporting.CRM.View
{
    /// <summary>
    /// Summary description for QuoteStatus.
    /// </summary>
    public partial class QuoteStatus : Telerik.Reporting.Report
    {
        public QuoteStatus()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}