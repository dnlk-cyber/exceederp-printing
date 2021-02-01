namespace ExceedERP.Reporting.Finance.Print
{

    /// <summary>
    /// Summary description for rptArInvoice.
    /// </summary>
    public partial class rptArInvoice : Telerik.Reporting.Report
    {
        public rptArInvoice()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = header;
        }
    }
}