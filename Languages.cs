namespace TranslationProject
{
    /// <summary>
    /// Language list shared by C# mode, Enigma2 mode and the language selector.
    /// </summary>
    public static class Languages
    {
        /// <summary>All languages the tool can translate to (code -> display name).</summary>
        public static readonly Dictionary<string, string> All = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "af", "Afrikaans" }, { "am", "Amharic" }, { "ar", "Arabic" }, { "az", "Azerbaijani" },
            { "be", "Belarusian" }, { "bg", "Bulgarian" }, { "bn", "Bengali" }, { "bs", "Bosnian" },
            { "ca", "Catalan" }, { "cs", "Czech" }, { "cy", "Welsh" }, { "da", "Danish" },
            { "de", "Deutsch" }, { "el", "Greek" }, { "en", "English" }, { "en_GB", "English_GB" },
            { "eo", "Esperanto" }, { "es", "Español" }, { "et", "Estonian" }, { "eu", "Basque" },
            { "fa", "Persian" }, { "fi", "Finnish" }, { "fr", "Français" }, { "fy", "Frisian" },
            { "ga", "Irish" }, { "gd", "Scottish Gaelic" }, { "gl", "Galician" }, { "gu", "Gujarati" },
            { "he", "Hebrew" }, { "hi", "Hindi" }, { "hr", "Croatian" }, { "hu", "Hungarian" },
            { "hy", "Armenian" }, { "id", "Indonesian" }, { "is", "Icelandic" }, { "it", "Italiano" },
            { "ja", "Japanese" }, { "ka", "Georgian" }, { "kk", "Kazakh" }, { "km", "Khmer" },
            { "kn", "Kannada" }, { "ko", "Korean" }, { "ku", "Kurdish" }, { "ky", "Kyrgyz" },
            { "lt", "Lithuanian" }, { "lv", "Latvian" }, { "mk", "Macedonian" }, { "ml", "Malayalam" },
            { "mn", "Mongolian" }, { "mr", "Marathi" }, { "ms", "Malay" }, { "mt", "Maltese" },
            { "my", "Burmese" }, { "nb", "Norwegian Bokmål" }, { "ne", "Nepali" }, { "nl", "Dutch" },
            { "no", "Norwegian" }, { "oc", "Occitan" }, { "or", "Odia" }, { "pa", "Punjabi" },
            { "pl", "Polski" }, { "ps", "Pashto" }, { "pt", "Português" }, { "pt_BR", "Português_BR" },
            { "pt_PT", "Português_PT" }, { "ro", "Romanian" }, { "ru", "Русский" }, { "si", "Sinhala" },
            { "sk", "Slovak" }, { "sl", "Slovenian" }, { "sq", "Shqip" }, { "sq_AL", "Shqip_AL" },
            { "sr", "Serbian" }, { "sr_Latn", "Serbian_Latin" }, { "sv", "Swedish" }, { "sw", "Swahili" },
            { "ta", "Tamil" }, { "te", "Telugu" }, { "tg", "Tajik" }, { "th", "Thai" }, { "tk", "Turkmen" },
            { "tl", "Tagalog" }, { "tr", "Türk" }, { "tt", "Tatar" }, { "ug", "Uyghur" }, { "uk", "Ukrainian" },
            { "ur", "Urdu" }, { "uz", "Uzbek" }, { "vi", "Vietnamese" }, { "yi", "Yiddish" },
            { "zh", "中国人" }, { "zh_CN", "简体中文" }, { "zh_HK", "Chinese_HK" }, { "zh_SG", "Chinese_SG" }, { "zh_TW", "Chinese_TW" }
        };

        /// <summary>
        /// Default selection: the most widely spoken languages that matter for
        /// Enigma2 users (Europe, Middle East, Asia). English is left out on
        /// purpose: Enigma2 msgids are already English.
        /// </summary>
        public static readonly string[] Essential =
        {
            "it", "de", "fr", "es", "pt", "ru", "uk", "pl",
            "nl", "el", "tr", "ar", "fa", "cs", "zh_CN"
        };

        public static bool IsEssential(string code) =>
            Array.Exists(Essential, e => e.Equals(code, StringComparison.OrdinalIgnoreCase));

        /// <summary>The essential languages as (code -> name).</summary>
        public static Dictionary<string, string> EssentialSet()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var code in Essential)
                if (All.TryGetValue(code, out var name))
                    result[code] = name;
            return result;
        }

        /// <summary>
        /// Code expected by Google Translate. Locale codes use '_' (zh_CN, pt_BR)
        /// while Google wants '-' (zh-CN) or a plain code: sending "zh_CN"
        /// makes the request fail and the text comes back untranslated.
        /// </summary>
        public static string GoogleCode(string code)
        {
            switch ((code ?? "").ToLowerInvariant())
            {
                case "zh":
                case "zh_cn":
                case "zh_sg": return "zh-CN";
                case "zh_tw":
                case "zh_hk": return "zh-TW";
                case "pt_br": return "pt";
                case "pt_pt": return "pt-PT";
                case "en_gb": return "en";
                case "sq_al": return "sq";
                case "sr_latn": return "sr";
                case "nb": return "no";
                case "he": return "iw";
                default: return (code ?? "").Replace('_', '-');
            }
        }
    }
}
