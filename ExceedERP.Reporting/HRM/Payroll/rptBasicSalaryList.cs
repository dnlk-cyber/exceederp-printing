using System.Drawing;

namespace ExceedERP.Reporting.HRM.Payroll
{
    /// <summary>
    /// Summary description for rptBasicSalaryList.
    /// </summary>
    public partial class rptBasicSalaryList : Telerik.Reporting.Report
    {
        public rptBasicSalaryList()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            //this.txtOrgName.Value = "Company Name";
            //this.txtOrgAddress.Value = "www.example.com";
            //this.txtOrgNameAmharic.Value = "Amharic Name";
            //this.txtOrgPhone.Value = "0110000000";
            // Image image1 = Image.FromFile(@"C:\Users\EXCEED-3\Documents\visual studio 2013\Projects\ExceedERP.Core\ExceedERP.Reporting\Images\logo.jpg");
            //this.pbOrgLogo.Value = image1;
        }
    }
}