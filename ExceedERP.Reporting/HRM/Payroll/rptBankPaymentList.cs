using System.Drawing;

namespace ExceedERP.Reporting.HRM.Payroll
{
  

    /// <summary>
    /// Summary description for rptBankPaymentList.
    /// </summary>
    public partial class rptBankPaymentList : Telerik.Reporting.Report
    {
        public rptBankPaymentList()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            this.DataSource = Organization;

            //Image image1 = Image.FromFile(@"C:\Users\EXCEED-3\Documents\visual studio 2013\Projects\ExceedERP.Core\ExceedERP.Reporting\Images\logo.jpg");
            //this.pbOrgLogo.Value = image1;
        }
    }
}