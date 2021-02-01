namespace ExceedERP.Reporting.Finance.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptApDirectPayment.
    /// </summary>
    public partial class rptApDirectPayment : Telerik.Reporting.Report
    {
        public rptApDirectPayment()
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