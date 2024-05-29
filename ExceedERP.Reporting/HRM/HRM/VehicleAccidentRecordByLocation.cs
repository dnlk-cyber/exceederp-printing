using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class VehicleAccidentRecordByLocation : Telerik.Reporting.Report
    {
        public VehicleAccidentRecordByLocation()
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
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.Year.Value = Resources.ResourceManager.GetString("Rpt_Year", cultures);
            this.YearT.Value = Resources.ResourceManager.GetString("Rpt_Year", cultures);
            this.AccidentByMoney.Value = Resources.ResourceManager.GetString("Rpt_AmountByMoney", cultures);
            this.InsuranceRpt.Value = Resources.ResourceManager.GetString("Rpt_InsuranceFollowupByProject", cultures);
            this.AccidentByMoneyT.Value = Resources.ResourceManager.GetString("Rpt_AmountByMoney", cultures);
            this.ProjectName.Value = Resources.ResourceManager.GetString("Rpt_ProjectName", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.Diffrence.Value = Resources.ResourceManager.GetString("Rpt_InsuDiffrence", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "period");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_FiscalYear", cultures);
        }
    }
}