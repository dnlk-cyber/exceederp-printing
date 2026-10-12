namespace ExceedERP.Reporting.HRM.HRM
{
    partial class DayAbsentReport
    {
        #region Component Designer generated code
        private void InitializeComponent()
        {
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter6 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter7 = new Telerik.Reporting.ReportParameter();
            this.thSN = new Telerik.Reporting.TextBox();
            this.thEmpId = new Telerik.Reporting.TextBox();
            this.thEmpName = new Telerik.Reporting.TextBox();
            this.thWorkUnit = new Telerik.Reporting.TextBox();
            this.thDynamicPeriod = new Telerik.Reporting.TextBox();
            this.thMonthsParent = new Telerik.Reporting.TextBox();
            this.thTotalCol = new Telerik.Reporting.TextBox();
            this.headerSubReportLandScape1 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.EmployeeCodeODS = new Telerik.Reporting.ObjectDataSource();
            this.FiscalYearODS = new Telerik.Reporting.ObjectDataSource();
            this.tbSN = new Telerik.Reporting.TextBox();
            this.tbEmpId = new Telerik.Reporting.TextBox();
            this.tbEmpName = new Telerik.Reporting.TextBox();
            this.tbWorkUnit = new Telerik.Reporting.TextBox();
            this.tbAbsentVal = new Telerik.Reporting.TextBox();
            this.tbTotalRowVal = new Telerik.Reporting.TextBox();
            this.DayAbsentODS = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.txtMetaFilter = new Telerik.Reporting.TextBox();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.crosstab1 = new Telerik.Reporting.Crosstab();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.txtPageNumber = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // thSN
            // 
            this.thSN.Name = "thSN";
            this.thSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34999999403953552D), Telerik.Reporting.Drawing.Unit.Inch(0.43999999761581421D));
            this.thSN.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thSN.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thSN.Style.Font.Bold = true;
            this.thSN.Style.Font.Name = "Segoe UI";
            this.thSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.thSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thSN.Value = "S.N";
            // 
            // thEmpId
            // 
            this.thEmpId.Name = "thEmpId";
            this.thEmpId.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.949999988079071D), Telerik.Reporting.Drawing.Unit.Inch(0.43999999761581421D));
            this.thEmpId.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thEmpId.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thEmpId.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thEmpId.Style.Font.Bold = true;
            this.thEmpId.Style.Font.Name = "Segoe UI";
            this.thEmpId.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.thEmpId.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thEmpId.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thEmpId.Value = "Employee ID";
            // 
            // thEmpName
            // 
            this.thEmpName.Name = "thEmpName";
            this.thEmpName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.43999999761581421D));
            this.thEmpName.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thEmpName.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thEmpName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thEmpName.Style.Font.Bold = true;
            this.thEmpName.Style.Font.Name = "Segoe UI";
            this.thEmpName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.thEmpName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thEmpName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thEmpName.Value = "Employee Name";
            // 
            // thWorkUnit
            // 
            this.thWorkUnit.Name = "thWorkUnit";
            this.thWorkUnit.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.43999999761581421D));
            this.thWorkUnit.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thWorkUnit.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thWorkUnit.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thWorkUnit.Style.Font.Bold = true;
            this.thWorkUnit.Style.Font.Name = "Segoe UI";
            this.thWorkUnit.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.thWorkUnit.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thWorkUnit.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thWorkUnit.Value = "Work unit";
            // 
            // thDynamicPeriod
            // 
            this.thDynamicPeriod.Name = "thDynamicPeriod";
            this.thDynamicPeriod.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.75D), Telerik.Reporting.Drawing.Unit.Inch(0.2199999988079071D));
            this.thDynamicPeriod.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thDynamicPeriod.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thDynamicPeriod.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thDynamicPeriod.Style.Font.Bold = true;
            this.thDynamicPeriod.Style.Font.Name = "Segoe UI";
            this.thDynamicPeriod.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
            this.thDynamicPeriod.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thDynamicPeriod.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thDynamicPeriod.Value = "= Fields.PeriodName";
            // 
            // thMonthsParent
            // 
            this.thMonthsParent.Name = "thMonthsParent";
            this.thMonthsParent.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.75D), Telerik.Reporting.Drawing.Unit.Inch(0.2199999988079071D));
            this.thMonthsParent.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thMonthsParent.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thMonthsParent.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thMonthsParent.Style.Font.Bold = true;
            this.thMonthsParent.Style.Font.Name = "Segoe UI";
            this.thMonthsParent.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.thMonthsParent.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thMonthsParent.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thMonthsParent.Value = "Months";
            // 
            // thTotalCol
            // 
            this.thTotalCol.Name = "thTotalCol";
            this.thTotalCol.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D), Telerik.Reporting.Drawing.Unit.Inch(0.43999999761581421D));
            this.thTotalCol.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.thTotalCol.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.thTotalCol.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thTotalCol.Style.Font.Bold = true;
            this.thTotalCol.Style.Font.Name = "Segoe UI";
            this.thTotalCol.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.thTotalCol.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thTotalCol.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thTotalCol.Value = "Total";
            // 
            // headerSubReportLandScape1
            // 
            this.headerSubReportLandScape1.Name = "HeaderSubReportLandScape";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
            // 
            // WorkUnitObjectDataSource
            // 
            this.WorkUnitObjectDataSource.DataMember = "GetDepartment";
            this.WorkUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);
            this.WorkUnitObjectDataSource.Name = "WorkUnitObjectDataSource";
            // 
            // EmployeeCodeODS
            // 
            this.EmployeeCodeODS.DataMember = "GetEmployeeCode";
            this.EmployeeCodeODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EmployeeCodeObjectDataSource);
            this.EmployeeCodeODS.Name = "EmployeeCodeODS";
            // 
            // FiscalYearODS
            // 
            this.FiscalYearODS.DataMember = "GetFiscalYearss";
            this.FiscalYearODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.FiscalYearDataSource);
            this.FiscalYearODS.Name = "FiscalYearODS";
            // 
            // tbSN
            // 
            this.tbSN.Name = "tbSN";
            this.tbSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34999999403953552D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.tbSN.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tbSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbSN.Style.Font.Name = "Segoe UI";
            this.tbSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.tbSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tbSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbSN.Value = "= RowNumber(\"crosstab1\")";
            // 
            // tbEmpId
            // 
            this.tbEmpId.Name = "tbEmpId";
            this.tbEmpId.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.949999988079071D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.tbEmpId.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tbEmpId.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbEmpId.Style.Font.Name = "Segoe UI";
            this.tbEmpId.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.tbEmpId.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
            this.tbEmpId.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbEmpId.Value = "= Fields.EmployeeCode";
            // 
            // tbEmpName
            // 
            this.tbEmpName.Name = "tbEmpName";
            this.tbEmpName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.tbEmpName.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tbEmpName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbEmpName.Style.Font.Bold = true;
            this.tbEmpName.Style.Font.Name = "Segoe UI";
            this.tbEmpName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.tbEmpName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
            this.tbEmpName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbEmpName.Value = "= Fields.FullName";
            // 
            // tbWorkUnit
            // 
            this.tbWorkUnit.Name = "tbWorkUnit";
            this.tbWorkUnit.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.tbWorkUnit.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tbWorkUnit.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbWorkUnit.Style.Font.Name = "Segoe UI";
            this.tbWorkUnit.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.tbWorkUnit.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
            this.tbWorkUnit.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbWorkUnit.Value = "= Fields.WorkUnit";
            // 
            // tbAbsentVal
            // 
            this.tbAbsentVal.Name = "tbAbsentVal";
            this.tbAbsentVal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.75D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.tbAbsentVal.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tbAbsentVal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbAbsentVal.Style.Font.Name = "Segoe UI";
            this.tbAbsentVal.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.tbAbsentVal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tbAbsentVal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbAbsentVal.Value = "= Sum(Fields.AbsentDays)";
            // 
            // tbTotalRowVal
            // 
            this.tbTotalRowVal.Name = "tbTotalRowVal";
            this.tbTotalRowVal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D), Telerik.Reporting.Drawing.Unit.Inch(0.23999999463558197D));
            this.tbTotalRowVal.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tbTotalRowVal.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tbTotalRowVal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbTotalRowVal.Style.Font.Bold = true;
            this.tbTotalRowVal.Style.Font.Name = "Segoe UI";
            this.tbTotalRowVal.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.tbTotalRowVal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tbTotalRowVal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbTotalRowVal.Value = "= Sum(Fields.AbsentDays)";
            // 
            // DayAbsentODS
            // 
            this.DayAbsentODS.DataMember = "GetDayAbsentSummary";
            this.DayAbsentODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.DayAbsentReportDataSource);
            this.DayAbsentODS.Name = "DayAbsentODS";
            this.DayAbsentODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(object[]), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("businessUnitId", typeof(string), null),
            new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("employee", typeof(string), "= Parameters.employee.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("fiscalYearId", typeof(System.Nullable<int>), "= Parameters.FiscalYearId.Value")});
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.0999999046325684D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D), Telerik.Reporting.Drawing.Unit.Inch(0.10000000894069672D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReportLandScape1;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.0999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.69999992847442627D));
            // 
            // txtMetaFilter
            // 
            this.txtMetaFilter.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.txtMetaFilter.Name = "txtMetaFilter";
            this.txtMetaFilter.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.5999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtMetaFilter.Style.Font.Bold = true;
            this.txtMetaFilter.Style.Font.Name = "Segoe UI";
            this.txtMetaFilter.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtMetaFilter.Value = "= \'Fiscal Year: \' + Parameters.FiscalYearTitle.Label + \'        Date: \' + Now().T" +
    "oString(\'dd.MM.yyyy\')";
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.1000001430511475D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.crosstab1,
            this.txtMetaFilter});
            this.detailSection1.Name = "detailSection1";
            // 
            // crosstab1
            // 
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D)));
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.9500001072883606D)));
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5D)));
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D)));
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.75D)));
            this.crosstab1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.70000004768371582D)));
            this.crosstab1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24000000953674316D)));
            this.crosstab1.Body.SetCellContent(0, 0, this.tbSN);
            this.crosstab1.Body.SetCellContent(0, 1, this.tbEmpId);
            this.crosstab1.Body.SetCellContent(0, 2, this.tbEmpName);
            this.crosstab1.Body.SetCellContent(0, 3, this.tbWorkUnit);
            this.crosstab1.Body.SetCellContent(0, 4, this.tbAbsentVal);
            this.crosstab1.Body.SetCellContent(0, 5, this.tbTotalRowVal);
            tableGroup1.ReportItem = this.thSN;
            tableGroup2.ReportItem = this.thEmpId;
            tableGroup3.ReportItem = this.thEmpName;
            tableGroup4.ReportItem = this.thWorkUnit;
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.GLPeriodId"));
            tableGroup6.Name = "tgDynamicPeriod";
            tableGroup6.ReportItem = this.thDynamicPeriod;
            tableGroup6.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.PeriodIndex", Telerik.Reporting.SortDirection.Asc));
            tableGroup5.ChildGroups.Add(tableGroup6);
            tableGroup5.Name = "tgMonthsParent";
            tableGroup5.ReportItem = this.thMonthsParent;
            tableGroup7.Name = "tgTotalCol";
            tableGroup7.ReportItem = this.thTotalCol;
            this.crosstab1.ColumnGroups.Add(tableGroup1);
            this.crosstab1.ColumnGroups.Add(tableGroup2);
            this.crosstab1.ColumnGroups.Add(tableGroup3);
            this.crosstab1.ColumnGroups.Add(tableGroup4);
            this.crosstab1.ColumnGroups.Add(tableGroup5);
            this.crosstab1.ColumnGroups.Add(tableGroup7);
            this.crosstab1.DataSource = this.DayAbsentODS;
            this.crosstab1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tbSN,
            this.tbEmpId,
            this.tbEmpName,
            this.tbWorkUnit,
            this.tbAbsentVal,
            this.tbTotalRowVal,
            this.thSN,
            this.thEmpId,
            this.thEmpName,
            this.thWorkUnit,
            this.thMonthsParent,
            this.thDynamicPeriod,
            this.thTotalCol});
            this.crosstab1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000000894069672D), Telerik.Reporting.Drawing.Unit.Inch(0.41996055841445923D));
            this.crosstab1.Name = "crosstab1";
            tableGroup8.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.EmployeeId"));
            tableGroup8.Name = "tgEmpGroup";
            tableGroup8.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.WorkUnit", Telerik.Reporting.SortDirection.Asc));
            tableGroup8.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.FullName", Telerik.Reporting.SortDirection.Asc));
            this.crosstab1.RowGroups.Add(tableGroup8);
            this.crosstab1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5500001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.68000000715255737D));
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtPageNumber});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // txtPageNumber
            // 
            this.txtPageNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.6999993324279785D), Telerik.Reporting.Drawing.Unit.Inch(0.099960751831531525D));
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtPageNumber.Style.Font.Name = "Segoe UI";
            this.txtPageNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtPageNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtPageNumber.Value = "Page: {PageNumber} of {TotalPages}";
            // 
            // DayAbsentReport
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.reportHeaderSection1,
            this.detailSection1,
            this.pageFooterSection1});
            this.Name = "DayAbsentReport";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.AmharicName";
            reportParameter1.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter1.MultiValue = true;
            reportParameter1.Name = "branchId";
            reportParameter1.Text = "Branch";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.Name = "orgStructureId";
            reportParameter2.Text = "Department";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.EmployeeCodeODS;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.EmployeeCode";
            reportParameter3.AvailableValues.ValueMember = "= Fields.EmployeeId";
            reportParameter3.Name = "employee";
            reportParameter3.Text = "Employee";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.FiscalYearODS;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter4.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
            reportParameter4.Name = "FiscalYearId";
            reportParameter4.Text = "Fiscal Year";
            reportParameter4.Visible = true;
            reportParameter5.AllowNull = true;
            reportParameter5.Name = "FiscalYearTitle";
            reportParameter5.Text = "Fiscal Year Label";
            reportParameter5.Value = "2018 (2025/2026)";
            reportParameter6.AllowNull = true;
            reportParameter6.Name = "FormTitle";
            reportParameter7.AllowNull = true;
            reportParameter7.Name = "FormNumber";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            this.ReportParameters.Add(reportParameter5);
            this.ReportParameters.Add(reportParameter6);
            this.ReportParameters.Add(reportParameter7);
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Crosstab crosstab1;

        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.ObjectDataSource EmployeeCodeODS;
        private Telerik.Reporting.ObjectDataSource FiscalYearODS;
        private Telerik.Reporting.ObjectDataSource DayAbsentODS;
        private Telerik.Reporting.TextBox txtMetaFilter;
        private Telerik.Reporting.TextBox txtPageNumber;

        // Column Headers
        private Telerik.Reporting.TextBox thSN;
        private Telerik.Reporting.TextBox thEmpId;
        private Telerik.Reporting.TextBox thEmpName;
        private Telerik.Reporting.TextBox thWorkUnit;
        private Telerik.Reporting.TextBox thMonthsParent;
        private Telerik.Reporting.TextBox thDynamicPeriod;
        private Telerik.Reporting.TextBox thTotalCol;

        // Row Headers & Value Cells
        private Telerik.Reporting.TextBox tbSN;
        private Telerik.Reporting.TextBox tbEmpId;
        private Telerik.Reporting.TextBox tbEmpName;
        private Telerik.Reporting.TextBox tbWorkUnit;
        private Telerik.Reporting.TextBox tbAbsentVal;
        private Telerik.Reporting.TextBox tbTotalRowVal;
        private Telerik.Reporting.SubReport subReport1;
        private SharedDesigners.HeaderSubReportLandScape headerSubReportLandScape1;
    }
}

