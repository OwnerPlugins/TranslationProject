## Translation Tool 2.8

**Download:** `TranslationProject.exe` below – single file, no installation, no .NET required.

### What's new
- Only **15 essential languages** selected by default (was ~100), with a **Default (15)** button.
- Chinese and other region languages (`zh_CN`, `pt_BR`, `zh_TW`…) are now really translated.
- Manual corrections in `.po` files are **kept** (multi-line entries from Poedit/msgmerge were read as empty and overwritten).
- No more double-escaped quotes in `.po` files.
- More strings extracted from Python plugins (`'...'` inside `"..."`, `_("a" "b")`, `u"..."`).
- C# mode: `.lng` files are no longer re-translated at every run.
- Import / Delete Cache in Enigma2 mode now use `locale/translation_cache.json`.
- 0 compiler warnings.

Full changelog: see README.
