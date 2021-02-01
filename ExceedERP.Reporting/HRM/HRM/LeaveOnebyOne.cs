using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class LeaveOnebyOne : Telerik.Reporting.Report
    {
        public LeaveOnebyOne()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = Organization ;
            var culture = "am-ET";//System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.IdNUm.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.LeavePeriod.Value = Resources.ResourceManager.GetString("Rpt_LeavePeriod", cultures);
            this.LeaveTakenAmount.Value = Resources.ResourceManager.GetString("Rpt_LeaveTakenAmount", cultures);
            this.LeaveTakenDetail.Value = Resources.ResourceManager.GetString("Rpt_LeaveTakenDetail", cultures);
            this.TransferredAmount.Value = Resources.ResourceManager.GetString("Rpt_Leave_Balance", cultures);
            this.EntitleAmount.Value = Resources.ResourceManager.GetString("Rpt_Leave_Entitle", cultures);
            //this.Entitlement.Value = Resources.ResourceManager.GetString("Rpt_Entitlement", cultures);
            this.From.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            this.To.Value = Resources.ResourceManager.GetString("Rpt_To", cultures);
            this.PaidInCash.Value = Resources.ResourceManager.GetString("Rpt_PaidInCash", cultures); 
            this.TransferredYears.Value = Resources.ResourceManager.GetString("Rpt_TransferredYears", cultures);
            //this.TransferredAmount.Value = Resources.ResourceManager.GetString("Rpt_TransferredAmount", cultures);
            this.TransfredForm.Value = Resources.ResourceManager.GetString("Rpt_Entitlement", cultures); //Resources.ResourceManager.GetString("Rpt_Transferred_FromOther",cultures);
            this.TotalAmountTaken.Value = Resources.ResourceManager.GetString("Rtp_TotalAmountTaken", cultures);
            this.LeaveUsage.Value = Resources.ResourceManager.GetString("Rpt_LeaveUsage", cultures);
            //this..Value = Resources.ResourceManager.GetString("Rpt_LeaveUsage", cultures);
            //this.LeaveUsage.Value = Resources.ResourceManager.GetString("Rpt_LeaveUsage", cultures);
            //this.LeaveUsage.Value = Resources.ResourceManager.GetString("Rpt_LeaveUsage", cultures);
            this.FiscalYear.Value = Resources.ResourceManager.GetString("Leave_Usage_FiscalYear", cultures);
            this.LeaveType.Value = Resources.ResourceManager.GetString("Leave_Usage_LeaveType", cultures);
            this.Status.Value = Resources.ResourceManager.GetString("Leave_Usage_Status", cultures);
            var parameter = this.ReportParameters.FirstOrDefault(x => x.Name == "periodN");
            if (parameter != null)
                parameter.Text = Resources.ResourceManager.GetString("HR_LeaveManagment_LeavePaid_LeavePeriod", cultures);
            var paramet = this.ReportParameters.FirstOrDefault(x => x.Name == "empCode");
            if (paramet != null)
                paramet.Text = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
        }
    }
}