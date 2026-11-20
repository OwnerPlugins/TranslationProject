## Translation Tool 2.9

**Download:** `TranslationProject.exe` below – single file, no installation, no .NET required.

### What's new
- **Google "Too Many Requests" handled properly.** Requests go out one at a time with a short pause between them. When Google blocks (429 / "Sorry..." page) the program waits 30 s, 1 min, 2 min, 5 min and retries the same string, instead of hammering Google and making the block longer.
- If Google keeps blocking, translation stops asking for 15 minutes: the remaining strings are **left empty** and translated on the next run.
- **Fixed:** when Google refused, the English text was written as the "translation" (`.lng` and `.po`) and never retried. Now nothing is written, and translations equal to the English text left by older versions are translated again.

Full changelog: see README.
