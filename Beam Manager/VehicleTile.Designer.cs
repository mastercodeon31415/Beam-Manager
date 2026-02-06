namespace Beam_Manager
{
    partial class VehicleTile
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picThumbnail = new System.Windows.Forms.PictureBox();
            this.lblVehicleName = new System.Windows.Forms.Label();
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblZipName = new System.Windows.Forms.Label();
            this.lblConfigCount = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // picThumbnail
            // 
            this.picThumbnail.Dock = System.Windows.Forms.DockStyle.Top;
            this.picThumbnail.Location = new System.Drawing.Point(0, 0);
            this.picThumbnail.Name = "picThumbnail";
            this.picThumbnail.Size = new System.Drawing.Size(240, 135);
            // VISUAL FIX: Changed from Zoom to StretchImage.
            // This ensures the image fills the entire top area pixel-perfectly,
            // preventing background highlight colors from bleeding through on the edges.
            this.picThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picThumbnail.TabIndex = 0;
            this.picThumbnail.TabStop = false;
            // 
            // lblVehicleName
            // 
            this.lblVehicleName.AutoEllipsis = true;
            this.lblVehicleName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblVehicleName.Location = new System.Drawing.Point(8, 155);
            this.lblVehicleName.Name = "lblVehicleName";
            this.lblVehicleName.Size = new System.Drawing.Size(225, 23);
            this.lblVehicleName.TabIndex = 1;
            this.lblVehicleName.Text = "Model Name";
            // 
            // lblBrand
            // 
            this.lblBrand.AutoEllipsis = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBrand.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblBrand.Location = new System.Drawing.Point(10, 140);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(223, 15);
            this.lblBrand.TabIndex = 2;
            this.lblBrand.Text = "Make";
            // 
            // lblZipName
            // 
            this.lblZipName.AutoEllipsis = true;
            this.lblZipName.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblZipName.ForeColor = System.Drawing.Color.Gray;
            this.lblZipName.Location = new System.Drawing.Point(10, 180);
            this.lblZipName.Name = "lblZipName";
            this.lblZipName.Size = new System.Drawing.Size(180, 15);
            this.lblZipName.TabIndex = 3;
            this.lblZipName.Text = "file.zip";
            // 
            // lblConfigCount
            // 
            this.lblConfigCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConfigCount.AutoSize = true;
            this.lblConfigCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblConfigCount.ForeColor = System.Drawing.Color.DimGray;
            this.lblConfigCount.Location = new System.Drawing.Point(210, 180);
            this.lblConfigCount.Name = "lblConfigCount";
            this.lblConfigCount.Size = new System.Drawing.Size(14, 15);
            this.lblConfigCount.TabIndex = 4;
            this.lblConfigCount.Text = "0";
            this.lblConfigCount.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // VehicleTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblConfigCount);
            this.Controls.Add(this.lblZipName);
            this.Controls.Add(this.lblBrand);
            this.Controls.Add(this.lblVehicleName);
            this.Controls.Add(this.picThumbnail);
            this.Name = "VehicleTile";
            this.Size = new System.Drawing.Size(240, 205);
            ((System.ComponentModel.ISupportInitialize)(this.picThumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.PictureBox picThumbnail;
        private System.Windows.Forms.Label lblVehicleName;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label lblZipName;
        private System.Windows.Forms.Label lblConfigCount;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}