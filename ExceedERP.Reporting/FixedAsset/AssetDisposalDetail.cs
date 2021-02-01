namespace ExceedERP.Reporting.FixedAsset
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for AssetDisposalDetail.
    /// </summary>
    public partial class AssetDisposalDetail : Telerik.Reporting.Report
    {
        public AssetDisposalDetail()
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