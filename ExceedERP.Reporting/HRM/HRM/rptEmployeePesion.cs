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
    /// Summary description for rptEmployeePesion.
    /// </summary>
    public partial class rptEmployeePesion : Telerik.Reporting.Report
    {
        public rptEmployeePesion()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.EmployeePensionNumber.Value = Resources.ResourceManager.GetString("Rpt_EmployeePensionNumber", cultures);
            this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Age.Value = Resources.ResourceManager.GetString("Rpt_Age", cultures);
            this.PensionNumber.Value = Resources.ResourceManager.GetString("Rpt_PensionNumber", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
          
        }
    }
}