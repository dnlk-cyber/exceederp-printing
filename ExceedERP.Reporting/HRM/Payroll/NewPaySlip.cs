using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.Payroll
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class NewPaySlip : Telerik.Reporting.Report
    {
        public NewPaySlip()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = PaySlipObjectDataSource;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            //this.DataSource = HeaderObjectDataSource;
        }
    }
}