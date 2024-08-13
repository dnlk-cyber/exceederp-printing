using ExceedERP.Reporting.Health.DataSource;
namespace ExceedERP.Reporting.Health.Design
{
   
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for MedicalRequestReport.
    /// </summary>
    public partial class MedicalRequestReport : Telerik.Reporting.Report
    {
        public MedicalRequestReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();
            //this.DataSource = MedicalRequestODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}