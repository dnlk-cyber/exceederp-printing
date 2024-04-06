namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TrainingAndEducationOneByOne.
    /// </summary>
    public partial class TrainingAndEducationOneByOne : Telerik.Reporting.Report
    {
        public TrainingAndEducationOneByOne()
        {
            //
            // Required for telerik Reporting designer support
            //
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}