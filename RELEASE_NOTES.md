## Translation Tool 2.9.1

**Download:** `TranslationProject.exe` below – single file, no installation, no .NET required.

### What's new
- **Fixed:** strings with spaces at the start or end (e.g. `_("  Export Custom1 colors (OK)")`, `_("Directory creation error: ")`) lost those spaces in the `.pot`/`.po`, so Enigma2 never showed the translation. They are now extracted exactly as written, and the translation keeps the same spaces.
- Includes 2.9: Google "Too Many Requests" handled with pauses, failed strings are no longer saved in English.

Full changelog: see README.
