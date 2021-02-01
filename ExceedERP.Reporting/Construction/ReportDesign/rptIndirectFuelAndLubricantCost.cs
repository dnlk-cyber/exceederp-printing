namespace ExceedERP.Reporting.Construction.ReportDesign
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptIndirectFuelAndLubricantCost.
    /// </summary>
    public partial class rptIndirectFuelAndLubricantCost : Telerik.Reporting.Report
    {
        public rptIndirectFuelAndLubricantCost()
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