namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Summary.
    /// </summary>
    public partial class Summary : Telerik.Reporting.Report
    {
        public Summary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();


            this.pageHeaderSection1.Report.DataSource = headerComp;
            
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}