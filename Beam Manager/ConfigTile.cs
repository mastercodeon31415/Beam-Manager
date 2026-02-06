// Relative Path: ConfigTile.cs
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Beam_Manager
{
    public partial class ConfigTile : UserControl
    {
        private bool _isHovered = false;
        private readonly Color _cNormal = Color.FromArgb(40, 40, 40);
        private readonly Color _cHover = Color.FromArgb(60, 60, 60);

        private Image _directImageRef;

        public ConfigTile()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = _cNormal;

            picThumbnail.Image = null;
            picThumbnail.Paint += PicThumbnail_Paint;

            BindEvents(this);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            // Same rounding logic as VehicleTile: Top corners only
            int radius = 12;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddLine(this.Width, radius, this.Width, this.Height);
            path.AddLine(this.Width, this.Height, 0, this.Height);
            path.AddLine(0, this.Height, 0, radius);
            path.CloseFigure();

            this.Region = new Region(path);
        }

        private void PicThumbnail_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (_directImageRef != null)
            {
                e.Graphics.DrawImage(_directImageRef, new Rectangle(0, 0, picThumbnail.Width, picThumbnail.Height));
            }
        }

        private void BindEvents(Control c)
        {
            c.MouseEnter += (s, e) => { _isHovered = true; UpdateVisuals(); };
            c.MouseLeave += CheckMouseLeave;
            foreach (Control child in c.Controls) BindEvents(child);
        }

        private void CheckMouseLeave(object sender, EventArgs e)
        {
            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
            {
                _isHovered = false;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            this.BackColor = _isHovered ? _cHover : _cNormal;
        }

        public void SetData(string configName, string fileName, Image thumbnail)
        {
            lblConfigName.Text = configName;
            lblFileName.Text = fileName;
            _directImageRef = thumbnail;
            picThumbnail.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try { base.OnPaint(e); } catch { }
        }
    }
}