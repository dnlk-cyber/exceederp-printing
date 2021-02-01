namespace ExceedERP.Reporting.Insurance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAccident.
    /// </summary>
    public partial class rptAccident : Telerik.Reporting.Report
    {
        public rptAccident()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = AccidentODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}