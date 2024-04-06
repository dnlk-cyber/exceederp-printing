using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptVacancyInternal : Telerik.Reporting.Report
    {
        public rptVacancyInternal()
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