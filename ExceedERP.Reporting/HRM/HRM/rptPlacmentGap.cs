using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptPlacmentGap : Telerik.Reporting.Report
    {
        public rptPlacmentGap()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.ToBeStaff.Value = Resources.ResourceManager.GetString("Rpt_ToBeStaff", cultures);
            this.TotalEmployee.Value = Resources.ResourceManager.GetString("Rpt_TotalEmployee", cultures);
            this.NoOfMaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfMaleEmployee", cultures);
            this.NoOfFemaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfFemaleEmployee", cultures);
            this.VacantPositions.Value = Resources.ResourceManager.GetString("Rpt_VacantPositions", cultures);
            this.VacantPostion.Value = Resources.ResourceManager.GetString("Rpt_VacantPositions", cultures);

            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "org");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);

            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "title");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
        }
    }
}