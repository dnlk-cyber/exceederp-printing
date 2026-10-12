using ExceedERP.Reporting.HRM.HRM.Letters.DataSource;

namespace ExceedERP.Reporting.HRM.HRM.MCG.Design
{
    /// <summary>
    /// Summary description for ExperienceMCG.
    /// </summary>
    public partial class ExperienceMCG : Telerik.Reporting.Report
    {
        public ExperienceMCG()
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