namespace ExceedERP.Reporting.Insurance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptCostsToGetMednInsurance.
    /// </summary>
    public partial class rptCostsToGetMednInsurance : Telerik.Reporting.Report
    {
        public rptCostsToGetMednInsurance()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}