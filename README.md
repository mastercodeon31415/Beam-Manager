# Beam Manager 🚗💨

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-10.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

**Beam Manager** is a high-performance, external management tool for BeamNG.drive vehicle mods. It bridges the gap between Windows Explorer and the in-game repository, allowing you to visualize, inspect, and edit your mod collection instantly without launching the game.


---

## 📸 Visual Tour

| **The Main Library** |
|:---:|
| ![Screenshot: Main Grid View](https://github.com/user-attachments/assets/63f1bbc8-1d99-4b99-9a4d-8477dab6dfc4) |
| *The main window showing the grid of vehicle tiles. Notice the rounded corners and "Leaf" shaped overlays.* |

| **Deep Inspection** | **Live Editing** |
|:---:|:---:|
| ![Screenshot: Config View](https://github.com/user-attachments/assets/698cea86-9a0d-481a-a6dd-c2f16598a8a3) | ![Screenshot: Edit Context Menu](https://github.com/user-attachments/assets/54b3b3f1-50b7-425f-80b7-dd3625794dc9) |
| *The view after clicking a vehicle, showing all the .pc config tiles inside.* | *The right-click context menu on a vehicle tile, showing the "Edit Make & Model" option.* |

---

## ✨ Key Features

### 🔄 **True Auto-Synchronization**
Beam Manager features a self-healing **Binary Cache System (`.bmc`)**. You only need to run a full scan **once**. After that, the app handles everything automatically on startup:
*   **📂 New Files:** Detects new zips added to your mods folder (or any subfolder) and adds them to the library.
*   **🗑️ Deletions:** Instantly removes mods from the library if the zip file was deleted from disk.
*   **✏️ Renames:** Intelligently detects if a zip file was renamed and updates the record without losing data.
*   **📦 Content Changes:** If you modify a texture or file *inside* a zip (or update a mod), the app detects the hash change and re-scans only that specific file.

### 🎨 **Pixel-Perfect UI**
Designed to feel native to BeamNG.drive.
*   **Visual Library:** View your mods in a grid exactly like the in-game selector.
*   **Authentic Styling:** Features rounded window corners, "Leaf" shaped overlays, and dynamic visual feedback.
*   **Config Inspection:** Click any vehicle to see every `.pc` configuration available inside the zip, complete with thumbnails.

### 🛠 **Live Metadata Editor**
Fix broken mod names forever.
*   **Edit & Save:** Right-click any tile to edit the **Make** and **Model**.
*   **Deep Sync:** Changes are written directly to the `info.json` inside the mod's Zip archive AND updated in the local cache instantly.
*   **No Re-Scan Needed:** Edits are reflected immediately in the UI.

### ⚡ **Performance First**
*   **Low Memory Usage:** Uses an LRU (Least Recently Used) image cache. Even with 500+ mods, the app keeps RAM usage minimal (~100-200MB).
*   **Non-Blocking UI:** Heavy I/O and Image processing happen on background threads. The interface remains buttery smooth and responsive at all times.

---

## 🔧 Technical Overview (For Developers)

Beam Manager is built on **.NET 10 (WinForms)** and implements several advanced patterns to ensure performance:

1.  **Concurrency & GDI+**
    *   **Producer-Consumer Pattern:** Background threads scan zips and queue UI updates. A precise timer on the UI thread dequeues them in batches to prevent UI freezing.
    *   **GDI+ Semaphore:** Image resizing is thread-safe, preventing `InvalidOperationException` during parallel processing.

2.  **Memory Management**
    *   **LRU Image Cache:** The app never holds all thumbnails in memory. It maintains a fixed buffer of active images. As you scroll, old images are disposed, and new ones are streamed from the `.bmc` file on demand.

### 🗄️ The .bmc File Format Specification

The `.bmc` (Beam Mod Cache) file is a custom binary container designed for random-access reads and high-speed append operations. It eliminates the need to parse massive JSON files on startup.

#### 1. File Structure
The file consists of three parts: **Header**, **Data Blocks**, and a **Footer Index**.

| Section | Size | Description |
| :--- | :--- | :--- |
| **Header** | 64 Bytes | Contains Magic ID, Version, and the pointer to the Index. |
| **Data Blocks** | Variable | Sequential blocks containing images and metadata for each mod. |
| **Footer Index** | Variable | A lookup table enabling O(1) access to any mod's data block. |

#### 2. The Header (Start of File)
```csharp
[0x00] Magic "BMC2" (ASCII, 4 bytes)
[0x04] Version (Int32, 4 bytes) - Current version is 2
[0x08] IndexOffset (Int64, 8 bytes) - Absolute position of the Footer Index
[0x10] Timestamp (Int64, 8 bytes) - Last write time ticks
[0x18] Reserved/Padding (Remaining bytes up to 64)
```

#### 3. Data Block Structure (Repeated per Mod)
Each mod's data is written sequentially. Images are stored as raw PNG bytes.
*   **Metadata:** InternalName (String), Brand (String), DisplayName (String), JsonPath (String).
*   **Main Thumbnail:** Size (Int32) + Byte Array.
*   **Configurations:** 
    *   Count (Int32)
    *   *For each config:* ConfigName (String), FileName (String), ConfigThumb Size (Int32) + Byte Array.

#### 4. The Footer Index & Append Strategy
To avoid rewriting the entire file when a new mod is added, the code uses a "Moveable Index" strategy:

1.  **Reading:** The app reads the Header to find the `IndexOffset`. It jumps to the end of the file, reads the Index into a `Dictionary<string, CacheEntry>`, and is ready to go.
2.  **Writing/Appending:** 
    *   The app seeks to the end of the file (overwriting the old Index).
    *   It writes the new Mod Data Block.
    *   It generates a *new* Index (containing the old entries + the new entry).
    *   It writes the new Index at the very end.
    *   It updates the Header's `IndexOffset` to point to the new location.

This ensures that adding 1 mod to a cache of 1,000 takes milliseconds, as only the new data and the index map are written.

---

## 📥 Installation

1.  Go to the [Releases Page](../../releases).
2.  Download the version that suits you (see **Downloads** section below).
3.  Extract the zip anywhere on your computer.
4.  Run `Beam Manager.exe`.
5.  Select your BeamNG `mods` folder (usually `Documents\BeamNG.drive\mods`).

---

## 🏗 Building from Source

**Prerequisites:**
*   Visual Studio 2026 (v11429.125 or newer)
*   .NET 10 SDK

**Steps:**
1.  Clone the repository:
    ```bash
    git clone https://github.com/mastercodeon31415/Beam-Manager.git
    ```
2.  Open `Beam Manager.sln` in Visual Studio.
3.  Restore NuGet packages.
4.  Build the solution in **Release** mode.
5.  Run!

---

## 🤝 Contributing
Pull requests are welcome! If you find a bug or have a feature idea, please open an issue first to discuss it.

## Donation links

Anything is super helpful! Anything donated helps me keep developing this program and others!
- https://www.paypal.com/paypalme/lifeline42
- https://cash.app/$codoen314
- BTC: bc1qp8pay5qrg77kg2yyguvlwjxpnl8u0wl4r8hddp

## 📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

## Contact The Developers
- Discord handle: hatersgonnahate314