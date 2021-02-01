namespace ExceedERP.Reporting.Budget.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for PlanVsActualCostCenterSummary.
    /// </summary>
    public partial class PlanVsActualCostCenterSummary : Telerik.Reporting.Report
    {
        public PlanVsActualCostCenterSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = this.PlanVsActualCostCenterSummaryDS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}