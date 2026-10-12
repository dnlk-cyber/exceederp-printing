using System.Globalization;
using System.Threading;

namespace ExceedERP.Reporting.HRM.HRMCourt
{
    /// <summary>
    /// Summary description for rptEmployeeMaster.
    /// </summary>
    public partial class rptEmployeeMaster : Telerik.Reporting.Report
    {
        public rptEmployeeMaster()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
    }
}