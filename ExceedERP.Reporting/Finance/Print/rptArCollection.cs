namespace ExceedERP.Reporting.Finance.Print
{

    /// <summary>
    /// Summary description for rtpArCollection.
    /// </summary>
    public partial class rptArCollection : Telerik.Reporting.Report
    {
        public rptArCollection()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = header;
        }
    }
}