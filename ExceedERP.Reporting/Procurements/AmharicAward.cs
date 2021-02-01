namespace ExceedERP.Reporting.Procurements
{
    /// <summary>
    /// Summary description for AmharicAward.
    /// </summary>
    public partial class AmharicAward : Telerik.Reporting.Report
    {
        public AmharicAward()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = AwardOds;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}