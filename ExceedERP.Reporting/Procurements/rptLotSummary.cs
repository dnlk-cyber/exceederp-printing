namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptLotSummary.
    /// </summary>
    public partial class rptLotSummary : Telerik.Reporting.Report
    {
        public rptLotSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = letterOds;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}