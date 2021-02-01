namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAmharicResultNotification.
    /// </summary>
    public partial class rptAmharicResultNotification : Telerik.Reporting.Report
    {
        public rptAmharicResultNotification()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = letterOds;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}