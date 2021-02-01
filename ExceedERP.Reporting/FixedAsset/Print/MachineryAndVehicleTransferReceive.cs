namespace ExceedERP.Reporting.FixedAsset.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for MachineryAndVehicleTransferReceive.
    /// </summary>
    public partial class MachineryAndVehicleTransferReceive : Telerik.Reporting.Report
    {
        public MachineryAndVehicleTransferReceive()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Header;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}