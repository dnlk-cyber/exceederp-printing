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
    public partial class ClearanceLetter : Telerik.Reporting.Report
    {
        public ClearanceLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            InitializeComponent();

            Telerik.Reporting.Drawing.PictureWatermark pictureWatermark1 = new Telerik.Reporting.Drawing.PictureWatermark();
            pictureWatermark1.Image = WaterMarkHelper.GetImageForWaterMark();
            pictureWatermark1.Position = Telerik.Reporting.Drawing.WatermarkPosition.Behind;
            pictureWatermark1.PrintOnFirstPage = true;
            pictureWatermark1.PrintOnLastPage = true;
            pictureWatermark1.Sizing = Telerik.Reporting.Drawing.WatermarkSizeMode.ScaleProportional;
            pictureWatermark1.Opacity = 0.5D;

            this.PageSettings.Watermarks.Add(pictureWatermark1);

            reportFooterSection1.Visible = false;
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = ClearanceLetterObjectDataSource;
            
            if(this.DataSource !=null)
            {
                reportFooterSection1.Visible = true;
            }
    //        Telerik.Reporting.Processing.ReportProcessor reportProcessor =
    //new Telerik.Reporting.Processing.ReportProcessor();

    //        // set any deviceInfo settings if necessary
    //        System.Collections.Hashtable deviceInfo =
    //            new System.Collections.Hashtable();

    //        Telerik.Reporting.TypeReportSource typeReportSource =
    //                     new Telerik.Reporting.TypeReportSource();

    //        // reportName is the Assembly Qualified Name of the report
    //        typeReportSource.TypeName = "Clearance Letter";

    //        Telerik.Reporting.Processing.RenderingResult result =
    //            reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

    //        string fileName = result.DocumentName + "." + result.Extension;
    //        string path = System.IO.Path.GetTempPath();
    //        string filePath = System.IO.Path.Combine(path, fileName);

    //        using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
    //        {
    //            fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
    //        }
        }
    }
}