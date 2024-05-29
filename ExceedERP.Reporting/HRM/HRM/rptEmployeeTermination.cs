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
    public partial class rptEmployeeTermination : Telerik.Reporting.Report
    {
        public rptEmployeeTermination()
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
            this.Termination.Value = Resources.ResourceManager.GetString("Rpt_Termination", cultures);
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.Branch.Value = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            this.TerminationDate.Value = Resources.ResourceManager.GetString("Rpt_TerminationDate", cultures);
            this.Reason.Value = Resources.ResourceManager.GetString("Rpt_Reason", cultures);
        }
    }
}