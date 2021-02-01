namespace ExceedERP.Reporting.HRM.Payroll
{
    

    /// <summary>
    /// Summary description for rptPayrollDetail.
    /// </summary>
    public partial class rptPayrollDetail : Telerik.Reporting.Report
    {
        public rptPayrollDetail()
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