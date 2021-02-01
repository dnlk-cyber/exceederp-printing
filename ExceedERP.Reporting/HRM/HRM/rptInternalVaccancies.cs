using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptInternalVaccancies : Telerik.Reporting.Report
    {
        public rptInternalVaccancies()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            var culture = "am-ET";//System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_No", cultures);
            this.No.Value = Resources.ResourceManager.GetString("Rpt_No", cultures);
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            //this.Expriance.Value = Resources.ResourceManager.GetString("Rpt_RelatedExperience", cultures);
            this.Location.Value = Resources.ResourceManager.GetString("Rpt_Location", cultures);
            this.Quantity.Value = Resources.ResourceManager.GetString("Rpt_Quantity", cultures);
            this.Salary.Value = Resources.ResourceManager.GetString("Rpt_Salary", cultures);
            this.EmployementType.Value = Resources.ResourceManager.GetString("Rpt_EmployementType", cultures);
            this.Vacancy.Value = Resources.ResourceManager.GetString("External_Vacancy", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "vacancyCod");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_VacancyCode", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "jobTitle");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "vacancyCod");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_VacancyCode", cultures);
        }
    }
}