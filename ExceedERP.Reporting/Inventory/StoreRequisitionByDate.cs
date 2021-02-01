namespace ExceedERP.Reporting.Inventory
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for StoreRequisitionByDate.
    /// </summary>
    public partial class StoreRequisitionByDate : Telerik.Reporting.Report
    {
        public StoreRequisitionByDate()
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