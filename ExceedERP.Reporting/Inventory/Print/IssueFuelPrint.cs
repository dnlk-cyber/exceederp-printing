namespace ExceedERP.Reporting.Inventory.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for IssueFuelPrint.
    /// </summary>
    public partial class IssueFuelPrint : Telerik.Reporting.Report
    {
        public IssueFuelPrint()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = HeaderDs;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}