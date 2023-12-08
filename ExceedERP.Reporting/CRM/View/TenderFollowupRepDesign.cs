namespace ExceedERP.Reporting.CRM.View
{
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