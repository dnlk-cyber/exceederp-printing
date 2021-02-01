namespace ExceedERP.Reporting.Finance.Budget.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BalanceSheetSingleQuarter.
    /// </summary>
    public partial class BalanceSheetSingleQuarter : Telerik.Reporting.Report
    {
        public BalanceSheetSingleQuarter()
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