//namespace ExceedERP.Reporting.HRM.HRM
//{
//    partial class DayAbsentReport
//    {
//        #region Component Designer generated code
//        private void InitializeComponent()
//        {
//            Telerik.Reporting.TableGroup tgSN = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgEmpId = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgEmpName = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgWorkUnit = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgMonthsParent = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM1 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM2 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM3 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM4 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM5 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM6 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM7 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM8 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM9 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM10 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM11 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgM12 = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgTotal = new Telerik.Reporting.TableGroup();
//            Telerik.Reporting.TableGroup tgDetailRow = new Telerik.Reporting.TableGroup();

//            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
//            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
//            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
//            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
//            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();

//            this.thSN = new Telerik.Reporting.TextBox();
//            this.thEmpId = new Telerik.Reporting.TextBox();
//            this.thEmpName = new Telerik.Reporting.TextBox();
//            this.thWorkUnit = new Telerik.Reporting.TextBox();
//            this.thMonths = new Telerik.Reporting.TextBox();
//            this.thM1 = new Telerik.Reporting.TextBox();
//            this.thM2 = new Telerik.Reporting.TextBox();
//            this.thM3 = new Telerik.Reporting.TextBox();
//            this.thM4 = new Telerik.Reporting.TextBox();
//            this.thM5 = new Telerik.Reporting.TextBox();
//            this.thM6 = new Telerik.Reporting.TextBox();
//            this.thM7 = new Telerik.Reporting.TextBox();
//            this.thM8 = new Telerik.Reporting.TextBox();
//            this.thM9 = new Telerik.Reporting.TextBox();
//            this.thM10 = new Telerik.Reporting.TextBox();
//            this.thM11 = new Telerik.Reporting.TextBox();
//            this.thM12 = new Telerik.Reporting.TextBox();
//            this.thTotal = new Telerik.Reporting.TextBox();

