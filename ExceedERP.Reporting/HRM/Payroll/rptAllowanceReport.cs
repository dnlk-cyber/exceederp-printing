using System.Drawing;

namespace ExceedERP.Reporting.HRM.Payroll
{
   

    /// <summary>
    /// Summary description for rptPayrollSummary.
    /// </summary>
    public partial class rptAllowanceReport : Telerik.Reporting.Report
    {
        public rptAllowanceReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //



            // Image image1 = Image.FromFile(@"C:\Users\EXCEED-3\Documents\visual studio 2013\Projects\ExceedERP.Core\ExceedERP.Reporting\Images\logo.jpg");
            //this.pbOrgLogo.Value = image1;
        }
    }
}