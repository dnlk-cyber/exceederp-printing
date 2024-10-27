namespace ExceedERP.Reporting.Finance
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptAPTravelOrderPayList.
    /// </summary>
    public partial class rptAPTravelOrderPayList : Telerik.Reporting.Report
    {
        public rptAPTravelOrderPayList()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = header;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}