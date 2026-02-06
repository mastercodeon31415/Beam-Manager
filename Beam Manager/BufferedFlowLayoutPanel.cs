// Relative Path: BufferedFlowLayoutPanel.cs
using System;
using System.Windows.Forms;
using System.Drawing;

namespace Beam_Manager
{
    public class BufferedFlowLayoutPanel : FlowLayoutPanel
    {
        public BufferedFlowLayoutPanel()
        {
            // Enable double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw, true);

            this.UpdateStyles();
        }

        // CONSTANTS
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_THUMBTRACK = 5;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Intercept Vertical or Horizontal scroll messages
            if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
            {
                // This command forces the control to redraw IMMEDIATELY.
                // It prevents the OS from doing a "BitBlt" (Pixel copy) which causes the smearing/ghosting
                // when the child controls (PictureBoxes) haven't finished painting yet.
                this.Refresh();
            }
        }
    }
}