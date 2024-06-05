namespace ExceedERP.Reporting.Maintenance.Design
{
    partial class InternalMaintenanceCostDesign
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox38 = new Telerik.Reporting.TextBox();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.HOds = new Telerik.Reporting.ObjectDataSource();
            this.MROds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.994031548500061D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox11.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Value = "S/N";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(5.2401289939880371D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox17.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox17.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox17.Value = "Plate No";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(6.8441901206970215D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox19.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.StyleName = "";
            this.textBox19.Value = "Maintenance Cost";
            // 
            // textBox21
            // 
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(6.3892436027526855D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox21.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox21.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox21.StyleName = "";
            this.textBox21.Value = "Spare Part Cost";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox38,
            this.textBox35});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.9937504529953003D), Telerik.Reporting.Drawing.Unit.Inch(1.9709268599399365E-05D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.8062496185302734D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(1.9709268599399365E-05D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2244089841842651D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // textBox38
            // 
            this.textBox38.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0041670799255371D), Telerik.Reporting.Drawing.Unit.Inch(0.2999802827835083D));
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.795832633972168D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox38.Style.Font.Bold = true;
            this.textBox38.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox38.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox38.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox38.TextWrap = false;
            this.textBox38.Value = "= Fields.Name";
            // 
            // textBox35
            // 
            this.textBox35.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7000788450241089D), Telerik.Reporting.Drawing.Unit.Inch(0.600019633769989D));
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.0999212265014648D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox35.Style.Font.Bold = true;
            this.textBox35.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox35.Style.Font.Underline = false;
            this.textBox35.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox35.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox35.TextWrap = false;
            this.textBox35.Value = "Internal Maintenance Cost Report";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table2});
            this.detail.Name = "detail";
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(1.994031548500061D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(5.2401289939880371D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(6.8441901206970215D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(6.3892436027526855D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D)));
            this.table2.Body.SetCellContent(0, 0, this.textBox14);
            this.table2.Body.SetCellContent(0, 1, this.textBox18);
            this.table2.Body.SetCellContent(0, 2, this.textBox20);
            this.table2.Body.SetCellContent(0, 3, this.textBox22);
            tableGroup1.Name = "tableGroup3";
            tableGroup1.ReportItem = this.textBox11;
            tableGroup2.Name = "tableGroup5";
            tableGroup2.ReportItem = this.textBox17;
            tableGroup3.Name = "group2";
            tableGroup3.ReportItem = this.textBox19;
            tableGroup4.Name = "group3";
            tableGroup4.ReportItem = this.textBox21;
            this.table2.ColumnGroups.Add(tableGroup1);
            this.table2.ColumnGroups.Add(tableGroup2);
            this.table2.ColumnGroups.Add(tableGroup3);
            this.table2.ColumnGroups.Add(tableGroup4);
            this.table2.DataSource = this.MROds;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14,
            this.textBox18,
            this.textBox20,
            this.textBox22,
            this.textBox11,
            this.textBox17,
            this.textBox19,
            this.textBox21});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.36040499806404114D), Telerik.Reporting.Drawing.Unit.Cm(1.7314581871032715D));
            this.table2.Name = "table2";
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup5.Name = "detailTableGroup1";
            this.table2.RowGroups.Add(tableGroup5);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(20.467594146728516D), Telerik.Reporting.Drawing.Unit.Cm(1.6170835494995117D));
            this.table2.Style.Font.Bold = true;
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.994031548500061D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox14.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Value = "= RowNumber()\r\n";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(5.2401289939880371D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox18.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Value = "=Fields.PlateNo";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(6.8441901206970215D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox20.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.StyleName = "";
            this.textBox20.Value = "=Fields.MaintenanceCost";
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(6.3892436027526855D), Telerik.Reporting.Drawing.Unit.Cm(0.80854177474975586D));
            this.textBox22.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.textBox22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.StyleName = "";
            this.textBox22.Value = "=Fields.SparePartCost";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // HOds
            // 
            this.HOds.DataMember = "GetOrganization";
            this.HOds.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.HOds.Name = "HOds";
            // 
            // MROds
            // 
            this.MROds.DataMember = "GetData";
            this.MROds.DataSource = typeof(ExceedERP.Reporting.Maintenance.DataSource.MaintenanceInternalReportOds);
            this.MROds.Name = "MROds";
            this.MROds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("startDate", typeof(System.DateTime), "= Parameters.startDate.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("endDate", typeof(System.DateTime), "= Parameters.endDate.Value")});
            // 
            // InternalMaintenanceCostDesign
            // 
            this.DataSource = this.HOds;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "InternalMeintenanceCost";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.Name = "startDate";
            reportParameter1.Text = "Start Date";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter1.Value = "=Today()";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "endDate";
            reportParameter2.Text = "End Date";
            reportParameter2.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter2.Value = "=Today()";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8.1999998092651367D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.ObjectDataSource MROds;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox38;
        private Telerik.Reporting.TextBox textBox35;
        private Telerik.Reporting.ObjectDataSource HOds;
    }
}