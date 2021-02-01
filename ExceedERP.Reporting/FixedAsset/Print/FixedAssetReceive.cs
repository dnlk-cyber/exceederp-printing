namespace ExceedERP.Reporting.FixedAsset.Print
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for FixedAssetReceive.
    /// </summary>
    public partial class FixedAssetReceive : Telerik.Reporting.Report
    {
        public FixedAssetReceive()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Header; 
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}