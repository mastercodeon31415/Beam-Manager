// Relative Path: ImageCache.cs
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Beam_Manager
{
    public class ImageCache
    {
        private const int MAX_CAPACITY = 60;
        private readonly Dictionary<string, Image> _cache = new Dictionary<string, Image>();
        private readonly LinkedList<string> _lruList = new LinkedList<string>();
        private readonly ModCacheManager _diskManager;
        private readonly object _lock = new object();

        public ImageCache(ModCacheManager diskManager)
        {
            _diskManager = diskManager;
        }

        public Image GetImage(string zipName, string configName = null)
        {
            string key = zipName + "::" + (configName ?? "MAIN");

            lock (_lock)
            {
                // 1. Check Memory Cache
                if (_cache.TryGetValue(key, out Image img))
                {
                    _lruList.Remove(key);
                    _lruList.AddLast(key);
                    return img;
                }
            }

            // 2. Load from Disk (Outside lock to allow concurrency)
            Image diskImg = null;
            if (_diskManager != null)
            {
                diskImg = _diskManager.GetImage(zipName, configName);
            }

            if (diskImg != null)
            {
                lock (_lock)
                {
                    // Check if added by another thread while we were loading
                    if (_cache.TryGetValue(key, out Image existing))
                    {
                        diskImg.Dispose();
                        _lruList.Remove(key);
                        _lruList.AddLast(key);
                        return existing;
                    }

                    AddToCache(key, diskImg);
                    return diskImg;
                }
            }

            return null;
        }

        private void AddToCache(string key, Image img)
        {
            // Lock is held by caller
            if (_cache.Count >= MAX_CAPACITY)
            {
                string oldKey = _lruList.First.Value;
                _lruList.RemoveFirst();

                if (_cache.TryGetValue(oldKey, out Image oldImg))
                {
                    _cache.Remove(oldKey);
                    oldImg.Dispose();
                }
            }

            _cache[key] = img;
            _lruList.AddLast(key);
        }

        public void Clear()
        {
            lock (_lock)
            {
                foreach (var img in _cache.Values) img.Dispose();
                _cache.Clear();
                _lruList.Clear();
            }
        }
    }
}