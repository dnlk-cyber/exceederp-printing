namespace ExceedERP.Reporting.Finance.IFRS
{
    /// <summary>
    /// Summary description for rptSOCIE.
    /// </summary>
    public partial class rptSOCIE : Telerik.Reporting.Report
    {
        public rptSOCIE()
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