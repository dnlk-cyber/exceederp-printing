namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class ContractLetter : Telerik.Reporting.Report
    {
        public ContractLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = ContarctObjectDataSource  ;
           
        }
    }
}