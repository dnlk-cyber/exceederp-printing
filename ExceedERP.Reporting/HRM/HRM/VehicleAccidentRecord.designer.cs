using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class VehicleAccidentRecord
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.No = new Telerik.Reporting.TextBox();
            this.Type = new Telerik.Reporting.TextBox();
            this.PlateNo = new Telerik.Reporting.TextBox();
            this.Amount = new Telerik.Reporting.TextBox();
            this.Remark = new Telerik.Reporting.TextBox();
            this.PeriodobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.Date = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblInsu = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.Total = new Telerik.Reporting.TextBox();
            this.InsuranceObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.InsuranceRpt = new Telerik.Reporting.TextBox();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // No
            // 
            this.No.Name = "No";
            this.No.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.41492351889610291D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.No.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.No.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.No.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.No.Style.Font.Bold = true;
            this.No.Style.Font.Name = "Nyala";
            this.No.StyleName = "Civic.TableHeader";
            this.No.Value = "ተ.ቁ";
            // 
            // Type
            // 
            this.Type.Name = "Type";
            this.Type.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4967294931411743D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.Type.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Type.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Type.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.Type.Style.Font.Bold = true;
            this.Type.Style.Font.Name = "Nyala";
            this.Type.StyleName = "Civic.TableHeader";
            this.Type.Value = "የተሸከርካሪው አይነት";
            // 
            // PlateNo
            // 
            this.PlateNo.Name = "PlateNo";
            this.PlateNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93379443883895874D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.PlateNo.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.PlateNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.PlateNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.PlateNo.Style.Font.Bold = true;
            this.PlateNo.Style.Font.Name = "Nyala";
            this.PlateNo.StyleName = "Civic.TableHeader";
            this.PlateNo.Value = "የሰሌዳ ቁጥር";
            // 
            // Amount
            // 
            this.Amount.Name = "Amount";
            this.Amount.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9354647397994995D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.Amount.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Amount.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Amount.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.Amount.Style.Font.Bold = true;
            this.Amount.Style.Font.Name = "Nyala";
            this.Amount.StyleName = "Civic.TableHeader";
            this.Amount.Value = "የጉዳት መጠን በገንዘብ ሲተመን";
            // 
            // Remark
            // 
            this.Remark.Name = "Remark";
            this.Remark.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.Remark.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Remark.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Remark.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.Remark.Style.Font.Bold = true;
            this.Remark.Style.Font.Name = "Nyala";
            this.Remark.StyleName = "Civic.TableHeader";
            this.Remark.Value = "ምርመራ";
            // 
            // PeriodobjectDataSource
            // 
            this.PeriodobjectDataSource.DataMember = "GetFiscalYear";
            this.PeriodobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.FiscalYearObjectDataSource);
            this.PeriodobjectDataSource.Name = "PeriodobjectDataSource";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3000396490097046D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Date,
            this.textBox17,
            this.panel2});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0041910805739462376D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7770614624023438D), Telerik.Reporting.Drawing.Unit.Inch(1.300039529800415D));
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.6958091259002686D), Telerik.Reporting.Drawing.Unit.Inch(1.1000003814697266D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000060200691223D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            this.Date.Value = "Date:";
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.0958089828491211D), Telerik.Reporting.Drawing.Unit.Inch(1.1000003814697266D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4747236967086792D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox17.Value = "= Fields.Date";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7770218849182129D), Telerik.Reporting.Drawing.Unit.Inch(1.0998821258544922D));
            this.panel2.Style.Font.Name = "Nyala";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(0.30007860064506531D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4769039154052734D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(1.0998427867889404D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4769039154052734D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.7000001072883606D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblInsu});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblInsu
            // 
            this.tblInsu.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.41492331027984619D)));
            this.tblInsu.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4967288970947266D)));
            this.tblInsu.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.93379408121109009D)));
            this.tblInsu.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.9354645013809204D)));
            this.tblInsu.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0000003576278687D)));
            this.tblInsu.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D)));
            this.tblInsu.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D)));
            this.tblInsu.Body.SetCellContent(0, 0, this.textBox2);
            this.tblInsu.Body.SetCellContent(0, 2, this.textBox4);
            this.tblInsu.Body.SetCellContent(0, 3, this.textBox6);
            this.tblInsu.Body.SetCellContent(0, 1, this.textBox8);
            this.tblInsu.Body.SetCellContent(1, 3, this.textBox11);
            this.tblInsu.Body.SetCellContent(0, 4, this.textBox13);
            this.tblInsu.Body.SetCellContent(1, 4, this.textBox14);
            this.tblInsu.Body.SetCellContent(1, 0, this.Total, 1, 3);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.No;
            tableGroup2.Name = "group";
            tableGroup2.ReportItem = this.Type;
            tableGroup3.Name = "tableGroup1";
            tableGroup3.ReportItem = this.PlateNo;
            tableGroup4.Name = "tableGroup2";
            tableGroup4.ReportItem = this.Amount;
            tableGroup5.Name = "group2";
            tableGroup5.ReportItem = this.Remark;
            this.tblInsu.ColumnGroups.Add(tableGroup1);
            this.tblInsu.ColumnGroups.Add(tableGroup2);
            this.tblInsu.ColumnGroups.Add(tableGroup3);
            this.tblInsu.ColumnGroups.Add(tableGroup4);
            this.tblInsu.ColumnGroups.Add(tableGroup5);
            this.tblInsu.DataSource = this.InsuranceObjectDataSource;
            this.tblInsu.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox8,
            this.textBox4,
            this.textBox6,
            this.textBox13,
            this.Total,
            this.textBox11,
            this.textBox14,
            this.No,
            this.Type,
            this.PlateNo,
            this.Amount,
            this.Remark});
            this.tblInsu.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.tblInsu.Name = "tblInsu";
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup6.Name = "detailTableGroup";
            tableGroup7.Name = "group1";
            this.tblInsu.RowGroups.Add(tableGroup6);
            this.tblInsu.RowGroups.Add(tableGroup7);
            this.tblInsu.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7809114456176758D), Telerik.Reporting.Drawing.Unit.Inch(0.67916679382324219D));
            this.tblInsu.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tblInsu.StyleName = "Civic.TableHeader";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.41492351889610291D), Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Value = "= RowNumber(\"tblInsu\")";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93379443883895874D), Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Value = "= Fields.PlateNo";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9354647397994995D), Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Value = "= Fields.Amount";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4967294931411743D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833432674408D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "= Fields.Vehicle";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9354647397994995D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Underline = true;
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.StyleName = "";
            this.textBox11.Value = "= Sum(Fields.Amount)";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D));
            this.textBox13.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox13.StyleName = "";
            this.textBox13.Value = "= Fields.Remark";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.StyleName = "";
            // 
            // Total
            // 
            this.Total.Name = "Total";
            this.Total.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8454465866088867D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.Total.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Total.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.Total.Style.Font.Bold = true;
            this.Total.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Total.StyleName = "";
            this.Total.Value = "ድምር";
            // 
            // InsuranceObjectDataSource
            // 
            this.InsuranceObjectDataSource.DataMember = "GetVehicleAccidentRecordInsurance";
            this.InsuranceObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.VehicleAccidentRecordInsuranceObjectDataSource);
            this.InsuranceObjectDataSource.Name = "InsuranceObjectDataSource";
            this.InsuranceObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("period", typeof(string), "= Parameters.period.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox16,
            this.Page});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.2064495086669922D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Value = "=PageNumber";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.7379381656646729D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.46843251585960388D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "Page:";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.55204373598098755D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.InsuranceRpt,
            this.textBox3,
            this.textBox1});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.49996042251586914D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.781252384185791D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // InsuranceRpt
            // 
            this.InsuranceRpt.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.InsuranceRpt.Name = "InsuranceRpt";
            this.InsuranceRpt.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.776982307434082D), Telerik.Reporting.Drawing.Unit.Inch(0.19992078840732575D));
            this.InsuranceRpt.Style.Font.Bold = true;
            this.InsuranceRpt.Style.Font.Name = "Nyala";
            this.InsuranceRpt.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.InsuranceRpt.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.InsuranceRpt.Value = "የኢንሹራንስ ክትትል ክፍል ሪፖርት";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // textBox1
            // 
            this.textBox1.Format = "{0:d}";
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.199960395693779D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.3496766090393066D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox1.Style.Font.Underline = true;
            this.textBox1.Value = "= Parameters.period.Label";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.9472862068996619E-08D), Telerik.Reporting.Drawing.Unit.Inch(0.199960395693779D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.49992120265960693D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Value = "Period";
            // 
            // VehicleAccidentRecord
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "Report1";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.PeriodobjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Name";
            reportParameter1.Name = "period";
            reportParameter1.Text = "በጀት ዓመት";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(5.8000006675720215D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.ObjectDataSource InsuranceObjectDataSource;
        private Telerik.Reporting.Table tblInsu;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox No;
        private Telerik.Reporting.TextBox PlateNo;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox Type;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox Amount;
        private Telerik.Reporting.TextBox Total;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox InsuranceRpt;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox Remark;
        private Telerik.Reporting.ObjectDataSource PeriodobjectDataSource;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox1;
    }
}