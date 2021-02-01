namespace ExceedERP.Reporting.Insurance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptArbonPaidForAssets.
    /// </summary>
    public partial class rptArbonPaidForAssets : Telerik.Reporting.Report
    {
        public rptArbonPaidForAssets()
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