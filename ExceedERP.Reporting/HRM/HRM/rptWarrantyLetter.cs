namespace ExceedERP.Reporting.HRM.HRM
{
    using ExceedERP.Helpers.Common;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptWarrantyLetter.
    /// </summary>
    public partial class rptWarrantyLetter : Telerik.Reporting.Report
    {
        public rptWarrantyLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            InitializeComponent();
            this.DataSource = WarrantyODS;
            
        }
    }
}