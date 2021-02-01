
 

namespace ExceedERP.Reporting.Inventory
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for GoodsReceive.
    /// </summary>
    public partial class GoodsReceive : Telerik.Reporting.Report
    {
        public GoodsReceive()
        {
            
            InitializeComponent();
             

            this.DataSource = headerComp;

             
        }
    }
}