using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TranslationProject
{
    public class TranslationService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Action<string> _log;
        private readonly string _pluginPath;
        private readonly string _cacheFile;
        private Dictionary<string, string> _cache = new Dictionary<string, string>();

        public TranslationService(Action<string> logCallback, string pluginPath)
        {
            _log = logCallback;
            _pluginPath = pluginPath;
            _cacheFile = Path.Combine(pluginPath, "translation_cache.json");
            LoadCache();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task AutoTranslatePoFilesAsync(List<string> languages, CancellationToken token)
        {
            string pluginName = Path.GetFileName(_pluginPath);

            foreach (var lang in languages)
            {
                string poFile = Path.Combine(_pluginPath, "locale", lang, "LC_MESSAGES", $"{pluginName}.po");
                if (!File.Exists(poFile)) continue;

                _log($"  Auto-translating: {lang}");

                var lines = File.ReadAllLines(poFile, Encoding.UTF8).ToList();
                var newLines = new List<string>();
                int translated = 0;

                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (line.StartsWith("msgid \""))
                    {
                        string msgid = ExtractQuoted(line);
                        newLines.Add(line);
                        i++;

                        while (i < lines.Count && lines[i].Trim().StartsWith("\""))
                        {
                            newLines.Add(lines[i]);
                            i++;
                        }

                        if (i < lines.Count && lines[i].StartsWith("msgstr \""))
                        {
                            string msgstrLine = lines[i];
                            string msgstrContent = ExtractQuoted(msgstrLine);

                            // Solo se VUOTO (come in Python)
                            if (string.IsNullOrEmpty(msgstrContent.Trim()))
                            {
                                string translatedText = await TranslateTextAsync(msgid, lang, token);
                                if (!string.IsNullOrEmpty(translatedText) && translatedText != msgid)
                                {
                                    msgstrLine = $"msgstr \"{EscapeForPo(translatedText)}\"";
                                    translated++;
                                }
                            }
                            newLines.Add(msgstrLine);
                            i++;
                        }
                        continue;
                    }
                    newLines.Add(line);
                }

                if (translated > 0)
                {
                    _log($"    {lang}: translated {translated} new strings");
                    File.WriteAllLines(poFile, newLines, Encoding.UTF8);
                }
                else
                {
                    _log($"    {lang}: no new translations");
                }
            }

            SaveCache();
        }

        private async Task<string> TranslateTextAsync(string text, string targetLang, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            // Placeholder protection
            var placeholders = new Dictionary<string, string>();
            int idx = 0;
            string textWithPlaceholders = text;
            var placeholderRegex = new Regex(@"\{[^{}]+\}");
            foreach (Match match in placeholderRegex.Matches(text))
            {
                string placeholder = match.Value;
                string replacement = $"__PH_{idx}__";
                textWithPlaceholders = textWithPlaceholders.Replace(placeholder, replacement);
                placeholders[replacement] = placeholder;
                idx++;
            }

            // Cache
            string cacheKey = ComputeMd5($"{targetLang}:{text}");
            if (_cache.TryGetValue(cacheKey, out string cached))
                return cached;

            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={targetLang}&dt=t&q={Uri.EscapeDataString(textWithPlaceholders)}";

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

                var response = await _httpClient.GetAsync(url, linked.Token);
                string resp = await response.Content.ReadAsStringAsync(linked.Token);
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
                    foreach (var kvp in placeholders)
                        translated = translated.Replace(kvp.Key, kvp.Value);

                    _cache[cacheKey] = translated;
                    return translated;
                }
            }
            catch { }

            return text;
        }

        private string ExtractQuoted(string line)
        {
            int start = line.IndexOf('"');
            if (start < 0) return "";
            int end = line.LastIndexOf('"');
            if (end <= start) return "";
            return line.Substring(start + 1, end - start - 1);
        }

        private string EscapeForPo(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input;
            /*
            return input
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");
            */
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
                    string json = File.ReadAllText(_cacheFile);
                    _cache = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                catch { _cache = new Dictionary<string, string>(); }
            }
        }

        private void SaveCache()
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_cache, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_cacheFile, json);
            }
            catch { }
        }
    }
}