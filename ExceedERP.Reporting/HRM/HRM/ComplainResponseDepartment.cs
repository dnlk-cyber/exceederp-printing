namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ComplainResponseDepartment.
    /// </summary>
    public partial class ComplainResponseDepartment : Telerik.Reporting.Report
    {
        public ComplainResponseDepartment()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = ComplainODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}