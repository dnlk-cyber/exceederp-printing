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
    /// Summary description for RetirementNote.
    /// </summary>
    public partial class rptEmployeeByBranch : Telerik.Reporting.Report
    {
        public rptEmployeeByBranch()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            var culture = "am-ET"; //System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.EduactionBa.Value = Resources.ResourceManager.GetString("Rpt_EduactionBack", cultures);
            this.Training.Value = Resources.ResourceManager.GetString("Rpt_Training", cultures);
            this.TrainingIns.Value = Resources.ResourceManager.GetString("Rpt_TrainingIns", cultures);
            this.No.Value = Resources.ResourceManager.GetString("Rpt_No", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.EmployType.Value = Resources.ResourceManager.GetString("Rpt_EmployementType", cultures);
            this.JobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            this.EmployeeList.Value = Resources.ResourceManager.GetString("Rpt_EmployeeList", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
        }
    }
}