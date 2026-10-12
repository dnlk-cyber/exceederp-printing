namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using ExceedERP.Helpers.Common;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class ExperienceLetter : Telerik.Reporting.Report
    {
        public ExperienceLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            InitializeComponent();

            // reportFooterSection1.Visible = false;
            //
            // TODO: Add any constructor code after InitializeComponent call

            // this is to add watermark conditionaly 

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