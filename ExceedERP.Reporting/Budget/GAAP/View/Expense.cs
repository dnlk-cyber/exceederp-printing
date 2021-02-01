namespace ExceedERP.Reporting.Budget.GAAP.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Expense.
    /// </summary>
    public partial class Expense : Telerik.Reporting.Report
    {
        public Expense()
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