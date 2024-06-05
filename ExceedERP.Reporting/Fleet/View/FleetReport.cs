namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for FleetReport.
    /// </summary>
    public partial class FleetReport : Telerik.Reporting.Report
    {
        public FleetReport()
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