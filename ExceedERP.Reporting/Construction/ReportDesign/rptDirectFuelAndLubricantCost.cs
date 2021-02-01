namespace ExceedERP.Reporting.Construction.ReportDesign
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptDirectFuelAndLubricantCost.
    /// </summary>
    public partial class rptDirectFuelAndLubricantCost : Telerik.Reporting.Report
    {
        public rptDirectFuelAndLubricantCost()
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