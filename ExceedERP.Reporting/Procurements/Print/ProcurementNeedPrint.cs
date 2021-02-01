namespace ExceedERP.Reporting.Procurements.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ProcurementNeedPrint.
    /// </summary>
    public partial class ProcurementNeedPrint : Telerik.Reporting.Report
    {
        public ProcurementNeedPrint()
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