# Translation Tool v2.0 – C# & Enigma2 Plugin Manager

<p align="center">
  <img src="https://komarev.com/ghpvc/?username=Belfagor2005&label=Repository%20Views&color=blueviolet" alt="Views">
</p>

<p align="center">
  <a href="https://ko-fi.com/lululla">
    <img src="https://img.shields.io/badge/_-Donate-red.svg?logo=ko-fi&labelColor=555555&style=for-the-badge" alt="Donate Ko-fi">
  </a>
  <a href="https://paypal.me/belfagor2005">
    <img src="https://img.shields.io/badge/_-Donate-green.svg?logo=paypal&labelColor=555555&style=for-the-badge" alt="Donate PayPal">
  </a>
</p>

<p align="center">
  <a href="https://github.com/OwnerPlugins/TranslationProject">
    <img src="https://img.shields.io/badge/Version-2.0-blue.svg" alt="Version">
  </a>
  <a href="https://www.gnu.org/licenses/gpl-3.0.html">
    <img src="https://img.shields.io/badge/License-GPLv3-blue.svg" alt="License">
  </a>
  <a href="https://github.com/OwnerPlugins/TranslationProject/releases">
    <img src="https://img.shields.io/badge/Download-Latest-green.svg" alt="Download">
  </a>
</p>


**Translation Tool** is a Windows desktop application that simplifies the translation workflow for **C# projects** and **Enigma2 plugins**. It extracts translatable strings, translates them using Google Translate, and compiles them into the required format – all with a user-friendly GUI.

---

## ✨ Features

### 🔹 C# Project Mode
- Scans `.cs` files recursively for `GetTranslation("...")` keys.
- Translates keys into **95+ languages** using Google Translate API.
- Generates `.lng` files ready for use in your application.
- Preserves existing translations and removes obsolete keys.

### 🔹 Enigma2 Plugin Mode
- Extracts strings from `.py` files and `setup.xml`.
- Generates/updates `.pot` and `.po` files.
- Auto-translates empty `msgstr` entries.
- Compiles `.mo` files ready for Enigma2 devices.

### 🔹 General Features
- **Dual Mode:** Switch between C# and Enigma2 modes.
- **Language Selection:** Choose from over 90 languages.
- **Select / Unselect All:** Quickly manage language selection.
- **Smart Cache:** Saves translations locally to avoid repeated API calls.
- **Import Cache:** Import translations from Python scripts.
- **Delete Cache:** Clear the cache to force re-translation.
- **Progress Monitor:** Real-time progress, status, and elapsed time.
- **Stop Button:** Cancel operations at any time.
- **Save Log:** Export the log to a text file.
- **Custom Output Folder:** Choose where to save translated files.
- **Logo Click:** Click the logo to open the GitHub repository.

---

## 📸 Screenshots

<table align="center">
  <tr>
    <td align="center">
      <img src="screen/screen1.png?sanitize=true&raw=true" title="Main Interface" width="400"/><br/>
      <b>Main Interface</b>
    </td>
    <td align="center">
      <img src="screen/screen2.png?sanitize=true&raw=true" title="Enigma2 Mode" width="400"/><br/>
      <b>Enigma2 Mode</b>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="screen/screen3.png?sanitize=true&raw=true" title="Language Selection" width="400"/><br/>
      <b>Language Selection</b>
    </td>
    <td align="center">
      <img src="screen/screen4.png?sanitize=true&raw=true" title="Progress Monitor" width="400"/><br/>
      <b>Progress Monitor</b>
    </td>
  </tr>
</table>

---

## 📥 Requirements

- **Windows** (7, 8, 10, 11)
- **.NET 10.0 Runtime** (or newer)
- **No Python required** – everything is self-contained in C#.

---

## 🚀 How to Use

### For C# Projects

1. Launch the tool and select **"C# Translation Project"** from the Mode dropdown.
2. Click **"Browse..."** and select your project folder (containing `.cs` files).
3. (Optional) Choose a custom output folder.
4. Click **"Select Languages"** and choose the languages you need.
5. Click **"Start"** to begin extraction and translation.
6. Find your `.lng` files in the output folder.

### For Enigma2 Plugins

1. Launch the tool and select **"Enigma2 Plugin Manager"** from the Mode dropdown.
2. Click **"Browse..."** and select your plugin folder (containing `__init__.py` and `locale/`).
3. Select the languages you want.
4. Use the buttons:
   - **Extract Strings** – Scan Python and XML files.
   - **Auto Translate** – Translate empty `msgstr` entries.
   - **Compile .mo** – Compile `.po` files to `.mo`.
   - **Full Update** – Run all steps in one click.
5. Monitor progress and wait for completion.

---

## ⚙️ Output Format

### C# Projects
Each language gets its own `.lng` file (e.g., `Italiano.lng`, `Deutsch.lng`) with the format:

```
Key1: Translated text
Key2: Another translation
```

### Enigma2 Plugins
The tool generates the standard gettext structure:

```
plugin/locale/
├── it/
│   └── LC_MESSAGES/
│       ├── plugin.po
│       └── plugin.mo
└── de/
    └── LC_MESSAGES/
        ├── plugin.po
        └── plugin.mo
```

---

## 🛠️ Cache Management

- **Use Cache:** Enable/disable the translation cache.
- **Delete Cache:** Remove all cached translations (forces re-translation).
- **Import Cache:** Import translations from a Python script's `translation_cache.json` file. This is useful if you already have translations from the Python version.

---

## 📁 Project Structure

```
TranslationProject/
├── TranslationProject.exe        # Main executable
├── Google-AI.png                 # Logo (optional)
├── app.ico                       # Application icon
├── translation_cache.json        # Cache file (auto-generated)
└── locale/                       # Enigma2 translation files
    ├── it/
    │   └── LC_MESSAGES/
    │       ├── plugin.po
    │       └── plugin.mo
    └── ...
```

---

## 🔧 Changelog

### Version 2.0 – 2026-06-18
- **Added:** Full Enigma2 plugin support (extract, translate, compile).
- **Added:** Dual mode selector (C# / Enigma2).
- **Added:** Language selection dialog with Select All / Unselect All.
- **Added:** Progress monitor with status, counter, and timer.
- **Added:** Stop button to cancel operations.
- **Added:** Import/Delete cache functionality.
- **Added:** Save log to file.
- **Added:** Logo with clickable link to GitHub.
- **Improved:** Cache handling and performance.
- **Fixed:** Various UI and stability issues.

### Version 1.0 – Initial Release
- Extract `GetTranslation("...")` keys from C# projects.
- Translate into 95+ languages using Google Translate.
- Generate `.lng` files.
- Persistent cache to avoid repeated translations.
- GUI with real-time log and progress bar.

---

## 💬 Support

- **Forum:** [LinuxSat-Support](https://www.linuxsat-support.com)
- **GitHub:** [OwnerPlugins/TranslationProject](https://github.com/OwnerPlugins/TranslationProject)
- **Developer:** Lululla

---

## 📜 License

This project is licensed under the **GPLv3 License** – see the [LICENSE](LICENSE) file for details.

---

## 🙏 Credits

Special thanks to the **Enigma2 community**, **CORVOBOYS**, and all testers for their support and feedback.

---

**Enjoy!** 😊

*– Lululla © 2026*
```
