namespace ExceedERP.Reporting.Finance.SubLedger
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for APJournalEntries.
    /// </summary>
    public partial class APJournalEntries : Telerik.Reporting.Report
    {
        public APJournalEntries()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Header;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}