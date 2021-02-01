namespace ExceedERP.Reporting.Inventory.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ItemTransferPrint.
    /// </summary>
    public partial class ItemTransferPrint : Telerik.Reporting.Report
    {
        public ItemTransferPrint()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = objectDataSource1;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}