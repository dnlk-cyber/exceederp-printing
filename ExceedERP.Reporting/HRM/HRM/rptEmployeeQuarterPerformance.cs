using System.Globalization;
using System.Text;
using System.Threading;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for rptEmployeeJobTitle.
    /// </summary>
    public partial class rptEmployeeQuarterPerformance : Telerik.Reporting.Report
    {
        public rptEmployeeQuarterPerformance()
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
        public static string ConvertArrayToString(object[] values)
        {
            if (values != null)
            {
                var result = new StringBuilder(values[0].ToString(), values.Length);

                for (int i = 1; i < values.Length; i++)
                {
                    result.AppendFormat(", {0}", values[i]);
                }
                return result.ToString();
            }
            return "";
        } 
    }
}