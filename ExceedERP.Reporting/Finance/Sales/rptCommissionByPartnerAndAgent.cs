namespace ExceedERP.Reporting.Finance.Sales
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptCommissionByPartnerAndAgent.
    /// </summary>
    public partial class rptCommissionByPartnerAndAgent : Telerik.Reporting.Report
    {
        public rptCommissionByPartnerAndAgent()
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