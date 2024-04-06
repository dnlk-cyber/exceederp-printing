namespace ExceedERP.Reporting.HRM.HRM
{
    using ExceedERP.Helpers.Common;
    using System;
    using System.ComponentModel;
    using System.Drawing;
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
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();
            this.DataSource = WarrantyODS;
            
        }
    }
}