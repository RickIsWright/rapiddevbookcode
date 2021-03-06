namespace AW.Win
{
    partial class FrmOrganization
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
          this.components = new System.ComponentModel.Container();
          this.tvOrganization = new System.Windows.Forms.TreeView();
          this.btnSearch = new System.Windows.Forms.Button();
          this.panel1 = new System.Windows.Forms.Panel();
          this.buttonSearchLINQ = new System.Windows.Forms.Button();
          this.btnSearchSP = new System.Windows.Forms.Button();
          this.cbEmployee = new System.Windows.Forms.ComboBox();
          this.btnSearchNon = new System.Windows.Forms.Button();
          this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
          this.panel1.SuspendLayout();
          this.SuspendLayout();
          // 
          // tvOrganization
          // 
          this.tvOrganization.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tvOrganization.Location = new System.Drawing.Point(0, 59);
          this.tvOrganization.Name = "tvOrganization";
          this.tvOrganization.Size = new System.Drawing.Size(380, 367);
          this.tvOrganization.TabIndex = 2;
          // 
          // btnSearch
          // 
          this.btnSearch.Location = new System.Drawing.Point(13, 30);
          this.btnSearch.Name = "btnSearch";
          this.btnSearch.Size = new System.Drawing.Size(75, 23);
          this.btnSearch.TabIndex = 5;
          this.btnSearch.Text = "Search";
          this.toolTip1.SetToolTip(this.btnSearch, "By using a prefetch all a manager\'s employees a bought back in 1 query");
          this.btnSearch.UseVisualStyleBackColor = true;
          this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
          // 
          // panel1
          // 
          this.panel1.Controls.Add(this.buttonSearchLINQ);
          this.panel1.Controls.Add(this.btnSearchSP);
          this.panel1.Controls.Add(this.cbEmployee);
          this.panel1.Controls.Add(this.btnSearchNon);
          this.panel1.Controls.Add(this.btnSearch);
          this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
          this.panel1.Location = new System.Drawing.Point(0, 0);
          this.panel1.Name = "panel1";
          this.panel1.Size = new System.Drawing.Size(380, 59);
          this.panel1.TabIndex = 6;
          // 
          // buttonSearchLINQ
          // 
          this.buttonSearchLINQ.Location = new System.Drawing.Point(245, 30);
          this.buttonSearchLINQ.Name = "buttonSearchLINQ";
          this.buttonSearchLINQ.Size = new System.Drawing.Size(115, 23);
          this.buttonSearchLINQ.TabIndex = 9;
          this.buttonSearchLINQ.Text = "Search using LINQ";
          this.toolTip1.SetToolTip(this.buttonSearchLINQ, "Search using LINQ with prefetch");
          this.buttonSearchLINQ.UseVisualStyleBackColor = true;
          this.buttonSearchLINQ.Click += new System.EventHandler(this.BtnSearchLinqPrefetchClick);
          // 
          // btnSearchSP
          // 
          this.btnSearchSP.Location = new System.Drawing.Point(203, 4);
          this.btnSearchSP.Name = "btnSearchSP";
          this.btnSearchSP.Size = new System.Drawing.Size(171, 23);
          this.btnSearchSP.TabIndex = 8;
          this.btnSearchSP.Text = "Search using stored procedure";
          this.toolTip1.SetToolTip(this.btnSearchSP, "Search using stored procedure");
          this.btnSearchSP.UseVisualStyleBackColor = true;
          this.btnSearchSP.Click += new System.EventHandler(this.btnSearchSP_Click);
          // 
          // cbEmployee
          // 
          this.cbEmployee.FormattingEnabled = true;
          this.cbEmployee.Location = new System.Drawing.Point(13, 5);
          this.cbEmployee.Name = "cbEmployee";
          this.cbEmployee.Size = new System.Drawing.Size(166, 21);
          this.cbEmployee.TabIndex = 7;
          // 
          // btnSearchNon
          // 
          this.btnSearchNon.Location = new System.Drawing.Point(94, 30);
          this.btnSearchNon.Name = "btnSearchNon";
          this.btnSearchNon.Size = new System.Drawing.Size(136, 23);
          this.btnSearchNon.TabIndex = 6;
          this.btnSearchNon.Text = "Search without Prefetch";
          this.toolTip1.SetToolTip(this.btnSearchNon, "Search without Prefetch maybe slow");
          this.btnSearchNon.UseVisualStyleBackColor = true;
          this.btnSearchNon.Click += new System.EventHandler(this.buttonSearchNoPrefetch_Click);
          // 
          // FrmOrganization
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(380, 426);
          this.Controls.Add(this.tvOrganization);
          this.Controls.Add(this.panel1);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
          this.Name = "FrmOrganization";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Tag = "True";
          this.Text = "Adventure Works Organization";
          this.Load += new System.EventHandler(this.frmOrganization_Load);
          this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOrganization_FormClosing);
          this.panel1.ResumeLayout(false);
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvOrganization;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSearchNon;
        private System.Windows.Forms.ComboBox cbEmployee;
        private System.Windows.Forms.Button btnSearchSP;
        private System.Windows.Forms.Button buttonSearchLINQ;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}