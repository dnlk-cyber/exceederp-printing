namespace ExceedERP.Reporting.CRM.View
{
    /// <summary>
    /// Summary description for OrderStatus.
    /// </summary>
    public partial class OrderStatus : Telerik.Reporting.Report
    {
        public OrderStatus()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.DataSource = headerComp;
        }
    }
}