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
    /// Summary description for rptDisciplinaryMeasure.
    /// </summary>
    public partial class rptEmployeeDisciplinaryMeasure : Telerik.Reporting.Report
    {
        public rptEmployeeDisciplinaryMeasure()
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
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.EmployeeId.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.MeasureRank.Value = Resources.ResourceManager.GetString("Rpt_MeasureRank", cultures);
            this.MeasureTakenDate.Value = Resources.ResourceManager.GetString("Rpt_MeasureTakenDate", cultures);
            this.MeasureType.Value = Resources.ResourceManager.GetString("Rpt_MeasureType", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
          
        }
    }
}