// Relative Path: EditModDialog.Designer.cs
namespace Beam_Manager
{
    partial class EditModDialog
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditModDialog));
            label1 = new Label();
            label2 = new Label();
            txtMake = new TextBox();
            txtModel = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            lblFile = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 58);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 0;
            label1.Text = "Make (Brand)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 104);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 1;
            label2.Text = "Model (Name)";
            // 
            // txtMake
            // 
            txtMake.BorderStyle = BorderStyle.FixedSingle;
            txtMake.Location = new Point(19, 76);
            txtMake.Margin = new Padding(4, 3, 4, 3);
            txtMake.Name = "txtMake";
            txtMake.Size = new Size(298, 23);
            txtMake.TabIndex = 2;
            // 
            // txtModel
            // 
            txtModel.BorderStyle = BorderStyle.FixedSingle;
            txtModel.Location = new Point(19, 122);
            txtModel.Margin = new Padding(4, 3, 4, 3);
            txtModel.Name = "txtModel";
            txtModel.Size = new Size(298, 23);
            txtModel.TabIndex = 3;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(135, 173);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(230, 173);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // lblFile
            // 
            lblFile.AutoSize = true;
            lblFile.ForeColor = Color.Gray;
            lblFile.Location = new Point(15, 15);
            lblFile.Margin = new Padding(4, 0, 4, 0);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(71, 15);
            lblFile.TabIndex = 6;
            lblFile.Text = "filename.zip";
            // 
            // EditModDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(331, 220);
            Controls.Add(lblFile);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtModel);
            Controls.Add(txtMake);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditModDialog";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMake;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblFile;
    }
}