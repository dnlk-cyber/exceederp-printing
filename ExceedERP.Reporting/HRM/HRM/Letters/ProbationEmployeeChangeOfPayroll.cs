namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ProbationEmployeeChangeOfPayroll.
    /// </summary>
    public partial class ProbationEmployeeChangeOfPayroll : Telerik.Reporting.Report
    {
        public ProbationEmployeeChangeOfPayroll()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = ChangeOFPayrollODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}