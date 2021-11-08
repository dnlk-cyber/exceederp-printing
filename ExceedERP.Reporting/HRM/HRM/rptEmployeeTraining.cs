using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptEmployeeJobTitle.
    /// </summary>
    public partial class rptEmployeeTraining : Telerik.Reporting.Report
    {
        public rptEmployeeTraining()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            this.DataSource = Organization;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
           // this.Training.Value = Resources.ResourceManager.GetString("Rpt_Training", cultures);
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            this.Type.Value = Resources.ResourceManager.GetString("Rpt_Type", cultures);
            this.Provider.Value = Resources.ResourceManager.GetString("Rpt_Provider", cultures);
            this.Course.Value = Resources.ResourceManager.GetString("Rpt_Course", cultures);
            this.From.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            this.To.Value = Resources.ResourceManager.GetString("Rpt_To", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "type");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_Type", cultures);
            var parameter = this.ReportParameters.FirstOrDefault(x => x.Name == "status");
            if (parameter != null)
                parameter.Text = Resources.ResourceManager.GetString("Rpt_Status", cultures);

            var paramete = this.ReportParameters.FirstOrDefault(x => x.Name == "from");
            if (paramete != null)
                paramete.Text = Resources.ResourceManager.GetString("Rpt_From", cultures);
            var paramet = this.ReportParameters.FirstOrDefault(x => x.Name == "to");
            if (paramet != null)
                paramet.Text = Resources.ResourceManager.GetString("Rpt_To", cultures);
        }
    }
}