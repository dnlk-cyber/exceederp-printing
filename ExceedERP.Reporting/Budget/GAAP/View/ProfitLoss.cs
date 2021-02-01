namespace ExceedERP.Reporting.Budget.GAAP.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ProfitLoss.
    /// </summary>
    public partial class ProfitLoss : Telerik.Reporting.Report
    {
        public ProfitLoss()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();


            this.pageHeaderSection1.Report.DataSource = this.headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}