//            this.tbSN = new Telerik.Reporting.TextBox();
//            this.tbEmpId = new Telerik.Reporting.TextBox();
//            this.tbEmpName = new Telerik.Reporting.TextBox();
//            this.tbWorkUnit = new Telerik.Reporting.TextBox();
//            this.tbM1 = new Telerik.Reporting.TextBox();
//            this.tbM2 = new Telerik.Reporting.TextBox();
//            this.tbM3 = new Telerik.Reporting.TextBox();
//            this.tbM4 = new Telerik.Reporting.TextBox();
//            this.tbM5 = new Telerik.Reporting.TextBox();
//            this.tbM6 = new Telerik.Reporting.TextBox();
//            this.tbM7 = new Telerik.Reporting.TextBox();
//            this.tbM8 = new Telerik.Reporting.TextBox();
//            this.tbM9 = new Telerik.Reporting.TextBox();
//            this.tbM10 = new Telerik.Reporting.TextBox();
//            this.tbM11 = new Telerik.Reporting.TextBox();
//            this.tbM12 = new Telerik.Reporting.TextBox();
//            this.tbTotal = new Telerik.Reporting.TextBox();

//            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
//            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
//            this.EmployeeCodeODS = new Telerik.Reporting.ObjectDataSource();
//            this.FiscalYearODS = new Telerik.Reporting.ObjectDataSource();
//            this.DayAbsentODS = new Telerik.Reporting.ObjectDataSource();

