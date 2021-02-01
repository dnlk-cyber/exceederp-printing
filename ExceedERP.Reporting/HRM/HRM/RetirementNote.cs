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
    /// Summary description for RetirementNote.
    /// </summary>
    public partial class RetirementNote : Telerik.Reporting.Report
    {
        public RetirementNote()
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
                 this.IdNum.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
                 this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
                 this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
                 this.RemainingYear.Value = Resources.ResourceManager.GetString("Rpt_RemainingYear", cultures);
                 this.RetirementDate.Value = Resources.ResourceManager.GetString("Rpt_RetirementDate", cultures);
                 this.RetirementNotification.Value = Resources.ResourceManager.GetString("Rpt_RetirementNotification", cultures);
                 var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "value");
                 if (firstOrDefault != null)
                     firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_NoOfYear", cultures);
                 var firstOrDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (firstOrDefaul != null)
                firstOrDefaul.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var firstOrDefau = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefau != null)
                firstOrDefau.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);

        }
    }
}