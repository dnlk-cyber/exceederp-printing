namespace ExceedERP.Reporting.Finance.IFRS
{

    /// <summary>
    /// Summary description for rptSOCI.
    /// </summary>
    public partial class rptSOCI : Telerik.Reporting.Report
    {
        public rptSOCI()
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