//            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
//            this.detailSection1 = new Telerik.Reporting.DetailSection();
//            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
//            this.table1 = new Telerik.Reporting.Table();

//            this.txtCompanyAmh = new Telerik.Reporting.TextBox();
//            this.txtCompanyEng = new Telerik.Reporting.TextBox();
//            this.txtDocNo = new Telerik.Reporting.TextBox();
//            this.txtReportTitle = new Telerik.Reporting.TextBox();
//            this.txtMetaFilter = new Telerik.Reporting.TextBox();
//            this.txtPageNumber = new Telerik.Reporting.TextBox();

//            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

//            // txtCompanyAmh
//            this.txtCompanyAmh.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.05D));
//            this.txtCompanyAmh.Name = "txtCompanyAmh";
//            this.txtCompanyAmh.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.5D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
//            this.txtCompanyAmh.Style.Font.Name = "Nyala";
//            this.txtCompanyAmh.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
//            this.txtCompanyAmh.Style.Font.Bold = true;
//            this.txtCompanyAmh.Value = "ሚድሮክ ኮንስትራክሽን ኢትዮጵያ ኋ. የተ. የግ. ማ";

//            // txtCompanyEng
//            this.txtCompanyEng.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
//            this.txtCompanyEng.Name = "txtCompanyEng";
//            this.txtCompanyEng.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.5D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
//            this.txtCompanyEng.Style.Font.Name = "Segoe UI";
//            this.txtCompanyEng.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(11D);
//            this.txtCompanyEng.Style.Font.Bold = true;
//            this.txtCompanyEng.Value = "MIDROC Construction Ethiopia Plc";

//            // txtDocNo
//            this.txtDocNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.5D), Telerik.Reporting.Drawing.Unit.Inch(0.05D));
//            this.txtDocNo.Name = "txtDocNo";
//            this.txtDocNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
//            this.txtDocNo.Style.Font.Name = "Segoe UI";
//            this.txtDocNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.txtDocNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
//            this.txtDocNo.Value = "Doc No: MCE/HRM/1/ERPD/46\nEffective Date: 11/08/2025 | Issue No: 1";

//            // txtReportTitle
//            this.txtReportTitle.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.65D));
//            this.txtReportTitle.Name = "txtReportTitle";
//            this.txtReportTitle.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.0D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
//            this.txtReportTitle.Style.Font.Name = "Segoe UI";
//            this.txtReportTitle.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
//            this.txtReportTitle.Style.Font.Bold = true;
//            this.txtReportTitle.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.txtReportTitle.Value = "Day Absent Report";

//            // txtMetaFilter
//            this.txtMetaFilter.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.0D));
//            this.txtMetaFilter.Name = "txtMetaFilter";
//            this.txtMetaFilter.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.0D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
//            this.txtMetaFilter.Style.Font.Name = "Segoe UI";
//            this.txtMetaFilter.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
//            this.txtMetaFilter.Style.Font.Bold = true;
//            this.txtMetaFilter.Value = "= 'Fiscal Year: ' + Parameters.FiscalYearTitle.Value + '        Date: ' + Now().ToString('dd.MM.yyyy')";

