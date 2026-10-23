namespace ExceedERP.Reporting.Budget.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BudgetSummary.
    /// </summary>
    public partial class BudgetSummary : Telerik.Reporting.Report
    {
        public BudgetSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}