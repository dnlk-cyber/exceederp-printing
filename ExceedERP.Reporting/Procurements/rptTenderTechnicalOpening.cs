namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptTenderTechnicalOpening.
    /// </summary>
    public partial class rptTenderTechnicalOpening : Telerik.Reporting.Report
    {
        public rptTenderTechnicalOpening()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = TechnicalTenderOpeningODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}