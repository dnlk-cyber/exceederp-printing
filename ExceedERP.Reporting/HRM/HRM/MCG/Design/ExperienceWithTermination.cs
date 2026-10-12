using ExceedERP.Reporting.HRM.HRM.Letters.DataSource;

namespace ExceedERP.Reporting.HRM.HRM.MCG.Design
{
    /// <summary>
    /// Summary description for ExperienceWithTermination.
    /// </summary>
    public partial class ExperienceWithTermination : Telerik.Reporting.Report
    {
        public ExperienceWithTermination()
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