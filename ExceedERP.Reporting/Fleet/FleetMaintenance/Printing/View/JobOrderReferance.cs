namespace ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for JobOrderReferance.
    /// </summary>
    public partial class JobOrderReferance : Telerik.Reporting.Report
    {
        public JobOrderReferance()
        {
            //
            // Required for telerik Reporting designer support
            //

            InitializeComponent();
            this.detail.Report.DataSource = HeaderDS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}