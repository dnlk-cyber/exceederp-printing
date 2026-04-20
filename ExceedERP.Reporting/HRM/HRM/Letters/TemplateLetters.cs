namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using ExceedERP.Helpers.Common;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TemplateLetters.
    /// </summary>
    public partial class TemplateLetters : Telerik.Reporting.Report
    {
        public TemplateLetters()
        {
            //
            // Required for telerik Reporting designer support
            //
            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
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