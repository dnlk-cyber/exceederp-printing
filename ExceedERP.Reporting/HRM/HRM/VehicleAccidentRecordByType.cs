using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class VehicleAccidentRecordByType : Telerik.Reporting.Report
    {
        public VehicleAccidentRecordByType()
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
            this.VehicleType.Value = Resources.ResourceManager.GetString("Rpt_VehicleType", cultures);
            this.AccidentLevel.Value = Resources.ResourceManager.GetString("Rpt_AccidentLevel", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.HighCrash.Value = Resources.ResourceManager.GetString("Rpt_HighCrash", cultures);
            this.High.Value = Resources.ResourceManager.GetString("Rpt_High", cultures);
            this.HighThirdParty.Value = Resources.ResourceManager.GetString("Rpt_HighThirdParty", cultures);
            this.Low.Value = Resources.ResourceManager.GetString("Rpt_Low", cultures);
            this.LowCrash.Value = Resources.ResourceManager.GetString("Rpt_HighCrash", cultures);
            this.LowRevrese.Value = Resources.ResourceManager.GetString("Rpt_HighRevrese", cultures);
            this.LowThirdParty.Value = Resources.ResourceManager.GetString("Rpt_HighThirdParty", cultures);
            this.Medium.Value = Resources.ResourceManager.GetString("Rpt_Medium", cultures);
            this.MediumCrash.Value = Resources.ResourceManager.GetString("Rpt_HighCrash", cultures);
            this.MediumRevrese.Value = Resources.ResourceManager.GetString("Rpt_HighRevrese", cultures);
            this.MediumThirdParty.Value = Resources.ResourceManager.GetString("Rpt_HighThirdParty", cultures);
            this.InsuranceRpt.Value = Resources.ResourceManager.GetString("Rpt_InsuranceReportByAcc", cultures);
            this.TotalResult.Value = Resources.ResourceManager.GetString("Rpt_TotalResult", cultures);
            this.HighRevrese.Value = Resources.ResourceManager.GetString("Rpt_HighRevrese", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "period");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_FiscalYear", cultures);
        }
    }
}