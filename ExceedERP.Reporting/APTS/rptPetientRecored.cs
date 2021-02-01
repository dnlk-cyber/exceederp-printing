namespace ExceedERP.Reporting.APTS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptPetientRecored.
    /// </summary>
    public partial class rptPetientRecored : Telerik.Reporting.Report
    {
        public rptPetientRecored()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}