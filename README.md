## [Tool] Auto Translation Generator for C# projects – extract & translate GetTranslation strings into 90+ languages


Hi everyone,
I needed a tool that could extract all the strings to translate from the C# files and create the appropriate language files.
I developed a small Windows tool that automatically extracts all GetTranslation("...") keys from any C# project (WinForms, WPF, etc.) and translates them into 95+ languages using Google Translate (free API). It then generates the corresponding .lng files ready to be used in your application.



## 📸 Screenshots


<table align="center">
  <tr>
    <td align="center">
      <img src="screen/screen1.png?sanitize=true&raw=true" title="preview1" width="400"/><br/>
      <b>Preview 1</b>
    </td>
    <td align="center">
      <img src="screen/screen2.png?sanitize=true&raw=true" title="preview2" width="400"/><br/>
      <b>Preview 2</b>
    </td>
  </tr>
</table>



🚀 **Features**

- Scans recursively all .cs files in the selected project folder.

- Finds every string inside GetTranslation("...") – custom regex can be adapted.

- Uses Google Translate (free) to translate each string into all supported languages.

- Persistent cache: avoids re-translating the same text multiple times (saves time and bandwidth).

- Option to choose output folder; otherwise creates a "languages" subfolder inside the project.

- GUI with real-time log, progress bar, Start/Stop buttons.

- Preserves existing translations in .lng files (only adds new keys).

- Removes obsolete keys (keys no longer in the code) – optional but recommended.



⚙️ **How to use**

1. Download the attached .exe (or compile from source – see GitHub link).

2. Run the executable (no installation required).

3. Select the root folder of your C# project (the one containing all .cs files).

4. (Optional) Choose a custom output folder for the .lng files.

5. Click "Start Translation".

6. Wait – first run may take a while depending on the number of keys and languages (cache speeds up subsequent runs).

7. Find all your .lng files in the output folder, ready to be embedded or distributed.



📁 **Output format**

Each language gets its own file, named according to the language (e.g. Italiano.lng, Deutsch.lng, English.lng). The format is:




Changelog – Version 1.1

English

New Features:

Language selection dialog – Choose which languages to translate instead of processing all 95+ every time.
"Select Languages" button – Opens a checklist with all available languages (Select All / Unselect All buttons included).
Performance improvement – Translation time now proportional to the number of selected languages (much faster when only a few needed).
Changes:

Removed hardcoded full language list from main form; now managed via selector.
Improved cache handling – cache remains valid across different language selections.
Bug fixes:

Fixed occasional UI freeze during long operations.
Better error handling for network timeouts.
Credits: Lululla © 2026 – Support: linuxsat-support.com
