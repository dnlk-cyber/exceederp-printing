namespace ExceedERP.Reporting.Finance
{

    /// <summary>
    /// Summary description for rptARAgingByInvoice.
    /// </summary>
    public partial class rptARAgingByInvoice : Telerik.Reporting.Report
    {
        public rptARAgingByInvoice()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
    }
}