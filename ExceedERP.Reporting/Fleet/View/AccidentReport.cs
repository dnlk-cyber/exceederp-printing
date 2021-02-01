namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for AccidentReport.
    /// </summary>
    public partial class AccidentReport : Telerik.Reporting.Report
    {
        public AccidentReport()
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