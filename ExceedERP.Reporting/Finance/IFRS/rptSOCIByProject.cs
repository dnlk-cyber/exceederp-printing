namespace ExceedERP.Reporting.Finance.IFRS
{
    /// <summary>
    /// Summary description for rptSOCIByProject.
    /// </summary>
    public partial class rptSOCIByProject : Telerik.Reporting.Report
    {
        public rptSOCIByProject()
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