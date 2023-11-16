namespace ExceedERP.Reporting.Finance.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptPaymentRequestNew.
    /// </summary>
    public partial class rptPaymentRequestNew : Telerik.Reporting.Report
    {
        public rptPaymentRequestNew()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = header;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}