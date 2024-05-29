namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ReferalSheet.
    /// </summary>
    public partial class ReferalSheet : Telerik.Reporting.Report
    {
        public ReferalSheet()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = ReferalSheetODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}