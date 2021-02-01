namespace ExceedERP.Reporting.Inventoryy
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ItemInStore.
    /// </summary>
    public partial class ItemInStore : Telerik.Reporting.Report
    {
        public ItemInStore()
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
    }
}