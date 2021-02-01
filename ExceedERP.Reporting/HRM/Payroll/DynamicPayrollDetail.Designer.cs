using ExceedERP.Reporting.HRM.HRM;
using ExceedERP.Reporting.HRM.Payroll.DataSource;

//using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.Payroll
{
    partial class DynamicPayrollDetail
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
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.periodObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.CostCneter = new Telerik.Reporting.ObjectDataSource();
            this.TypeObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.shape1 = new Telerik.Reporting.Shape();
            this.detail = new Telerik.Reporting.DetailSection();
            this.crosstab2 = new Telerik.Reporting.Crosstab();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.DynamicobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            this.textBox61 = new Telerik.Reporting.TextBox();
            this.textBox60 = new Telerik.Reporting.TextBox();
            this.textBox59 = new Telerik.Reporting.TextBox();
            this.textBox58 = new Telerik.Reporting.TextBox();
            this.textBox57 = new Telerik.Reporting.TextBox();
            this.textBox56 = new Telerik.Reporting.TextBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.textBox54 = new Telerik.Reporting.TextBox();
            this.textBox53 = new Telerik.Reporting.TextBox();
            this.textBox52 = new Telerik.Reporting.TextBox();
            this.textBox51 = new Telerik.Reporting.TextBox();
            this.textBox50 = new Telerik.Reporting.TextBox();
            this.textBox49 = new Telerik.Reporting.TextBox();
            this.textBox48 = new Telerik.Reporting.TextBox();
            this.textBox47 = new Telerik.Reporting.TextBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox45 = new Telerik.Reporting.TextBox();
            this.textBox44 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.textBox42 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox36 = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5395820140838623D), Telerik.Reporting.Drawing.Unit.Inch(0.27500009536743164D));
            this.textBox5.Style.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.Color = System.Drawing.SystemColors.ActiveCaptionText;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Italic = true;
            this.textBox5.Style.Font.Name = "Georgia";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox5.StyleName = "Office.TableGroup";
            this.textBox5.Value = "= Fields.columnName";
            // 
            // textBox9
            // 
            this.textBox9.CanGrow = true;
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.83124983310699463D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.Font.Name = "Georgia";
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox9.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox9.StyleName = "Office.SubTotal";
            this.textBox9.Value = "=Fields.rowNumber";
            // 
            // textBox2
            // 
            this.textBox2.CanGrow = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.83124983310699463D), Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Name = "Georgia";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox2.StyleName = "Office.SubTotal";
            this.textBox2.Value = "Grand Total";
            // 
            // periodObjectDataSource
            // 
            this.periodObjectDataSource.DataMember = "GetPayrollPeriods";
            this.periodObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.Payroll.DataSource.PayrollPeriodsObjectDataSource);
            this.periodObjectDataSource.Name = "periodObjectDataSource";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.UserBranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
            this.BranchObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("user", typeof(string), "= Parameters.user.Value")});
            // 
            // CostCneter
            // 
            this.CostCneter.DataMember = "GetPayrollDepartment";
            this.CostCneter.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.PayrollDepartmentObjectDataSource);
            this.CostCneter.Name = "CostCneter";
            // 
            // TypeObjectDataSource
            // 
            this.TypeObjectDataSource.DataMember = "GetPaymentType";
            this.TypeObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.PaymentTypeObjectDataSource);
            this.TypeObjectDataSource.Name = "TypeObjectDataSource";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25,
            this.textBox21,
            this.textBox10,
            this.textBox39,
            this.shape1});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.600001335144043D), Telerik.Reporting.Drawing.Unit.Inch(1.0749605894088745D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3043087720870972D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.7954921722412109D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3043085336685181D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.7954931259155273D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(9.0000791549682617D), Telerik.Reporting.Drawing.Unit.Inch(0.59999990463256836D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.29984268546104431D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Value = "Date:";
            // 
            // textBox10
            // 
            this.textBox10.Format = "{0:d}";
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(9.5001583099365234D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.299842119216919D), Telerik.Reporting.Drawing.Unit.Inch(0.32502856850624084D));
            this.textBox10.Value = "= Today()";
            // 
            // textBox39
            // 
            this.textBox39.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.300118088722229D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.6998820304870605D), Telerik.Reporting.Drawing.Unit.Inch(0.30000007152557373D));
            this.textBox39.Style.Font.Bold = true;
            this.textBox39.Style.Font.Name = "Segoe UI";
            this.textBox39.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox39.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox39.Value = "Payroll Detail";
            // 
            // shape1
            // 
            this.shape1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.shape1.Name = "shape1";
            this.shape1.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.600000381469727D), Telerik.Reporting.Drawing.Unit.Point(5.3971595764160156D));
            this.shape1.Stretch = true;
            this.shape1.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape1.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape1.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape1.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape1.Style.Color = System.Drawing.Color.Transparent;
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.6958727240562439D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.crosstab2});
            this.detail.Name = "detail";
            // 
            // crosstab2
            // 
            this.crosstab2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5395822525024414D)));
            this.crosstab2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.crosstab2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.crosstab2.Body.SetCellContent(0, 0, this.textBox13);
            this.crosstab2.Body.SetCellContent(1, 0, this.textBox3);
            tableGroup1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.columnName"));
            tableGroup1.Name = "CompName1";
            tableGroup1.ReportItem = this.textBox5;
            this.crosstab2.ColumnGroups.Add(tableGroup1);
            this.crosstab2.ColumnHeadersPrintOnEveryPage = true;
            this.crosstab2.Corner.SetCellContent(0, 0, this.textBox11);
            this.crosstab2.DataSource = this.DynamicobjectDataSource;
            this.crosstab2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox11,
            this.textBox13,
            this.textBox3,
            this.textBox5,
            this.textBox9,
            this.textBox2});
            this.crosstab2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.crosstab2.Name = "crosstab2";
            tableGroup3.ReportItem = this.textBox9;
            tableGroup2.ChildGroups.Add(tableGroup3);
            tableGroup2.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.rowNumber"));
            tableGroup2.Name = "Name1";
            tableGroup2.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.rowNumber", Telerik.Reporting.SortDirection.Asc));
            tableGroup5.Name = "group1";
            tableGroup5.ReportItem = this.textBox2;
            tableGroup4.ChildGroups.Add(tableGroup5);
            tableGroup4.Name = "group";
            this.crosstab2.RowGroups.Add(tableGroup2);
            this.crosstab2.RowGroups.Add(tableGroup4);
            this.crosstab2.RowHeadersPrintOnEveryPage = true;
            this.crosstab2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.3708319664001465D), Telerik.Reporting.Drawing.Unit.Inch(0.67500013113021851D));
            this.crosstab2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.crosstab2.StyleName = "Office.TableNormal";
            // 
            // textBox13
            // 
            this.textBox13.Format = "{0:N2}";
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5395820140838623D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.Font.Name = "Georgia";
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox13.StyleName = "Office.TableBody";
            this.textBox13.Value = "= Fields.cellContent";
            // 
            // textBox3
            // 
            this.textBox3.Format = "{0:N2}";
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5395820140838623D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.Font.Name = "Georgia";
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox3.StyleName = "Office.TableBody";
            this.textBox3.Value = "= Sum(Fields.values)";
            // 
            // textBox11
            // 
            this.textBox11.CanGrow = true;
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8312496542930603D), Telerik.Reporting.Drawing.Unit.Inch(0.27500009536743164D));
            this.textBox11.Style.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.Color = System.Drawing.SystemColors.ActiveCaptionText;
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Name = "Georgia";
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox11.StyleName = "Office.TableCorner";
            this.textBox11.Value = "No";
            // 
            // DynamicobjectDataSource
            // 
            this.DynamicobjectDataSource.DataMember = "GetAdvance";
            this.DynamicobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.Payroll.DataSource.DynamicColumnObjectDataSource);
            this.DynamicobjectDataSource.Name = "DynamicobjectDataSource";
            this.DynamicobjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("period", typeof(string), "= Parameters.period.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("workunit", typeof(string), "= Parameters.workunit.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("payment", typeof(string), "= Parameters.payment.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("user", typeof(string), "= Parameters.user.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.44583332538604736D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(10.100000381469727D), Telerik.Reporting.Drawing.Unit.Inch(0.1458333283662796D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4081364870071411D), Telerik.Reporting.Drawing.Unit.Inch(0.19996006786823273D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Name = "Segoe UI";
            this.textBox1.Value = "Page: {PageNumber}";
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.4582939147949219D);
            this.reportFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox61,
            this.textBox60,
            this.textBox59,
            this.textBox58,
            this.textBox57,
            this.textBox56,
            this.textBox55,
            this.textBox54,
            this.textBox53,
            this.textBox52,
            this.textBox51,
            this.textBox50,
            this.textBox49,
            this.textBox48,
            this.textBox47,
            this.textBox46,
            this.textBox45,
            this.textBox44,
            this.textBox43,
            this.textBox42,
            this.textBox4,
            this.textBox35,
            this.textBox12,
            this.textBox36});
            this.reportFooterSection1.Name = "reportFooterSection1";
            // 
            // textBox61
            // 
            this.textBox61.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1354169845581055D), Telerik.Reporting.Drawing.Unit.Inch(1.1582940816879273D));
            this.textBox61.Name = "textBox61";
            this.textBox61.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.16658751666545868D));
            this.textBox61.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox61.Value = "__________________________";
            // 
            // textBox60
            // 
            this.textBox60.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1354169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.95829391479492188D));
            this.textBox60.Name = "textBox60";
            this.textBox60.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox60.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox60.Value = "Date";
            // 
            // textBox59
            // 
            this.textBox59.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1354169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.55829399824142456D));
            this.textBox59.Name = "textBox59";
            this.textBox59.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox59.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox59.Value = "Sign";
            // 
            // textBox58
            // 
            this.textBox58.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1354169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.75829380750656128D));
            this.textBox58.Name = "textBox58";
            this.textBox58.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox58.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox58.Value = "__________________________";
            // 
            // textBox57
            // 
            this.textBox57.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1354169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.35829386115074158D));
            this.textBox57.Name = "textBox57";
            this.textBox57.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.15857028961181641D));
            this.textBox57.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox57.Value = "__________________________";
            // 
            // textBox56
            // 
            this.textBox56.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1354169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.textBox56.Name = "textBox56";
            this.textBox56.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox56.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox56.Value = "Paid by";
            // 
            // textBox55
            // 
            this.textBox55.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4375D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox55.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox55.Value = "Approved by";
            // 
            // textBox54
            // 
            this.textBox54.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4375D), Telerik.Reporting.Drawing.Unit.Inch(0.35829386115074158D));
            this.textBox54.Name = "textBox54";
            this.textBox54.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.15857028961181641D));
            this.textBox54.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox54.Value = "__________________________";
            // 
            // textBox53
            // 
            this.textBox53.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4375D), Telerik.Reporting.Drawing.Unit.Inch(0.75829380750656128D));
            this.textBox53.Name = "textBox53";
            this.textBox53.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19992096722126007D));
            this.textBox53.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox53.Value = "__________________________";
            // 
            // textBox52
            // 
            this.textBox52.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4375D), Telerik.Reporting.Drawing.Unit.Inch(0.55829399824142456D));
            this.textBox52.Name = "textBox52";
            this.textBox52.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox52.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox52.Value = "Sign";
            // 
            // textBox51
            // 
            this.textBox51.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4375D), Telerik.Reporting.Drawing.Unit.Inch(0.95829391479492188D));
            this.textBox51.Name = "textBox51";
            this.textBox51.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox51.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox51.Value = "Date";
            // 
            // textBox50
            // 
            this.textBox50.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4375D), Telerik.Reporting.Drawing.Unit.Inch(1.1582940816879273D));
            this.textBox50.Name = "textBox50";
            this.textBox50.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox50.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox50.Value = "__________________________";
            // 
            // textBox49
            // 
            this.textBox49.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7291667461395264D), Telerik.Reporting.Drawing.Unit.Inch(0.058293979614973068D));
            this.textBox49.Name = "textBox49";
            this.textBox49.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox49.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox49.Value = "Checked by";
            // 
            // textBox48
            // 
            this.textBox48.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7291667461395264D), Telerik.Reporting.Drawing.Unit.Inch(0.35829386115074158D));
            this.textBox48.Name = "textBox48";
            this.textBox48.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.15857028961181641D));
            this.textBox48.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox48.Value = "__________________________";
            // 
            // textBox47
            // 
            this.textBox47.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7291667461395264D), Telerik.Reporting.Drawing.Unit.Inch(0.75829380750656128D));
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox47.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox47.Value = "__________________________";
            // 
            // textBox46
            // 
            this.textBox46.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7291667461395264D), Telerik.Reporting.Drawing.Unit.Inch(0.55829399824142456D));
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox46.Value = "Sign";
            // 
            // textBox45
            // 
            this.textBox45.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7291667461395264D), Telerik.Reporting.Drawing.Unit.Inch(0.95829391479492188D));
            this.textBox45.Name = "textBox45";
            this.textBox45.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox45.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox45.Value = "Date";
            // 
            // textBox44
            // 
            this.textBox44.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.7291667461395264D), Telerik.Reporting.Drawing.Unit.Inch(1.1582940816879273D));
            this.textBox44.Name = "textBox44";
            this.textBox44.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.16658751666545868D));
            this.textBox44.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox44.Value = "__________________________";
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.3333333432674408D), Telerik.Reporting.Drawing.Unit.Inch(1.1582940816879273D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.16658751666545868D));
            this.textBox43.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox43.Value = "__________________________";
            // 
            // textBox42
            // 
            this.textBox42.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.3333333432674408D), Telerik.Reporting.Drawing.Unit.Inch(0.95829391479492188D));
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox42.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox42.Value = "Date";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.3333333432674408D), Telerik.Reporting.Drawing.Unit.Inch(0.058293979614973068D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox4.Value = "Prepared by";
            // 
            // textBox35
            // 
            this.textBox35.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.3333333432674408D), Telerik.Reporting.Drawing.Unit.Inch(0.75829380750656128D));
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox35.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox35.Value = "__________________________";
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.3333333432674408D), Telerik.Reporting.Drawing.Unit.Inch(0.35829386115074158D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.15857028961181641D));
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox12.Value = "__________________________";
            // 
            // textBox36
            // 
            this.textBox36.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.3333333432674408D), Telerik.Reporting.Drawing.Unit.Inch(0.55829399824142456D));
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox36.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox36.Value = "Sign";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox19});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // textBox19
            // 
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.599961280822754D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.Value = "Period: {=Parameters.period.Label}          Branch: {=Parameters.branchId.Label} " +
    "      Work Unit: {=Parameters.workunit.Label}        Payment Type: {=Parameters." +
    "payment.Label}";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // DynamicPayrollDetail
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportFooterSection1,
            this.reportHeaderSection1});
            this.Name = "DynamicPayrollDetail";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.periodObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter1.Name = "period";
            reportParameter1.Text = "Period";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter2.Name = "branchId";
            reportParameter2.Text = "Branch";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AutoRefresh = true;
            reportParameter3.AvailableValues.DataSource = this.CostCneter;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Code+\"-\"+Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.Code";
            reportParameter3.Name = "workunit";
            reportParameter3.Text = "Work Unit";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.AutoRefresh = true;
            reportParameter4.AvailableValues.DataSource = this.TypeObjectDataSource;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter4.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter4.Name = "payment";
            reportParameter4.Text = "Payment Type";
            reportParameter4.Visible = true;
            reportParameter5.AllowNull = true;
            reportParameter5.AutoRefresh = true;
            reportParameter5.Name = "user";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            this.ReportParameters.Add(reportParameter5);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.600001335144043D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.ObjectDataSource DynamicobjectDataSource;
        private Telerik.Reporting.Crosstab crosstab2;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.ObjectDataSource periodObjectDataSource;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource CostCneter;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.ObjectDataSource TypeObjectDataSource;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
        private Telerik.Reporting.TextBox textBox61;
        private Telerik.Reporting.TextBox textBox60;
        private Telerik.Reporting.TextBox textBox59;
        private Telerik.Reporting.TextBox textBox58;
        private Telerik.Reporting.TextBox textBox57;
        private Telerik.Reporting.TextBox textBox56;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.TextBox textBox54;
        private Telerik.Reporting.TextBox textBox53;
        private Telerik.Reporting.TextBox textBox52;
        private Telerik.Reporting.TextBox textBox51;
        private Telerik.Reporting.TextBox textBox50;
        private Telerik.Reporting.TextBox textBox49;
        private Telerik.Reporting.TextBox textBox48;
        private Telerik.Reporting.TextBox textBox47;
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.TextBox textBox45;
        private Telerik.Reporting.TextBox textBox44;
        private Telerik.Reporting.TextBox textBox43;
        private Telerik.Reporting.TextBox textBox42;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox35;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox36;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.Shape shape1;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.TextBox textBox19;
    }
}