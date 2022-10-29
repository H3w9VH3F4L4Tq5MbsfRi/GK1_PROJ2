namespace GK1_PROJ2
{
    partial class mainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.clearCanvasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadobjFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.showGbox = new System.Windows.Forms.GroupBox();
            this.showVerticiesCbox = new System.Windows.Forms.CheckBox();
            this.showEdgesCbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.showGbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearCanvasToolStripMenuItem,
            this.loadobjFileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(982, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // clearCanvasToolStripMenuItem
            // 
            this.clearCanvasToolStripMenuItem.Name = "clearCanvasToolStripMenuItem";
            this.clearCanvasToolStripMenuItem.Size = new System.Drawing.Size(105, 24);
            this.clearCanvasToolStripMenuItem.Text = "Clear canvas";
            this.clearCanvasToolStripMenuItem.Click += new System.EventHandler(this.clearCanvasToolStripMenuItem_Click);
            // 
            // loadobjFileToolStripMenuItem
            // 
            this.loadobjFileToolStripMenuItem.Name = "loadobjFileToolStripMenuItem";
            this.loadobjFileToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.loadobjFileToolStripMenuItem.Text = "Load .obj file ";
            this.loadobjFileToolStripMenuItem.Click += new System.EventHandler(this.loadobjFileToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 30);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.canvas);
            this.splitContainer1.Panel1MinSize = 721;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.showGbox);
            this.splitContainer1.Size = new System.Drawing.Size(982, 721);
            this.splitContainer1.SplitterDistance = 721;
            this.splitContainer1.TabIndex = 1;
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(721, 721);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // showGbox
            // 
            this.showGbox.AutoSize = true;
            this.showGbox.Controls.Add(this.showVerticiesCbox);
            this.showGbox.Controls.Add(this.showEdgesCbox);
            this.showGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.showGbox.Location = new System.Drawing.Point(0, 0);
            this.showGbox.Name = "showGbox";
            this.showGbox.Size = new System.Drawing.Size(257, 110);
            this.showGbox.TabIndex = 6;
            this.showGbox.TabStop = false;
            this.showGbox.Text = "Visibility";
            // 
            // showVerticiesCbox
            // 
            this.showVerticiesCbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.showVerticiesCbox.AutoSize = true;
            this.showVerticiesCbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.showVerticiesCbox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.showVerticiesCbox.Location = new System.Drawing.Point(3, 65);
            this.showVerticiesCbox.Name = "showVerticiesCbox";
            this.showVerticiesCbox.Size = new System.Drawing.Size(251, 42);
            this.showVerticiesCbox.TabIndex = 4;
            this.showVerticiesCbox.Text = "Show verticies";
            this.showVerticiesCbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.showVerticiesCbox.UseVisualStyleBackColor = true;
            this.showVerticiesCbox.CheckedChanged += new System.EventHandler(this.showCbox_CheckedChanged);
            // 
            // showEdgesCbox
            // 
            this.showEdgesCbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.showEdgesCbox.AutoSize = true;
            this.showEdgesCbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.showEdgesCbox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.showEdgesCbox.Location = new System.Drawing.Point(3, 23);
            this.showEdgesCbox.Name = "showEdgesCbox";
            this.showEdgesCbox.Size = new System.Drawing.Size(251, 42);
            this.showEdgesCbox.TabIndex = 5;
            this.showEdgesCbox.Text = "Show edges";
            this.showEdgesCbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.showEdgesCbox.UseVisualStyleBackColor = true;
            this.showEdgesCbox.CheckedChanged += new System.EventHandler(this.showCbox_CheckedChanged);
            // 
            // mainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 751);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 798);
            this.MinimumSize = new System.Drawing.Size(1000, 798);
            this.Name = "mainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Triangle mesh filler";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.showGbox.ResumeLayout(false);
            this.showGbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem clearCanvasToolStripMenuItem;
        private ToolStripMenuItem loadobjFileToolStripMenuItem;
        private SplitContainer splitContainer1;
        private PictureBox canvas;
        private CheckBox showVerticiesCbox;
        private CheckBox showEdgesCbox;
        private GroupBox showGbox;
    }
}