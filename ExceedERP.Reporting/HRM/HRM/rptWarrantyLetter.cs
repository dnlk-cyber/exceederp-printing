namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptWarrantyLetter.
    /// </summary>
    public partial class rptWarrantyLetter : Telerik.Reporting.Report
    {
        public rptWarrantyLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = WarrantyODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}