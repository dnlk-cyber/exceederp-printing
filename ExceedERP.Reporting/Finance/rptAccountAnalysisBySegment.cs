namespace ExceedERP.Reporting.Finance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAccountAnalysisBySegment.
    /// </summary>
    public partial class rptAccountAnalysisBySegment : Telerik.Reporting.Report
    {
        public rptAccountAnalysisBySegment()
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