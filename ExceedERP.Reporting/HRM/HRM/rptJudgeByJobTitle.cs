namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptJudgebyJobTitle : Telerik.Reporting.Report
    {
        public rptJudgebyJobTitle()
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