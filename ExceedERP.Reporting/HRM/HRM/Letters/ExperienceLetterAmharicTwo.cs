namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ExperienceLetterAmharicTwo.
    /// </summary>
    public partial class ExperienceLetterAmharicTwo : Telerik.Reporting.Report
    {
        public ExperienceLetterAmharicTwo()
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