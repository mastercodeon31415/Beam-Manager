# Beam Manager 🚗💨

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-10.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

**Beam Manager** is a high-performance, external management tool for BeamNG.drive vehicle mods. It bridges the gap between Windows Explorer and the in-game repository, allowing you to visualize, inspect, and edit your mod collection instantly without launching the game.

---

## ✨ Key Features

### 🚀 **Instant Loading with Smart Caching**
Beam Manager uses a custom **Binary Cache System (`.bmc`)**.
*   **Zero-Wait:** After the initial scan, your library loads instantly, even with thousands of mods.
*   **Smart Sync:** The app detects file changes, renames, and deletions automatically. It only rescans files that have actually changed on disk.

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

1.  **The `.bmc` Binary Format**
    *   Instead of slow JSON/XML parsing, the app streams scan results into a custom binary file.
    *   Supports **Random Access Reads**, allowing specific mod data to be fetched from disk only when needed.
    *   Contains a movable **Footer Index** to allow appending new scan data without rewriting the entire file.

2.  **Concurrency & GDI+**
    *   **Producer-Consumer Pattern:** Background threads scan zips and queue UI updates. A precise timer on the UI thread dequeues them in batches to prevent UI freezing.
    *   **GDI+ Semaphore:** Image resizing is thread-safe, preventing `InvalidOperationException` during parallel processing.

3.  **Memory Management**
    *   **LRU Image Cache:** The app never holds all thumbnails in memory. It maintains a fixed buffer of active images. As you scroll, old images are disposed, and new ones are streamed from the `.bmc` file on demand.

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
*   Visual Studio 2022 (v17.12 or newer)
*   .NET 10 SDK

**Steps:**
1.  Clone the repository:
    ```bash
    git clone https://github.com/YourUsername/Beam-Manager.git
    ```
2.  Open `Beam Manager.sln` in Visual Studio.
3.  Restore NuGet packages.
4.  Build the solution in **Release** mode.
5.  Run!

---

## 🤝 Contributing
Pull requests are welcome! If you find a bug or have a feature idea, please open an issue first to discuss it.

## 📄 License
This project is licensed under the MIT License - see the LICENSE file for details.