namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAmharicExperience.
    /// </summary>
    public partial class rptAmharicExperience : Telerik.Reporting.Report
    {
        public rptAmharicExperience()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            
            var culture = "am-ET";//System.Threading.Thread.CurrentThread.CurrentUICulture.ToString()
            this.DataSource = ExpirianceLetterObjectDataSource;
        }
    }
}