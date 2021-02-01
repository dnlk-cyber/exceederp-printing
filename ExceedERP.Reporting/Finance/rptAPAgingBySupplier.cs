namespace ExceedERP.Reporting.Finance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAPAgingBySupplier.
    /// </summary>
    public partial class rptAPAgingBySupplier : Telerik.Reporting.Report
    {
        public rptAPAgingBySupplier()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}