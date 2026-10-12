using System.Globalization;
using System.Linq;
using System.Threading;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class LeaveOnebyOne : Telerik.Reporting.Report
    {
        public LeaveOnebyOne()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = Organization ;          
        }
    }
}