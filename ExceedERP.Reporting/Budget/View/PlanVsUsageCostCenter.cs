namespace ExceedERP.Reporting.Budget.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for PlanVsUsageCostCenter.
    /// </summary>
    public partial class PlanVsUsageCostCenter : Telerik.Reporting.Report
    {
        public PlanVsUsageCostCenter()
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