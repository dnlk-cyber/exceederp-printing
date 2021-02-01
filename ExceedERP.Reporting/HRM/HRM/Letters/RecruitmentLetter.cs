namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class RecruitmentLetter : Telerik.Reporting.Report
    {
        public RecruitmentLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = RecruitmentObjectDataSource;
           
        }
    }
}