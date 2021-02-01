namespace ExceedERP.Reporting.Finance.Receivable
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptDeferedRevenue.
    /// </summary>
    public partial class rptDeferedRevenue : Telerik.Reporting.Report
    {
        public rptDeferedRevenue()
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