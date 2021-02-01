namespace ExceedERP.Reporting.Finance.GAAP
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptGaapBalanceSheet.
    /// </summary>
    public partial class rptGaapBalanceSheet : Telerik.Reporting.Report
    {
        public rptGaapBalanceSheet()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
    }
}