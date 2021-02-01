namespace ExceedERP.Reporting.Finance.IFRS
{

    /// <summary>
    /// Summary description for rptSOCF.
    /// </summary>
    public partial class rptSOCF : Telerik.Reporting.Report
    {
        public rptSOCF()
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