//            // thSN
//            this.thSN.Name = "thSN";
//            this.thSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.35D), Telerik.Reporting.Drawing.Unit.Inch(0.44D));
//            this.thSN.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thSN.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thSN.Style.Font.Name = "Segoe UI";
//            this.thSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.thSN.Style.Font.Bold = true;
//            this.thSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thSN.Value = "S.N";

//            // thEmpId
//            this.thEmpId.Name = "thEmpId";
//            this.thEmpId.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.95D), Telerik.Reporting.Drawing.Unit.Inch(0.44D));
//            this.thEmpId.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thEmpId.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thEmpId.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thEmpId.Style.Font.Name = "Segoe UI";
//            this.thEmpId.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.thEmpId.Style.Font.Bold = true;
//            this.thEmpId.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thEmpId.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thEmpId.Value = "Employee ID";

//            // thEmpName
//            this.thEmpName.Name = "thEmpName";
//            this.thEmpName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.44D));
//            this.thEmpName.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thEmpName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thEmpName.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thEmpName.Style.Font.Name = "Segoe UI";
//            this.thEmpName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.thEmpName.Style.Font.Bold = true;
//            this.thEmpName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thEmpName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thEmpName.Value = "Employee Name";

//            // thWorkUnit
//            this.thWorkUnit.Name = "thWorkUnit";
//            this.thWorkUnit.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3D), Telerik.Reporting.Drawing.Unit.Inch(0.44D));
//            this.thWorkUnit.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thWorkUnit.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thWorkUnit.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thWorkUnit.Style.Font.Name = "Segoe UI";
//            this.thWorkUnit.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.thWorkUnit.Style.Font.Bold = true;
//            this.thWorkUnit.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thWorkUnit.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thWorkUnit.Value = "Work unit";

//            // thMonths
//            this.thMonths.Name = "thMonths";
//            this.thMonths.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.24D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thMonths.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thMonths.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thMonths.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thMonths.Style.Font.Name = "Segoe UI";
//            this.thMonths.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.thMonths.Style.Font.Bold = true;
//            this.thMonths.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thMonths.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thMonths.Value = "Months";

//            // thM1
//            this.thM1.Name = "thM1";
//            this.thM1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM1.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM1.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM1.Style.Font.Name = "Segoe UI";
//            this.thM1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM1.Style.Font.Bold = true;
//            this.thM1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM1.Value = "July 2025";

//            // thM2
//            this.thM2.Name = "thM2";
//            this.thM2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM2.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM2.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM2.Style.Font.Name = "Segoe UI";
//            this.thM2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM2.Style.Font.Bold = true;
//            this.thM2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM2.Value = "August 2025";

//            // thM3
//            this.thM3.Name = "thM3";
//            this.thM3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM3.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM3.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM3.Style.Font.Name = "Segoe UI";
//            this.thM3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM3.Style.Font.Bold = true;
//            this.thM3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM3.Value = "Sep 2025";

//            // thM4
//            this.thM4.Name = "thM4";
//            this.thM4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM4.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM4.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM4.Style.Font.Name = "Segoe UI";
//            this.thM4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM4.Style.Font.Bold = true;
//            this.thM4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM4.Value = "Oct 2025";

//            // thM5
//            this.thM5.Name = "thM5";
//            this.thM5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM5.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM5.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM5.Style.Font.Name = "Segoe UI";
//            this.thM5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM5.Style.Font.Bold = true;
//            this.thM5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM5.Value = "Nov 2025";

//            // thM6
//            this.thM6.Name = "thM6";
//            this.thM6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM6.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM6.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM6.Style.Font.Name = "Segoe UI";
//            this.thM6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM6.Style.Font.Bold = true;
//            this.thM6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM6.Value = "Dec 2025";

//            // thM7
//            this.thM7.Name = "thM7";
//            this.thM7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM7.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM7.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM7.Style.Font.Name = "Segoe UI";
//            this.thM7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM7.Style.Font.Bold = true;
//            this.thM7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM7.Value = "Jan 2026";

//            // thM8
//            this.thM8.Name = "thM8";
//            this.thM8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM8.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM8.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM8.Style.Font.Name = "Segoe UI";
//            this.thM8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM8.Style.Font.Bold = true;
//            this.thM8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM8.Value = "Feb 2026";

//            // thM9
//            this.thM9.Name = "thM9";
//            this.thM9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM9.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM9.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM9.Style.Font.Name = "Segoe UI";
//            this.thM9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM9.Style.Font.Bold = true;
//            this.thM9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM9.Value = "Mar 2026";

//            // thM10
//            this.thM10.Name = "thM10";
//            this.thM10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM10.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM10.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM10.Style.Font.Name = "Segoe UI";
//            this.thM10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM10.Style.Font.Bold = true;
//            this.thM10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM10.Value = "Apr 2026";

//            // thM11
//            this.thM11.Name = "thM11";
//            this.thM11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM11.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM11.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM11.Style.Font.Name = "Segoe UI";
//            this.thM11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM11.Style.Font.Bold = true;
//            this.thM11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM11.Value = "May 2026";

//            // thM12
//            this.thM12.Name = "thM12";
//            this.thM12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.22D));
//            this.thM12.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thM12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thM12.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thM12.Style.Font.Name = "Segoe UI";
//            this.thM12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.5D);
//            this.thM12.Style.Font.Bold = true;
//            this.thM12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thM12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thM12.Value = "Jun 2026";

//            // thTotal
//            this.thTotal.Name = "thTotal";
//            this.thTotal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.65D), Telerik.Reporting.Drawing.Unit.Inch(0.44D));
//            this.thTotal.Style.BackgroundColor = System.Drawing.Color.FromArgb(241, 245, 249);
//            this.thTotal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.thTotal.Style.BorderColor.Default = System.Drawing.Color.FromArgb(148, 163, 184);
//            this.thTotal.Style.Font.Name = "Segoe UI";
//            this.thTotal.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.thTotal.Style.Font.Bold = true;
//            this.thTotal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.thTotal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.thTotal.Value = "Total";

