namespace ExceedERP.Reporting.CRM.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for SalesOrderIssue.
    /// </summary>
    public partial class SalesOrderIssue : Telerik.Reporting.Report
    {
        public SalesOrderIssue()
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