namespace ExceedERP.Reporting.Inventory.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for StoreReturnPrint.
    /// </summary>
    public partial class StoreReturnPrint : Telerik.Reporting.Report
    {
        public StoreReturnPrint()
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