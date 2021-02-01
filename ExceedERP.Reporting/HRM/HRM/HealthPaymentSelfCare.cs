namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for HealthPaymentSelfCare.
    /// </summary>
    public partial class HealthPaymentSelfCare : Telerik.Reporting.Report
    {
        public HealthPaymentSelfCare()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = EmployeeHealthPaymentODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}