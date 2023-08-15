namespace ExceedERP.Reporting.Procurements.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for PurchasePaymentPrint.
    /// </summary>
    public partial class PurchasePaymentPrint : Telerik.Reporting.Report
    {
        public PurchasePaymentPrint()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = PurchsePaymentHeaderobjectDataSource;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}