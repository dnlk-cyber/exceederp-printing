

namespace ExceedERP.Reporting.Inventory
{

    /// <summary>
    /// Summary description for StockAdjustmentItem.
    /// </summary>
    public partial class StockAdjustmentItem : Telerik.Reporting.Report
    {
        public StockAdjustmentItem()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            this.DataSource = headerComp;

            //Image image1 = Image.FromFile(@"C:\Users\EXCEED-3\Documents\visual studio 2013\Projects\ExceedERP.Core\ExceedERP.Reporting\Images\logo.jpg");
            //this.pbOrgLogo.Value = image1;
        }
    }
}