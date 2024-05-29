using System.Globalization;
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
    public partial class rptEmployeeEductions : Telerik.Reporting.Report
    {
        public rptEmployeeEductions()
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
          //  this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.EmployeeId.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Level.Value = Resources.ResourceManager.GetString("Rpt_Level", cultures);
            this.Field.Value = Resources.ResourceManager.GetString("Rpt_Field", cultures);
            this.From.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            this.To.Value = Resources.ResourceManager.GetString("Rpt_To", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            this.Education.Value = Resources.ResourceManager.GetString("Rpt_Education", cultures);
        }
    }
}