namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptBuildingPurchaseAgreementFollowingForm.
    /// </summary>
    public partial class rptContractListReport : Telerik.Reporting.Report
    {
        public rptContractListReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
    }
}