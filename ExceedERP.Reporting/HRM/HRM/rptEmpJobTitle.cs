using ExceedERP.Core.Localization;
using System.Globalization;
using System.Linq;

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
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.NoOfFemaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfFemaleEmployee", cultures);
            this.NoOfMaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfMaleEmployee", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.TotalEmployee.Value = Resources.ResourceManager.GetString("Rpt_TotalEmployee", cultures);
            this.ByJobTitle.Value = Resources.ResourceManager.GetString("Rpt_ByJobTitle", cultures);
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            var parameter = this.ReportParameters.FirstOrDefault(x => x.Name == "title");
            if (parameter != null)
                parameter.Text = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            var param = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (param != null)
                param.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var para = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (para != null)
                para.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
        }
    }
}