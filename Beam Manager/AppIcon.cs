// Relative Path: AppIcon.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Beam_Manager
{
    public class AppIcon : PictureBox
    {
        private Image _appIconImg;

        [Category("Appearance")]
        [Description("The image that will be used for the icon")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Image AppIconImage
        {
            get { return _appIconImg; }
            set
            {
                _appIconImg = value;
                this.Image = value;
                this.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        public AppIcon()
        {
            this.Size = new Size(64, 64);
            this.BackColor = Color.Transparent;
        }
    }
}