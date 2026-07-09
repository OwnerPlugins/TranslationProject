using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;


namespace TranslationProject
{
    public class Enigma2TranslationManager
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private Dictionary<string, string> _escapeMap = new Dictionary<string, string>();
        private readonly string _cacheFile;
        private readonly Action<string> _log;
        private bool _useCache = true;
        private readonly ManualResetEventSlim _pauseEvent;
        private int _cacheHits = 0;
        private int _cacheMisses = 0;

        public Enigma2TranslationManager(Action<string> logCallback, string cachePath, ManualResetEventSlim pauseEvent = null)
        {
            _log = logCallback;
            _cacheFile = cachePath;
            _pauseEvent = pauseEvent;
            LoadCache();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public void SetCacheEnabled(bool enabled)
        {
            _useCache = enabled;
            _log?.Invoke($"Cache: {(enabled ? "ENABLED" : "DISABLED")}");
        }

        public List<string> ExtractPythonStrings(string pluginPath)
        {
            var strings = new HashSet<string>();
            var regex = new Regex(
                @"(?:_|gettext|pgettext)\s*\(\s*(?:""[^""]*""\s*,\s*)?[""']([^""']+)[""']\s*\)",
                RegexOptions.Compiled | RegexOptions.Multiline
            );

            foreach (var file in Directory.GetFiles(pluginPath, "*.py", SearchOption.AllDirectories))
            {
                string name = Path.GetFileName(file);
                if (name.StartsWith("test_") || name == "update_translation.py" || name == "translate_utils.py")
                    continue;

                try
                {
                    string content = File.ReadAllText(file, Encoding.UTF8);
                    foreach (Match m in regex.Matches(content))
                    {
                        string text = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(text) && !text.StartsWith("#") && !text.StartsWith("/*"))
                            strings.Add(text);
                    }
                }
                catch { }
            }

            _log?.Invoke($"Python: {strings.Count} strings");
            return strings.ToList();
        }

        public List<string> ExtractXmlStrings(string pluginPath)
        {
            var strings = new HashSet<string>();
            string xmlFile = Path.Combine(pluginPath, "setup.xml");
            if (!File.Exists(xmlFile))
            {
                _log?.Invoke("XML: setup.xml not found");
                return new List<string>();
            }

            try
            {
                var doc = new XmlDocument();
                doc.Load(xmlFile);

                foreach (string attr in new[] { "text", "description", "title" })
                {
                    var nodes = doc.SelectNodes($"//*[@{attr}]");
                    if (nodes == null) continue;
                    foreach (XmlNode node in nodes)
                    {
                        string value = node.Attributes?[attr]?.Value?.Trim();
                        if (!string.IsNullOrEmpty(value) && !value.StartsWith("#") && !value.StartsWith("0x"))
                            strings.Add(value);
                    }
                }
            }
            catch { }

            _log?.Invoke($"XML: {strings.Count} strings");
            return strings.ToList();
        }

        public void UpdatePot(string potFile, List<string> strings, string pluginName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(potFile));

            var lines = new List<string>();
            lines.Add($"# {pluginName} translations");
            lines.Add("# Copyright (C) 2026 Lululla Team");
            lines.Add("#");
            lines.Add("msgid \"\"");
            lines.Add("msgstr \"\"");
            lines.Add("\"Project-Id-Version: PACKAGE VERSION\\n\"");
            lines.Add("\"POT-Creation-Date: \\n\"");
            lines.Add("\"PO-Revision-Date: \\n\"");
            lines.Add("\"Last-Translator: \\n\"");
            lines.Add("\"Language-Team: \\n\"");
            lines.Add("\"Language: \\n\"");
            lines.Add("\"MIME-Version: 1.0\\n\"");
            lines.Add("\"Content-Type: text/plain; charset=UTF-8\\n\"");
            lines.Add("\"Content-Transfer-Encoding: 8bit\\n\"");
            lines.Add("");

            foreach (var str in strings.OrderBy(s => s))
            {
                lines.Add($"msgid \"{Escape(str)}\"");
                lines.Add("msgstr \"\"");
                lines.Add("");
            }

