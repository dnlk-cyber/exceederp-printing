namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for rptEmployeeJobTitle.
    /// </summary>
    public partial class EmployeePerfromanceLeaveDispilineBonusSummary : Telerik.Reporting.Report
    {
        public EmployeePerfromanceLeaveDispilineBonusSummary()
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