//            // tbSN
//            this.tbSN.Name = "tbSN";
//            this.tbSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.35D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbSN.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbSN.Style.Font.Name = "Segoe UI";
//            this.tbSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbSN.Value = "= RowNumber(\"table1\")";

//            // tbEmpId
//            this.tbEmpId.Name = "tbEmpId";
//            this.tbEmpId.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.95D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbEmpId.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbEmpId.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbEmpId.Style.Font.Name = "Segoe UI";
//            this.tbEmpId.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbEmpId.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
//            this.tbEmpId.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbEmpId.Value = "= Fields.EmployeeCode";

//            // tbEmpName
//            this.tbEmpName.Name = "tbEmpName";
//            this.tbEmpName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbEmpName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbEmpName.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbEmpName.Style.Font.Name = "Segoe UI";
//            this.tbEmpName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbEmpName.Style.Font.Bold = true;
//            this.tbEmpName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
//            this.tbEmpName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbEmpName.Value = "= Fields.FullName";

//            // tbWorkUnit
//            this.tbWorkUnit.Name = "tbWorkUnit";
//            this.tbWorkUnit.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbWorkUnit.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbWorkUnit.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbWorkUnit.Style.Font.Name = "Segoe UI";
//            this.tbWorkUnit.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbWorkUnit.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(3D);
//            this.tbWorkUnit.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbWorkUnit.Value = "= Fields.WorkUnit";

//            // tbM1
//            this.tbM1.Name = "tbM1";
//            this.tbM1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM1.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM1.Style.Font.Name = "Segoe UI";
//            this.tbM1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM1.Value = "= Fields.Month1_Jul";

//            // tbM2
//            this.tbM2.Name = "tbM2";
//            this.tbM2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM2.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM2.Style.Font.Name = "Segoe UI";
//            this.tbM2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM2.Value = "= Fields.Month2_Aug";

//            // tbM3
//            this.tbM3.Name = "tbM3";
//            this.tbM3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM3.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM3.Style.Font.Name = "Segoe UI";
//            this.tbM3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM3.Value = "= Fields.Month3_Sep";

//            // tbM4
//            this.tbM4.Name = "tbM4";
//            this.tbM4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM4.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM4.Style.Font.Name = "Segoe UI";
//            this.tbM4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM4.Value = "= Fields.Month4_Oct";

//            // tbM5
//            this.tbM5.Name = "tbM5";
//            this.tbM5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM5.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM5.Style.Font.Name = "Segoe UI";
//            this.tbM5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM5.Value = "= Fields.Month5_Nov";

//            // tbM6
//            this.tbM6.Name = "tbM6";
//            this.tbM6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM6.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM6.Style.Font.Name = "Segoe UI";
//            this.tbM6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM6.Value = "= Fields.Month6_Dec";

//            // tbM7
//            this.tbM7.Name = "tbM7";
//            this.tbM7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM7.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM7.Style.Font.Name = "Segoe UI";
//            this.tbM7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM7.Value = "= Fields.Month7_Jan";

//            // tbM8
//            this.tbM8.Name = "tbM8";
//            this.tbM8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM8.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM8.Style.Font.Name = "Segoe UI";
//            this.tbM8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM8.Value = "= Fields.Month8_Feb";

//            // tbM9
//            this.tbM9.Name = "tbM9";
//            this.tbM9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM9.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM9.Style.Font.Name = "Segoe UI";
//            this.tbM9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM9.Value = "= Fields.Month9_Mar";

//            // tbM10
//            this.tbM10.Name = "tbM10";
//            this.tbM10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM10.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM10.Style.Font.Name = "Segoe UI";
//            this.tbM10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM10.Value = "= Fields.Month10_Apr";

//            // tbM11
//            this.tbM11.Name = "tbM11";
//            this.tbM11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM11.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM11.Style.Font.Name = "Segoe UI";
//            this.tbM11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM11.Value = "= Fields.Month11_May";

//            // tbM12
//            this.tbM12.Name = "tbM12";
//            this.tbM12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.52D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbM12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbM12.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbM12.Style.Font.Name = "Segoe UI";
//            this.tbM12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbM12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbM12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbM12.Value = "= Fields.Month12_Jun";

//            // tbTotal
//            this.tbTotal.Name = "tbTotal";
//            this.tbTotal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.65D), Telerik.Reporting.Drawing.Unit.Inch(0.24D));
//            this.tbTotal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
//            this.tbTotal.Style.BorderColor.Default = System.Drawing.Color.FromArgb(203, 213, 225);
//            this.tbTotal.Style.Font.Name = "Segoe UI";
//            this.tbTotal.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.tbTotal.Style.Font.Bold = true;
//            this.tbTotal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//            this.tbTotal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
//            this.tbTotal.Style.BackgroundColor = System.Drawing.Color.FromArgb(248, 250, 252);
//            this.tbTotal.Value = "= Fields.TotalAbsentDays";

//            // ---------------------------------------------------------------------
//            // Multi-Tier Header Column Group Assembly
//            // ---------------------------------------------------------------------
//            tgSN.ReportItem = this.thSN;
//            tgEmpId.ReportItem = this.thEmpId;
//            tgEmpName.ReportItem = this.thEmpName;
//            tgWorkUnit.ReportItem = this.thWorkUnit;

