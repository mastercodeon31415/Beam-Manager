// Relative Path: Form1.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Beam_Manager
{
    public partial class Form1 : Form
    {
        private ConcurrentQueue<Control> _configControlQueue = new ConcurrentQueue<Control>();
        private System.Windows.Forms.Timer _uiUpdateTimer;
        private ScanStatus _currentScanStatus;

        // Cache Systems
        private ModCacheManager _modCacheManager;
        private ImageCache _imageCache;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Beam Manager";
            this.StartPosition = FormStartPosition.CenterScreen;

            _uiUpdateTimer = new System.Windows.Forms.Timer();
            _uiUpdateTimer.Interval = 30;
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            _uiUpdateTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            scanModsBtn.Enabled = false;
            flowConfigs.Visible = false;
            flowConfigs.Bounds = flowVehicles.Bounds;
            flowConfigs.Anchor = flowVehicles.Anchor;
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_currentScanStatus != null)
            {
                if (progressBarScan.Maximum != _currentScanStatus.TotalFiles)
                    progressBarScan.Maximum = _currentScanStatus.TotalFiles;

                if (progressBarScan.Value != _currentScanStatus.ProcessedCount)
                    progressBarScan.Value = _currentScanStatus.ProcessedCount;

                statusLabelFile.Text = $"Scanning: {_currentScanStatus.CurrentFile}";
                statusLabelCount.Text = $"{_currentScanStatus.ProcessedCount} / {_currentScanStatus.TotalFiles}";
            }

            if (flowConfigs.Visible && !_configControlQueue.IsEmpty)
            {
                flowConfigs.SuspendLayout();
                int count = 0;
                while (count < 5 && _configControlQueue.TryDequeue(out Control ctrl))
                {
                    flowConfigs.Controls.Add(ctrl);
                    count++;
                }
                flowConfigs.ResumeLayout();
            }
        }

        private void selectModsPathBtn_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                modsPathBox.Text = dialog.FileName;
                scanModsBtn.Enabled = true;

                // Initialize Cache System
                DisposeCache();
                _modCacheManager = new ModCacheManager(dialog.FileName);
                _imageCache = new ImageCache(_modCacheManager);
                VehicleTile.GlobalImageCache = _imageCache;

                // FIX: Automatically Start Scanning / Loading from Cache
                scanModsBtn.PerformClick();
            }
        }

        private void DisposeCache()
        {
            if (_imageCache != null) _imageCache.Clear();
            if (_modCacheManager != null) _modCacheManager.Dispose();
        }

        private async void scanModsBtn_Click(object sender, EventArgs e)
        {
            flowVehicles.Visible = true;
            flowConfigs.Visible = false;
            backBtn.Visible = false;
            flowVehicles.Controls.Clear();
            if (_imageCache != null) _imageCache.Clear(); // Clear RAM cache on new scan

            scanModsBtn.Enabled = false;
            progressBarScan.Visible = true;
            progressBarScan.Value = 0;
            statusLabelFile.Text = "Initializing Cache...";
            statusLabelCount.Text = "0 / 0";

            string path = modsPathBox.Text;
            _currentScanStatus = new ScanStatus();

            BeamNGModProcessor processor = new BeamNGModProcessor(_modCacheManager);

            try
            {
                // This is now fully async and won't freeze UI
                List<VehicleModInfo> unsortedMods = await processor.ScanDirectoryManualPool(path, _currentScanStatus);

                progressBarScan.Value = progressBarScan.Maximum;
                statusLabelFile.Text = "Sorting results...";

                var sortedMods = unsortedMods
                    .OrderBy(x => x.Brand)
                    .ThenBy(x => x.DisplayName)
                    .ToList();

                statusLabelFile.Text = "Building UI...";
                PopulateVehicleList(sortedMods);
            }
            finally
            {
                _currentScanStatus = null;
                scanModsBtn.Enabled = true;
                progressBarScan.Visible = false;
                statusLabelFile.Text = "Ready.";
                statusLabelCount.Text = $"{flowVehicles.Controls.Count} Vehicles Loaded";
                GC.Collect();

                Application.DoEvents();

                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Size = new Size(this.Size.Width + 8, this.Size.Height);
                }
            }
        }

        private void PopulateVehicleList(List<VehicleModInfo> mods)
        {
            flowVehicles.SuspendLayout();
            List<Control> tiles = new List<Control>(mods.Count);
            foreach (var mod in mods)
            {
                VehicleTile tile = new VehicleTile();
                tile.SetData(mod);
                tile.OnTileClicked += (s, m) => ShowModDetails(m);
                tile.OnEditRequested += (s, m) => EditMod(m, tile);
                tiles.Add(tile);
            }
            flowVehicles.Controls.AddRange(tiles.ToArray());
            flowVehicles.ResumeLayout();
        }

        private void ShowModDetails(VehicleModInfo mod)
        {
            flowVehicles.Visible = false;
            flowConfigs.Visible = true;
            backBtn.Visible = true;

            statusLabelFile.Text = $"{mod.Brand} {mod.DisplayName}";
            statusLabelCount.Text = $"{mod.Configurations.Count} Configs";

            flowConfigs.Controls.Clear();
            _configControlQueue = new ConcurrentQueue<Control>();

            Task.Run(() =>
            {
                if (mod.Configurations.Count == 0)
                {
                    this.Invoke(new Action(() => {
                        Label l = new Label { Text = "No configs found.", AutoSize = true, ForeColor = Color.Gray };
                        _configControlQueue.Enqueue(l);
                    }));
                }
                else
                {
                    foreach (var config in mod.Configurations)
                    {
                        this.Invoke(new Action(() =>
                        {
                            ConfigTile cTile = new ConfigTile();
                            Image thumb = _imageCache.GetImage(mod.ZipName, config.Name);
                            cTile.SetData(config.Name, config.InternalFileName, thumb);
                            _configControlQueue.Enqueue(cTile);
                        }));
                    }
                }
            });
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            flowConfigs.Visible = false;
            flowVehicles.Visible = true;
            backBtn.Visible = false;
            statusLabelFile.Text = "Vehicle List";
            statusLabelCount.Text = $"{flowVehicles.Controls.Count} Loaded";
            _configControlQueue = new ConcurrentQueue<Control>();
        }

        private void EditMod(VehicleModInfo mod, VehicleTile tileControl)
        {
            using (var editForm = new EditModDialog(mod))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    string oldBrand = mod.Brand;
                    string oldName = mod.DisplayName;

                    mod.Brand = editForm.NewMake;
                    mod.DisplayName = editForm.NewModel;

                    BeamNGModProcessor processor = new BeamNGModProcessor(_modCacheManager);

                    bool success = processor.UpdateModJson(mod.FullPath, mod.JsonPathInsideZip, mod.Brand, mod.DisplayName);

                    if (success)
                    {
                        // Reload images from cache (safe to use old ones)
                        mod.TempThumbnail = _imageCache.GetImage(mod.ZipName);
                        foreach (var cfg in mod.Configurations)
                        {
                            cfg.TempConfigThumbnail = _imageCache.GetImage(mod.ZipName, cfg.Name);
                        }

                        _modCacheManager.AppendOrUpdateMod(mod, mod.FullPath);

                        tileControl.UpdateDisplay();
                        MessageBox.Show("Saved & Cached!");
                    }
                    else
                    {
                        mod.Brand = oldBrand;
                        mod.DisplayName = oldName;
                        MessageBox.Show("Failed to save to zip.");
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DisposeCache();
            base.OnFormClosing(e);
        }
    }
}