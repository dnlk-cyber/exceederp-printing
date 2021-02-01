namespace ExceedERP.Reporting.Finance.IFRS.ByBizUnit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptSOCIByBizUnit.
    /// </summary>
    public partial class rptSOCIByBizUnit : Telerik.Reporting.Report
    {
        public rptSOCIByBizUnit()
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