namespace ExceedERP.Reporting.Construction.BOQ
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class BOQSummaryView : Telerik.Reporting.Report
    {
        public BOQSummaryView()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //this.DataSource = Summary;
            this.DataSource =Summary;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}