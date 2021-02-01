namespace ExceedERP.Reporting.Inventory.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ItemtransferReceivePrint.
    /// </summary>
    public partial class ItemtransferReceivePrint : Telerik.Reporting.Report
    {
        public ItemtransferReceivePrint()
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