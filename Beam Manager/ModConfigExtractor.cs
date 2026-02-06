// Relative Path: ModConfigExtractor.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Beam_Manager
{
    public class ScanStatus
    {
        public int ProcessedCount;
        public int TotalFiles;
        public string CurrentFile = "";
        public bool IsComplete = false;
    }

    public class VehicleModInfo
    {
        public string DisplayName { get; set; }
        public string Brand { get; set; }
        public string InternalName { get; set; }
        public string ZipName { get; set; }
        public string FullPath { get; set; }
        public string JsonPathInsideZip { get; set; }

        // These are used ONLY during scan/save. They are nullified after caching to save RAM.
        [JsonIgnore] public Image TempThumbnail { get; set; }
        public List<VehicleConfigInfo> Configurations { get; set; } = new List<VehicleConfigInfo>();
    }

    public class VehicleConfigInfo
    {
        public string Name { get; set; }
        public string InternalFileName { get; set; }
        [JsonIgnore] public Image TempConfigThumbnail { get; set; }
    }

    public class BeamNGModProcessor
    {
        private static SemaphoreSlim _gdiSemaphore = new SemaphoreSlim(1, 1);
        private ModCacheManager _cacheManager;

        public BeamNGModProcessor(ModCacheManager cacheManager = null)
        {
            _cacheManager = cacheManager;
        }

        public async Task<List<VehicleModInfo>> ScanDirectoryManualPool(string directoryPath, ScanStatus status)
        {
            // FIX: Wrap everything in Task.Run to prevent UI freeze during File Enumeration and Hashing
            return await Task.Run(async () =>
            {
                var results = new ConcurrentBag<VehicleModInfo>();

                if (!Directory.Exists(directoryPath))
                {
                    status.IsComplete = true;
                    return new List<VehicleModInfo>();
                }

                // Phase 1 might take time with 450+ files, so doing it inside Task.Run is crucial
                string[] zipFiles = Directory.GetFiles(directoryPath, "*.zip", SearchOption.AllDirectories);
                status.TotalFiles = zipFiles.Length;

                // --- PHASE 1: RECONCILIATION ---
                // Determine what is cached vs what needs scanning
                var filesToScan = new ConcurrentQueue<string>();

                if (_cacheManager == null)
                {
                    foreach (var f in zipFiles) filesToScan.Enqueue(f);
                }
                else
                {
                    foreach (var file in zipFiles)
                    {
                        string zipName = Path.GetFileName(file);

                        // Hashing involves IO, so this was freezing the UI before
                        ulong currentHash = FastHash.CalculateHash(file);

                        if (_cacheManager.TryGetMod(zipName, currentHash, out VehicleModInfo cachedMod))
                        {
                            // Cache HIT
                            cachedMod.FullPath = file;
                            results.Add(cachedMod);
                            Interlocked.Increment(ref status.ProcessedCount);
                        }
                        else
                        {
                            // Cache MISS
                            filesToScan.Enqueue(file);
                        }
                    }
                }

                // --- PHASE 2: PROCESSING (Threads) ---
                if (!filesToScan.IsEmpty)
                {
                    int concurrency = Math.Max(1, Environment.ProcessorCount - 2);
                    List<Thread> threads = new List<Thread>();
                    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                    int threadsRunning = concurrency;

                    for (int i = 0; i < concurrency; i++)
                    {
                        Thread t = new Thread(() =>
                        {
                            while (filesToScan.TryDequeue(out string filePath))
                            {
                                status.CurrentFile = Path.GetFileName(filePath);

                                try
                                {
                                    var modInfo = ExtractVehicleDataFromZip(filePath);
                                    if (modInfo != null)
                                    {
                                        if (_cacheManager != null)
                                        {
                                            _cacheManager.AppendOrUpdateMod(modInfo, filePath);

                                            // Dispose Images immediately to free RAM
                                            if (modInfo.TempThumbnail != null) { modInfo.TempThumbnail.Dispose(); modInfo.TempThumbnail = null; }
                                            foreach (var c in modInfo.Configurations)
                                            {
                                                if (c.TempConfigThumbnail != null) { c.TempConfigThumbnail.Dispose(); c.TempConfigThumbnail = null; }
                                            }

                                            // Reload lightweight version
                                            ulong hash = FastHash.CalculateHash(filePath);
                                            if (_cacheManager.TryGetMod(modInfo.ZipName, hash, out var cleanMod))
                                            {
                                                cleanMod.FullPath = filePath;
                                                results.Add(cleanMod);
                                            }
                                        }
                                        else
                                        {
                                            results.Add(modInfo);
                                        }
                                    }
                                }
                                catch { }

                                Interlocked.Increment(ref status.ProcessedCount);
                            }

                            if (Interlocked.Decrement(ref threadsRunning) == 0)
                            {
                                tcs.SetResult(true);
                            }
                        });

                        t.Priority = ThreadPriority.Lowest;
                        t.IsBackground = true;
                        t.Start();
                        threads.Add(t);
                    }

                    await tcs.Task.ConfigureAwait(false);
                }

                status.IsComplete = true;
                return results.ToList();
            });
        }

        private VehicleModInfo ExtractVehicleDataFromZip(string zipFilePath)
        {
            using (FileStream fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                string internalName = null;

                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".jbeam", StringComparison.OrdinalIgnoreCase) && entry.FullName.StartsWith("vehicles/"))
                    {
                        var parts = entry.FullName.Split('/');
                        if (parts.Length >= 3)
                        {
                            string folder = parts[1];
                            string file = Path.GetFileNameWithoutExtension(entry.Name);
                            if (string.Equals(folder, file, StringComparison.OrdinalIgnoreCase))
                            {
                                internalName = folder;
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(internalName)) return null;

                string brand = "Unknown";
                string displayName = internalName;
                string jsonPath = $"vehicles/{internalName}/info.json";
                Image thumb = null;
                List<VehicleConfigInfo> configs = new List<VehicleConfigInfo>();

                ZipArchiveEntry infoEntry = null;
                ZipArchiveEntry thumbEntry = null;
                List<ZipArchiveEntry> configEntries = new List<ZipArchiveEntry>();
                Dictionary<string, ZipArchiveEntry> configImages = new Dictionary<string, ZipArchiveEntry>();

                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.StartsWith($"vehicles/{internalName}/", StringComparison.OrdinalIgnoreCase)) continue;
                    string entryNameLower = entry.Name.ToLowerInvariant();

                    if (entryNameLower == "info.json") infoEntry = entry;
                    else if ((entryNameLower == "default.png" || entryNameLower == "default.jpg") && thumbEntry == null) thumbEntry = entry;
                    else if (entryNameLower.EndsWith(".pc")) configEntries.Add(entry);
                    else if (entryNameLower.EndsWith(".jpg") || entryNameLower.EndsWith(".png"))
                    {
                        string key = Path.GetFileNameWithoutExtension(entry.Name);
                        if (!configImages.ContainsKey(key)) configImages[key] = entry;
                    }
                }

                if (infoEntry != null)
                {
                    using (var stream = infoEntry.Open())
                    using (var reader = new StreamReader(stream))
                    {
                        try
                        {
                            dynamic json = JsonConvert.DeserializeObject(reader.ReadToEnd());
                            if (json != null)
                            {
                                if (json.Name != null) displayName = (string)json.Name;
                                if (json.Brand != null) brand = (string)json.Brand;
                            }
                        }
                        catch { }
                    }
                }

                if (thumbEntry != null) thumb = LoadImageFromEntry(thumbEntry);

                foreach (var pcEntry in configEntries)
                {
                    string configName = Path.GetFileNameWithoutExtension(pcEntry.Name);
                    Image configThumb = null;

                    if (configImages.TryGetValue(configName, out var imgEntry)) configThumb = LoadImageFromEntry(imgEntry);
                    else if (thumb != null) configThumb = (Image)thumb.Clone();

                    configs.Add(new VehicleConfigInfo
                    {
                        Name = configName,
                        InternalFileName = pcEntry.Name,
                        TempConfigThumbnail = configThumb
                    });
                }

                return new VehicleModInfo
                {
                    ZipName = Path.GetFileName(zipFilePath),
                    FullPath = zipFilePath,
                    InternalName = internalName,
                    Brand = brand,
                    DisplayName = displayName,
                    JsonPathInsideZip = jsonPath,
                    TempThumbnail = thumb,
                    Configurations = configs
                };
            }
        }

        private Image LoadImageFromEntry(ZipArchiveEntry entry)
        {
            try
            {
                using (var stream = entry.Open())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    using (var tempBmp = new Bitmap(ms))
                    {
                        return DeepCopyResize(tempBmp, 240, 135);
                    }
                }
            }
            catch { return null; }
        }

        private Image DeepCopyResize(Image image, int width, int height)
        {
            _gdiSemaphore.Wait();
            try
            {
                var destImage = new Bitmap(width, height);
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                return destImage;
            }
            finally
            {
                _gdiSemaphore.Release();
            }
        }

        public bool UpdateModJson(string zipPath, string jsonPath, string newBrand, string newName)
        {
            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.ReadWrite))
                    using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Update))
                    {
                        var entry = archive.GetEntry(jsonPath);
                        JObject jsonObj = new JObject();

                        if (entry != null)
                        {
                            using (var reader = new StreamReader(entry.Open()))
                            {
                                try { jsonObj = JObject.Parse(reader.ReadToEnd()); } catch { }
                            }
                            entry.Delete();
                        }

                        jsonObj["Brand"] = newBrand;
                        jsonObj["Name"] = newName;

                        var newEntry = archive.CreateEntry(jsonPath);
                        using (var writer = new StreamWriter(newEntry.Open()))
                        {
                            writer.Write(jsonObj.ToString());
                        }
                    }
                    return true;
                }
                catch (IOException)
                {
                    retries--;
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error updating mod: " + ex.Message);
                    return false;
                }
            }
            System.Windows.Forms.MessageBox.Show("File is locked by another process.");
            return false;
        }
    }
}