//            tgM1.ReportItem = this.thM1;
//            tgM2.ReportItem = this.thM2;
//            tgM3.ReportItem = this.thM3;
//            tgM4.ReportItem = this.thM4;
//            tgM5.ReportItem = this.thM5;
//            tgM6.ReportItem = this.thM6;
//            tgM7.ReportItem = this.thM7;
//            tgM8.ReportItem = this.thM8;
//            tgM9.ReportItem = this.thM9;
//            tgM10.ReportItem = this.thM10;
//            tgM11.ReportItem = this.thM11;
//            tgM12.ReportItem = this.thM12;

//            tgMonthsParent.ReportItem = this.thMonths;
//            tgMonthsParent.ChildGroups.Add(tgM1);
//            tgMonthsParent.ChildGroups.Add(tgM2);
//            tgMonthsParent.ChildGroups.Add(tgM3);
//            tgMonthsParent.ChildGroups.Add(tgM4);
//            tgMonthsParent.ChildGroups.Add(tgM5);
//            tgMonthsParent.ChildGroups.Add(tgM6);
//            tgMonthsParent.ChildGroups.Add(tgM7);
//            tgMonthsParent.ChildGroups.Add(tgM8);
//            tgMonthsParent.ChildGroups.Add(tgM9);
//            tgMonthsParent.ChildGroups.Add(tgM10);
//            tgMonthsParent.ChildGroups.Add(tgM11);
//            tgMonthsParent.ChildGroups.Add(tgM12);

//            tgTotal.ReportItem = this.thTotal;

//            this.table1.ColumnGroups.Add(tgSN);
//            this.table1.ColumnGroups.Add(tgEmpId);
//            this.table1.ColumnGroups.Add(tgEmpName);
//            this.table1.ColumnGroups.Add(tgWorkUnit);
//            this.table1.ColumnGroups.Add(tgMonthsParent);
//            this.table1.ColumnGroups.Add(tgTotal);

//            // Columns and row content
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.35D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.95D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.52D)));
//            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.65D)));

//            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24D)));

//            this.table1.Body.SetCellContent(0, 0, this.tbSN);
//            this.table1.Body.SetCellContent(0, 1, this.tbEmpId);
//            this.table1.Body.SetCellContent(0, 2, this.tbEmpName);
//            this.table1.Body.SetCellContent(0, 3, this.tbWorkUnit);
//            this.table1.Body.SetCellContent(0, 4, this.tbM1);
//            this.table1.Body.SetCellContent(0, 5, this.tbM2);
//            this.table1.Body.SetCellContent(0, 6, this.tbM3);
//            this.table1.Body.SetCellContent(0, 7, this.tbM4);
//            this.table1.Body.SetCellContent(0, 8, this.tbM5);
//            this.table1.Body.SetCellContent(0, 9, this.tbM6);
//            this.table1.Body.SetCellContent(0, 10, this.tbM7);
//            this.table1.Body.SetCellContent(0, 11, this.tbM8);
//            this.table1.Body.SetCellContent(0, 12, this.tbM9);
//            this.table1.Body.SetCellContent(0, 13, this.tbM10);
//            this.table1.Body.SetCellContent(0, 14, this.tbM11);
//            this.table1.Body.SetCellContent(0, 15, this.tbM12);
//            this.table1.Body.SetCellContent(0, 16, this.tbTotal);

//            tgDetailRow.Groupings.Add(new Telerik.Reporting.Grouping(null));
//            tgDetailRow.Name = "detailRow";
//            this.table1.RowGroups.Add(tgDetailRow);

//            // ---------------------------------------------------------------------
//            // Data Sources & Parameter Integration
//            // ---------------------------------------------------------------------
//            this.BranchObjectDataSource.DataMember = "GetBranch";
//            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);

//            this.WorkUnitObjectDataSource.DataMember = "GetDepartment";
//            this.WorkUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);

//            this.EmployeeCodeODS.DataMember = "GetEmployeeCode";
//            this.EmployeeCodeODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EmployeeCodeObjectDataSource);

//            this.FiscalYearODS.DataMember = "GetFiscalYearss";
//            this.FiscalYearODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.FiscalYearDataSource);

//            this.DayAbsentODS.DataMember = "GetDayAbsentSummary";
//            this.DayAbsentODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.DayAbsentReportDataSource);
//            this.DayAbsentODS.Name = "DayAbsentODS";
//            this.DayAbsentODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
//                new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(object[]), "= Parameters.branchId.Value"),
//                new Telerik.Reporting.ObjectDataSourceParameter("businessUnitId", typeof(string), null),
//                new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value"),
//                new Telerik.Reporting.ObjectDataSourceParameter("employee", typeof(string), "= Parameters.employee.Value"),
//                new Telerik.Reporting.ObjectDataSourceParameter("fiscalYearId", typeof(int?), "= Parameters.FiscalYearId.Value")
//            });

//            this.table1.DataSource = this.DayAbsentODS;
//            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.05D));
//            this.table1.Name = "table1";

//            // Sections
//            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3D);
//            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
//                this.txtCompanyAmh, this.txtCompanyEng, this.txtDocNo, this.txtReportTitle, this.txtMetaFilter
//            });

//            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.8D);
//            this.detailSection1.Items.Add(this.table1);

//            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.3D);
//            this.txtPageNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(9.0D), Telerik.Reporting.Drawing.Unit.Inch(0.05D));
//            this.txtPageNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
//            this.txtPageNumber.Style.Font.Name = "Segoe UI";
//            this.txtPageNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
//            this.txtPageNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
//            this.txtPageNumber.Value = "Page: {PageNumber} of {TotalPages}";
//            this.pageFooterSection1.Items.Add(this.txtPageNumber);

