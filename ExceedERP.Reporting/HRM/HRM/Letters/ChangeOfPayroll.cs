namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ChangeOfPayroll.
    /// </summary>
    public partial class ChangeOfPayroll : Telerik.Reporting.Report
    {
        public ChangeOfPayroll()
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