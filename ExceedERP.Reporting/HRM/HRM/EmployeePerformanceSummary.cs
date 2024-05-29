namespace ExceedERP.Reporting.HRM.HRM
{
    using ExceedERP.Core.Localization;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class EmployeePerformanceSummary : Telerik.Reporting.Report
    {
        public EmployeePerformanceSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

           // reportFooterSection1.Visible = false;
            //
            // TODO: Add any constructor code after InitializeComponent call
            

           // var culture = /*"am-ET";*/System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
           // var cultures = new CultureInfo(culture);
            
        }
    }
}