namespace ExceedERP.Reporting.Procurements
{

    /// <summary>
    /// Summary description for ProcurementPlan.
    /// </summary>
    public partial class ProcurementPlan : Telerik.Reporting.Report
    {
        public ProcurementPlan()
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