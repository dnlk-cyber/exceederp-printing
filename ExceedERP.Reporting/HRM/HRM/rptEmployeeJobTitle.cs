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
    public partial class rptEmployeeJobTitle : Telerik.Reporting.Report
    {
        public rptEmployeeJobTitle()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            //var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            //var cultures = new CultureInfo(culture);
            //this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            //this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            //this.EmployeeJobTitle.Value = Resources.ResourceManager.GetString("Rpt_EmployeeJobTitle", cultures);
            //this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            //this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            //this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            //this.Rank.Value = Resources.ResourceManager.GetString("Rpt_Rank", cultures);
            //this.Salary.Value = Resources.ResourceManager.GetString("Rpt_Salary", cultures);
         
            //this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            //this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
        }
    }
}