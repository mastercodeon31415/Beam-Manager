// Relative Path: EditModDialog.cs
using System;
using System.Windows.Forms;

namespace Beam_Manager
{
    public partial class EditModDialog : Form
    {
        public string NewMake { get; private set; }
        public string NewModel { get; private set; }

        public EditModDialog(VehicleModInfo mod)
        {
            InitializeComponent();
            txtMake.Text = mod.Brand;
            txtModel.Text = mod.DisplayName;
            lblFile.Text = mod.ZipName;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Edit Mod Details";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            NewMake = txtMake.Text;
            NewModel = txtModel.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}