namespace ExceedERP.Reporting.Finance
{
    /// <summary>
    /// Summary description for rptByContributionType.
    /// </summary>
    public partial class TaxReport : Telerik.Reporting.Report
    {
        public TaxReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            this.DataSource = Organization;

            // Image image1 = Image.FromFile(@"C:\Users\EXCEED-3\Documents\visual studio 2013\Projects\ExceedERP.Core\ExceedERP.Reporting\Images\logo.jpg");
            //this.pbOrgLogo.Value = image1;
        }
    }
}