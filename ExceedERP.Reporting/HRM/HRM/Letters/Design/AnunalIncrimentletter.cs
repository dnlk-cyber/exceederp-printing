using ExceedERP.Reporting.HRM.HRM.Letters.DataSource;

namespace ExceedERP.Reporting.HRM.HRM.Letters.Design
{
    /// <summary>
    /// Summary description for AnunalIncrimentletter.
    /// </summary>
    public partial class AnunalIncrimentletter : Telerik.Reporting.Report
    {
        public AnunalIncrimentletter()
        {
            //
            // Required for telerik Reporting designer support
            //
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            //this.DataSource = ContractHirementObjectDatasource;
           
        }
    }
}