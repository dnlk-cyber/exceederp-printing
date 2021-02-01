namespace ExceedERP.Reporting.Finance.SubLedger
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ARJournalEntries.
    /// </summary>
    public partial class ARJournalEntries : Telerik.Reporting.Report
    {
        public ARJournalEntries()
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