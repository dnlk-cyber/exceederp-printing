namespace ExceedERP.Reporting.Finance.Budget.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BudgetCashFlow.
    /// </summary>
    public partial class BudgetCashFlow : Telerik.Reporting.Report
    {
        public BudgetCashFlow()
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