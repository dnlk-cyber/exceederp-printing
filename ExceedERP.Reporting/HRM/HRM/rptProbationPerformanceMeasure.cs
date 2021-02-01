namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptProbationPerformanceMeasure.
    /// </summary>
    public partial class rptProbationPerformanceMeasure : Telerik.Reporting.Report
    {
        public rptProbationPerformanceMeasure()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}