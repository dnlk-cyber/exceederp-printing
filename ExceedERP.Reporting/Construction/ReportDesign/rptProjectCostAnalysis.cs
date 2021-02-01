namespace ExceedERP.Reporting.Construction.ReportDesign
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptProjectCostAnalysis.
    /// </summary>
    public partial class rptProjectCostAnalysis : Telerik.Reporting.Report
    {
        public rptProjectCostAnalysis()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = headerOds;
        }
    }
}