// Relative Path: Form1.Designer.cs
namespace Beam_Manager
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            modsPathBox = new TextBox();
            selectModsPathBtn = new Button();
            label1 = new Label();
            scanModsBtn = new Button();
            flowVehicles = new BufferedFlowLayoutPanel();
            flowConfigs = new BufferedFlowLayoutPanel();
            statusLabelFile = new Label();
            statusLabelCount = new Label();
            progressBarScan = new ProgressBar();
            backBtn = new Button();
            appIcon1 = new AppIcon();
            ((System.ComponentModel.ISupportInitialize)appIcon1).BeginInit();
            SuspendLayout();
            // 
            // modsPathBox
            // 
            modsPathBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            modsPathBox.BorderStyle = BorderStyle.FixedSingle;
            modsPathBox.Location = new Point(50, 84);
            modsPathBox.Name = "modsPathBox";
            modsPathBox.Size = new Size(1127, 23);
            modsPathBox.TabIndex = 12;
            // 
            // selectModsPathBtn
            // 
            selectModsPathBtn.FlatStyle = FlatStyle.Flat;
            selectModsPathBtn.Image = Properties.Resources.open_folderBlue32;
            selectModsPathBtn.Location = new Point(12, 75);
            selectModsPathBtn.Name = "selectModsPathBtn";
            selectModsPathBtn.Size = new Size(32, 32);
            selectModsPathBtn.TabIndex = 11;
            selectModsPathBtn.UseVisualStyleBackColor = true;
            selectModsPathBtn.Click += selectModsPathBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(50, 65);
            label1.Name = "label1";
            label1.Size = new Size(123, 15);
            label1.TabIndex = 13;
            label1.Text = "BeamNG Mods Folder";
            // 
            // scanModsBtn
            // 
            scanModsBtn.FlatStyle = FlatStyle.System;
            scanModsBtn.Location = new Point(12, 130);
            scanModsBtn.Name = "scanModsBtn";
            scanModsBtn.Size = new Size(100, 30);
            scanModsBtn.TabIndex = 15;
            scanModsBtn.Text = "Scan Mods";
            scanModsBtn.UseVisualStyleBackColor = true;
            scanModsBtn.Click += scanModsBtn_Click;
            // 
            // flowVehicles
            // 
            flowVehicles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowVehicles.AutoScroll = true;
            flowVehicles.BackColor = Color.FromArgb(33, 33, 33);
            flowVehicles.BorderStyle = BorderStyle.FixedSingle;
            flowVehicles.Location = new Point(12, 172);
            flowVehicles.Name = "flowVehicles";
            flowVehicles.Size = new Size(1165, 726);
            flowVehicles.TabIndex = 16;
            // 
            // flowConfigs
            // 
            flowConfigs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowConfigs.AutoScroll = true;
            flowConfigs.BackColor = Color.FromArgb(33, 33, 33);
            flowConfigs.BorderStyle = BorderStyle.FixedSingle;
            flowConfigs.Location = new Point(12, 172);
            flowConfigs.Name = "flowConfigs";
            flowConfigs.Size = new Size(1165, 726);
            flowConfigs.TabIndex = 21;
            flowConfigs.Visible = false;
            // 
            // statusLabelFile
            // 
            statusLabelFile.AutoEllipsis = true;
            statusLabelFile.Location = new Point(120, 138);
            statusLabelFile.Name = "statusLabelFile";
            statusLabelFile.Size = new Size(250, 15);
            statusLabelFile.TabIndex = 17;
            statusLabelFile.Text = "Ready";
            // 
            // statusLabelCount
            // 
            statusLabelCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            statusLabelCount.AutoSize = true;
            statusLabelCount.Location = new Point(1073, 116);
            statusLabelCount.Name = "statusLabelCount";
            statusLabelCount.Size = new Size(0, 15);
            statusLabelCount.TabIndex = 22;
            statusLabelCount.TextAlign = ContentAlignment.TopRight;
            // 
            // progressBarScan
            // 
            progressBarScan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarScan.Location = new Point(524, 134);
            progressBarScan.Name = "progressBarScan";
            progressBarScan.Size = new Size(653, 23);
            progressBarScan.TabIndex = 18;
            progressBarScan.Visible = false;
            // 
            // backBtn
            // 
            backBtn.FlatStyle = FlatStyle.System;
            backBtn.Location = new Point(440, 134);
            backBtn.Name = "backBtn";
            backBtn.Size = new Size(75, 23);
            backBtn.TabIndex = 19;
            backBtn.Text = "< Back";
            backBtn.UseVisualStyleBackColor = true;
            backBtn.Visible = false;
            backBtn.Click += backBtn_Click;
            // 
            // appIcon1
            // 
            appIcon1.BackColor = Color.Transparent;
            appIcon1.Image = Properties.Resources.icons8_kenny_mccormick_96;
            appIcon1.Location = new Point(12, 8);
            appIcon1.Name = "appIcon1";
            appIcon1.Size = new Size(54, 54);
            appIcon1.SizeMode = PictureBoxSizeMode.Zoom;
            appIcon1.TabIndex = 20;
            appIcon1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1189, 910);
            Controls.Add(appIcon1);
            Controls.Add(backBtn);
            Controls.Add(progressBarScan);
            Controls.Add(statusLabelCount);
            Controls.Add(statusLabelFile);
            Controls.Add(flowConfigs);
            Controls.Add(flowVehicles);
            Controls.Add(scanModsBtn);
            Controls.Add(label1);
            Controls.Add(modsPathBox);
            Controls.Add(selectModsPathBtn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Beam Manager";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)appIcon1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button selectModsPathBtn;
        private System.Windows.Forms.TextBox modsPathBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button scanModsBtn;
        private Beam_Manager.BufferedFlowLayoutPanel flowVehicles;
        private Beam_Manager.BufferedFlowLayoutPanel flowConfigs;
        private System.Windows.Forms.Label statusLabelFile;
        private System.Windows.Forms.Label statusLabelCount;
        private System.Windows.Forms.ProgressBar progressBarScan;
        private System.Windows.Forms.Button backBtn;
        private Beam_Manager.AppIcon appIcon1;
    }
}