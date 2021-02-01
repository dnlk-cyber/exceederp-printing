namespace ExceedERP.Reporting.Inventory
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for VariationReport.
    /// </summary>
    public partial class VariationReport : Telerik.Reporting.Report
    {
        public VariationReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = headerComp; 

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}