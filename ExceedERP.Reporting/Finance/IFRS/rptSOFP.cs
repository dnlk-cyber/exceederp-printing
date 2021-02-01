namespace ExceedERP.Reporting.Finance.IFRS
{

    /// <summary>
    /// Summary description for rptSOFP.
    /// </summary>
    public partial class rptSOFP : Telerik.Reporting.Report
    {
        public rptSOFP()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
    }
}