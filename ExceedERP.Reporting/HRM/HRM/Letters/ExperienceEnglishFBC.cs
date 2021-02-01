namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ExperienceEnglishFBC.
    /// </summary>
    public partial class ExperienceEnglishFBC : Telerik.Reporting.Report
    {
        public ExperienceEnglishFBC()
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