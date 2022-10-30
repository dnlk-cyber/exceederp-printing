namespace ExceedERP.Reporting.CRM.Printing.View
{
    /// <summary>
    /// Summary description for StoreIssue.
    /// </summary>
    public partial class StoreIssue : Telerik.Reporting.Report
    {
        public StoreIssue()
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