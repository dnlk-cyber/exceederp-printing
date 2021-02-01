namespace ExceedERP.Reporting.Fleet.View 
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for EquipmentSummary.
    /// </summary>
    public partial class EquipmentSummary : Telerik.Reporting.Report
    {
        public EquipmentSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
           //   this.reportHeader.Report.DataSource = headerComp;
        }
    }
}