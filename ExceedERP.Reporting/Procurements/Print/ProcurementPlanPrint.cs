namespace ExceedERP.Reporting.Procurements.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ProcurementPlanPrint.
    /// </summary>
    public partial class ProcurementPlanPrint : Telerik.Reporting.Report
    {
        public ProcurementPlanPrint()
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