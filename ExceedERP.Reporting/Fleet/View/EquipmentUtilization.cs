namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for EquipmentUtilization.
    /// </summary>
    public partial class EquipmentUtilization : Telerik.Reporting.Report
    {
        public EquipmentUtilization()
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