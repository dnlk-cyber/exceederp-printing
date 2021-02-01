namespace ExceedERP.Reporting.Finance.IFRS.ByBizUnit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptSOFPByBizUnit.
    /// </summary>
    public partial class rptSOFPByBizUnit : Telerik.Reporting.Report
    {
        public rptSOFPByBizUnit()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}