namespace ExceedERP.Reporting.Finance.Ratio.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for RatioAnnualSummary.
    /// </summary>
    public partial class RatioAnnualSummary : Telerik.Reporting.Report
    {
        public RatioAnnualSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.pageHeaderSection1.Report.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}