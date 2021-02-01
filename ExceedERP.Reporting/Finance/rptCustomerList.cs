namespace ExceedERP.Reporting.Finance
{

    /// <summary>
    /// Summary description for rptCustomerList.
    /// </summary>
    public partial class rptCustomerList : Telerik.Reporting.Report
    {
        public rptCustomerList()
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