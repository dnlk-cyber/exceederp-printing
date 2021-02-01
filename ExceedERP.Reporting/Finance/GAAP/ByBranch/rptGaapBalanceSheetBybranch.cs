namespace ExceedERP.Reporting.Finance.GAAP.ByBranch
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptGaapBalanceSheetBybranch.
    /// </summary>
    public partial class rptGaapBalanceSheetBybranch : Telerik.Reporting.Report
    {
        public rptGaapBalanceSheetBybranch()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}