using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TranslationProject
{
    public static class GettextTools
    {
        private static readonly string GettextPath = Environment.GetEnvironmentVariable("PATH") ?? "";

        public static bool IsGettextInstalled()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "xgettext",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                })?.WaitForExit(1000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RunXGettext(string pluginDir, string pluginName, Action<string> log)
        {
            try
            {
                log($"Running: xgettext for {pluginName}");

                var pyFiles = Directory.GetFiles(pluginDir, "*.py", SearchOption.AllDirectories);
                var filteredFiles = new System.Collections.Generic.List<string>();
                foreach (var f in pyFiles)
                {
                    var name = Path.GetFileName(f);
                    if (!name.StartsWith("test_") && name != "update_translation.py" && name != "translate_utils.py")
                        filteredFiles.Add($"\"{f}\"");
                }

                if (filteredFiles.Count == 0)
                {
                    log("No Python files found for extraction.");
                    return false;
                }

                string tempPot = Path.Combine(pluginDir, "temp_python.pot");
                string potFile = Path.Combine(pluginDir, "locale", $"{pluginName}.pot");

                Directory.CreateDirectory(Path.Combine(pluginDir, "locale"));

                var args = $"--no-wrap -L Python --from-code=UTF-8 -kpgettext:1c,2 --add-comments=TRANSLATORS: -d {pluginName} -s -o \"{tempPot}\" {string.Join(" ", filteredFiles)}";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "xgettext",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    log($"xgettext error: {error}");
                    return false;
                }

                if (File.Exists(tempPot))
                {
                    File.Copy(tempPot, potFile, true);
                    File.Delete(tempPot);
                    log($"POT created: {potFile}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                log($"xgettext exception: {ex.Message}");
                return false;
            }
        }

        public static bool RunMsgMerge(string poFile, string potFile, Action<string> log)
        {
            try
            {
                log($"  Running: msgmerge for {Path.GetFileName(poFile)}");

                var args = $"--update --backup=none --no-wrap \"{poFile}\" \"{potFile}\"";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msgmerge",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                log($"  msgmerge error: {ex.Message}");
                return false;
            }
        }

        public static bool RunMsgInit(string poFile, string potFile, string langCode, Action<string> log)
        {
            try
            {
                log($"  Running: msginit for {langCode}");

                string langHyphen = langCode.Replace('_', '-');

                var args = $"--no-wrap -i \"{potFile}\" -o \"{poFile}\" -l {langHyphen}";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msginit",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                log($"  msginit error: {ex.Message}");
                return false;
            }
        }

        public static bool RunMsgFmt(string poFile, string moFile, Action<string> log)
        {
            try
            {
                log($"  Running: msgfmt for {Path.GetFileName(poFile)}");

                var args = $"\"{poFile}\" -o \"{moFile}\"";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msgfmt",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                log($"  msgfmt error: {ex.Message}");
                return false;
            }
        }
    }
}