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
    /// Summary description for rptWorkandCostSummary.
    /// </summary>
    public partial class rptWorkandCostSummary : Telerik.Reporting.Report
    {
        public rptWorkandCostSummary()
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
            this.no.Value = Resources.ResourceManager.GetString("Training_no", cultures);
            this.trainingType.Value = Resources.ResourceManager.GetString("Training_Training_Type", cultures);
            this.workunit.Value = Resources.ResourceManager.GetString("Training_Workunit", cultures);
            this.fisicalyear.Value = Resources.ResourceManager.GetString("Training_Period", cultures);
            this.nooftrainee.Value = Resources.ResourceManager.GetString("Training_Trainee_Quantity", cultures);
            this.cost.Value = Resources.ResourceManager.GetString("Training_Cost_Amount", cultures);
            this.provider.Value = Resources.ResourceManager.GetString("Training_Provider", cultures);

            //var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "org");
            //if (firstOrDefault != null)
            //    firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            //var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            //if (reportParameter != null)
            //    reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);

            //var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "title");
            //if (orDefaul != null)
            //    orDefaul.Text = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
        }
    }
}