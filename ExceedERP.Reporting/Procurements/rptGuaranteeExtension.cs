namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptGuaranteeExtension.
    /// </summary>
    public partial class rptGuaranteeExtension : Telerik.Reporting.Report
    {
        public rptGuaranteeExtension()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = GuaranteeExtensionODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}