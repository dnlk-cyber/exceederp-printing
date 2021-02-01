namespace ExceedERP.Reporting.Finance.GAAP
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptIncomeStatementByProject.
    /// </summary>
    public partial class rptIncomeStatementByProject : Telerik.Reporting.Report
    {
        public rptIncomeStatementByProject()
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