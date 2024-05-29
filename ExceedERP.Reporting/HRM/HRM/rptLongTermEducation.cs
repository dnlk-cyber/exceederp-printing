namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptLongTermEducation.
    /// </summary>
    public partial class rptLongTermEducation : Telerik.Reporting.Report
    {
        public rptLongTermEducation()
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