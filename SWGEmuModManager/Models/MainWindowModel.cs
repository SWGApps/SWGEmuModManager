using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using SWGEmuModManager.Util;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Models
{
    internal static class MainWindowModel
    {
        public static Action<long, long, int>? OnDownloadProgressUpdated { get; set; }

        public static List<MainWindowViewModelResponses.ModsDisplay> SetModDisplay(List<MainWindowViewModelResponses.Mod> mods)
        {
            List<MainWindowViewModelResponses.ModsDisplay> modsDisplay = new();

            mods.ForEach(mod =>
            {
                modsDisplay.Add(new MainWindowViewModelResponses.ModsDisplay()
                {
                    Id = mod.Id,
                    Name = mod.Name,
                    BannerUrl = mod.BannerUrl,
                    Description = mod.Description,
                    Author = $"{mod.Author}",
                    Version = $"{mod.Version}",
                    Size = $"{UnitConversion.ToSize((long)mod.Size!, UnitConversion.SizeUnits.MB)}",
                    Downloads = $"{mod.Downloads}",
                    Released = $"{mod.Released.ToString("d", DateTimeFormatInfo.InvariantInfo)}"
                });
            });

            return modsDisplay;
        }

        private static bool CheckBaseInstallation(string location)
        {
            if (!Directory.Exists(location)) return false;

            // Files required to exist
            List<string> filesToCheck = new()
            {
                "dpvs.dll",
                "Mss32.dll",
                "dbghelp.dll"
            };

            // Files in selected SWG directory
            List<string> files = Directory.GetFiles(location, "*.*", SearchOption.AllDirectories).ToList();

            int requiredFiles = 0;

            filesToCheck.ForEach(fileToCheck =>
            {
                files.ForEach(file =>
                {
                    if (fileToCheck == file.Split(location + "\\")[1].Trim()) requiredFiles++;
                });
            });

            if (requiredFiles == 3) return true;

            return false;
        }

        public static void SetSwgDirectory()
        {
            using FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();

            if (result.ToString().Trim() == "OK")
            {
                if (CheckBaseInstallation(
                        location: dialog.SelectedPath.Replace(oldValue: "\\", newValue: "/")))
                {
                    ConfigFile config = ConfigFile.GetConfig()!;

                    config!.SwgDirectory = dialog.SelectedPath.Replace("\\", "/");

                    ConfigFile.SetConfig(config);
                }
                else
                {
                    MessageBox.Show(text: "Invalid SWG Directory, please select a directory containing a SWG installation.",
                        caption: "Invalid SWG Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public static List<int> CheckConflictList(List<int>? conflictList)
        {
            ConfigFile config = ConfigFile.GetConfig()!;

            if (config.InstalledMods is not null && config.InstalledMods.Count > 0)
            {
                return conflictList!.Intersect(config.InstalledMods).ToList();
            }

            return new List<int>();
        }

        public static bool ModIsInstalled(int id)
        {
            ConfigFile? config = ConfigFile.GetConfig();

            if (config is not null && config.InstalledMods!.Count > 0)
            {
                if (config.InstalledMods.Any(x => x == id))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task DownloadModAsync(int modId, string downloadUrl, string archiveName)
        {
            using HttpClient client = new();

            ConfigFile? config = ConfigFile.GetConfig();

            if (config is not null && !string.IsNullOrEmpty(config.SwgDirectory))
            {
                using HttpResponseMessage response = client.GetAsync(new Uri($"{downloadUrl}{archiveName}"),
                    HttpCompletionOption.ResponseHeadersRead).Result;

                long length = int.Parse(response.Content.Headers.First(h => 
                    h.Key.Equals("Content-Length")).Value.First());

                // Shouldn't be needed with status code check below
                // response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    using Stream contentStream = await response.Content.ReadAsStreamAsync();

                    using Stream fileStream = new FileStream(Path.Join(config.SwgDirectory, archiveName),
                        FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                    await DoStreamWriteAsync(contentStream, fileStream, length);

                    await fileStream.DisposeAsync();

                    await ZipArchiveExtension.UnzipModAsync(modId, archiveName);
                }
            }
        }

        private static async Task DoStreamWriteAsync(Stream contentStream, Stream fileStream, long length)
        {
            long bytesReceived = 0L;
            long totalBytesToReceive = 0L;
            byte[] buffer = new byte[8192];
            bool endOfStream = false;

            while (!endOfStream)
            {
                var read = await contentStream.ReadAsync(buffer);
                if (read == 0)
                {
                    endOfStream = true;
                }
                else
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, read));

                    bytesReceived += read;
                    totalBytesToReceive += 1;

                    if (totalBytesToReceive % 100 == 0)
                    {
                        int progressPercentage = (int)Math.Round((double)bytesReceived / length * 1000, 0);
                        OnDownloadProgressUpdated?.Invoke(bytesReceived, totalBytesToReceive, progressPercentage);
                    }
                }
            }
        }

        public static async Task UninstallMod(int id, List<string> fileList)
        {
            ConfigFile? config = ConfigFile.GetConfig();

            await Task.Run(() =>
            {
                if (config is not null)
                {
                    if (!string.IsNullOrEmpty(config.SwgDirectory))
                    {
                        foreach (string file in fileList)
                        {
                            string filePath = Path.Join(config.SwgDirectory, file);

                            if (File.Exists(filePath)) File.Delete(filePath);
                        }
                    }
                }
            });

            config!.InstalledMods!.RemoveAll(mid => mid == id);

            ConfigFile.SetConfig(config);
        }
    }
}
