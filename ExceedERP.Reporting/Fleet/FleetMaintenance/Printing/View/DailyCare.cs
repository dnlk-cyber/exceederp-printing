namespace ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for DailyCare.
    /// </summary>
    public partial class DailyCare : Telerik.Reporting.Report
    {
        public DailyCare()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.pageHeaderSection1.Report.DataSource = DailyCareDS;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}