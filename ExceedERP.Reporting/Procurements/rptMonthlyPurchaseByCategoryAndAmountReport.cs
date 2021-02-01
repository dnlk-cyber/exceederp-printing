namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptGeneralBuysInMonth.
    /// </summary>
    public partial class rptMonthlyPurchaseByCategoryAndAmountReport : Telerik.Reporting.Report
    {
        public rptMonthlyPurchaseByCategoryAndAmountReport()
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