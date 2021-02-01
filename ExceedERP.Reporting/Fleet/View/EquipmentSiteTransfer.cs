namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for EquipmentSiteTransfer.
    /// </summary>
    public partial class EquipmentSiteTransfer : Telerik.Reporting.Report
    {
        public EquipmentSiteTransfer()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.pageHeaderSection1.Report.DataSource = headerComp;

        }
    }
}