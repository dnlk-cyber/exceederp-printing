namespace ExceedERP.Reporting.HRM.HRM
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
    /// Summary description for ComplainResponseDepartment.
    /// </summary>
    public partial class ComplainResponseDepartment : Telerik.Reporting.Report
    {
        public ComplainResponseDepartment()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            InitializeComponent();
            this.DataSource = ComplainODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}