namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ExperienceAmharicFBC.
    /// </summary>
    public partial class ExperienceAmharicFBC : Telerik.Reporting.Report
    {
        public ExperienceAmharicFBC()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = ExperienceODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}