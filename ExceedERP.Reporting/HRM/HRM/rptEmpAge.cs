using System.Globalization;
using System.Linq;
using System.Threading;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptEmpAge : Telerik.Reporting.Report
    {
        public rptEmpAge()
        {
            // var cultureInfo = new System.Globalization.CultureInfo("am");


            //
            // 
            // Required for telerik Reporting designer support
            //
            // Preserve the current culture
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();
    
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;           
        }
       
    }
}