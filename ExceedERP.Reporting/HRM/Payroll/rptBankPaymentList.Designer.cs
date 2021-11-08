using ExceedERP.Reporting.HRM.Payroll.DataSource;

namespace ExceedERP.Reporting.HRM.Payroll
{
    partial class rptBankPaymentList
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
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.shape1 = new Telerik.Reporting.Shape();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.bankPaymentObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.periodObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.TypeObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BankList = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3822957277297974D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.StyleName = "Civic.TableHeader";
            this.textBox1.Value = "Emp ID";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.StyleName = "Civic.TableHeader";
            this.textBox2.Value = "Emp Full Name";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.StyleName = "Civic.TableHeader";
            this.textBox3.Value = "Bank Account/Signature";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox4.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.StyleName = "Civic.TableHeader";
            this.textBox4.Value = "Net Earning";
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detailSection1.Name = "detailSection1";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3822957277297974D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.62396240234375D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.62396240234375D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.62396240234375D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D)));
            this.table1.Body.SetCellContent(0, 1, this.textBox5);
            this.table1.Body.SetCellContent(0, 2, this.textBox6);
            this.table1.Body.SetCellContent(0, 3, this.textBox7);
            this.table1.Body.SetCellContent(0, 4, this.textBox8);
            this.table1.Body.SetCellContent(1, 4, this.textBox15);
            this.table1.Body.SetCellContent(1, 1, this.textBox11, 1, 3);
            this.table1.Body.SetCellContent(0, 0, this.textBox16);
            this.table1.Body.SetCellContent(1, 0, this.textBox17);
            tableGroup1.Name = "group5";
            tableGroup1.ReportItem = this.textBox13;
            tableGroup2.Name = "group1";
            tableGroup2.ReportItem = this.textBox1;
            tableGroup3.Name = "group2";
            tableGroup3.ReportItem = this.textBox2;
            tableGroup4.Name = "group3";
            tableGroup4.ReportItem = this.textBox3;
            tableGroup5.Name = "group4";
            tableGroup5.ReportItem = this.textBox4;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.DataSource = this.bankPaymentObjectDataSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox5,
            this.textBox6,
            this.textBox7,
            this.textBox8,
            this.textBox11,
            this.textBox15,
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.textBox13,
            this.textBox16,
            this.textBox17});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.table1.Name = "table1";
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup6.Name = "Detail";
            tableGroup7.Name = "group";
            this.table1.RowGroups.Add(tableGroup6);
            this.table1.RowGroups.Add(tableGroup7);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.6541829109191895D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.table1.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.EmpID", Telerik.Reporting.SortDirection.Asc));
            this.table1.StyleName = " Civic.TableNormal";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3822957277297974D), Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D));
            this.textBox5.StyleName = "Office.TableBody";
            this.textBox5.Value = "= Fields.EmpID";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox6.StyleName = "Office.TableBody";
            this.textBox6.Value = "= Fields.EmpFullName";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.StyleName = "Office.TableBody";
            this.textBox7.Value = "= Fields.BankAccount";
            // 
            // textBox8
            // 
            this.textBox8.Format = "{0:N2}";
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.StyleName = "Office.TableBody";
            this.textBox8.Value = "= Fields.NetEarning";
            // 
            // textBox15
            // 
            this.textBox15.Format = "{0:N2}";
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6239621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.StyleName = "Office.TableBody";
            this.textBox15.Value = "=Sum(Fields.NetEarning)";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.6302204132080078D), Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D));
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.StyleName = "Office.TableBody";
            this.textBox11.Value = "Total";
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.9251971244812012D), Telerik.Reporting.Drawing.Unit.Inch(0.14791679382324219D));
            this.textBox10.Name = "ReportPageNumberTextBox";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.Font.Name = "Segoe UI";
            this.textBox10.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.34791693091392517D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox10});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox9});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.7000789642333984D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Value = "Period: {=Parameters.period.Label}       Payment Type:{=Parameters.type.Label}   " +
    "           Branch:{=Parameters.branchid.Label} ";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.1000001430511475D);
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
            this.textBox12,
            this.textBox14,
            this.shape1});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.70015811920166D), Telerik.Reporting.Drawing.Unit.Inch(1.0749605894088745D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3043087720870972D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.3958492279052734D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
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
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.3958492279052734D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.0000786781311035D), Telerik.Reporting.Drawing.Unit.Inch(0.59999990463256836D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.29984268546104431D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Value = "Date:";
            // 
            // textBox12
            // 
            this.textBox12.Format = "{0:d}";
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.5001578330993652D), Telerik.Reporting.Drawing.Unit.Inch(0.59999990463256836D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.27481412887573242D));
            this.textBox12.Value = "= Today()";
            // 
            // textBox14
            // 
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.300118088722229D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6998817920684814D), Telerik.Reporting.Drawing.Unit.Inch(0.30000007152557373D));
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.Font.Name = "Segoe UI";
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox14.Value = "Report for Payment List";
            // 
            // shape1
            // 
            this.shape1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.shape1.Name = "shape1";
            this.shape1.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.5D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape1.Stretch = true;
            this.shape1.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape1.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape1.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape1.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape1.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox13.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.StyleName = "Civic.TableHeader";
            this.textBox13.Value = "S.N";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D), Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D));
            this.textBox16.StyleName = "Office.TableBody";
            this.textBox16.Value = "= RowNumber(\"table1\")";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D), Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D));
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.StyleName = "Office.TableBody";
            // 
            // bankPaymentObjectDataSource
            // 
            this.bankPaymentObjectDataSource.DataMember = "GetPayrollBankSlipLists";
            this.bankPaymentObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.Payroll.DataSource.BankPaymentListObjectDataSource);
            this.bankPaymentObjectDataSource.Name = "bankPaymentObjectDataSource";
            this.bankPaymentObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("period", typeof(string), "= Parameters.period.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("type", typeof(string), "= Parameters.type.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("user", typeof(string), "= Parameters.user.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("bank", typeof(string), "= Parameters.bank.Value")});
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
            // TypeObjectDataSource
            // 
            this.TypeObjectDataSource.DataMember = "GetPaymentType";
            this.TypeObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.PaymentTypeObjectDataSource);
            this.TypeObjectDataSource.Name = "TypeObjectDataSource";
            // 
            // BankList
            // 
            this.BankList.DataMember = "GetBankList";
            this.BankList.DataSource = typeof(ExceedERP.Reporting.HRM.Payroll.DataSource.BankOds);
            this.BankList.Name = "BankList";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // rptBankPaymentList
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "rptBankPaymentList";
            this.PageSettings.Landscape = false;
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
            reportParameter3.AvailableValues.DataSource = this.TypeObjectDataSource;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter3.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter3.Name = "type";
            reportParameter3.Text = "Payment Type";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.AutoRefresh = true;
            reportParameter4.Name = "user";
            reportParameter5.AllowBlank = false;
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DataSource = this.BankList;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter5.AvailableValues.ValueMember = "= Fields.Code";
            reportParameter5.Name = "bank";
            reportParameter5.Text = "Bank";
            reportParameter5.Visible = true;
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
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), " Civic.TableNormal")});
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(77)))));
            styleRule2.Style.Font.Name = "Calibri";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Office.TableBody")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Font.Name = "Calibri";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableHeader")});
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule4.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(167)))), ((int)(((byte)(227)))));
            styleRule4.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule4.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(77)))));
            styleRule4.Style.Font.Name = "Calibri";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.700157642364502D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.ObjectDataSource bankPaymentObjectDataSource;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.ObjectDataSource periodObjectDataSource;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.ObjectDataSource TypeObjectDataSource;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.Shape shape1;
        private Telerik.Reporting.ObjectDataSource BankList;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox13;
    }
}