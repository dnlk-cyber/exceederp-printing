namespace ExceedERP.Reporting.Finance.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptPaymentRequest.
    /// </summary>
    public partial class rptPaymentRequest : Telerik.Reporting.Report
    {
        public rptPaymentRequest()
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