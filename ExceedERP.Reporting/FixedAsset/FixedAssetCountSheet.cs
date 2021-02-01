namespace ExceedERP.Reporting.FixedAsset
{
    /// <summary>
    /// Summary description for FixedAssetCountSheet.
    /// </summary>
    public partial class FixedAssetCountSheet : Telerik.Reporting.Report
    {
        public FixedAssetCountSheet()
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