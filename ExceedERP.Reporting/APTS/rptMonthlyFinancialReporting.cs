namespace ExceedERP.Reporting.APTS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptMonthlyFinancialReporting.
    /// </summary>
    public partial class rptMonthlyFinancialReporting : Telerik.Reporting.Report
    {
        public rptMonthlyFinancialReporting()
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