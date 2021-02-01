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
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = PromotionObjectDataSource  ;
           
        }
    }
}