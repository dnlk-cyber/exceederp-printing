using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptLeaveSummary : Telerik.Reporting.Report
    {
        public rptLeaveSummary()
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
           // this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.LeavePeriod.Value = Resources.ResourceManager.GetString("Rpt_LeavePeriod", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Entitlemnt.Value = Resources.ResourceManager.GetString("Rpt_Entitlement", cultures);
            //this.TransferredAmount.Value = Resources.ResourceManager.GetString("Rpt_TransferredAmount", cultures);
            this.TotalTaken.Value = Resources.ResourceManager.GetString("Rpt_LeaveTakenAmount", cultures);
            this.PaidInCash.Value = Resources.ResourceManager.GetString("Rpt_PaidInCash", cultures); 
            this.Balance.Value = Resources.ResourceManager.GetString("Rpt_Balance", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Report_Department", cultures);

            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);

            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "Period");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_LeavePeriod", cultures);
        }
    }
}
