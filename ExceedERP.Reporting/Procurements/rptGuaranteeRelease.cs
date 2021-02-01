namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptGuaranteeRelease.
    /// </summary>
    public partial class rptGuaranteeRelease : Telerik.Reporting.Report
    {
        public rptGuaranteeRelease()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = GuaranteeReleaseODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}