            File.WriteAllLines(potFile, lines, new UTF8Encoding(false));
            _log?.Invoke($"POT: {potFile} ({strings.Count} entries)");
        }

        /// <summary>
        /// Updates a .po file with translations from the .pot template.
        /// Preserves existing translations and auto-translates missing ones.
        /// Supports Pause (async via ManualResetEventSlim) and Stop (via CancellationToken).
        /// </summary>
        /// <param name="poFile">Path to the .po file to update</param>
        /// <param name="potFile">Path to the .pot template file</param>
        /// <param name="targetLang">Target language code (e.g., "it", "fr")</param>
        /// <param name="token">Cancellation token for Stop operation</param>
        public async Task UpdatePoFileAsync(string poFile, string potFile, string targetLang, CancellationToken token)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _log?.Invoke($"  ⏱️ Starting translation for: {targetLang}");

            // ============================================================
            // 1. READ ALL MSGIDS FROM POT FILE
            // ============================================================
            var potLines = File.ReadAllLines(potFile, Encoding.UTF8);
            var msgids = new List<string>();
            foreach (var line in potLines)
            {
                if (line.StartsWith("msgid \""))
                {
                    string msgid = line.Substring(7, line.Length - 8);
                    if (!string.IsNullOrEmpty(msgid))
                        msgids.Add(msgid);
                }
            }

            // ============================================================
            // 2. LOAD EXISTING TRANSLATIONS FROM PO FILE
            // ============================================================
            var existing = new Dictionary<string, string>();
            if (File.Exists(poFile))
            {
                var poLines = File.ReadAllLines(poFile, Encoding.UTF8);
                string currentId = null;
                foreach (var line in poLines)
                {
                    if (line.StartsWith("msgid \""))
                        currentId = line.Substring(7, line.Length - 8);
                    else if (line.StartsWith("msgstr \"") && currentId != null)
                    {
                        existing[currentId] = line.Substring(8, line.Length - 9);
                        currentId = null;
                    }
                }
            }

            // ============================================================
            // 3. BUILD NEW PO FILE CONTENT
            // ============================================================
            var po = new List<string>();
            po.Add($"# {targetLang} translations");
            po.Add("# Copyright (C) 2026 Lululla Team");
            po.Add("#");
            po.Add("msgid \"\"");
            po.Add("msgstr \"\"");
            po.Add($"\"Project-Id-Version: PACKAGE VERSION\\n\"");
            po.Add($"\"POT-Creation-Date: \\n\"");
            po.Add($"\"PO-Revision-Date: \\n\"");
            po.Add($"\"Last-Translator: \\n\"");
            po.Add($"\"Language-Team: {targetLang} <ekekaz@gmail.com>\\n\"");
            po.Add($"\"Language: {targetLang}\\n\"");
            po.Add("\"MIME-Version: 1.0\\n\"");
            po.Add("\"Content-Type: text/plain; charset=UTF-8\\n\"");
            po.Add("\"Content-Transfer-Encoding: 8bit\\n\"");
            po.Add("");

            // ============================================================
            // 4. PROCESS EACH STRING
            // ============================================================
            int translated = 0;
            int totalStrings = msgids.Count;
            int processed = 0;

            foreach (var msgid in msgids)
            {
                // ------------------------------------------------------------
                // PAUSE CHECK (async) - waits if paused, does not block UI
                // ------------------------------------------------------------
                if (_pauseEvent != null)
                {
                    await Task.Run(() => _pauseEvent.Wait(token), token);
                }

                // ------------------------------------------------------------
                // STOP CHECK - throws if cancellation requested
                // ------------------------------------------------------------
                token.ThrowIfCancellationRequested();

                processed++;
                po.Add($"msgid \"{EscapeForPo(msgid)}\"");

                string translationToWrite;
                if (existing.TryGetValue(msgid, out string existingTranslation) && !string.IsNullOrEmpty(existingTranslation))
                {
                    translationToWrite = existingTranslation;
                }
                else
                {
                    translationToWrite = await TranslateAsync(msgid, targetLang, token);
                    translated++;
                }

                // ------------------------------------------------------------
                // FORCE FIX: Ensure \n matching for ALL translations
                // ------------------------------------------------------------
                if (msgid.StartsWith("\\n") && !translationToWrite.StartsWith("\\n"))
                {
                    translationToWrite = "\\n" + translationToWrite;
                }
                if (msgid.EndsWith("\\n") && !translationToWrite.EndsWith("\\n"))
                {
                    translationToWrite = translationToWrite + "\\n";
                }

                // ------------------------------------------------------------
                // ESCAPE QUOTES AND INVALID BACKSLASHES
                // ------------------------------------------------------------
                translationToWrite = EscapeForPo(translationToWrite);

                po.Add($"msgstr \"{translationToWrite}\"");
                po.Add("");

                // ------------------------------------------------------------
                // PAUSE CHECK AGAIN (async) - immediate response
                // ------------------------------------------------------------
                if (_pauseEvent != null)
                {
                    await Task.Run(() => _pauseEvent.Wait(token), token);
                }

                // Log progress every 50 strings
                if (processed % 50 == 0)
                {
                    _log?.Invoke($"  📊 [{targetLang.ToUpper()}]: {processed}/{totalStrings} strings processed");
                }
            }

            // ============================================================
            // 5. COMPLETE AND SAVE
            // ============================================================
            stopwatch.Stop();

            if (translated > 0)
                _log?.Invoke($"  {targetLang}: translated {translated} new strings");

            _log?.Invoke($"  ⏱️ {targetLang}: completed in {stopwatch.ElapsedMilliseconds}ms, {translated} new translations");

            Directory.CreateDirectory(Path.GetDirectoryName(poFile));
            File.WriteAllLines(poFile, po, new UTF8Encoding(false));
        }

        /*
        public async Task UpdatePoFileAsync(string poFile, string potFile, string targetLang, CancellationToken token)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _log?.Invoke($"  ⏱️ Starting translation for: {targetLang}");

            // ============================================================
            // 1. READ ALL MSGIDS FROM POT FILE
            // ============================================================
            var potLines = File.ReadAllLines(potFile, Encoding.UTF8);
            var msgids = new List<string>();
            foreach (var line in potLines)
            {
                if (line.StartsWith("msgid \""))
                {
                    string msgid = line.Substring(7, line.Length - 8);
                    if (!string.IsNullOrEmpty(msgid))
                        msgids.Add(msgid);
                }
            }

            // ============================================================
            // 2. LOAD EXISTING TRANSLATIONS FROM PO FILE (if it exists)
            // ============================================================
            var existing = new Dictionary<string, string>();
            if (File.Exists(poFile))
            {
                var poLines = File.ReadAllLines(poFile, Encoding.UTF8);
                string currentId = null;
                foreach (var line in poLines)
                {
                    if (line.StartsWith("msgid \""))
                        currentId = line.Substring(7, line.Length - 8);
                    else if (line.StartsWith("msgstr \"") && currentId != null)
                    {
                        existing[currentId] = line.Substring(8, line.Length - 9);
                        currentId = null;
                    }
                }
            }

            // ============================================================
            // 3. BUILD NEW PO FILE CONTENT
            // ============================================================
            var po = new List<string>();
            po.Add($"# {targetLang} translations");
            po.Add("# Copyright (C) 2026 Lululla Team");
            po.Add("#");
            po.Add("msgid \"\"");
            po.Add("msgstr \"\"");
            po.Add($"\"Project-Id-Version: PACKAGE VERSION\\n\"");
            po.Add($"\"POT-Creation-Date: \\n\"");
            po.Add($"\"PO-Revision-Date: \\n\"");
            po.Add($"\"Last-Translator: \\n\"");
            po.Add($"\"Language-Team: {targetLang} <ekekaz@gmail.com>\\n\"");
            po.Add($"\"Language: {targetLang}\\n\"");
            po.Add("\"MIME-Version: 1.0\\n\"");
            po.Add("\"Content-Type: text/plain; charset=UTF-8\\n\"");
            po.Add("\"Content-Transfer-Encoding: 8bit\\n\"");
            po.Add("");

            // ============================================================
            // 4. PROCESS EACH STRING
            // ============================================================
            int translated = 0;
            int totalStrings = msgids.Count;
            int processed = 0;

            foreach (var msgid in msgids)
            {
                // ------------------------------------------------------------
                // PAUSE CHECK (async) - waits if paused, does not block UI
                // ------------------------------------------------------------
                if (_pauseEvent != null)
                {
                    await Task.Run(() => _pauseEvent.Wait(token), token);
                }

                // ------------------------------------------------------------
                // STOP CHECK - throws if cancellation requested
                // ------------------------------------------------------------
                token.ThrowIfCancellationRequested();

                processed++;
                po.Add($"msgid \"{EscapeForPo(msgid)}\"");

                string translationToWrite;

                // Check if translation already exists in the .po file
                if (existing.TryGetValue(msgid, out string existingTranslation) && !string.IsNullOrEmpty(existingTranslation))
                {
                    // Existing translation - check if it needs escaping
                    // If it contains " without \", it's not properly escaped
                    if (existingTranslation.Contains("\"") && !existingTranslation.Contains("\\\""))
                    {
                        translationToWrite = EscapeForPo(existingTranslation);
                    }
                    else
                    {
                        translationToWrite = existingTranslation;
                    }
                }
                else
                {
                    // New translation - TranslateAsync already escapes quotes and handles \n
                    translationToWrite = await TranslateAsync(msgid, targetLang, token);
                    translated++;
                }

                // ------------------------------------------------------------
                // FIX: Ensure \n matches at start/end of string
                // ------------------------------------------------------------
                if (msgid.StartsWith("\\n") && !translationToWrite.StartsWith("\\n"))
                    translationToWrite = "\\n" + translationToWrite;
                if (msgid.EndsWith("\\n") && !translationToWrite.EndsWith("\\n"))
                    translationToWrite = translationToWrite + "\\n";

                // ------------------------------------------------------------
                // ADD TO PO FILE
                // IMPORTANT: DO NOT call EscapeForPo here again!
                // translationToWrite is already properly escaped by TranslateAsync
                // or by the check above for existing translations.
                // ------------------------------------------------------------
                po.Add($"msgstr \"{translationToWrite}\"");
                po.Add("");

                // ------------------------------------------------------------
                // PAUSE CHECK AGAIN (async) - immediate response
                // ------------------------------------------------------------
                if (_pauseEvent != null)
                {
                    await Task.Run(() => _pauseEvent.Wait(token), token);
                }

                // Log progress every 50 strings
                if (processed % 50 == 0)
                {
                    _log?.Invoke($"  📊 [{targetLang.ToUpper()}]: {processed}/{totalStrings} strings processed");
                }
            }

            // ============================================================
            // 5. COMPLETE AND SAVE
            // ============================================================
            stopwatch.Stop();

            if (translated > 0)
                _log?.Invoke($"  {targetLang}: translated {translated} new strings");

            _log?.Invoke($"  ⏱️ {targetLang}: completed in {stopwatch.ElapsedMilliseconds}ms, {translated} new translations");

            // Save .po file
            Directory.CreateDirectory(Path.GetDirectoryName(poFile));
            File.WriteAllLines(poFile, po, new UTF8Encoding(false));
        } */

        /// <summary>
        /// Escapes special characters for gettext .po file format.
        /// Handles double quotes and invalid backslash sequences.
        /// Keeps \n, \t, \r, \", \\ as valid escape sequences.
        /// </summary>
        private string EscapeForPo(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            // Escape double quotes
            string result = input.Replace("\"", "\\\"");

            // Remove invalid backslash sequences (keep only \n, \t, \r, \", \\)
            result = Regex.Replace(result, @"\\(?![ntr\""\\])", "");

            return result;
        }

        /// <summary>
        /// Full update workflow for Enigma2 plugins.
        /// Extracts strings, updates POT, translates all languages, and compiles MO files.
        /// Supports Pause (async) and Stop (CancellationToken).
        /// </summary>
        public async Task RunFullUpdateAsync(string pluginPath, List<string> languages, CancellationToken token, string customPluginName = null)
        {
            string pluginName = string.IsNullOrEmpty(customPluginName)
                ? Path.GetFileName(pluginPath)
                : customPluginName;

            _log?.Invoke($"=== Full Update ===");
            _log?.Invoke($"Plugin: {pluginPath}");
            _log?.Invoke($"Name: {pluginName}");

            var python = ExtractPythonStrings(pluginPath);
            var xml = ExtractXmlStrings(pluginPath);
            var all = python.Union(xml).Distinct().ToList();
            _log?.Invoke($"Extracted: {python.Count} Python, {xml.Count} XML, {all.Count} total");

            string potFile = Path.Combine(pluginPath, "locale", $"{pluginName}.pot");
            UpdatePot(potFile, all, pluginName);

            int total = languages.Count;
            int current = 0;
            foreach (var lang in languages)
            {
                // ============================================================
                // CHECK PAUSE (ASYNC) - does not block UI
                // ============================================================
                if (_pauseEvent != null)
                {
                    await Task.Run(() => _pauseEvent.Wait(token), token);
                }

                // ============================================================
                // CHECK STOP
                // ============================================================
                token.ThrowIfCancellationRequested();

                current++;
                _log?.Invoke($"[{current}/{total}] {lang}");

                string poFile = Path.Combine(pluginPath, "locale", lang, "LC_MESSAGES", $"{pluginName}.po");
                await UpdatePoFileAsync(poFile, potFile, lang, token);

                string moFile = Path.Combine(pluginPath, "locale", lang, "LC_MESSAGES", $"{pluginName}.mo");
                bool success = GettextTools.RunMsgFmt(poFile, moFile, _log);
                if (!success) _log($"  ERROR compiling {lang}");
            }

            SaveCache();
            _log?.Invoke("=== Update completed ===");
        }

        private async Task<string> TranslateAsync(string text, string targetLang, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            // ------------------------------------------------------------
            // 1. Protect Python placeholders: %(name)s, %(name)d, etc.
            // ------------------------------------------------------------
            var pythonPlaceholders = new Dictionary<string, string>();
            int idx = 0;
            string textWithPythonPlaceholders = text;
            var pythonRegex = new Regex(@"%\([a-zA-Z_][a-zA-Z0-9_]*\)[diouxXeEfFgGcrs]");
            foreach (Match match in pythonRegex.Matches(text))
            {
                string placeholder = match.Value;
                string replacement = $"__PYPH_{idx}__";
                textWithPythonPlaceholders = textWithPythonPlaceholders.Replace(placeholder, replacement);
                pythonPlaceholders[replacement] = placeholder;
                idx++;
            }

            // ------------------------------------------------------------
            // 2. Protect C# placeholders: {0}, {name}, ...
            // ------------------------------------------------------------
            var csharpPlaceholders = new Dictionary<string, string>();
            idx = 0;
            string textWithCSharpPlaceholders = textWithPythonPlaceholders;
            var csharpRegex = new Regex(@"\{[^{}]+\}");
            foreach (Match match in csharpRegex.Matches(textWithPythonPlaceholders))
            {
                string placeholder = match.Value;
                string replacement = $"__CSH_{idx}__";
                textWithCSharpPlaceholders = textWithCSharpPlaceholders.Replace(placeholder, replacement);
                csharpPlaceholders[replacement] = placeholder;
                idx++;
            }

            // ------------------------------------------------------------
            // 3. Protect escape sequences and double quotes
            // ------------------------------------------------------------
            var escapeMap = new Dictionary<string, string>();
            idx = 0;
            string protectedText = textWithCSharpPlaceholders;
            var escapeRegex = new Regex(@"\\[ntr\""]|\\\\");
            protectedText = escapeRegex.Replace(protectedText, match =>
            {
                string placeholder = $"ESC_{idx}";
                escapeMap[placeholder] = match.Value;
                idx++;
                return placeholder;
            });

            protectedText = protectedText.Replace("\"", "__QUOTE__");
            escapeMap["__QUOTE__"] = "\\\"";

            // ------------------------------------------------------------
            // 4. Check cache
            // ------------------------------------------------------------
            if (_useCache)
            {
                string cacheKey = ComputeMd5($"{targetLang}:{protectedText}");
                if (_cache.TryGetValue(cacheKey, out string cached))
                {
                    if (!string.IsNullOrEmpty(cached))
                    {
                        _cacheHits++;
                        string restored = RestoreEscapes(cached, escapeMap);
                        // Restore C# placeholders
                        foreach (var kvp in csharpPlaceholders)
                            restored = restored.Replace(kvp.Key, kvp.Value);
                        // Restore Python placeholders
                        foreach (var kvp in pythonPlaceholders)
                            restored = restored.Replace(kvp.Key, kvp.Value);
                        return restored;
                    }
                }
                _cacheMisses++;
            }

            // ------------------------------------------------------------
            // 5. Translate with retries
            // ------------------------------------------------------------
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={targetLang}&dt=t&q={Uri.EscapeDataString(protectedText)}";

            int maxRetries = 3;
            int baseDelay = 2000;
            Random random = new Random();

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var requestStopwatch = System.Diagnostics.Stopwatch.StartNew();

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

                    var response = await _httpClient.GetAsync(url, linked.Token);
                    string resp = await response.Content.ReadAsStringAsync(linked.Token);

                    requestStopwatch.Stop();

                    // Log with [LANG] in UPPERCASE
                    _log?.Invoke($"  📡 [{targetLang.ToUpper()}]: request {attempt + 1}/{maxRetries} took {requestStopwatch.ElapsedMilliseconds}ms, status: {response.StatusCode}");

                    if (resp.TrimStart().StartsWith("<"))
                    {
                        if (attempt >= maxRetries - 1)
                        {
                            _log?.Invoke($"  ❌ [{targetLang.ToUpper()}]: HTML response, all retries exhausted. Returning original text.");
                            return text;
                        }

                        int jitter = random.Next(0, 500);
                        int delay = baseDelay * (attempt + 1) + jitter;
                        _log?.Invoke($"  ⚠️ [{targetLang.ToUpper()}]: HTML response, retry {attempt + 1}/{maxRetries} (delay: {delay}ms)");

                        string htmlPreview = resp.Length > 200 ? resp.Substring(0, 200) + "..." : resp;
                        _log?.Invoke($"  📄 HTML preview: {htmlPreview}");

                        await Task.Delay(delay, token);
                        continue;
                    }

                    var json = JArray.Parse(resp);
                    string translated = "";
                    if (json[0] is JArray arr)
                    {
                        foreach (var item in arr)
                            if (item[0] != null)
                                translated += item[0].ToString();
                    }

                    if (!string.IsNullOrEmpty(translated))
                    {
                        translated = CleanWhitespace(translated);
                        translated = RestoreEscapes(translated, escapeMap);

                        // Restore C# placeholders
                        foreach (var kvp in csharpPlaceholders)
                            translated = translated.Replace(kvp.Key, kvp.Value);

                        // Restore Python placeholders
                        foreach (var kvp in pythonPlaceholders)
                            translated = translated.Replace(kvp.Key, kvp.Value);

                        // Save to cache
                        if (_useCache)
                        {
                            string cacheKey = ComputeMd5($"{targetLang}:{protectedText}");
                            _cache[cacheKey] = translated;
                            SaveCache();
                        }

                        // Log with [LANG] in UPPERCASE
                        _log?.Invoke($"  ✅ [{targetLang.ToUpper()}]: translated successfully (attempt {attempt + 1})");
                        return translated;
                    }

                    return text;
                }
                catch (Exception ex)
                {
                    if (attempt >= maxRetries - 1)
                    {
                        _log?.Invoke($"  ❌ [{targetLang.ToUpper()}]: all retries exhausted: {ex.Message}");
                        return text;
                    }

                    int jitter = random.Next(0, 500);
                    int delay = baseDelay * (attempt + 1) + jitter;
                    _log?.Invoke($"  ⚠️ [{targetLang.ToUpper()}]: error (attempt {attempt + 1}/{maxRetries}): {ex.Message}. Retrying in {delay}ms");
                    await Task.Delay(delay, token);
                }
            }

            return text;
        }

        private string RestoreEscapes(string input, Dictionary<string, string> escapeMap)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string result = input;

            // 1. Exact replacements
            foreach (var kvp in escapeMap)
            {
                result = result.Replace(kvp.Key, kvp.Value);
            }

            // 2. Fallback: remove ANY remaining placeholder
            // Matches: ESC_0, ESC_1, __ESC_0__, ESC__0, etc.
            result = Regex.Replace(result, @"_*ESC_+\d+_*", match =>
            {
                string key = match.Value;
                if (escapeMap.TryGetValue(key, out string replacement))
                    return replacement;

                // If the placeholder is for \n, return newline
                if (key.Contains("ESC"))
                {
                    // Check if this placeholder was for \n
                    foreach (var kvp in escapeMap)
                    {
                        if (kvp.Key == key || kvp.Value == "\\n")
                            return "\n";
                    }
                    return "\n"; // default
                }
                return match.Value;
            });

            return result;
        }

        private string Escape(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input;
            /*
            return input
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"")
                .Replace("\\", "\\\\"); */
        }

        private string CleanWhitespace(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var sb = new StringBuilder();
            bool space = false;
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    if (!space) sb.Append(c);
                    space = true;
                }
                else
                {
                    sb.Append(c);
                    space = false;
                }
            }
            return sb.ToString().Trim();
        }

        private string ComputeMd5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private void LoadCache()
        {
            if (File.Exists(_cacheFile))
            {
                try
                {
                    string json = File.ReadAllText(_cacheFile, new UTF8Encoding(false));
                    var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    if (temp != null)
                    {
                        foreach (var kvp in temp)
                            _cache[kvp.Key] = kvp.Value;
                        _log?.Invoke($"Cache loaded: {_cache.Count} entries");
                    }
                }
                catch { }
            }
        }

        private void SaveCache()
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_cache, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_cacheFile, json, new UTF8Encoding(false));
            }
            catch { }
        }

        public void LogCacheStats()
        {
            int total = _cacheHits + _cacheMisses;
            if (total == 0)
            {
                _log?.Invoke($"📊 Cache: No requests made");
                return;
            }

            double hitRate = (double)_cacheHits / total * 100;
            _log?.Invoke($"📊 Cache stats: Hits={_cacheHits}, Misses={_cacheMisses}, Total={total}, Hit Rate={hitRate:F1}%");
        }
    }
}