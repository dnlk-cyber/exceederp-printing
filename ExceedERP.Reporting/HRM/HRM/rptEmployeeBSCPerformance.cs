namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for rptEmployeeJobTitle.
    /// </summary>
    public partial class RptEmployeeBscPerformance : Telerik.Reporting.Report
    {
        public RptEmployeeBscPerformance()
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