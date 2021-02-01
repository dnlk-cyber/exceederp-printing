namespace ExceedERP.Reporting.FixedAsset
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for AnnualDepreciationList.
    /// </summary>
    public partial class AnnualDepreciationList : Telerik.Reporting.Report
    {
        public AnnualDepreciationList()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = headerComp;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}