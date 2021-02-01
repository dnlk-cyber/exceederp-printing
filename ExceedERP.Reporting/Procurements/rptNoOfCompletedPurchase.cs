namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptNoOfCompletedPurchase.
    /// </summary>
    public partial class rptNoOfCompletedPurchase : Telerik.Reporting.Report
    {
        public rptNoOfCompletedPurchase()
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