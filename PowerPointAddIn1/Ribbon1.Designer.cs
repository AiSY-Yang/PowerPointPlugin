namespace PowerPointAddIn1
{
	partial class Ribbon1 : Microsoft.Office.Tools.Ribbon.RibbonBase
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Ribbon1()
			: base(Globals.Factory.GetRibbonFactory())
		{
			InitializeComponent();
		}

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.tab1 = this.Factory.CreateRibbonTab();
			this.grouptitle = this.Factory.CreateRibbonGroup();
			this.buttonTitlePosition = this.Factory.CreateRibbonButton();
			this.buttonTitleFont = this.Factory.CreateRibbonButton();
			this.group2 = this.Factory.CreateRibbonGroup();
			this.buttonFont = this.Factory.CreateRibbonButton();
			this.CADSelect = this.Factory.CreateRibbonCheckBox();
			this.tab1.SuspendLayout();
			this.grouptitle.SuspendLayout();
			this.group2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tab1
			// 
			this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
			this.tab1.Groups.Add(this.grouptitle);
			this.tab1.Groups.Add(this.group2);
			this.tab1.Label = "yk";
			this.tab1.Name = "tab1";
			// 
			// grouptitle
			// 
			this.grouptitle.Items.Add(this.buttonTitlePosition);
			this.grouptitle.Items.Add(this.buttonTitleFont);
			this.grouptitle.Label = "标题";
			this.grouptitle.Name = "grouptitle";
			// 
			// buttonTitlePosition
			// 
			this.buttonTitlePosition.Label = "标题位置统一";
			this.buttonTitlePosition.Name = "buttonTitlePosition";
			this.buttonTitlePosition.Tag = "Position";
			this.buttonTitlePosition.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonTitle_Click);
			// 
			// buttonTitleFont
			// 
			this.buttonTitleFont.Label = "标题字体统一";
			this.buttonTitleFont.Name = "buttonTitleFont";
			this.buttonTitleFont.Tag = "Font";
			this.buttonTitleFont.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonTitle_Click);
			// 
			// group2
			// 
			this.group2.Items.Add(this.buttonFont);
			this.group2.Items.Add(this.CADSelect);
			this.group2.Label = "group2";
			this.group2.Name = "group2";
			// 
			// buttonFont
			// 
			this.buttonFont.Label = "全文字体统一";
			this.buttonFont.Name = "buttonFont";
			this.buttonFont.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonFont_Click);
			// 
			// CADSelect
			// 
			this.CADSelect.Checked = true;
			this.CADSelect.Label = "CAD式选择";
			this.CADSelect.Name = "CADSelect";
			this.CADSelect.ScreenTip = "像CAD一样选择元素";
			this.CADSelect.SuperTip = "左上到右下";
			this.CADSelect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CADSelect_Click);
			// 
			// Ribbon1
			// 
			this.Name = "Ribbon1";
			this.RibbonType = "Microsoft.PowerPoint.Presentation";
			this.Tabs.Add(this.tab1);
			this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
			this.tab1.ResumeLayout(false);
			this.tab1.PerformLayout();
			this.grouptitle.ResumeLayout(false);
			this.grouptitle.PerformLayout();
			this.group2.ResumeLayout(false);
			this.group2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
		internal Microsoft.Office.Tools.Ribbon.RibbonGroup grouptitle;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonTitlePosition;
		internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonFont;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonTitleFont;
		internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox CADSelect;
	}

	partial class ThisRibbonCollection
	{
		internal Ribbon1 Ribbon1
		{
			get { return this.GetRibbon<Ribbon1>(); }
		}
	}
}
