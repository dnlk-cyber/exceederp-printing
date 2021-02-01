namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ServiceType.
    /// </summary>
    public partial class ServiceType : Telerik.Reporting.Report
    {
        public ServiceType()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();


            this.reportHeader.Report.DataSource = ServiceTypeDS;
            
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}