namespace ExceedERP.Reporting.Insurance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptTotalAccidentInOurEmployees.
    /// </summary>
    public partial class rptTotalAccidentInOurEmployees : Telerik.Reporting.Report
    {
        public rptTotalAccidentInOurEmployees()
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