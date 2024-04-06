using System.Text;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for rptEmployeeJobTitle.
    /// </summary>
    public partial class rptEmployeePerformance : Telerik.Reporting.Report
    {
        public rptEmployeePerformance()
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