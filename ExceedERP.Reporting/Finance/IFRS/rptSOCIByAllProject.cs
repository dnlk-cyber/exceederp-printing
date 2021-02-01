namespace ExceedERP.Reporting.Finance.IFRS
{
    /// <summary>
    /// Summary description for rptSOCIByAllProject.
    /// </summary>
    public partial class rptSOCIByAllProject : Telerik.Reporting.Report
    {
        public rptSOCIByAllProject()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}