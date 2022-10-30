using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.CRM.Printing.View
{
    /// <summary>
    /// Summary description for QuotePrint.
    /// </summary>
    public partial class EstimationQuotePrint : Telerik.Reporting.Report
    {
        public EstimationQuotePrint()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            // water mark

            Telerik.Reporting.Drawing.PictureWatermark pictureWatermark1 = new Telerik.Reporting.Drawing.PictureWatermark();
            pictureWatermark1.Image = WaterMarkHelper.GetImageForWaterMark();
            pictureWatermark1.Position = Telerik.Reporting.Drawing.WatermarkPosition.Behind;
            pictureWatermark1.PrintOnFirstPage = true;
            pictureWatermark1.PrintOnLastPage = true;
            pictureWatermark1.Sizing = Telerik.Reporting.Drawing.WatermarkSizeMode.ScaleProportional;
            pictureWatermark1.Opacity = 0.5D;

            this.PageSettings.Watermarks.Add(pictureWatermark1);

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = organizationOds;
        }
    }
}