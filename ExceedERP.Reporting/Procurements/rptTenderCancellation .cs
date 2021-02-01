namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptTenderCanclation.
    /// </summary>
    public partial class rptTenderCancellation : Telerik.Reporting.Report
    {
        public rptTenderCancellation()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = TenderCanclationODS;
        }
    }
}