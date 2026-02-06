// Relative Path: ModCache.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

namespace Beam_Manager
{
    // High-performance binary cache manager
    public class ModCacheManager : IDisposable
    {
        private readonly string _cacheFilePath;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private Dictionary<string, CacheIndexEntry> _index = new Dictionary<string, CacheIndexEntry>(StringComparer.OrdinalIgnoreCase);

        // Magic header to identify our file format
        private const string MAGIC = "BMC2";
        private const int VERSION = 2;

        public ModCacheManager(string modsDirectory)
        {
            _cacheFilePath = Path.Combine(modsDirectory, "beam_manager.bmc");
            Initialize();
        }

        private void Initialize()
        {
            _index.Clear();
            if (!File.Exists(_cacheFilePath)) return;

            try
            {
                using (var fs = new FileStream(_cacheFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new BinaryReader(fs))
                {
                    // 1. Read Header
                    if (fs.Length < 64) return; // Invalid
                    string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    int version = reader.ReadInt32();
                    long indexOffset = reader.ReadInt64();

                    if (magic != MAGIC || version != VERSION) return;

                    // 2. Seek to Index
                    if (indexOffset > 0 && indexOffset < fs.Length)
                    {
                        fs.Seek(indexOffset, SeekOrigin.Begin);
                        int entryCount = reader.ReadInt32();

                        for (int i = 0; i < entryCount; i++)
                        {
                            var entry = new CacheIndexEntry();
                            entry.ZipName = reader.ReadString();
                            entry.FileHash = reader.ReadUInt64();
                            entry.DataOffset = reader.ReadInt64();

                            // Safe dictionary add (last write wins if duplicates exist)
                            _index[entry.ZipName] = entry;
                        }
                    }
                }
            }
            catch
            {
                // If corrupt, start fresh
                _index.Clear();
            }
        }

        // --- PUBLIC API ---

        public bool TryGetMod(string zipName, ulong currentHash, out VehicleModInfo mod)
        {
            mod = null;
            _lock.EnterReadLock();
            try
            {
                if (_index.TryGetValue(zipName, out var entry))
                {
                    if (entry.FileHash == currentHash)
                    {
                        mod = ReadModData(entry.DataOffset, zipName);
                        return mod != null;
                    }
                }
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Image GetImage(string zipName, string configName = null)
        {
            _lock.EnterReadLock();
            try
            {
                if (_index.TryGetValue(zipName, out var entry))
                {
                    return ReadImageFromBlock(entry.DataOffset, configName);
                }
                return null;
            }
            catch { return null; }
            finally { _lock.ExitReadLock(); }
        }

        public void AppendOrUpdateMod(VehicleModInfo mod, string zipFullPath)
        {
            ulong hash = FastHash.CalculateHash(zipFullPath);

            _lock.EnterWriteLock();
            try
            {
                // Open file in Update mode
                using (var fs = new FileStream(_cacheFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    long newEntryOffset = fs.Length;
                    // If file is new, write header space
                    if (fs.Length == 0)
                    {
                        newEntryOffset = 64; // Skip header
                        fs.SetLength(64);
                    }

                    // 1. Serialize Data Block to End of File
                    fs.Seek(newEntryOffset, SeekOrigin.Begin);
                    using (var writer = new BinaryWriter(fs, Encoding.UTF8, true))
                    {
                        WriteModBlock(writer, mod);
                    }

                    // 2. Update In-Memory Index
                    var entry = new CacheIndexEntry
                    {
                        ZipName = mod.ZipName,
                        FileHash = hash,
                        DataOffset = newEntryOffset
                    };
                    _index[mod.ZipName] = entry;

                    // 3. Write New Index at End
                    long indexOffset = fs.Position;
                    using (var writer = new BinaryWriter(fs, Encoding.UTF8, true))
                    {
                        writer.Write(_index.Count);
                        foreach (var kvp in _index)
                        {
                            writer.Write(kvp.Value.ZipName);
                            writer.Write(kvp.Value.FileHash);
                            writer.Write(kvp.Value.DataOffset);
                        }
                    }

                    // 4. Update Header
                    fs.Seek(0, SeekOrigin.Begin);
                    using (var writer = new BinaryWriter(fs, Encoding.UTF8, true))
                    {
                        writer.Write(Encoding.ASCII.GetBytes(MAGIC));
                        writer.Write(VERSION);
                        writer.Write(indexOffset); // Point to the new index
                        writer.Write(DateTime.Now.Ticks);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // --- INTERNAL IO ---

        private void WriteModBlock(BinaryWriter w, VehicleModInfo mod)
        {
            // Metadata
            w.Write(mod.InternalName ?? "");
            w.Write(mod.Brand ?? "");
            w.Write(mod.DisplayName ?? "");
            w.Write(mod.JsonPathInsideZip ?? "");

            // Main Thumbnail
            byte[] thumbBytes = ImageToBytes(mod.TempThumbnail); // Temp image from scan
            w.Write(thumbBytes.Length);
            w.Write(thumbBytes);

            // Configs
            w.Write(mod.Configurations.Count);
            foreach (var cfg in mod.Configurations)
            {
                w.Write(cfg.Name ?? "");
                w.Write(cfg.InternalFileName ?? "");

                byte[] cfgBytes = ImageToBytes(cfg.TempConfigThumbnail);
                w.Write(cfgBytes.Length);
                w.Write(cfgBytes);
            }
        }

        private VehicleModInfo ReadModData(long offset, string zipName)
        {
            try
            {
                using (var fs = new FileStream(_cacheFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var r = new BinaryReader(fs))
                {
                    fs.Seek(offset, SeekOrigin.Begin);

                    var mod = new VehicleModInfo();
                    mod.ZipName = zipName;
                    mod.InternalName = r.ReadString();
                    mod.Brand = r.ReadString();
                    mod.DisplayName = r.ReadString();
                    mod.JsonPathInsideZip = r.ReadString();

                    // Skip Thumbnail (Lazy Load)
                    int thumbLen = r.ReadInt32();
                    fs.Seek(thumbLen, SeekOrigin.Current);

                    // Read Config Metadata (Skip Images)
                    int configCount = r.ReadInt32();
                    mod.Configurations = new List<VehicleConfigInfo>();

                    for (int i = 0; i < configCount; i++)
                    {
                        var cfg = new VehicleConfigInfo();
                        cfg.Name = r.ReadString();
                        cfg.InternalFileName = r.ReadString();

                        int cfgThumbLen = r.ReadInt32();
                        fs.Seek(cfgThumbLen, SeekOrigin.Current); // Skip

                        mod.Configurations.Add(cfg);
                    }

                    return mod;
                }
            }
            catch { return null; }
        }

        private Image ReadImageFromBlock(long offset, string targetConfigName)
        {
            using (var fs = new FileStream(_cacheFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var r = new BinaryReader(fs))
            {
                fs.Seek(offset, SeekOrigin.Begin);

                // Skip Metadata
                r.ReadString(); r.ReadString(); r.ReadString(); r.ReadString();

                // Main Thumbnail
                int thumbLen = r.ReadInt32();
                if (string.IsNullOrEmpty(targetConfigName))
                {
                    if (thumbLen == 0) return null;
                    return BytesToImage(r.ReadBytes(thumbLen));
                }
                fs.Seek(thumbLen, SeekOrigin.Current);

                // Check Configs
                int configCount = r.ReadInt32();
                for (int i = 0; i < configCount; i++)
                {
                    string name = r.ReadString();
                    string file = r.ReadString(); // skip
                    int imgLen = r.ReadInt32();

                    if (name == targetConfigName)
                    {
                        if (imgLen == 0) return null;
                        return BytesToImage(r.ReadBytes(imgLen));
                    }
                    fs.Seek(imgLen, SeekOrigin.Current);
                }
            }
            return null;
        }

        private byte[] ImageToBytes(Image img)
        {
            if (img == null) return Array.Empty<byte>();
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private Image BytesToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            // CRITICAL FIX FOR GDI+ "PARAMETER IS NOT VALID"
            // We must create a new deep-copy Bitmap from the stream so we can
            // dispose the stream immediately. Image.FromStream keeps the stream open otherwise.
            using (var ms = new MemoryStream(bytes))
            using (var temp = Image.FromStream(ms))
            {
                // This 'temp' image relies on 'ms'
                // This 'safeCopy' copies the pixels to a new memory block, detached from 'ms'
                Bitmap safeCopy = new Bitmap(temp.Width, temp.Height);
                safeCopy.SetResolution(temp.HorizontalResolution, temp.VerticalResolution);
                using (Graphics g = Graphics.FromImage(safeCopy))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                }
                return safeCopy;
            }
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }

        private class CacheIndexEntry
        {
            public string ZipName;
            public ulong FileHash;
            public long DataOffset;
        }
    }

    // --- RE-ADDED FASTHASH CLASS ---
    // Simple, Fast Hash for File Change Detection (FNV-1a 64-bit modified)
    public static class FastHash
    {
        public static ulong CalculateHash(string filePath)
        {
            // We hash a combination of:
            // 1. File Size
            // 2. Last Write Time
            // 3. First 4KB of data (Header)
            // This is much faster than hashing 500MB zip files and reliable enough for mods.

            try
            {
                var info = new FileInfo(filePath);
                ulong hash = 14695981039346656037;

                hash = (hash ^ (ulong)info.Length) * 1099511628211;
                hash = (hash ^ (ulong)info.LastWriteTimeUtc.Ticks) * 1099511628211;

                // Read header bytes
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytesRead = fs.Read(buffer, 0, buffer.Length);
                }

                for (int i = 0; i < bytesRead; i++)
                {
                    hash = (hash ^ buffer[i]) * 1099511628211;
                }

                return hash;
            }
            catch
            {
                return 0;
            }
        }
    }
}