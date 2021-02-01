namespace ExceedERP.Reporting.Inventory
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for InventoryCountSheet.
    /// </summary>
    public partial class InventoryCountSheet : Telerik.Reporting.Report
    {
        public InventoryCountSheet()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = headerComp;
           
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void InventoryCountSheet_ItemDataBinding(object sender, EventArgs e)
        {
            
        }

        private void InventoryCountSheet_ItemDataBound(object sender, EventArgs e)
        {
            this.Report.DocumentName = this.txtStoreCode.Value;
        }
    }
}