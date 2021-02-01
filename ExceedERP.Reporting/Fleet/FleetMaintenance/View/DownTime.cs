namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for DownTime.
    /// </summary>
    public partial class DownTime : Telerik.Reporting.Report
    {
        public DownTime()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.reportHeader.Report.DataSource = headerComp;
            
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}