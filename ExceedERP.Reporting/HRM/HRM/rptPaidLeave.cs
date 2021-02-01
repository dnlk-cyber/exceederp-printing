using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptPaidLeave : Telerik.Reporting.Report
    {
        public rptPaidLeave()
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
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.LeavePeriod.Value = Resources.ResourceManager.GetString("Rpt_LeavePeriod", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.TotalPayable.Value = Resources.ResourceManager.GetString("Rpt_TotalPayable", cultures);
            this.PayableLeave.Value = Resources.ResourceManager.GetString("Rpt_PayableLeave", cultures);
            this.LeaveBalance.Value = Resources.ResourceManager.GetString("Rpt_Balance", cultures);

            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "dept");
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