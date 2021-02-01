namespace ExceedERP.Reporting.Fleet.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for RentTimeSheetPerformace.
    /// </summary>
    public partial class RentTimeSheet : Telerik.Reporting.Report
    {
        public RentTimeSheet()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

         //   this.reportHeader.Report.DataSource = headerComp; 
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}