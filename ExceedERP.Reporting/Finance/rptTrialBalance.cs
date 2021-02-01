namespace ExceedERP.Reporting.Finance
{

    /// <summary>
    /// Summary description for rptTrialBalance.
    /// </summary>
    public partial class rptTrialBalance : Telerik.Reporting.Report
    {
        public rptTrialBalance()
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