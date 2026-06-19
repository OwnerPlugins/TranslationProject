using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TranslationProject
{
    public static class MoCompiler
    {
        public static void Compile(string poPath, string moPath)
        {
            var entries = ParsePo(poPath);
            if (entries.Count == 0) return;

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            // 1. Magic number (4 bytes)
            writer.Write((uint)0x950412DE);
            // 2. Version (4 bytes)
            writer.Write((uint)0);
            // 3. Number of strings (4 bytes) → HEADER IS COMPLETE (12 bytes total)
            writer.Write((uint)entries.Count);

            // 4. Offset to original strings table (4 bytes)
            int origOffset = 12 + (entries.Count * 8); // 12 bytes header + 8 bytes per entry
            writer.Write((uint)origOffset);
            // 5. Offset to translated strings table (4 bytes)
            int transOffset = origOffset + (entries.Count * 8);
            writer.Write((uint)transOffset);

            // 6. Number of hash table entries (4 bytes) - usually 0
            writer.Write((uint)0);
            // 7. Offset to hash table (4 bytes) - usually 0
            writer.Write((uint)0);

            var sortedKeys = entries.Keys.OrderBy(k => k).ToList();
            var origTable = new List<byte[]>();
            var transTable = new List<byte[]>();

            foreach (var key in sortedKeys)
            {
                string trans = entries[key];
                origTable.Add(Encoding.UTF8.GetBytes(key + "\x00"));
                transTable.Add(Encoding.UTF8.GetBytes(trans + "\x00"));
            }

            // Write original offset table
            int origPos = 0;
            foreach (var bytes in origTable)
            {
                writer.Write((uint)origPos);
                writer.Write((uint)bytes.Length);
                origPos += bytes.Length;
            }

            // Write translated offset table
            int transPos = 0;
            foreach (var bytes in transTable)
            {
                writer.Write((uint)transPos);
                writer.Write((uint)bytes.Length);
                transPos += bytes.Length;
            }

            // Write original strings
            foreach (var bytes in origTable) writer.Write(bytes);
            // Write translated strings
            foreach (var bytes in transTable) writer.Write(bytes);

            Directory.CreateDirectory(Path.GetDirectoryName(moPath));
            File.WriteAllBytes(moPath, ms.ToArray());
        }

        private static Dictionary<string, string> ParsePo(string poPath)
        {
            var result = new Dictionary<string, string>();
            if (!File.Exists(poPath)) return result;

            var lines = File.ReadAllLines(poPath, new UTF8Encoding(false));
            string currentId = null;
            bool inHeader = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // Skip header block
                if (line.StartsWith("msgid \"\"") && !inHeader)
                {
                    inHeader = true;
                    continue;
                }
                if (inHeader && line == "" && i + 1 < lines.Length && lines[i + 1].Trim().StartsWith("msgid \""))
                {
                    inHeader = false;
                    continue;
                }

                if (line.StartsWith("msgid \""))
                {
                    currentId = ExtractQuoted(line);
                }
                else if (line.StartsWith("msgstr \"") && currentId != null)
                {
                    string currentStr = ExtractQuoted(line);
                    if (!string.IsNullOrEmpty(currentId) && !string.IsNullOrEmpty(currentStr))
                    {
                        result[currentId] = currentStr;
                        currentId = null;
                    }
                }
            }
            return result;
        }

        private static string ExtractQuoted(string line)
        {
            int start = line.IndexOf('"');
            if (start < 0) return "";
            int end = line.LastIndexOf('"');
            if (end <= start) return "";
            return line.Substring(start + 1, end - start - 1);
        }
    }
}