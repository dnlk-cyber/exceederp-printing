namespace ExceedERP.Reporting.Fleet.FleetMaintenance.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for HeadOfficeSummary.
    /// </summary>
    public partial class SparePartConsumption : Telerik.Reporting.Report
    {
        public SparePartConsumption()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.pageHeaderSection1.Report.DataSource = headerComp;
        }
    }
}