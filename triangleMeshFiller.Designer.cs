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
            this.lightColorGbox = new System.Windows.Forms.GroupBox();
            this.changeColorButton = new System.Windows.Forms.Button();
            this.lightColorPreview = new System.Windows.Forms.PictureBox();
            this.coefficientsGbox = new System.Windows.Forms.GroupBox();
            this.mGbox = new System.Windows.Forms.GroupBox();
            this.mTrackBar = new System.Windows.Forms.TrackBar();
            this.mTxtBox = new System.Windows.Forms.TextBox();
            this.ksGbox = new System.Windows.Forms.GroupBox();
            this.ksTrackBar = new System.Windows.Forms.TrackBar();
            this.ksTxtBox = new System.Windows.Forms.TextBox();
            this.kdGbox = new System.Windows.Forms.GroupBox();
            this.kdTrackBar = new System.Windows.Forms.TrackBar();
            this.kdTxtBox = new System.Windows.Forms.TextBox();
            this.colorDeterminationMethodGbox = new System.Windows.Forms.GroupBox();
            this.vertexInterpolationRbutton = new System.Windows.Forms.RadioButton();
            this.calcAtPointRbutton = new System.Windows.Forms.RadioButton();
            this.showGbox = new System.Windows.Forms.GroupBox();
            this.showVerticiesCbox = new System.Windows.Forms.CheckBox();
            this.showEdgesCbox = new System.Windows.Forms.CheckBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.lightColorGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightColorPreview)).BeginInit();
            this.coefficientsGbox.SuspendLayout();
            this.mGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mTrackBar)).BeginInit();
            this.ksGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ksTrackBar)).BeginInit();
            this.kdGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kdTrackBar)).BeginInit();
            this.colorDeterminationMethodGbox.SuspendLayout();
            this.showGbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
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
            this.splitContainer1.Panel2.Controls.Add(this.lightColorGbox);
            this.splitContainer1.Panel2.Controls.Add(this.coefficientsGbox);
            this.splitContainer1.Panel2.Controls.Add(this.colorDeterminationMethodGbox);
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
            // lightColorGbox
            // 
            this.lightColorGbox.Controls.Add(this.changeColorButton);
            this.lightColorGbox.Controls.Add(this.lightColorPreview);
            this.lightColorGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.lightColorGbox.Location = new System.Drawing.Point(0, 516);
            this.lightColorGbox.Name = "lightColorGbox";
            this.lightColorGbox.Size = new System.Drawing.Size(257, 70);
            this.lightColorGbox.TabIndex = 1;
            this.lightColorGbox.TabStop = false;
            this.lightColorGbox.Text = "Color of light";
            // 
            // changeColorButton
            // 
            this.changeColorButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.changeColorButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.changeColorButton.Location = new System.Drawing.Point(84, 23);
            this.changeColorButton.MaximumSize = new System.Drawing.Size(170, 40);
            this.changeColorButton.MinimumSize = new System.Drawing.Size(170, 40);
            this.changeColorButton.Name = "changeColorButton";
            this.changeColorButton.Size = new System.Drawing.Size(170, 40);
            this.changeColorButton.TabIndex = 1;
            this.changeColorButton.Text = "Change color";
            this.changeColorButton.UseVisualStyleBackColor = true;
            this.changeColorButton.Click += new System.EventHandler(this.changeColorButton_Click);
            // 
            // lightColorPreview
            // 
            this.lightColorPreview.BackColor = System.Drawing.Color.White;
            this.lightColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lightColorPreview.Dock = System.Windows.Forms.DockStyle.Left;
            this.lightColorPreview.Location = new System.Drawing.Point(3, 23);
            this.lightColorPreview.MaximumSize = new System.Drawing.Size(80, 40);
            this.lightColorPreview.MinimumSize = new System.Drawing.Size(80, 40);
            this.lightColorPreview.Name = "lightColorPreview";
            this.lightColorPreview.Size = new System.Drawing.Size(80, 40);
            this.lightColorPreview.TabIndex = 0;
            this.lightColorPreview.TabStop = false;
            // 
            // coefficientsGbox
            // 
            this.coefficientsGbox.AutoSize = true;
            this.coefficientsGbox.Controls.Add(this.mGbox);
            this.coefficientsGbox.Controls.Add(this.ksGbox);
            this.coefficientsGbox.Controls.Add(this.kdGbox);
            this.coefficientsGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.coefficientsGbox.Location = new System.Drawing.Point(0, 215);
            this.coefficientsGbox.Name = "coefficientsGbox";
            this.coefficientsGbox.Size = new System.Drawing.Size(257, 301);
            this.coefficientsGbox.TabIndex = 8;
            this.coefficientsGbox.TabStop = false;
            this.coefficientsGbox.Text = "Coefficients";
            // 
            // mGbox
            // 
            this.mGbox.AutoSize = true;
            this.mGbox.Controls.Add(this.mTrackBar);
            this.mGbox.Controls.Add(this.mTxtBox);
            this.mGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mGbox.Location = new System.Drawing.Point(3, 215);
            this.mGbox.Name = "mGbox";
            this.mGbox.Size = new System.Drawing.Size(251, 83);
            this.mGbox.TabIndex = 5;
            this.mGbox.TabStop = false;
            this.mGbox.Text = "m value";
            // 
            // mTrackBar
            // 
            this.mTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.mTrackBar.Location = new System.Drawing.Point(3, 50);
            this.mTrackBar.Maximum = 100000;
            this.mTrackBar.MaximumSize = new System.Drawing.Size(0, 30);
            this.mTrackBar.Minimum = 1000;
            this.mTrackBar.Name = "mTrackBar";
            this.mTrackBar.Size = new System.Drawing.Size(245, 30);
            this.mTrackBar.TabIndex = 0;
            this.mTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mTrackBar.Value = 50500;
            this.mTrackBar.ValueChanged += new System.EventHandler(this.mTrackBar_ValueChanged);
            // 
            // mTxtBox
            // 
            this.mTxtBox.BackColor = System.Drawing.SystemColors.Control;
            this.mTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTxtBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mTxtBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mTxtBox.Location = new System.Drawing.Point(3, 23);
            this.mTxtBox.Name = "mTxtBox";
            this.mTxtBox.ReadOnly = true;
            this.mTxtBox.Size = new System.Drawing.Size(245, 27);
            this.mTxtBox.TabIndex = 1;
            this.mTxtBox.Text = "50.500";
            this.mTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ksGbox
            // 
            this.ksGbox.Controls.Add(this.ksTrackBar);
            this.ksGbox.Controls.Add(this.ksTxtBox);
            this.ksGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ksGbox.Location = new System.Drawing.Point(3, 119);
            this.ksGbox.Name = "ksGbox";
            this.ksGbox.Size = new System.Drawing.Size(251, 96);
            this.ksGbox.TabIndex = 6;
            this.ksGbox.TabStop = false;
            this.ksGbox.Text = "ks value";
            // 
            // ksTrackBar
            // 
            this.ksTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.ksTrackBar.Location = new System.Drawing.Point(3, 50);
            this.ksTrackBar.Maximum = 1000;
            this.ksTrackBar.MaximumSize = new System.Drawing.Size(0, 30);
            this.ksTrackBar.Name = "ksTrackBar";
            this.ksTrackBar.Size = new System.Drawing.Size(245, 30);
            this.ksTrackBar.TabIndex = 0;
            this.ksTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.ksTrackBar.Value = 500;
            this.ksTrackBar.ValueChanged += new System.EventHandler(this.ksTrackBar_ValueChanged);
            // 
            // ksTxtBox
            // 
            this.ksTxtBox.BackColor = System.Drawing.SystemColors.Control;
            this.ksTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ksTxtBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ksTxtBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ksTxtBox.Location = new System.Drawing.Point(3, 23);
            this.ksTxtBox.Name = "ksTxtBox";
            this.ksTxtBox.ReadOnly = true;
            this.ksTxtBox.Size = new System.Drawing.Size(245, 27);
            this.ksTxtBox.TabIndex = 1;
            this.ksTxtBox.Text = "0.500";
            this.ksTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // kdGbox
            // 
            this.kdGbox.Controls.Add(this.kdTrackBar);
            this.kdGbox.Controls.Add(this.kdTxtBox);
            this.kdGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.kdGbox.Location = new System.Drawing.Point(3, 23);
            this.kdGbox.Name = "kdGbox";
            this.kdGbox.Size = new System.Drawing.Size(251, 96);
            this.kdGbox.TabIndex = 7;
            this.kdGbox.TabStop = false;
            this.kdGbox.Text = "kd value";
            // 
            // kdTrackBar
            // 
            this.kdTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.kdTrackBar.Location = new System.Drawing.Point(3, 50);
            this.kdTrackBar.Maximum = 1000;
            this.kdTrackBar.MaximumSize = new System.Drawing.Size(0, 30);
            this.kdTrackBar.Name = "kdTrackBar";
            this.kdTrackBar.Size = new System.Drawing.Size(245, 30);
            this.kdTrackBar.TabIndex = 0;
            this.kdTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.kdTrackBar.Value = 500;
            this.kdTrackBar.ValueChanged += new System.EventHandler(this.kdTrackBar_ValueChanged);
            // 
            // kdTxtBox
            // 
            this.kdTxtBox.BackColor = System.Drawing.SystemColors.Control;
            this.kdTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.kdTxtBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.kdTxtBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.kdTxtBox.Location = new System.Drawing.Point(3, 23);
            this.kdTxtBox.Name = "kdTxtBox";
            this.kdTxtBox.ReadOnly = true;
            this.kdTxtBox.Size = new System.Drawing.Size(245, 27);
            this.kdTxtBox.TabIndex = 1;
            this.kdTxtBox.Text = "0.500";
            this.kdTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colorDeterminationMethodGbox
            // 
            this.colorDeterminationMethodGbox.AutoSize = true;
            this.colorDeterminationMethodGbox.Controls.Add(this.vertexInterpolationRbutton);
            this.colorDeterminationMethodGbox.Controls.Add(this.calcAtPointRbutton);
            this.colorDeterminationMethodGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorDeterminationMethodGbox.Location = new System.Drawing.Point(0, 110);
            this.colorDeterminationMethodGbox.Name = "colorDeterminationMethodGbox";
            this.colorDeterminationMethodGbox.Size = new System.Drawing.Size(257, 105);
            this.colorDeterminationMethodGbox.TabIndex = 7;
            this.colorDeterminationMethodGbox.TabStop = false;
            this.colorDeterminationMethodGbox.Text = "Color determination method";
            // 
            // vertexInterpolationRbutton
            // 
            this.vertexInterpolationRbutton.Appearance = System.Windows.Forms.Appearance.Button;
            this.vertexInterpolationRbutton.AutoSize = true;
            this.vertexInterpolationRbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.vertexInterpolationRbutton.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.vertexInterpolationRbutton.Location = new System.Drawing.Point(3, 60);
            this.vertexInterpolationRbutton.Name = "vertexInterpolationRbutton";
            this.vertexInterpolationRbutton.Size = new System.Drawing.Size(251, 42);
            this.vertexInterpolationRbutton.TabIndex = 1;
            this.vertexInterpolationRbutton.Text = "Vertex interpolation";
            this.vertexInterpolationRbutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.vertexInterpolationRbutton.UseVisualStyleBackColor = true;
            // 
            // calcAtPointRbutton
            // 
            this.calcAtPointRbutton.Appearance = System.Windows.Forms.Appearance.Button;
            this.calcAtPointRbutton.AutoSize = true;
            this.calcAtPointRbutton.Checked = true;
            this.calcAtPointRbutton.Dock = System.Windows.Forms.DockStyle.Top;
            this.calcAtPointRbutton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.calcAtPointRbutton.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.calcAtPointRbutton.Location = new System.Drawing.Point(3, 23);
            this.calcAtPointRbutton.Name = "calcAtPointRbutton";
            this.calcAtPointRbutton.Size = new System.Drawing.Size(251, 37);
            this.calcAtPointRbutton.TabIndex = 0;
            this.calcAtPointRbutton.TabStop = true;
            this.calcAtPointRbutton.Text = "Calculated at point";
            this.calcAtPointRbutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.calcAtPointRbutton.UseVisualStyleBackColor = true;
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
            this.showVerticiesCbox.Checked = true;
            this.showVerticiesCbox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            this.showEdgesCbox.Checked = true;
            this.showEdgesCbox.CheckState = System.Windows.Forms.CheckState.Checked;
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
            this.BackColor = System.Drawing.SystemColors.Control;
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
            this.lightColorGbox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lightColorPreview)).EndInit();
            this.coefficientsGbox.ResumeLayout(false);
            this.coefficientsGbox.PerformLayout();
            this.mGbox.ResumeLayout(false);
            this.mGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mTrackBar)).EndInit();
            this.ksGbox.ResumeLayout(false);
            this.ksGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ksTrackBar)).EndInit();
            this.kdGbox.ResumeLayout(false);
            this.kdGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kdTrackBar)).EndInit();
            this.colorDeterminationMethodGbox.ResumeLayout(false);
            this.colorDeterminationMethodGbox.PerformLayout();
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
        private GroupBox colorDeterminationMethodGbox;
        private RadioButton vertexInterpolationRbutton;
        private RadioButton calcAtPointRbutton;
        private GroupBox coefficientsGbox;
        private TrackBar kdTrackBar;
        private TextBox mTxtBox;
        private TrackBar mTrackBar;
        private TextBox ksTxtBox;
        private TrackBar ksTrackBar;
        private TextBox kdTxtBox;
        private GroupBox mGbox;
        private GroupBox ksGbox;
        private GroupBox kdGbox;
        private GroupBox lightColorGbox;
        private Button changeColorButton;
        private PictureBox lightColorPreview;
        private ColorDialog colorDialog1;
    }
}