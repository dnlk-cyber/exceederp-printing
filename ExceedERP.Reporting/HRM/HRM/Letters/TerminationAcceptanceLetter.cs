namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TerminationAcceptanceLetter.
    /// </summary>
    public partial class TerminationAcceptanceLetter : Telerik.Reporting.Report
    {
        public TerminationAcceptanceLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = TerminationAcceptanceODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}