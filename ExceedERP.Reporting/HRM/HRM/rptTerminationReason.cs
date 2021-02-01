using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptTerminationReason : Telerik.Reporting.Report
    {
        public rptTerminationReason()
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
            this.NoOfMaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfMaleEmployee", cultures);
            this.NoOfFemaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfFemaleEmployee", cultures);
            this.TerminationReasonSummary.Value = Resources.ResourceManager.GetString("Rpt_TerminationReasonSummary", cultures);

            this.TotalEmployee.Value = Resources.ResourceManager.GetString("Rpt_TotalEmployee", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);

            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "from");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_From", cultures);
            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "to");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_To", cultures);
        }
    }
}