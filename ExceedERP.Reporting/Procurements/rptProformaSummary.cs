namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptProformaSummary.
    /// </summary>
    public partial class rptProformaSummary : Telerik.Reporting.Report
    {
        public rptProformaSummary()
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