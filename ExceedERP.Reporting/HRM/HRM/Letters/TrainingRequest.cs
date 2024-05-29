namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TrainingRequest.
    /// </summary>
    public partial class TrainingRequest : Telerik.Reporting.Report
    {
        public TrainingRequest()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = TrainingRequestODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}