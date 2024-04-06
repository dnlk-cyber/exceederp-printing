namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class PromotionLetter : Telerik.Reporting.Report
    {
        public PromotionLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = PromotionObjectDataSource  ;
           
        }
    }
}