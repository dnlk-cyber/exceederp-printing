 

namespace ExceedERP.Reporting.Inventory
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for StoreReturnItem.
    /// </summary>
    public partial class StoreReturnItem : Telerik.Reporting.Report
    {
        public StoreReturnItem()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = headerComp; 


        }
    }
}