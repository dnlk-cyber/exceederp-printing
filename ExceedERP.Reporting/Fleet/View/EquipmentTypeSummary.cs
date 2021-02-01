namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for EquipmentTypeSummary.
    /// </summary>
    public partial class EquipmentTypeSummary : Telerik.Reporting.Report
    {
        public EquipmentTypeSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();


          this.pageHeaderSection1.Report.DataSource = headerComp;

        }
    }
}