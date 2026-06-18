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

            writer.Write((uint)0x950412DE);
            writer.Write((uint)0);
            writer.Write((uint)entries.Count);

            int origOffset = 28;
            writer.Write((uint)origOffset);
            int transOffset = origOffset + (entries.Count * 8);
            writer.Write((uint)transOffset);
            writer.Write((uint)0);
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

            int origPos = 0;
            foreach (var bytes in origTable)
            {
                writer.Write((uint)origPos);
                writer.Write((uint)bytes.Length);
                origPos += bytes.Length;
            }

            int transPos = 0;
            foreach (var bytes in transTable)
            {
                writer.Write((uint)transPos);
                writer.Write((uint)bytes.Length);
                transPos += bytes.Length;
            }

            foreach (var bytes in origTable) writer.Write(bytes);
            foreach (var bytes in transTable) writer.Write(bytes);

            Directory.CreateDirectory(Path.GetDirectoryName(moPath));
            File.WriteAllBytes(moPath, ms.ToArray());
        }

        private static Dictionary<string, string> ParsePo(string poPath)
        {
            var result = new Dictionary<string, string>();
            if (!File.Exists(poPath)) return result;

            var lines = File.ReadAllLines(poPath, Encoding.UTF8);
            string currentId = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("msgid \""))
                {
                    currentId = ExtractQuoted(line);
                }
                else if (line.StartsWith("msgstr \"") && currentId != null)
                {
                    string currentStr = ExtractQuoted(line);
                    if (!string.IsNullOrEmpty(currentId))
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