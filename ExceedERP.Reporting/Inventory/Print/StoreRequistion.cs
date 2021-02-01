namespace ExceedERP.Reporting.Inventory.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for StoreRequistion.
    /// </summary>
    public partial class StoreRequistion : Telerik.Reporting.Report
    {
        public StoreRequistion()
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