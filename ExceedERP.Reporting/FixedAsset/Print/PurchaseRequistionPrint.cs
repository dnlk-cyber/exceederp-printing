namespace ExceedERP.Reporting.FixedAsset.Print
{
    /// <summary>
    /// Summary description for PurchaseRequistionPrint.
    /// </summary>
    public partial class PurchaseRequistionPrint : Telerik.Reporting.Report
    {
        public PurchaseRequistionPrint()
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