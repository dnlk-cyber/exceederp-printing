namespace ExceedERP.Reporting.CRM.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for OrderStatus.
    /// </summary>
    public partial class OrderStatus : Telerik.Reporting.Report
    {
        public OrderStatus()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.DataSource = headerComp;
        }
    }
}