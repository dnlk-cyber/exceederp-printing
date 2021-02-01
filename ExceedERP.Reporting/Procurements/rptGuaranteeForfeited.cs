namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptGuaranteeForfeitedODS.
    /// </summary>
    public partial class rptGuaranteeForfeited : Telerik.Reporting.Report
    {
        public rptGuaranteeForfeited()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = GuaranteeForfeitedODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}