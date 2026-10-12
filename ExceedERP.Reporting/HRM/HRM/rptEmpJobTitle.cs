using ExceedERP.Core.Localization;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptEmpJobTitle : Telerik.Reporting.Report
    {
        public rptEmpJobTitle()
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