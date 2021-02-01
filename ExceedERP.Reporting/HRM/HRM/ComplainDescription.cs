namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ComplainDescription.
    /// </summary>
    public partial class ComplainDescription : Telerik.Reporting.Report
    {
        public ComplainDescription()
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