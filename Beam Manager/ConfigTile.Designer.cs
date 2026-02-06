// Relative Path: ConfigTile.Designer.cs
namespace Beam_Manager
{
    partial class ConfigTile
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            picThumbnail = new PictureBox();
            lblConfigName = new Label();
            lblFileName = new Label();
            ((System.ComponentModel.ISupportInitialize)picThumbnail).BeginInit();
            SuspendLayout();
            // 
            // picThumbnail
            // 
            picThumbnail.Dock = DockStyle.Top;
            picThumbnail.Location = new Point(0, 0);
            picThumbnail.Margin = new Padding(4, 3, 4, 3);
            picThumbnail.Name = "picThumbnail";
            picThumbnail.Size = new Size(280, 156);
            picThumbnail.SizeMode = PictureBoxSizeMode.StretchImage;
            picThumbnail.TabIndex = 0;
            picThumbnail.TabStop = false;
            // 
            // lblConfigName
            // 
            lblConfigName.AutoEllipsis = true;
            lblConfigName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lblConfigName.Location = new Point(4, 159);
            lblConfigName.Margin = new Padding(4, 0, 4, 0);
            lblConfigName.Name = "lblConfigName";
            lblConfigName.Size = new Size(273, 27);
            lblConfigName.TabIndex = 1;
            lblConfigName.Text = "Config Name";
            // 
            // lblFileName
            // 
            lblFileName.AutoEllipsis = true;
            lblFileName.Font = new Font("Segoe UI", 8.25F);
            lblFileName.ForeColor = Color.Gray;
            lblFileName.Location = new Point(4, 186);
            lblFileName.Margin = new Padding(4, 0, 4, 0);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(273, 17);
            lblFileName.TabIndex = 2;
            lblFileName.Text = "file.pc";
            // 
            // ConfigTile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblFileName);
            Controls.Add(lblConfigName);
            Controls.Add(picThumbnail);
            Margin = new Padding(4, 3, 4, 3);
            Name = "ConfigTile";
            Size = new Size(280, 213);
            ((System.ComponentModel.ISupportInitialize)picThumbnail).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.PictureBox picThumbnail;
        private System.Windows.Forms.Label lblConfigName;
        private System.Windows.Forms.Label lblFileName;
    }
}