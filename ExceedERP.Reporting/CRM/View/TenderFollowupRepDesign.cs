namespace ExceedERP.Reporting.CRM.View
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TenderFollowupRepDesign.
    /// </summary>
    public partial class TenderFollowupRepDesign : Telerik.Reporting.Report
    {
        public TenderFollowupRepDesign()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = headerOds;
        }
    }
}