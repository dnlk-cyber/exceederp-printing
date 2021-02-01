namespace ExceedERP.Reporting.Procurements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TechnicalPassFail.
    /// </summary>
    public partial class TechnicalPassFail : Telerik.Reporting.Report
    {
        public TechnicalPassFail()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = letterOds;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
       
    }
}