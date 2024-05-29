using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class VehicleAccidentRecord : Telerik.Reporting.Report
    {
        public VehicleAccidentRecord()
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
            this.Type.Value = Resources.ResourceManager.GetString("Rpt_VehicleType", cultures);
            this.PlateNo.Value = Resources.ResourceManager.GetString("Rpt_PlateNo", cultures);
            this.Amount.Value = Resources.ResourceManager.GetString("Rpt_AmountByMoney", cultures);
            this.No.Value = Resources.ResourceManager.GetString("Rpt_No", cultures);
            this.Remark.Value = Resources.ResourceManager.GetString("Rpt_Remark", cultures);
            this.InsuranceRpt .Value = Resources.ResourceManager.GetString("Rpt_InsuranceFollowup", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "period");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_FiscalYear", cultures);
           
        }
    }
}