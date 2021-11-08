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
    public partial class rptEmployeePromotionStep : Telerik.Reporting.Report
    {
        public rptEmployeePromotionStep()
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
           // this.EmployeePromotionPerYear.Value = Resources.ResourceManager.GetString("Rpt_EmployeePromotionPerYear", cultures);
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            this.FromStep.Value = Resources.ResourceManager.GetString("Rpt_FromStep", cultures);
            this.ToStep.Value = Resources.ResourceManager.GetString("Rpt_ToStep", cultures);
            this.Salary.Value = Resources.ResourceManager.GetString("Rpt_Salary", cultures);
        }
    }
}