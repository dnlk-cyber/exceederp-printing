using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptVacantPostion : Telerik.Reporting.Report
    {
        public rptVacantPostion()
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
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.CurrentStaf.Value = Resources.ResourceManager.GetString("Rpt_CurrentStaff", cultures);

            this.TobeStaff.Value = Resources.ResourceManager.GetString("Rpt_ToBeStaff", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.VacantPostion.Value = Resources.ResourceManager.GetString("Rpt_VacantPositions", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "org");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
          
        }
       
    }
}