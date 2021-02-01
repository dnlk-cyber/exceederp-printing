namespace ExceedERP.Reporting.Procurements
{

    /// <summary>
    /// Summary description for ProcurementNeed.
    /// </summary>
    public partial class ProcurementNeed : Telerik.Reporting.Report
    {
        public ProcurementNeed()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}