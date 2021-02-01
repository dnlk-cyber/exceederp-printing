namespace ExceedERP.Reporting.Finance.IFRS.ByBranch
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptSOFPByBranch.
    /// </summary>
    public partial class rptSOFPByBranch : Telerik.Reporting.Report
    {
        public rptSOFPByBranch()
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