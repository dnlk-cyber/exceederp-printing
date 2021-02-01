namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptEmployeePerformanceSummaryDepartment.
    /// </summary>
    public partial class rptEmployeePerformanceSummaryDepartment : Telerik.Reporting.Report
    {
        public rptEmployeePerformanceSummaryDepartment()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
           
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = EvaluationTypeObjectDataSource;
            if (this.DataSource != null)
            {

                // reportFooterSection1.Visible = true;
            }
        }
      
    }
}