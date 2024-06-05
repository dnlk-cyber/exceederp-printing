namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TimeSheetSummary.
    /// </summary>
    public partial class TimeSheetSummary : Telerik.Reporting.Report
    {
        public TimeSheetSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();


            this.Report.DataSource = headerComp;
        }
    }
}