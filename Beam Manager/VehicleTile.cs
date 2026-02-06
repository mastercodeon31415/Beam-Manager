// Relative Path: VehicleTile.cs
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Beam_Manager
{
    public partial class VehicleTile : UserControl
    {
        private VehicleModInfo _modInfo;
        public event EventHandler<VehicleModInfo> OnTileClicked;
        public event EventHandler<VehicleModInfo> OnEditRequested;

        // Visual States
        private bool _isHovered = false;
        private bool _isPressed = false;
        private bool _isMenuOpen = false;

        private readonly Color _cNormal = Color.FromArgb(40, 40, 40);
        private readonly Color _cHover = Color.FromArgb(60, 60, 60);
        private readonly Color _cPressed = Color.FromArgb(30, 30, 30);
        private readonly Color _cMenuOpen = Color.FromArgb(80, 80, 80);

        public static ImageCache GlobalImageCache;

        // Visual Constants
        // Note: Using 12 for the bounding box size (Arc Diameter) creates a 6px Radius curve.
        private const int CORNER_RADIUS = 12;
        private const int CORNER_DIAMETER = CORNER_RADIUS;

        public VehicleTile()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            this.BackColor = _cNormal;

            lblConfigCount.Visible = false;

            picThumbnail.Image = null;
            picThumbnail.Paint += PicThumbnail_Paint;

            ContextMenuStrip cms = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Edit Make & Model");
            editItem.Click += (s, e) => { OnEditRequested?.Invoke(this, _modInfo); ForceStateCheck(); };
            cms.Items.Add(editItem);

            cms.Opening += (s, e) => { _isMenuOpen = true; UpdateVisuals(); };
            cms.Closing += (s, e) => { _isMenuOpen = false; ForceStateCheck(); };
            this.ContextMenuStrip = cms;

            BindEvents(this);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            // Matches the game tile shape: Rounded Top-Left and Top-Right
            GraphicsPath path = new GraphicsPath();

            // Top Left Arc
            path.AddArc(0, 0, CORNER_DIAMETER, CORNER_DIAMETER, 180, 90);
            // Top Right Arc
            path.AddArc(this.Width - CORNER_DIAMETER, 0, CORNER_DIAMETER, CORNER_DIAMETER, 270, 90);
            // Bottom Right Corner (Square)
            path.AddLine(this.Width, CORNER_DIAMETER, this.Width, this.Height);
            path.AddLine(this.Width, this.Height, 0, this.Height);
            // Close
            path.AddLine(0, this.Height, 0, CORNER_DIAMETER);
            path.CloseFigure();

            this.Region = new Region(path);
        }

        private void PicThumbnail_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (_modInfo != null && GlobalImageCache != null)
            {
                // 1. Draw Image
                Image img = GlobalImageCache.GetImage(_modInfo.ZipName);
                if (img != null)
                {
                    e.Graphics.DrawImage(img, new Rectangle(0, 0, picThumbnail.Width, picThumbnail.Height));
                }

                // 2. Draw Config Count Overlay
                // Style: Leaf shape (Rounded Top-Right & Bottom-Left)
                int count = _modInfo.Configurations.Count;
                if (count > 0)
                {
                    string countText = count.ToString();
                    using (Font f = new Font("Segoe UI", 9, FontStyle.Bold))
                    {
                        // Measure text to size box dynamically
                        SizeF textSize = e.Graphics.MeasureString(countText, f);
                        int paddingH = 10;
                        int paddingV = 4;

                        // Minimum size to ensure single digits look good
                        int boxW = (int)Math.Max(28, textSize.Width + paddingH);
                        int boxH = (int)Math.Max(22, textSize.Height + paddingV);

                        // Position: Top Right corner, flush with edges
                        Rectangle boxRect = new Rectangle(picThumbnail.Width - boxW, 0, boxW, boxH);

                        using (GraphicsPath badgePath = new GraphicsPath())
                        {
                            // A. Start Top-Left
                            // B. Go to Top-Right (SQUARE)
                            // FIX: We draw a straight line to the corner. The Parent Region (UpdateRegion) 
                            // will clip this to the perfect curve, eliminating any gap.
                            badgePath.AddLine(boxRect.X, boxRect.Y, boxRect.Right, boxRect.Y);

                            // C. Right Edge (Square)
                            badgePath.AddLine(boxRect.Right, boxRect.Y, boxRect.Right, boxRect.Bottom);

                            // D. Bottom Edge -> To start of Bottom-Left Curve
                            badgePath.AddLine(boxRect.Right, boxRect.Bottom, boxRect.Left + CORNER_DIAMETER, boxRect.Bottom);

                            // E. Bottom-Left Corner (Rounded)
                            // This creates the "Leaf" look diagonal to the top-right
                            badgePath.AddArc(boxRect.Left, boxRect.Bottom - CORNER_DIAMETER, CORNER_DIAMETER, CORNER_DIAMETER, 90, 90);

                            // F. Close to Top-Left
                            badgePath.AddLine(boxRect.Left, boxRect.Bottom - CORNER_DIAMETER, boxRect.Left, boxRect.Y);

                            badgePath.CloseFigure();

                            // Fill Background (Semi-transparent dark gray)
                            using (Brush b = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                            {
                                e.Graphics.FillPath(b, badgePath);
                            }
                        }

                        // Draw Text (Centered)
                        using (Brush bText = new SolidBrush(Color.White))
                        {
                            // Manual adjustment for visual centering
                            float x = boxRect.X + (boxRect.Width - textSize.Width) / 2 + 1;
                            float y = boxRect.Y + (boxRect.Height - textSize.Height) / 2;
                            e.Graphics.DrawString(countText, f, bText, x, y);
                        }
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw Gear/Settings Icon in Bottom Right if configs exist
            if (_modInfo != null && _modInfo.Configurations.Count > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                int iconSize = 14;
                int margin = 8;
                int x = this.Width - iconSize - margin;
                int y = this.Height - iconSize - margin;

                using (Brush b = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                using (Pen p = new Pen(b, 2))
                {
                    float cx = x + iconSize / 2f;
                    float cy = y + iconSize / 2f;
                    float r = iconSize / 2f - 2;

                    e.Graphics.DrawEllipse(p, cx - 2, cy - 2, 4, 4);

                    for (int i = 0; i < 8; i++)
                    {
                        double angle = i * (360.0 / 8) * (Math.PI / 180.0);
                        float tx = cx + (float)(Math.Cos(angle) * r);
                        float ty = cy + (float)(Math.Sin(angle) * r);
                        float tx2 = cx + (float)(Math.Cos(angle) * (r + 2));
                        float ty2 = cy + (float)(Math.Sin(angle) * (r + 2));
                        e.Graphics.DrawLine(p, tx, ty, tx2, ty2);
                    }
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible && !this.Disposing)
            {
                _isPressed = false;
                _isMenuOpen = false;
                ForceStateCheck();
            }
        }

        private void BindEvents(Control c)
        {
            if (c != this) c.ContextMenuStrip = this.ContextMenuStrip;
            c.MouseEnter += (s, e) => { _isHovered = true; UpdateVisuals(); };
            c.MouseLeave += CheckMouseLeave;
            c.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { _isPressed = true; UpdateVisuals(); } };
            c.MouseUp += (s, e) => { _isPressed = false; UpdateVisuals(); };
            c.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) OnTileClicked?.Invoke(this, _modInfo); };
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

        private void ForceStateCheck()
        {
            try { Point localCursor = this.PointToClient(Cursor.Position); _isHovered = this.ClientRectangle.Contains(localCursor); UpdateVisuals(); } catch { }
        }

        private void UpdateVisuals()
        {
            if (_isMenuOpen) { this.BackColor = _cMenuOpen; lblVehicleName.ForeColor = Color.Orange; }
            else if (_isPressed) { this.BackColor = _cPressed; lblVehicleName.ForeColor = Color.White; }
            else if (_isHovered) { this.BackColor = _cHover; lblVehicleName.ForeColor = Color.White; }
            else { this.BackColor = _cNormal; lblVehicleName.ForeColor = Color.White; }
        }

        public void SetData(VehicleModInfo mod)
        {
            _modInfo = mod;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (_modInfo == null) return;
            lblBrand.Text = _modInfo.Brand;
            lblVehicleName.Text = _modInfo.DisplayName;
            lblZipName.Text = _modInfo.ZipName;

            picThumbnail.Invalidate();
            this.Invalidate();

            toolTip1.SetToolTip(lblVehicleName, $"{_modInfo.Brand} {_modInfo.DisplayName}");
            toolTip1.SetToolTip(this, _modInfo.FullPath);
        }
    }
}