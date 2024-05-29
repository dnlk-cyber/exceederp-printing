using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptEmployeeLeavePlan.
    /// </summary>
    public partial class rptEmployeeLeavePlan : Telerik.Reporting.Report
    {
        public rptEmployeeLeavePlan()
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
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.From.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.To.Value = Resources.ResourceManager.GetString("Rpt_To", cultures);
            this.Departmant.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.Year.Value = Resources.ResourceManager.GetString("Rpt_Year", cultures);
            this.Amount.Value = Resources.ResourceManager.GetString("Rpt_Amount", cultures);
            this.total.Value = Resources.ResourceManager.GetString("Employee_LeavePlan_Total", cultures);
            this.subTotal.Value = Resources.ResourceManager.GetString("LeavePlan_Total", cultures);

            this.Signiture.Value = Resources.ResourceManager.GetString("Rpt_Signuture", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "dept");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);

            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "period");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_LeavePeriod", cultures);
        }
    }
}