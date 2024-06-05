namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for EquipmentPlanEvaluationsReport.
    /// </summary>
    public partial class PMPlanVsActual : Telerik.Reporting.Report
    {
        public PMPlanVsActual()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.Report.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}