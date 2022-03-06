using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Mvvm.Input;
using SWGEmuModManager.Util;

namespace SWGEmuModManager.ViewModels
{
    internal class MainWindowViewModel : MainWindowViewModelProperties
    {
        public IAsyncRelayCommand GenerateModManifestMenuItem { get; }
        

        public MainWindowViewModel()
        {
            Task.Run(() => Initialize());
            GenerateModManifestMenuItem = new AsyncRelayCommand(GenerateModManifest);
        }

        private async Task Initialize()
        {
            List<Mod> mods = await ApiHandler.GetMods();
            List<ModsDisplay> modsResponse = new();

            mods.ForEach(mod =>
            {
                modsResponse.Add(new ModsDisplay()
                {
                    Id = mod.Id,
                    Name = mod.Name,
                    BannerUrl = mod.BannerUrl,
                    Description = mod.Description,
                    Author = $"Author: {mod.Author}",
                    Version = $"Version: {mod.Version}",
                    Size = $"Size: {UnitConversion.ToSize((long)mod.Size!, UnitConversion.SizeUnits.MB)}MB",
                    Downloads = $"Total Downloads: {mod.Downloads}",
                    Released = $"Released: {mod.Released.ToString("D", DateTimeFormatInfo.InvariantInfo)}"
                });
            });

            ModList = modsResponse;
        }

        private async Task GenerateModManifest()
        {
            using FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();

            if (result.ToString().Trim() == "OK")
            {
                await ManifestGenerator.GenerateModManifest(dialog.SelectedPath.Replace("\\", "/"));
            }
        }
    }
}
