using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json.Linq;


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

        public Enigma2TranslationManager(Action<string> logCallback, string cachePath)
        {
            _log = logCallback;
            _cacheFile = cachePath;
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

        public async Task UpdatePoFileAsync(string poFile, string potFile, string targetLang, CancellationToken token)
        {
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

            int translated = 0;
            foreach (var msgid in msgids)
            {
                token.ThrowIfCancellationRequested();
                
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
                
                // FORCE FIX: Ensure \n matching for ALL translations
                if (msgid.StartsWith("\\n") && !translationToWrite.StartsWith("\\n"))
                {
                    translationToWrite = "\\n" + translationToWrite;
                }
                if (msgid.EndsWith("\\n") && !translationToWrite.EndsWith("\\n"))
                {
                    translationToWrite = translationToWrite + "\\n";
                }
                
                // FORCE FIX: Remove invalid backslashes AND escape quotes
                translationToWrite = EscapeForPo(translationToWrite);
                
                po.Add($"msgstr \"{translationToWrite}\"");
                po.Add("");
            }

            if (translated > 0)
                _log?.Invoke($"  {targetLang}: translated {translated} new strings");

            Directory.CreateDirectory(Path.GetDirectoryName(poFile));
            File.WriteAllLines(poFile, po, new UTF8Encoding(false));
        }

        private string EscapeForPo(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            // Escape double quotes
            string result = input.Replace("\"", "\\\"");

            // Remove invalid backslash sequences
            result = Regex.Replace(result, @"\\(?![ntr\""\\])", "");

            return result;
        }

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

            // 1. Protect escape sequences AND quotes
            var escapeMap = new Dictionary<string, string>();
            string protectedText = text;
            int idx = 0;

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

            // 2. Cache check
            if (_useCache)
            {
                string cacheKey = ComputeMd5($"{targetLang}:{protectedText}");
                if (_cache.TryGetValue(cacheKey, out string cached))
                {
                    if (!string.IsNullOrEmpty(cached))
                        return RestoreEscapes(cached, escapeMap);
                }
            }

            // 3. Translate with retry
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={targetLang}&dt=t&q={Uri.EscapeDataString(protectedText)}";

            int maxRetries = 3;
            int delay = 1000; // milliseconds

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
                    using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

                    var response = await _httpClient.GetAsync(url, linked.Token);
                    string resp = await response.Content.ReadAsStringAsync(linked.Token);

                    // Check if response is HTML (starts with <) – indicates error/block
                    if (resp.TrimStart().StartsWith("<"))
                    {
                        _log?.Invoke($"HTML response for {targetLang}, retry {attempt + 1}/{maxRetries}");
                        await Task.Delay(delay * (attempt + 1), token);
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

                        if (_useCache)
                        {
                            string cacheKey = ComputeMd5($"{targetLang}:{protectedText}");
                            _cache[cacheKey] = translated;
                            SaveCache();
                        }
                        return translated;
                    }

                    return text; // No translation returned
                }
                catch (Exception ex)
                {
                    _log?.Invoke($"Translation error for {targetLang}: {ex.Message}");
                    if (attempt < maxRetries - 1)
                    {
                        await Task.Delay(delay * (attempt + 1), token);
                    }
                }
            }

            return text; // Fallback after all retries
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
    }
}