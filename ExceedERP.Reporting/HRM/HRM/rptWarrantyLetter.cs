namespace ExceedERP.Reporting.HRM.HRM
{
    using ExceedERP.Helpers.Common;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rptWarrantyLetter.
    /// </summary>
    public partial class rptWarrantyLetter : Telerik.Reporting.Report
    {
        public rptWarrantyLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = WarrantyODS;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            Telerik.Reporting.Drawing.PictureWatermark pictureWatermark1 = new Telerik.Reporting.Drawing.PictureWatermark();
            pictureWatermark1.Image = WaterMarkHelper.GetImageForWaterMark();
            pictureWatermark1.Position = Telerik.Reporting.Drawing.WatermarkPosition.Behind;
            pictureWatermark1.PrintOnFirstPage = true;
            pictureWatermark1.PrintOnLastPage = true;
            pictureWatermark1.Sizing = Telerik.Reporting.Drawing.WatermarkSizeMode.ScaleProportional;
            pictureWatermark1.Opacity = 0.5D;

            this.PageSettings.Watermarks.Add(pictureWatermark1);
        }
    }
}