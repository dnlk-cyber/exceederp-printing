namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptPorchaseOrderPrint.
    /// </summary>
    public partial class rptPorchaseOrderPrint : Telerik.Reporting.Report
    {
        public rptPorchaseOrderPrint()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Header;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}