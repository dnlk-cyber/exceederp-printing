namespace ExceedERP.Reporting.Insurance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAccidentCoveredByInsuranceAndAskedForInsurance.
    /// </summary>
    public partial class rptAccidentCoveredByInsuranceAndAskedForInsurance : Telerik.Reporting.Report
    {
        public rptAccidentCoveredByInsuranceAndAskedForInsurance()
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