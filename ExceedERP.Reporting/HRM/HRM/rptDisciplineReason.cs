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
    public partial class rptDisciplineReason : Telerik.Reporting.Report
    {
        public rptDisciplineReason()
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
            this.DisciplineMeasureSummary.Value = Resources.ResourceManager.GetString("Rpt_DisciplineMeasureSummary", cultures);
            this.NoOfFemaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfFemaleEmployee", cultures);
            this.NoOfMaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfMaleEmployee", cultures);
            this.Measure.Value = Resources.ResourceManager.GetString("Rpt_Measure", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.TotalEmployee.Value = Resources.ResourceManager.GetString("Rpt_TotalEmployee", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "from");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_From", cultures);
            var firstOrDef = this.ReportParameters.FirstOrDefault(x => x.Name == "to");
            if (firstOrDef != null)
                firstOrDef.Text = Resources.ResourceManager.GetString("Rpt_To", cultures);
            var firstOrDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (firstOrDefaul != null)
                firstOrDefaul.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var firstOrDefau = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefau != null)
                firstOrDefau.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
        }
    }
}