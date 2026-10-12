namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for EmployeePerformance.
    /// </summary>
    public partial class EmployeePerformance : Telerik.Reporting.Report
    {
        public EmployeePerformance()
        {
            //
            // Required for telerik Reporting designer support
            //
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
    }
}