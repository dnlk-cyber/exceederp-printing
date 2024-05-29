namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Deligation.
    /// </summary>
    public partial class Deligation : Telerik.Reporting.Report
    {
        public Deligation()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = DeligationODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}