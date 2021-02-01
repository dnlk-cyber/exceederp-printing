namespace ExceedERP.Reporting.APTS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptDrugSaleWithDrugist.
    /// </summary>
    public partial class rptDrugSaleWithDrugist : Telerik.Reporting.Report
    {
        public rptDrugSaleWithDrugist()
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