
namespace ExceedERP.Reporting.Inventory
{

    /// <summary>
    /// Summary description for StoreStockLedger.
    /// </summary>
    public partial class StoreStockLedger : Telerik.Reporting.Report
    {
        public StoreStockLedger()
        {

            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource =headerComp;
            table1.DataSource = stockDs;

            //this.DataSource = s

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
       
    }
}