//            // Report Parameters
//            reportParameter1.AllowNull = true;
//            reportParameter1.AvailableValues.DataSource = this.BranchObjectDataSource;
//            reportParameter1.AvailableValues.DisplayMember = "= Fields.AmharicName";
//            reportParameter1.AvailableValues.ValueMember = "= Fields.BranchId";
//            reportParameter1.MultiValue = true;
//            reportParameter1.Name = "branchId";
//            reportParameter1.Text = "Branch";
//            reportParameter1.Visible = true;

//            reportParameter2.AllowNull = true;
//            reportParameter2.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
//            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
//            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
//            reportParameter2.Name = "orgStructureId";
//            reportParameter2.Text = "Department";
//            reportParameter2.Visible = true;

//            reportParameter3.AllowNull = true;
//            reportParameter3.AvailableValues.DataSource = this.EmployeeCodeODS;
//            reportParameter3.AvailableValues.DisplayMember = "= Fields.EmployeeCode";
//            reportParameter3.AvailableValues.ValueMember = "= Fields.EmployeeId";
//            reportParameter3.Name = "employee";
//            reportParameter3.Text = "Employee";
//            reportParameter3.Visible = true;

//            reportParameter4.AllowNull = true;
//            reportParameter4.AvailableValues.DataSource = this.FiscalYearODS;
//            reportParameter4.AvailableValues.DisplayMember = "= Fields.Name";
//            reportParameter4.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
//            reportParameter4.Name = "FiscalYearId";
//            reportParameter4.Text = "Fiscal Year";
//            reportParameter4.Visible = true;

//            reportParameter5.AllowNull = true;
//            reportParameter5.Name = "FiscalYearTitle";
//            reportParameter5.Text = "Fiscal Year Label";
//            reportParameter5.Value = "2018 (2025/2026)";

//            this.ReportParameters.Add(reportParameter1);
//            this.ReportParameters.Add(reportParameter2);
//            this.ReportParameters.Add(reportParameter3);
//            this.ReportParameters.Add(reportParameter4);
//            this.ReportParameters.Add(reportParameter5);

//            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
//                this.reportHeaderSection1, this.detailSection1, this.pageFooterSection1
//            });

//            this.Name = "DayAbsentReport";
//            this.PageSettings.Landscape = true;
//            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
//            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(
//                Telerik.Reporting.Drawing.Unit.Inch(0.4D),
//                Telerik.Reporting.Drawing.Unit.Inch(0.4D),
//                Telerik.Reporting.Drawing.Unit.Inch(0.4D),
//                Telerik.Reporting.Drawing.Unit.Inch(0.4D)
//            );
//            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.0D);

//            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
//        }
//        #endregion

//        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
//        private Telerik.Reporting.DetailSection detailSection1;
//        private Telerik.Reporting.PageFooterSection pageFooterSection1;
//        private Telerik.Reporting.Table table1;

//        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
//        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
//        private Telerik.Reporting.ObjectDataSource EmployeeCodeODS;
//        private Telerik.Reporting.ObjectDataSource FiscalYearODS;
//        private Telerik.Reporting.ObjectDataSource DayAbsentODS;

//        private Telerik.Reporting.TextBox txtCompanyAmh;
//        private Telerik.Reporting.TextBox txtCompanyEng;
//        private Telerik.Reporting.TextBox txtDocNo;
//        private Telerik.Reporting.TextBox txtReportTitle;
//        private Telerik.Reporting.TextBox txtMetaFilter;
//        private Telerik.Reporting.TextBox txtPageNumber;

//        // Column Headers
//        private Telerik.Reporting.TextBox thSN;
//        private Telerik.Reporting.TextBox thEmpId;
//        private Telerik.Reporting.TextBox thEmpName;
//        private Telerik.Reporting.TextBox thWorkUnit;
//        private Telerik.Reporting.TextBox thMonths;
//        private Telerik.Reporting.TextBox thM1;
//        private Telerik.Reporting.TextBox thM2;
//        private Telerik.Reporting.TextBox thM3;
//        private Telerik.Reporting.TextBox thM4;
//        private Telerik.Reporting.TextBox thM5;
//        private Telerik.Reporting.TextBox thM6;
//        private Telerik.Reporting.TextBox thM7;
//        private Telerik.Reporting.TextBox thM8;
//        private Telerik.Reporting.TextBox thM9;
//        private Telerik.Reporting.TextBox thM10;
//        private Telerik.Reporting.TextBox thM11;
//        private Telerik.Reporting.TextBox thM12;
//        private Telerik.Reporting.TextBox thTotal;

//        // Detail Cells
//        private Telerik.Reporting.TextBox tbSN;
//        private Telerik.Reporting.TextBox tbEmpId;
//        private Telerik.Reporting.TextBox tbEmpName;
//        private Telerik.Reporting.TextBox tbWorkUnit;
//        private Telerik.Reporting.TextBox tbM1;
//        private Telerik.Reporting.TextBox tbM2;
//        private Telerik.Reporting.TextBox tbM3;
//        private Telerik.Reporting.TextBox tbM4;
//        private Telerik.Reporting.TextBox tbM5;
//        private Telerik.Reporting.TextBox tbM6;
//        private Telerik.Reporting.TextBox tbM7;
//        private Telerik.Reporting.TextBox tbM8;
//        private Telerik.Reporting.TextBox tbM9;
//        private Telerik.Reporting.TextBox tbM10;
//        private Telerik.Reporting.TextBox tbM11;
//        private Telerik.Reporting.TextBox tbM12;
//        private Telerik.Reporting.TextBox tbTotal;
//    }
//}