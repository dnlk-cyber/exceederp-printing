using ExceedERP.Reporting.Finance.DataSource.Bank;

namespace ExceedERP.Reporting.Finance
{
    partial class rptBankJournal
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.SortingAction sortingAction1 = new Telerik.Reporting.SortingAction();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.SortingAction sortingAction2 = new Telerik.Reporting.SortingAction();
            Telerik.Reporting.SortingAction sortingAction3 = new Telerik.Reporting.SortingAction();
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
            this.table1 = new Telerik.Reporting.Table();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox32 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox26 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox36 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox37 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.textBox38 = new Telerik.Reporting.TextBox();
            this.textBox41 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.shape3 = new Telerik.Reporting.Shape();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.textBox45 = new Telerik.Reporting.TextBox();
            this.rptBankJournalods = new Telerik.Reporting.ObjectDataSource();
            this.fiscalYeardDs = new Telerik.Reporting.ObjectDataSource();
            this.periodDS = new Telerik.Reporting.ObjectDataSource();
            this.bankAccountOds = new Telerik.Reporting.ObjectDataSource();
            this.reconciliationStatus = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox1
            // 
            sortingAction1.SortingExpression = "= Fields.Date";
            sortingAction1.Targets.AddRange(new Telerik.Reporting.IActionTarget[] {
            this.table1});
            this.textBox1.Action = sortingAction1;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.68095284700393677D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox1.StyleName = "Civic.TableHeader";
            this.textBox1.Value = "Date";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.680952787399292D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.74642914533615112D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.89880979061126709D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.66309595108032227D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.1645839214324951D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.72261965274810791D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.90342265367507935D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox11);
            this.table1.Body.SetCellContent(0, 1, this.textBox12);
            this.table1.Body.SetCellContent(0, 3, this.textBox13);
            this.table1.Body.SetCellContent(0, 4, this.textBox14);
            this.table1.Body.SetCellContent(0, 5, this.textBox15);
            this.table1.Body.SetCellContent(0, 6, this.textBox16);
            this.table1.Body.SetCellContent(0, 7, this.textBox17);
            this.table1.Body.SetCellContent(0, 8, this.textBox18);
            this.table1.Body.SetCellContent(0, 9, this.textBox20);
            this.table1.Body.SetCellContent(1, 7, this.textBox30);
            this.table1.Body.SetCellContent(1, 8, this.textBox31);
            this.table1.Body.SetCellContent(1, 9, this.textBox32);
            this.table1.Body.SetCellContent(1, 0, this.textBox9, 1, 7);
            this.table1.Body.SetCellContent(0, 2, this.textBox26);
            this.table1.Body.SetCellContent(2, 9, this.textBox45);
            this.table1.Body.SetCellContent(2, 7, this.textBox43, 1, 2);
            this.table1.Body.SetCellContent(2, 0, this.textBox27, 1, 7);
            tableGroup1.Name = "date";
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.Name = "reference";
            tableGroup2.ReportItem = this.textBox2;
            tableGroup3.Name = "group1";
            tableGroup3.ReportItem = this.textBox19;
            tableGroup4.Name = "source";
            tableGroup4.ReportItem = this.textBox3;
            tableGroup5.Name = "sourceId";
            tableGroup5.ReportItem = this.textBox4;
            tableGroup6.Name = "transactDesc";
            tableGroup6.ReportItem = this.textBox5;
            tableGroup7.Name = "type";
            tableGroup7.ReportItem = this.textBox6;
            tableGroup8.Name = "debit";
            tableGroup8.ReportItem = this.textBox7;
            tableGroup9.Name = "credit";
            tableGroup9.ReportItem = this.textBox8;
            tableGroup10.Name = "reconciliationStatus";
            tableGroup10.ReportItem = this.textBox10;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.ColumnGroups.Add(tableGroup7);
            this.table1.ColumnGroups.Add(tableGroup8);
            this.table1.ColumnGroups.Add(tableGroup9);
            this.table1.ColumnGroups.Add(tableGroup10);
            this.table1.ColumnHeadersPrintOnEveryPage = true;
            this.table1.DataSource = this.rptBankJournalods;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox11,
            this.textBox12,
            this.textBox26,
            this.textBox13,
            this.textBox14,
            this.textBox15,
            this.textBox16,
            this.textBox17,
            this.textBox18,
            this.textBox20,
            this.textBox9,
            this.textBox30,
            this.textBox31,
            this.textBox32,
            this.textBox27,
            this.textBox2,
            this.textBox19,
            this.textBox3,
            this.textBox4,
            this.textBox5,
            this.textBox6,
            this.textBox7,
            this.textBox43,
            this.textBox8,
            this.textBox10,
            this.textBox1,
            this.textBox45});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.table1.Name = "table1";
            tableGroup11.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup11.Name = "detail";
            tableGroup12.Name = "group";
            tableGroup13.Name = "group2";
            this.table1.RowGroups.Add(tableGroup11);
            this.table1.RowGroups.Add(tableGroup12);
            this.table1.RowGroups.Add(tableGroup13);
            this.table1.RowHeadersPrintOnEveryPage = true;
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1799154281616211D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.table1.StyleName = "Civic.TableNormal";
            // 
            // textBox11
            // 
            this.textBox11.Format = "{0:d}";
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.68095284700393677D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox11.StyleName = "Civic.TableBody";
            this.textBox11.Value = "= Fields.Date";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.74642902612686157D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox12.StyleName = "Civic.TableBody";
            this.textBox12.Value = "= Fields.Reference";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox13.StyleName = "Civic.TableBody";
            this.textBox13.Value = "= Fields.Source";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66309571266174316D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox14.StyleName = "Civic.TableBody";
            this.textBox14.Value = "= Fields.SourceId";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.164583683013916D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox15.StyleName = "Civic.TableBody";
            this.textBox15.Value = "= Fields.TransactDesc";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.72261953353881836D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox16.StyleName = "Civic.TableBody";
            this.textBox16.Value = "= Fields.Type";
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:N2}";
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox17.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox17.StyleName = "Civic.TableBody";
            this.textBox17.Value = "= Fields.Debit";
            // 
            // textBox18
            // 
            this.textBox18.Format = "{0:N2}";
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox18.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox18.StyleName = "Civic.TableBody";
            this.textBox18.Value = "= Fields.Credit";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90342289209365845D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox20.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox20.StyleName = "Civic.TableBody";
            this.textBox20.Value = "= Fields.ReconciliationStatus";
            // 
            // textBox30
            // 
            this.textBox30.Format = "{0:N2}";
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80000042915344238D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox30.Style.Font.Bold = true;
            this.textBox30.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox30.StyleName = "Civic.TableBody";
            this.textBox30.Value = "=sum(Fields.Debit)";
            // 
            // textBox31
            // 
            this.textBox31.Format = "{0:N2}";
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80000042915344238D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox31.Style.Font.Bold = true;
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox31.StyleName = "Civic.TableBody";
            this.textBox31.Value = "=sum(Fields.Credit)";
            // 
            // textBox32
            // 
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90342289209365845D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox32.Style.Font.Bold = true;
            this.textBox32.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox32.StyleName = "Civic.TableBody";
            this.textBox32.Value = "";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.6764912605285645D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox9.StyleName = "Civic.TableBody";
            this.textBox9.Value = "Total";
            // 
            // textBox26
            // 
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89880967140197754D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox26.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox26.StyleName = "Civic.TableBody";
            this.textBox26.Value = "=Fields.ReferenceDoc";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.74642902612686157D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox2.StyleName = "Civic.TableHeader";
            this.textBox2.Value = "Reference";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89880967140197754D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox19.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox19.StyleName = "Civic.TableHeader";
            this.textBox19.Value = "Reference Doc";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox3.StyleName = "Civic.TableHeader";
            this.textBox3.Value = "Source";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66309571266174316D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox4.StyleName = "Civic.TableHeader";
            this.textBox4.Value = "Source Id";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.164583683013916D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox5.StyleName = "Civic.TableHeader";
            this.textBox5.Value = "Transact Desc";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.72261953353881836D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox6.StyleName = "Civic.TableHeader";
            this.textBox6.Value = "Type";
            // 
            // textBox7
            // 
            sortingAction2.SortingExpression = "= Fields.Debit";
            sortingAction2.Targets.AddRange(new Telerik.Reporting.IActionTarget[] {
            this.table1});
            this.textBox7.Action = sortingAction2;
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox7.StyleName = "Civic.TableHeader";
            this.textBox7.Value = "Debit";
            // 
            // textBox8
            // 
            sortingAction3.SortingExpression = "= Fields.Credit";
            sortingAction3.Targets.AddRange(new Telerik.Reporting.IActionTarget[] {
            this.table1});
            this.textBox8.Action = sortingAction3;
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox8.StyleName = "Civic.TableHeader";
            this.textBox8.Value = "Credit";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90342289209365845D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox10.StyleName = "Civic.TableHeader";
            this.textBox10.Value = "Reconciliation Status";
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.80000007152557373D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detailSection1.Name = "detailSection1";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.1000001430511475D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox25,
            this.textBox23,
            this.textBox36,
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox21,
            this.textBox37,
            this.textBox24,
            this.textBox39,
            this.textBox38,
            this.textBox41,
            this.textBox40,
            this.shape3});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox25
            // 
            this.textBox25.Format = "{0:d}";
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.3000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(0.78401416540145874D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox25.Value = "= Today()";
            // 
            // textBox23
            // 
            this.textBox23.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.8000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(0.78401416540145874D));
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox23.Style.Font.Bold = true;
            this.textBox23.Value = "Date:";
            // 
            // textBox36
            // 
            this.textBox36.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D), Telerik.Reporting.Drawing.Unit.Inch(0.69992130994796753D));
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0104160308837891D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox36.Style.Font.Bold = true;
            this.textBox36.Style.Font.Name = "Segoe UI";
            this.textBox36.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox36.Value = "BANK JOURNAL";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D), Telerik.Reporting.Drawing.Unit.Inch(0.0020046234130859375D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4500012397766113D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Name = "Microsoft Sans Serif";
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4104166030883789D), Telerik.Reporting.Drawing.Unit.Inch(0.28325462341308594D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4395856857299805D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox21.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox21.TextWrap = false;
            this.textBox21.Value = "= Fields.Name";
            // 
            // textBox37
            // 
            this.textBox37.Format = "{0}";
            this.textBox37.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.030208587646484375D), Telerik.Reporting.Drawing.Unit.Inch(1.5D));
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.173958420753479D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox37.Style.Font.Bold = true;
            this.textBox37.Style.Font.Name = "Segoe UI";
            this.textBox37.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox37.Value = "Bank Account";
            // 
            // textBox24
            // 
            this.textBox24.Format = "{0:d}";
            this.textBox24.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(1.5D));
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox24.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox24.Style.Font.Bold = false;
            this.textBox24.Style.Font.Name = "Segoe UI";
            this.textBox24.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox24.Style.Font.Underline = true;
            this.textBox24.Value = "=Parameters.bankaccount.Value +\" \"+ Parameters.bankaccount.Label";
            // 
            // textBox39
            // 
            this.textBox39.Format = "{0}";
            this.textBox39.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.364585280418396D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.104167103767395D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox39.Style.Font.Bold = true;
            this.textBox39.Style.Font.Name = "Segoe UI";
            this.textBox39.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox39.Value = "Period From";
            // 
            // textBox38
            // 
            this.textBox38.Format = "{0:d}";
            this.textBox38.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4687519073486328D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3957537412643433D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox38.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox38.Style.Font.Bold = false;
            this.textBox38.Style.Font.Name = "Segoe UI";
            this.textBox38.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox38.Style.Font.Underline = true;
            this.textBox38.Value = "= Parameters.fromperiod.Label";
            // 
            // textBox41
            // 
            this.textBox41.Format = "{0}";
            this.textBox41.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8645851612091064D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.50416606664657593D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox41.Style.Font.Bold = true;
            this.textBox41.Style.Font.Name = "Segoe UI";
            this.textBox41.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox41.Value = "To";
            // 
            // textBox40
            // 
            this.textBox40.Format = "{0:d}";
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.3645853996276855D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3957537412643433D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox40.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox40.Style.Font.Bold = false;
            this.textBox40.Style.Font.Name = "Segoe UI";
            this.textBox40.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox40.Style.Font.Underline = true;
            this.textBox40.Value = "= Parameters.toperiod.Label";
            // 
            // shape3
            // 
            this.shape3.Anchoring = ((Telerik.Reporting.AnchoringStyles)((Telerik.Reporting.AnchoringStyles.Left | Telerik.Reporting.AnchoringStyles.Right)));
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.9000000953674316D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1999607086181641D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox22
            // 
            this.textBox22.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.6251578330993652D), Telerik.Reporting.Drawing.Unit.Inch(0.0999995619058609D));
            this.textBox22.Name = "ReportPageNumberTextBox";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19996088743209839D));
            this.textBox22.Style.Font.Name = "Segoe UI";
            this.textBox22.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.39999929070472717D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox22});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox27
            // 
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.6764917373657227D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox27.Style.Font.Bold = true;
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox27.StyleName = "Civic.TableBody";
            this.textBox27.Value = "Difference";
            // 
            // textBox43
            // 
            this.textBox43.Format = "{0:N2}";
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000009775161743D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox43.Style.Font.Bold = true;
            this.textBox43.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox43.StyleName = "Civic.TableBody";
            this.textBox43.Value = "=sum(Fields.Debit)-sum(Fields.Credit)";
            // 
            // textBox45
            // 
            this.textBox45.Name = "textBox45";
            this.textBox45.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90342265367507935D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox45.Style.Font.Bold = true;
            this.textBox45.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox45.StyleName = "Civic.TableBody";
            // 
            // rptBankJournalods
            // 
            this.rptBankJournalods.DataMember = "GetBankJournal";
            this.rptBankJournalods.DataSource = typeof(BankJournalOds);
            this.rptBankJournalods.Name = "rptBankJournalods";
            this.rptBankJournalods.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("fromperiod", typeof(string), "= Parameters.fromperiod.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("toperiod", typeof(string), "= Parameters.fromperiod.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("bankaccount", typeof(string), "= Parameters.bankaccount.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("status", typeof(string), "= Parameters.status.Value")});
            // 
            // fiscalYeardDs
            // 
            this.fiscalYeardDs.DataMember = "GetFiscalYearss";
            this.fiscalYeardDs.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.FiscalYearDataSource);
            this.fiscalYeardDs.Name = "fiscalYeardDs";
            // 
            // periodDS
            // 
            this.periodDS.DataMember = "GetPeriods";
            this.periodDS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.PeriodDataSource);
            this.periodDS.Name = "periodDS";
            // 
            // bankAccountOds
            // 
            this.bankAccountOds.DataMember = "GetBankAccountList";
            this.bankAccountOds.DataSource = typeof(BankOds);
            this.bankAccountOds.Name = "bankAccountOds";
            // 
            // reconciliationStatus
            // 
            this.reconciliationStatus.DataMember = "BankReconciliationStatus";
            this.reconciliationStatus.DataSource = typeof(ExceedERP.Reporting.Finance.DataSource.CommonOds);
            this.reconciliationStatus.Name = "reconciliationStatus";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetBankJournalHeader";
            this.Organization.DataSource = typeof(BankJournalOds);
            this.Organization.Name = "Organization";
            this.Organization.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("fromperiod", typeof(string), "= Parameters.fromperiod.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("toperiod", typeof(string), "= Parameters.toperiod.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("bankaccount", typeof(string), "= Parameters.bankaccount.Value")});
            // 
            // rptBankJournal
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "rptBankJournal";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowBlank = false;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.fiscalYeardDs;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
            reportParameter1.Name = "fiscalyear";
            reportParameter1.Text = "Fiscal Year";
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            reportParameter2.AllowBlank = false;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.periodDS;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.Filters.Add(new Telerik.Reporting.Filter("= Fields.GlFiscalYearId", Telerik.Reporting.FilterOperator.Equal, "= Parameters.fiscalyear.Value"));
            reportParameter2.AvailableValues.ValueMember = "= Fields.GLPeriodId";
            reportParameter2.Name = "fromperiod";
            reportParameter2.Text = "From Period";
            reportParameter2.Visible = true;
            reportParameter3.AllowBlank = false;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.periodDS;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.Filters.Add(new Telerik.Reporting.Filter("= Fields.GlFiscalYearId", Telerik.Reporting.FilterOperator.Equal, "= Parameters.fiscalyear.Value"));
            reportParameter3.AvailableValues.ValueMember = "= Fields.GLPeriodId";
            reportParameter3.Name = "toperiod";
            reportParameter3.Text = "To Period";
            reportParameter3.Visible = true;
            reportParameter4.AllowBlank = false;
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.bankAccountOds;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.AccountName";
            reportParameter4.AvailableValues.ValueMember = "= Fields.AccountNumber";
            reportParameter4.Name = "bankaccount";
            reportParameter4.Text = "Bank Account";
            reportParameter4.Visible = true;
            reportParameter5.AllowBlank = false;
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DataSource = this.reconciliationStatus;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter5.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter5.Name = "status";
            reportParameter5.Text = "Status";
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
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), "Civic.TableNormal")});
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Georgia";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableBody")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Font.Name = "Georgia";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableHeader")});
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule4.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            styleRule4.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule4.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(238)))), ((int)(((byte)(243)))));
            styleRule4.Style.Font.Name = "Georgia";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8.1999998092651367D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.ObjectDataSource rptBankJournalods;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource periodDS;
        private Telerik.Reporting.ObjectDataSource fiscalYeardDs;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox36;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox37;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.TextBox textBox38;
        private Telerik.Reporting.TextBox textBox41;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.ObjectDataSource bankAccountOds;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.TextBox textBox32;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.ObjectDataSource reconciliationStatus;
        private Telerik.Reporting.TextBox textBox26;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox45;
        private Telerik.Reporting.TextBox textBox43;
    }
}