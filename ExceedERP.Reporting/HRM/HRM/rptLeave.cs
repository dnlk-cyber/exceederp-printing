using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptLeave : Telerik.Reporting.Report
    {
        public rptLeave()
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
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.NoOfMaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfMaleEmployee", cultures);
            this.NoOfFemaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfFemaleEmployee", cultures);
            this.LeaveSummary.Value = Resources.ResourceManager.GetString("Rpt_LeaveSummary", cultures);
            this.TotalEmployee.Value = Resources.ResourceManager.GetString("Rpt_TotalEmployee", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "leaveType");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_LeaveType", cultures);
            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "period");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_LeavePeriod", cultures);
        }
    }
}