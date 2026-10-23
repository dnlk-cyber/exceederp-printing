namespace ExceedERP.Reporting.Budget.View
{
    partial class BudgetUsageAgainstMonth
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule5 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.textBox54 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.reportFooter = new Telerik.Reporting.ReportFooterSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox28 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox26 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.MonthlyUsageDS = new Telerik.Reporting.ObjectDataSource();
            this.BudgetTypeDS = new Telerik.Reporting.ObjectDataSource();
            this.PlanCategoryDS = new Telerik.Reporting.ObjectDataSource();
            this.YearDS = new Telerik.Reporting.ObjectDataSource();
            this.BranchDS = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitDS = new Telerik.Reporting.ObjectDataSource();
            this.headerComp = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Cm(0.13229164481163025D);
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Style.Visible = false;
            // 
            // reportHeader
            // 
            this.reportHeader.Height = Telerik.Reporting.Drawing.Unit.Cm(3.0479996204376221D);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox31,
            this.textBox18,
            this.pbOrgLogo,
            this.textBox55,
            this.textBox54,
            this.textBox17});
            this.reportHeader.Name = "reportHeader";
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.8062500953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox31.Style.Font.Bold = true;
            this.textBox31.Value = "Date:";
            // 
            // textBox18
            // 
            this.textBox18.Format = "{0:d}";
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox18.Value = "= Today()";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9354959881165996E-05D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox55
            // 
            this.textBox55.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.0083334604278206825D));
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19996064901351929D));
            this.textBox55.Style.Font.Bold = true;
            this.textBox55.Value = "= Fields.Name";
            // 
            // textBox54
            // 
            this.textBox54.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.20837275683879852D));
            this.textBox54.Name = "textBox54";
            this.textBox54.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox54.Style.Font.Bold = true;
            this.textBox54.Value = "= Fields.AmharicName";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.5D), Telerik.Reporting.Drawing.Unit.Inch(0.19999994337558746D));
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.Value = "Budget Plan and Usage Againist Previous Year - Monthly";
            // 
            // reportFooter
            // 
            this.reportFooter.Height = Telerik.Reporting.Drawing.Unit.Cm(0.13229165971279144D);
            this.reportFooter.Name = "reportFooter";
            this.reportFooter.Style.Visible = false;
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5285072326660156D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox19,
            this.textBox24,
            this.textBox23,
            this.textBox22,
            this.textBox25,
            this.textBox29,
            this.textBox28,
            this.textBox27,
            this.textBox26,
            this.textBox20,
            this.textBox12,
            this.textBox11,
            this.textBox10,
            this.textBox9,
            this.textBox14,
            this.textBox13,
            this.textBox16,
            this.textBox15,
            this.textBox21,
            this.textBox30});
            this.detail.Name = "detail";
            this.detail.Style.Visible = true;
            // 
            // textBox19
            // 
            this.textBox19.CanGrow = true;
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.052916664630174637D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.4110422134399414D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox19.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.Style.Color = System.Drawing.Color.White;
            this.textBox19.Style.Font.Name = "Times New Roman";
            this.textBox19.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox19.StyleName = "Caption";
            this.textBox19.Value = "Account Code";
            // 
            // textBox24
            // 
            this.textBox24.CanGrow = true;
            this.textBox24.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(17.942173004150391D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.5992965698242188D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox24.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox24.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox24.Style.Color = System.Drawing.Color.White;
            this.textBox24.Style.Font.Name = "Times New Roman";
            this.textBox24.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox24.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox24.StyleName = "Caption";
            this.textBox24.Value = "Previous Year Actual";
            // 
            // textBox23
            // 
            this.textBox23.CanGrow = true;
            this.textBox23.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(14.841971397399902D), Telerik.Reporting.Drawing.Unit.Cm(1.3283039331436157D));
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.100001335144043D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox23.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox23.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox23.Style.Color = System.Drawing.Color.White;
            this.textBox23.Style.Font.Name = "Times New Roman";
            this.textBox23.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox23.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox23.StyleName = "Caption";
            this.textBox23.Value = "This Year Actual";
            // 
            // textBox22
            // 
            this.textBox22.CanGrow = true;
            this.textBox22.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(21.541969299316406D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.1000974178314209D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox22.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.Style.Color = System.Drawing.Color.White;
            this.textBox22.Style.Font.Name = "Times New Roman";
            this.textBox22.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox22.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox22.StyleName = "Caption";
            this.textBox22.Value = "Actual Difference";
            // 
            // textBox25
            // 
            this.textBox25.CanGrow = true;
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(24.6419677734375D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.4000027179718018D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox25.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox25.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox25.Style.Color = System.Drawing.Color.White;
            this.textBox25.Style.Font.Name = "Times New Roman";
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox25.StyleName = "Caption";
            this.textBox25.Value = "Difference %";
            // 
            // textBox29
            // 
            this.textBox29.CanGrow = true;
            this.textBox29.Format = "{0:P2}";
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(24.6419677734375D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.4000034332275391D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox29.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox29.StyleName = "Data";
            this.textBox29.Value = "= Fields.ActualDifferencePercentage";
            // 
            // textBox28
            // 
            this.textBox28.CanGrow = true;
            this.textBox28.Format = "{0:N2}";
            this.textBox28.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(21.541671752929688D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.0998001098632812D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox28.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox28.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox28.StyleName = "Data";
            this.textBox28.Value = "= Fields.ActualDifference";
            // 
            // textBox27
            // 
            this.textBox27.CanGrow = true;
            this.textBox27.Format = "{0:N2}";
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(17.942173004150391D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.5992972850799561D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox27.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox27.StyleName = "Data";
            this.textBox27.Value = "= Fields.PreviousYearActual";
            // 
            // textBox26
            // 
            this.textBox26.CanGrow = true;
            this.textBox26.Format = "{0:N2}";
            this.textBox26.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(14.841971397399902D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.1000006198883057D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox26.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox26.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox26.StyleName = "Data";
            this.textBox26.Value = "= Fields.ThisYearActual";
            // 
            // textBox20
            // 
            this.textBox20.CanGrow = true;
            this.textBox20.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.052916664630174637D), Telerik.Reporting.Drawing.Unit.Cm(1.9285038709640503D));
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.4110422134399414D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox20.StyleName = "Data";
            this.textBox20.Value = "= Fields.AccountCode";
            // 
            // textBox12
            // 
            this.textBox12.CanGrow = true;
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(5.6643595695495605D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.7775146961212158D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox12.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.Color = System.Drawing.Color.White;
            this.textBox12.Style.Font.Name = "Times New Roman";
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox12.StyleName = "Caption";
            this.textBox12.Value = "Previous Year Budget ";
            // 
            // textBox11
            // 
            this.textBox11.CanGrow = true;
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.4641590118408203D), Telerik.Reporting.Drawing.Unit.Cm(1.328304648399353D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.2000000476837158D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.Color = System.Drawing.Color.White;
            this.textBox11.Style.Font.Name = "Times New Roman";
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox11.StyleName = "Caption";
            this.textBox11.Value = "This Year Budget ";
            // 
            // textBox10
            // 
            this.textBox10.CanGrow = true;
            this.textBox10.Format = "{0:N2}";
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(5.6643595695495605D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.7775146961212158D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox10.StyleName = "Data";
            this.textBox10.Value = "= Fields.PreviousMonthlyBudget";
            // 
            // textBox9
            // 
            this.textBox9.CanGrow = true;
            this.textBox9.Format = "{0:N2}";
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.4641590118408203D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.2000002861022949D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox9.StyleName = "Data";
            this.textBox9.Value = "= Fields.ThisYearMonthlyBudget";
            // 
            // textBox14
            // 
            this.textBox14.CanGrow = true;
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(12.141874313354492D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.6998965740203857D), Telerik.Reporting.Drawing.Unit.Cm(0.59165799617767334D));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.Color = System.Drawing.Color.White;
            this.textBox14.Style.Font.Name = "Times New Roman";
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox14.StyleName = "Caption";
            this.textBox14.Value = "Diffrencnce %";
            // 
            // textBox13
            // 
            this.textBox13.CanGrow = true;
            this.textBox13.Format = "{0:P2}";
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(12.142468452453613D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.6995992660522461D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox13.StyleName = "Data";
            this.textBox13.Value = "= Fields.MonthlyDiffrencePecent";
            // 
            // textBox16
            // 
            this.textBox16.CanGrow = true;
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(9.441777229309082D), Telerik.Reporting.Drawing.Unit.Cm(1.3283071517944336D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.6998965740203857D), Telerik.Reporting.Drawing.Unit.Cm(0.59165799617767334D));
            this.textBox16.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox16.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Style.Color = System.Drawing.Color.White;
            this.textBox16.Style.Font.Name = "Times New Roman";
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox16.StyleName = "Caption";
            this.textBox16.Value = "Diffrencnce ";
            // 
            // textBox15
            // 
            this.textBox15.CanGrow = true;
            this.textBox15.Format = "{0:N2}";
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(9.4423713684082031D), Telerik.Reporting.Drawing.Unit.Cm(1.9285070896148682D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.6995992660522461D), Telerik.Reporting.Drawing.Unit.Cm(0.46572527289390564D));
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox15.StyleName = "Data";
            this.textBox15.Value = "= Fields.MonthlyDiffrence";
            // 
            // textBox21
            // 
            this.textBox21.CanGrow = true;
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0.12790805101394653D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(7.5D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox21.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox21.Style.Color = System.Drawing.Color.White;
            this.textBox21.Style.Font.Name = "Times New Roman";
            this.textBox21.StyleName = "Caption";
            this.textBox21.Value = "Budget Month";
            // 
            // textBox30
            // 
            this.textBox30.CanGrow = true;
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(-6.9388939039072284E-17D), Telerik.Reporting.Drawing.Unit.Cm(0.728107213973999D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(7.5D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox30.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox30.StyleName = "Data";
            this.textBox30.Value = "= Fields.BudgetMonth";
            // 
            // MonthlyUsageDS
            // 
            this.MonthlyUsageDS.DataMember = "GetUsageList";
            this.MonthlyUsageDS.DataSource = typeof(ExceedERP.Reporting.Budget.DataSource.BudgetUsagePerMonth);
            this.MonthlyUsageDS.Name = "MonthlyUsageDS";
            this.MonthlyUsageDS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("BudgetType", typeof(string), "= Parameters.BudgetType.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("BugetCategory", typeof(string), "= Parameters.BugetCategory.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("Year", typeof(string), "= Parameters.Year.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("WorkUnit", typeof(string), "= Parameters.WorkUnit.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("Branch", typeof(string), "= Parameters.Branch.Value")});
            // 
            // BudgetTypeDS
            // 
            this.BudgetTypeDS.DataMember = "CategoryTypeDDBind";
            this.BudgetTypeDS.DataSource = typeof(ExceedERP.Reporting.Budget.DataSource.BudgetUsagePerYear);
            this.BudgetTypeDS.Name = "BudgetTypeDS";
            // 
            // PlanCategoryDS
            // 
            this.PlanCategoryDS.DataMember = "GetBudgetCategory";
            this.PlanCategoryDS.DataSource = typeof(ExceedERP.Reporting.Budget.DataSource.BudgetAllocationUsage);
            this.PlanCategoryDS.Name = "PlanCategoryDS";
            this.PlanCategoryDS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("name", typeof(string), "= Parameters.BudgetType.Value")});
            // 
            // YearDS
            // 
            this.YearDS.DataMember = "GetFiscalYearss";
            this.YearDS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.FiscalYearDataSource);
            this.YearDS.Name = "YearDS";
            // 
            // BranchDS
            // 
            this.BranchDS.DataMember = "BranchDDBind";
            this.BranchDS.DataSource = typeof(ExceedERP.Reporting.Budget.DataSource.BudgetAllocationUsage);
            this.BranchDS.Name = "BranchDS";
            // 
            // WorkUnitDS
            // 
            this.WorkUnitDS.DataMember = "GetWorkUnit";
            this.WorkUnitDS.DataSource = typeof(ExceedERP.Reporting.Budget.DataSource.BudgetAllocationUsage);
            this.WorkUnitDS.Name = "WorkUnitDS";
            // 
            // headerComp
            // 
            this.headerComp.DataMember = "GetOrganization";
            this.headerComp.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.headerComp.Name = "headerComp";
            // 
            // BudgetUsageAgainstMonth
            // 
            this.DataSource = this.MonthlyUsageDS;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageFooter,
            this.reportHeader,
            this.reportFooter,
            this.detail});
            this.Name = "BudgetUsageAgainstMonth";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.BudgetTypeDS;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Value";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Value";
            reportParameter1.Name = "BudgetType";
            reportParameter1.Text = "Budget Type";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.PlanCategoryDS;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.Name";
            reportParameter2.Name = "BugetCategory";
            reportParameter2.Text = "Buget Category";
            reportParameter2.Visible = true;
            reportParameter3.AllowBlank = false;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.YearDS;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.Name";
            reportParameter3.Name = "Year";
            reportParameter3.Text = "Fiscal Year";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.BranchDS;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter4.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter4.Name = "Branch";
            reportParameter4.Text = "Branch";
            reportParameter4.Visible = true;
            reportParameter5.AllowBlank = false;
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DataSource = this.WorkUnitDS;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.Description";
            reportParameter5.AvailableValues.ValueMember = "= Fields.Values";
            reportParameter5.Name = "WorkUnit";
            reportParameter5.Text = "Work Unit";
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
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule2.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(97)))), ((int)(((byte)(74)))));
            styleRule2.Style.Font.Name = "Georgia";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(18D);
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule3.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(168)))), ((int)(((byte)(212)))));
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Dotted;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(238)))), ((int)(((byte)(243)))));
            styleRule3.Style.Font.Name = "Georgia";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule4.Style.Font.Name = "Georgia";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule5.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule5.Style.Font.Name = "Georgia";
            styleRule5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            styleRule5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4,
            styleRule5});
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(27.041973114013672D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.ObjectDataSource MonthlyUsageDS;
        private Telerik.Reporting.ObjectDataSource YearDS;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.ReportFooterSection reportFooter;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox28;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox26;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.ObjectDataSource PlanCategoryDS;
        private Telerik.Reporting.ObjectDataSource WorkUnitDS;
        private Telerik.Reporting.ObjectDataSource BranchDS;
        private Telerik.Reporting.ObjectDataSource BudgetTypeDS;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.TextBox textBox54;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.ObjectDataSource headerComp;
    }
}