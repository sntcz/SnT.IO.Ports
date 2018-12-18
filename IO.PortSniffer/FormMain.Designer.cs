namespace SnT.IO.PortSniffer
{
    partial class FormMain
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
            this.propertyGridLeftPort = new System.Windows.Forms.PropertyGrid();
            this.groupBoxLeftPort = new System.Windows.Forms.GroupBox();
            this.splitContainerPorts = new System.Windows.Forms.SplitContainer();
            this.groupBoxRightPort = new System.Windows.Forms.GroupBox();
            this.propertyGridRightPort = new System.Windows.Forms.PropertyGrid();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.listBoxData = new System.Windows.Forms.ListBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxLeftPort.SuspendLayout();
            this.splitContainerPorts.Panel1.SuspendLayout();
            this.splitContainerPorts.Panel2.SuspendLayout();
            this.splitContainerPorts.SuspendLayout();
            this.groupBoxRightPort.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridLeftPort
            // 
            this.propertyGridLeftPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridLeftPort.Location = new System.Drawing.Point(3, 16);
            this.propertyGridLeftPort.Name = "propertyGridLeftPort";
            this.propertyGridLeftPort.Size = new System.Drawing.Size(295, 310);
            this.propertyGridLeftPort.TabIndex = 0;
            // 
            // groupBoxLeftPort
            // 
            this.groupBoxLeftPort.Controls.Add(this.propertyGridLeftPort);
            this.groupBoxLeftPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLeftPort.Location = new System.Drawing.Point(0, 0);
            this.groupBoxLeftPort.Name = "groupBoxLeftPort";
            this.groupBoxLeftPort.Size = new System.Drawing.Size(301, 329);
            this.groupBoxLeftPort.TabIndex = 0;
            this.groupBoxLeftPort.TabStop = false;
            this.groupBoxLeftPort.Text = "Left";
            // 
            // splitContainerPorts
            // 
            this.splitContainerPorts.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainerPorts.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPorts.Name = "splitContainerPorts";
            // 
            // splitContainerPorts.Panel1
            // 
            this.splitContainerPorts.Panel1.Controls.Add(this.groupBoxLeftPort);
            // 
            // splitContainerPorts.Panel2
            // 
            this.splitContainerPorts.Panel2.Controls.Add(this.groupBoxRightPort);
            this.splitContainerPorts.Size = new System.Drawing.Size(603, 329);
            this.splitContainerPorts.SplitterDistance = 301;
            this.splitContainerPorts.TabIndex = 2;
            // 
            // groupBoxRightPort
            // 
            this.groupBoxRightPort.Controls.Add(this.propertyGridRightPort);
            this.groupBoxRightPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRightPort.Location = new System.Drawing.Point(0, 0);
            this.groupBoxRightPort.Name = "groupBoxRightPort";
            this.groupBoxRightPort.Size = new System.Drawing.Size(298, 329);
            this.groupBoxRightPort.TabIndex = 2;
            this.groupBoxRightPort.TabStop = false;
            this.groupBoxRightPort.Text = "Right";
            // 
            // propertyGridRightPort
            // 
            this.propertyGridRightPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridRightPort.Location = new System.Drawing.Point(3, 16);
            this.propertyGridRightPort.Name = "propertyGridRightPort";
            this.propertyGridRightPort.Size = new System.Drawing.Size(292, 310);
            this.propertyGridRightPort.TabIndex = 0;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonAbout);
            this.panelButtons.Controls.Add(this.buttonSave);
            this.panelButtons.Controls.Add(this.buttonClear);
            this.panelButtons.Controls.Add(this.buttonClose);
            this.panelButtons.Controls.Add(this.buttonOpen);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 329);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(603, 29);
            this.panelButtons.TabIndex = 3;
            // 
            // buttonAbout
            // 
            this.buttonAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAbout.Location = new System.Drawing.Point(577, 3);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(23, 23);
            this.buttonAbout.TabIndex = 4;
            this.buttonAbout.Text = "?";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(405, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(80, 23);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(491, 3);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(80, 23);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(89, 3);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(3, 3);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(80, 23);
            this.buttonOpen.TabIndex = 0;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // listBoxData
            // 
            this.listBoxData.DisplayMember = "Text";
            this.listBoxData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxData.HorizontalScrollbar = true;
            this.listBoxData.Location = new System.Drawing.Point(0, 358);
            this.listBoxData.Name = "listBoxData";
            this.listBoxData.Size = new System.Drawing.Size(603, 192);
            this.listBoxData.TabIndex = 0;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 528);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(603, 22);
            this.statusBar.TabIndex = 4;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(557, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 550);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.listBoxData);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.splitContainerPorts);
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "FormMain";
            this.Text = "Serial Port Sniffer";
            this.groupBoxLeftPort.ResumeLayout(false);
            this.splitContainerPorts.Panel1.ResumeLayout(false);
            this.splitContainerPorts.Panel2.ResumeLayout(false);
            this.splitContainerPorts.ResumeLayout(false);
            this.groupBoxRightPort.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGridLeftPort;
        private System.Windows.Forms.GroupBox groupBoxLeftPort;
        private System.Windows.Forms.SplitContainer splitContainerPorts;
        private System.Windows.Forms.GroupBox groupBoxRightPort;
        private System.Windows.Forms.PropertyGrid propertyGridRightPort;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.ListBox listBoxData;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonAbout;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}

