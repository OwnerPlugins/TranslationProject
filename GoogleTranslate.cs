using Newtonsoft.Json.Linq;
using System.Net;

namespace TranslationProject
{
    /// <summary>
    /// Shared client for the free Google Translate endpoint.
    /// - one request at a time, with a minimum pause between requests;
    /// - when Google answers 429 / "Sorry..." page, the whole program pauses
    ///   (30s, 60s, 2m, 5m) and then retries the SAME string;
    /// - if Google keeps refusing, it stops asking for 15 minutes and returns
    ///   null: callers must leave the string untranslated (never write the
    ///   English text as a translation).
    /// </summary>
    internal static class GoogleTranslate
    {
        private const int MinIntervalMs = 400;
        private static readonly int[] CooldownSeconds = { 30, 60, 120, 300 };
        private static readonly TimeSpan BlockedFor = TimeSpan.FromMinutes(15);

        private static readonly HttpClient Http = CreateClient();
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);
        private static readonly Random Rng = new Random();
        private static DateTime _nextRequestUtc = DateTime.MinValue;
        private static DateTime _blockedUntilUtc = DateTime.MinValue;
        private static int _cooldownStep;
        private static bool _blockedLogged;

        /// <summary>True while Google is refusing requests (strings are skipped).</summary>
        public static bool IsBlocked => DateTime.UtcNow < _blockedUntilUtc;

        private static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0 Safari/537.36");
            return client;
        }

        /// <summary>
        /// Translates <paramref name="text"/>. Returns null when the text could
        /// not be translated (Google limit or network error).
        /// </summary>
        public static async Task<string?> TranslateAsync(string text, string googleLang,
            Action<string>? log, CancellationToken token)
        {
            string tag = googleLang.ToUpperInvariant();

            if (IsBlocked)
            {
                if (!_blockedLogged)
                {
                    _blockedLogged = true;
                    log?.Invoke($"  ⛔ [{tag}]: Google is still blocking requests. Remaining strings are left empty " +
                                $"and will be translated on the next run (try again after {_blockedUntilUtc.ToLocalTime():HH:mm}).");
                }
                return null;
            }
            _blockedLogged = false;

            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=" +
                         Uri.EscapeDataString(googleLang) + "&dt=t&q=" + Uri.EscapeDataString(text);
            int errors = 0;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                await WaitTurnAsync(token);

                HttpStatusCode status;
                string body;
                try
                {
                    using var response = await Http.GetAsync(url, token);
                    status = response.StatusCode;
                    body = await response.Content.ReadAsStringAsync(token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (++errors >= 3)
                    {
                        log?.Invoke($"  ❌ [{tag}]: network error, string left untranslated: {ex.Message}");
                        return null;
                    }
                    log?.Invoke($"  ⚠️ [{tag}]: network error ({ex.Message}), retry {errors}/3");
                    await Task.Delay(2000 * errors, token);
                    continue;
                }

                bool refused = status == HttpStatusCode.TooManyRequests
                               || body.TrimStart().StartsWith("<", StringComparison.Ordinal);
                if (refused)
                {
                    if (_cooldownStep >= CooldownSeconds.Length)
                    {
                        _cooldownStep = 0;
                        _blockedUntilUtc = DateTime.UtcNow + BlockedFor;
                        _blockedLogged = true;
                        log?.Invoke($"  ⛔ [{tag}]: Google keeps refusing (IP temporarily blocked). " +
                                    $"Translation paused until {_blockedUntilUtc.ToLocalTime():HH:mm}: " +
                                    "untranslated strings stay empty and will be done on the next run.");
                        return null;
                    }
                    int wait = CooldownSeconds[_cooldownStep++];
                    log?.Invoke($"  ⏸️ [{tag}]: Google limit reached ({(int)status}). Waiting {wait}s, " +
                                $"then retrying the same string ({_cooldownStep}/{CooldownSeconds.Length})");
                    _nextRequestUtc = DateTime.UtcNow.AddSeconds(wait);
                    continue;
                }

                if (!((int)status >= 200 && (int)status < 300))
                {
                    if (++errors >= 3)
                    {
                        log?.Invoke($"  ❌ [{tag}]: HTTP {(int)status}, string left untranslated");
                        return null;
                    }
                    await Task.Delay(2000 * errors, token);
                    continue;
                }

                _cooldownStep = 0;
                try
                {
                    var json = JArray.Parse(body);
                    var sb = new System.Text.StringBuilder();
                    if (json[0] is JArray parts)
                    {
                        foreach (var item in parts)
                        {
                            string? part = item[0]?.ToString();
                            if (!string.IsNullOrEmpty(part))
                                sb.Append(part);
                        }
                    }
                    string result = sb.ToString();
                    return result.Length > 0 ? result : null;
                }
                catch (Exception ex)
                {
                    log?.Invoke($"  ❌ [{tag}]: unexpected answer from Google: {ex.Message}");
                    return null;
                }
            }
        }

        /// <summary>One request at a time, at least MinIntervalMs apart.</summary>
        private static async Task WaitTurnAsync(CancellationToken token)
        {
            await Gate.WaitAsync(token);
            try
            {
                var now = DateTime.UtcNow;
                if (_nextRequestUtc > now)
                    await Task.Delay(_nextRequestUtc - now, token);
                _nextRequestUtc = DateTime.UtcNow.AddMilliseconds(MinIntervalMs + Rng.Next(0, 300));
            }
            finally
            {
                Gate.Release();
            }
        }
    }
}
