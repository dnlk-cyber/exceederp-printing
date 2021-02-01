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
    /// Summary description for rptEmployeeLeave.
    /// </summary>
    public partial class rptEmployeeInLeave : Telerik.Reporting.Report
    {
        public rptEmployeeInLeave()
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
            this.EmployeeInLeave.Value = Resources.ResourceManager.GetString("Rpt_EmployeeInLeave", cultures);
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.From.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            this.To.Value = Resources.ResourceManager.GetString("Rpt_To", cultures);
            this.AmountTaken.Value = Resources.ResourceManager.GetString("Rpt_LeaveTakenAmount", cultures);
            this.LeaveType.Value = Resources.ResourceManager.GetString("Rpt_LeaveType", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
        }
    }
}