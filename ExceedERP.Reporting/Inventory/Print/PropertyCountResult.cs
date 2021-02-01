using ExceedERP.Reporting.Inventory.DataSource.Print;

namespace ExceedERP.Reporting.Inventory.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for PropertyCountResult.
    /// </summary>
    public partial class PropertyCountResult : Telerik.Reporting.Report
    {
        public PropertyCountResult()
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