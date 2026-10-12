using System;

namespace ExceedERP.Reporting.HRM.HRM
{
    // <summary>
    /// Summary description for EmployeePayrollSummaryReport.
    /// </summary>
    public partial class EmployeePayrollSummaryReport : Telerik.Reporting.Report
    {
        public EmployeePayrollSummaryReport()
        {
            //
            // Required for Telerik Reporting designer support
            //
            InitializeComponent();

            //
            // Hook report events for custom processing if needed
            //
            this.NeedDataSource += EmployeePayrollSummaryReport_NeedDataSource;
        }

        private void EmployeePayrollSummaryReport_NeedDataSource(object sender, EventArgs e)
        {
            // Telerik Reporting assigns the processing element as the sender
            Telerik.Reporting.Processing.Report report = (Telerik.Reporting.Processing.Report)sender;

            // Optional: You can inspect or modify parameters programmatically at runtime
            // Example:
            // var selectedBranch = report.Parameters["branchId"].Value;
            // var status = report.Parameters["status"].Value;
        }
    }
}
