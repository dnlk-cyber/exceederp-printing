namespace ExceedERP.Reporting.Budget.Print.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BudgetRequestTransfer.
    /// </summary>
    public partial class BudgetTransfer : Telerik.Reporting.Report
    {
        public BudgetTransfer()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.DataSource = BudgetRequestTransferDS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}