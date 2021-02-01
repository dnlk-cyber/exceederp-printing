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
    public partial class rptEmployeeReferalSheet : Telerik.Reporting.Report
    {
        public rptEmployeeReferalSheet()
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
            this.TransfredPatinets.Value = Resources.ResourceManager.GetString("Rpt_TransfredPatinets", cultures);
            this.CardNo.Value = Resources.ResourceManager.GetString("Rpt_CardNo", cultures);
            this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            this.Sex.Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
            this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            this.TDate.Value = Resources.ResourceManager.GetString("Rpt_TransfredDate", cultures);
            this.DrName.Value = Resources.ResourceManager.GetString("Rpt_DrName", cultures);
            this.Diagnosis.Value = Resources.ResourceManager.GetString("Rpt_Diagnosis", cultures);
            this.Age.Value = Resources.ResourceManager.GetString("Rpt_Age", cultures);
            this.Hospital.Value = Resources.ResourceManager.GetString("Rpt_HospitalName", cultures);
        }
    }
}