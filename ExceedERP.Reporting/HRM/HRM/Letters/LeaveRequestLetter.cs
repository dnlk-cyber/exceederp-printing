namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for LeaveRequestLetter.
    /// </summary>
    public partial class LeaveRequestLetter : Telerik.Reporting.Report
    {
        public LeaveRequestLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            InitializeComponent();
            this.DataSource = LeaveRequestODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}