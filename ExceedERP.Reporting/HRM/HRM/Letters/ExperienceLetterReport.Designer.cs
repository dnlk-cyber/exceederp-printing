using System;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class ExperienceLetterReport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExperienceLetterReport));
            Telerik.Reporting.Drawing.PictureWatermark pictureWatermark1 = new Telerik.Reporting.Drawing.PictureWatermark();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageHeader = new Telerik.Reporting.PageHeaderSection();
            this.imgHeaderLogo = new Telerik.Reporting.PictureBox();
            this.txtHeaderCompName = new Telerik.Reporting.HtmlTextBox();
            this.txtHeaderCompSlogan = new Telerik.Reporting.HtmlTextBox();
            this.shapeHeaderLine = new Telerik.Reporting.Shape();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pnlRef = new Telerik.Reporting.Panel();
            this.htmlRef = new Telerik.Reporting.HtmlTextBox();
            this.pnlEnglish = new Telerik.Reporting.Panel();
            this.htmlContentEn = new Telerik.Reporting.HtmlTextBox();
            this.dividerLine = new Telerik.Reporting.Shape();
            this.pnlAmharic = new Telerik.Reporting.Panel();
            this.htmlContentAm = new Telerik.Reporting.HtmlTextBox();
            this.pnlSignatureArea = new Telerik.Reporting.Panel();
            this.imgStamp = new Telerik.Reporting.PictureBox();
            this.imgSign = new Telerik.Reporting.PictureBox();
            this.htmlSigner = new Telerik.Reporting.HtmlTextBox();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.shapeFooterLine = new Telerik.Reporting.Shape();
            this.htmlFooterCol1 = new Telerik.Reporting.HtmlTextBox();
            this.htmlFooterCol2 = new Telerik.Reporting.HtmlTextBox();
            this.shapeFooterBottomBar = new Telerik.Reporting.Shape();
            this.objectDataSource1 = new Telerik.Reporting.ObjectDataSource();
            this.employeelistobjectDataSource2 = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D);
            this.pageHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.imgHeaderLogo,
            this.txtHeaderCompName,
            this.txtHeaderCompSlogan,
            this.shapeHeaderLine});
            this.pageHeader.Name = "pageHeader";
            // 
            // imgHeaderLogo
            // 
            this.imgHeaderLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.11999999731779099D), Telerik.Reporting.Drawing.Unit.Inch(0.079999998211860657D));
            this.imgHeaderLogo.Name = "imgHeaderLogo";
            this.imgHeaderLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.85000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.85000002384185791D));
            this.imgHeaderLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            // 
            // txtHeaderCompName
            // 
            this.txtHeaderCompName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.11999999731779099D));
            this.txtHeaderCompName.Name = "txtHeaderCompName";
            this.txtHeaderCompName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.txtHeaderCompName.Style.Font.Bold = true;
            this.txtHeaderCompName.Style.Font.Name = "Segoe UI";
            this.txtHeaderCompName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtHeaderCompName.Value = "<span style=\'color:#4189C8;\'>MCG CONSTRUCTION PLC</span>&nbsp;&nbsp;&nbsp;<span s" +
    "tyle=\'color:#F0A64C;\'>ኤምሲጂ ኮንስትራክሽን ኃ/የተ/የግ/ማህበር</span>";
            // 
            // txtHeaderCompSlogan
            // 
            this.txtHeaderCompSlogan.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D));
            this.txtHeaderCompSlogan.Name = "txtHeaderCompSlogan";
            this.txtHeaderCompSlogan.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.txtHeaderCompSlogan.Style.Font.Bold = true;
            this.txtHeaderCompSlogan.Style.Font.Name = "Segoe UI";
            this.txtHeaderCompSlogan.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtHeaderCompSlogan.Value = "<span style=\'color:#F0A64C;\'>WE BUILD YOUR CHOICE!</span>&nbsp;&nbsp;&nbsp;<span " +
    "style=\'color:#4189C8;\'>ም ር ጫ ዎ ት ን   እ ን ገ ነ ባ ለ ን !</span>";
            // 
            // shapeHeaderLine
            // 
            this.shapeHeaderLine.Angle = 0D;
            this.shapeHeaderLine.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.shapeHeaderLine.Name = "shapeHeaderLine";
            this.shapeHeaderLine.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shapeHeaderLine.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.0999999046325684D), Telerik.Reporting.Drawing.Unit.Point(3D));
            this.shapeHeaderLine.Stretch = true;
            this.shapeHeaderLine.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(137)))), ((int)(((byte)(200)))));
            this.shapeHeaderLine.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shapeHeaderLine.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(2.5D);
            this.shapeHeaderLine.Style.Color = System.Drawing.Color.Transparent;
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(6.5999999046325684D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pnlRef,
            this.pnlEnglish,
            this.dividerLine,
            this.pnlAmharic,
            this.pnlSignatureArea});
            this.detail.Name = "detail";
            // 
            // pnlRef
            // 
            this.pnlRef.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.htmlRef});
            this.pnlRef.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.019999999552965164D));
            this.pnlRef.Name = "pnlRef";
            this.pnlRef.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D));
            // 
            // htmlRef
            // 
            this.htmlRef.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.htmlRef.Name = "htmlRef";
            this.htmlRef.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D));
            this.htmlRef.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(137)))), ((int)(((byte)(200)))));
            this.htmlRef.Style.Font.Name = "Segoe UI";
            this.htmlRef.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.htmlRef.Value = "=\"Ref. No / ቁጥር: <b>\" + Fields.ReferenceNumber + \"</b><br />Date / ቀን: <b>\" + Fie" +
    "lds.LetterDateGregorian + \" (\" + Fields.LetterDateAmharic + \")</b>\"";
            // 
            // pnlEnglish
            // 
            this.pnlEnglish.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.htmlContentEn});
            this.pnlEnglish.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.60000002384185791D));
            this.pnlEnglish.Name = "pnlEnglish";
            this.pnlEnglish.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4500000476837158D), Telerik.Reporting.Drawing.Unit.Inch(4.4499998092651367D));
            // 
            // htmlContentEn
            // 
            this.htmlContentEn.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.htmlContentEn.Name = "htmlContentEn";
            this.htmlContentEn.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4500000476837158D), Telerik.Reporting.Drawing.Unit.Inch(4.4499998092651367D));
            this.htmlContentEn.Style.Font.Name = "Segoe UI";
            this.htmlContentEn.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.htmlContentEn.Value = resources.GetString("htmlContentEn.Value");
            // 
            // dividerLine
            // 
            this.dividerLine.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5199999809265137D), Telerik.Reporting.Drawing.Unit.Inch(0.60000002384185791D));
            this.dividerLine.Name = "dividerLine";
            this.dividerLine.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NS);
            this.dividerLine.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Point(1.5D), Telerik.Reporting.Drawing.Unit.Inch(4.4499998092651367D));
            this.dividerLine.Style.Color = System.Drawing.Color.Gainsboro;
            // 
            // pnlAmharic
            // 
            this.pnlAmharic.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.htmlContentAm});
            this.pnlAmharic.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.60000002384185791D));
            this.pnlAmharic.Name = "pnlAmharic";
            this.pnlAmharic.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(4.4499998092651367D));
            // 
            // htmlContentAm
            // 
            this.htmlContentAm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.htmlContentAm.Name = "htmlContentAm";
            this.htmlContentAm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(4.4499998092651367D));
            this.htmlContentAm.Style.Font.Name = "Nyala";
            this.htmlContentAm.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.htmlContentAm.Value = resources.GetString("htmlContentAm.Value");
            // 
            // pnlSignatureArea
            // 
            this.pnlSignatureArea.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.imgStamp,
            this.imgSign,
            this.htmlSigner});
            this.pnlSignatureArea.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(5.4499998092651367D));
            this.pnlSignatureArea.Name = "pnlSignatureArea";
            this.pnlSignatureArea.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5D), Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D));
            // 
            // imgStamp
            // 
            this.imgStamp.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.imgStamp.MimeType = "";
            this.imgStamp.Name = "imgStamp";
            this.imgStamp.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D));
            this.imgStamp.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.imgStamp.Value = "=Fields.CompanyStamp";
            // 
            // imgSign
            // 
            this.imgSign.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.15999999642372131D));
            this.imgSign.MimeType = "";
            this.imgSign.Name = "imgSign";
            this.imgSign.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D));
            this.imgSign.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.imgSign.Value = "=Fields.SignatureImage";
            // 
            // htmlSigner
            // 
            this.htmlSigner.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.htmlSigner.Name = "htmlSigner";
            this.htmlSigner.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D));
            this.htmlSigner.Style.Font.Bold = true;
            this.htmlSigner.Style.Font.Name = "Segoe UI";
            this.htmlSigner.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.htmlSigner.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.htmlSigner.Value = resources.GetString("htmlSigner.Value");
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(1.0499999523162842D);
            this.pageFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shapeFooterLine,
            this.htmlFooterCol1,
            this.htmlFooterCol2,
            this.shapeFooterBottomBar});
            this.pageFooter.Name = "pageFooter";
            // 
            // shapeFooterLine
            // 
            this.shapeFooterLine.Angle = 0D;
            this.shapeFooterLine.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.019999999552965164D));
            this.shapeFooterLine.Name = "shapeFooterLine";
            this.shapeFooterLine.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shapeFooterLine.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.0999999046325684D), Telerik.Reporting.Drawing.Unit.Point(1.5D));
            this.shapeFooterLine.Stretch = true;
            this.shapeFooterLine.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(137)))), ((int)(((byte)(200)))));
            this.shapeFooterLine.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // htmlFooterCol1
            // 
            this.htmlFooterCol1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000000149011612D), Telerik.Reporting.Drawing.Unit.Inch(0.079999998211860657D));
            this.htmlFooterCol1.Name = "htmlFooterCol1";
            this.htmlFooterCol1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D));
            this.htmlFooterCol1.Style.Font.Name = "Segoe UI";
            this.htmlFooterCol1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.htmlFooterCol1.Value = resources.GetString("htmlFooterCol1.Value");
            // 
            // htmlFooterCol2
            // 
            this.htmlFooterCol2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.079999998211860657D));
            this.htmlFooterCol2.Name = "htmlFooterCol2";
            this.htmlFooterCol2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D));
            this.htmlFooterCol2.Style.Font.Name = "Segoe UI";
            this.htmlFooterCol2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.htmlFooterCol2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.htmlFooterCol2.Value = "<span style=\'color:#F0A64C;\'>&#128205;</span> <b style=\'color:#F0A64C;\'>ADDIS ABA" +
    "BA/ETHIOPIA</b><br />Senegal Street, Around Semen Hotel<br />Adu Building";
            // 
            // shapeFooterBottomBar
            // 
            this.shapeFooterBottomBar.Angle = 0D;
            this.shapeFooterBottomBar.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.85000002384185791D));
            this.shapeFooterBottomBar.Name = "shapeFooterBottomBar";
            this.shapeFooterBottomBar.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shapeFooterBottomBar.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.0999999046325684D), Telerik.Reporting.Drawing.Unit.Point(4D));
            this.shapeFooterBottomBar.Stretch = true;
            this.shapeFooterBottomBar.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(137)))), ((int)(((byte)(200)))));
            this.shapeFooterBottomBar.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shapeFooterBottomBar.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(4D);
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataMember = "GetExperienceLetterData";
            this.objectDataSource1.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.ExperienceLetterReportDataSource);
            this.objectDataSource1.Name = "objectDataSource1";
            this.objectDataSource1.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("employeeId", typeof(string), "=Parameters.employeeId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("refNumber", typeof(string), "=Parameters.refNumber.Value")});
            // 
            // employeelistobjectDataSource2
            // 
            this.employeelistobjectDataSource2.DataMember = "GetAllActivePlacedEmployees";
            this.employeelistobjectDataSource2.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AllActiveEmployeesObjectDataSource);
            this.employeelistobjectDataSource2.Name = "employeelistobjectDataSource2";
            // 
            // ExperienceLetterReport
            // 
            this.DataSource = this.objectDataSource1;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeader,
            this.detail,
            this.pageFooter});
            this.Name = "ExperienceLetterReport";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            pictureWatermark1.Image = ((object)(resources.GetObject("pictureWatermark1.Image")));
            pictureWatermark1.Sizing = Telerik.Reporting.Drawing.WatermarkSizeMode.ScaleProportional;
            this.PageSettings.Watermarks.Add(pictureWatermark1);
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.employeelistobjectDataSource2;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.FullName";
            reportParameter1.AvailableValues.ValueMember = "= Fields.EmpId";
            reportParameter1.Name = "employeeId";
            reportParameter1.Text = "Employee";
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "refNumber";
            reportParameter2.Text = "Ref. Number";
            reportParameter2.Value = "";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.Name = "FormNumber";
            reportParameter4.AllowNull = true;
            reportParameter4.Name = "FormTitle";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.0999999046325684D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeader;
        private Telerik.Reporting.PictureBox imgHeaderLogo;
        private Telerik.Reporting.HtmlTextBox txtHeaderCompName;
        private Telerik.Reporting.HtmlTextBox txtHeaderCompSlogan;
        private Telerik.Reporting.Shape shapeHeaderLine;

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.Panel pnlEnglish;
        private Telerik.Reporting.HtmlTextBox htmlContentEn;
        private Telerik.Reporting.Shape dividerLine;

        private Telerik.Reporting.Panel pnlRef;
        private Telerik.Reporting.HtmlTextBox htmlRef;
        private Telerik.Reporting.Panel pnlAmharic;
        private Telerik.Reporting.HtmlTextBox htmlContentAm;

        private Telerik.Reporting.Panel pnlSignatureArea;
        private Telerik.Reporting.PictureBox imgStamp;
        private Telerik.Reporting.PictureBox imgSign;
        private Telerik.Reporting.HtmlTextBox htmlSigner;

        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.Shape shapeFooterLine;
        private Telerik.Reporting.HtmlTextBox htmlFooterCol1;
        private Telerik.Reporting.HtmlTextBox htmlFooterCol2;
        private Telerik.Reporting.Shape shapeFooterBottomBar;

        private Telerik.Reporting.ObjectDataSource objectDataSource1;
        private Telerik.Reporting.ObjectDataSource employeelistobjectDataSource2;
    }
}


