using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Mvvm.Input;
using SWGEmuModManager.Models;
using SWGEmuModManager.Util;

namespace SWGEmuModManager.ViewModels
{
    internal class MainWindowViewModel : MainWindowViewModelProperties
    {
        public IAsyncRelayCommand GenerateModManifestMenuItem { get; }
        public IRelayCommand SetSwgDirectoryMenuItem { get; set; }
        public IAsyncRelayCommand DownloadModButton { get; }

        public MainWindowViewModel()
        {
            Task.Run(() => Initialize());
            GenerateModManifestMenuItem = new AsyncRelayCommand(GenerateModManifestAsync);
            SetSwgDirectoryMenuItem = new RelayCommand(SetSwgDirectory);
            DownloadModButton = new AsyncRelayCommand<int>(GetModDataAsync);

            MainWindowModel.OnDownloadProgressUpdated += DownloadProgressUpdated;
        }

        private async Task Initialize()
        {
            ModList = MainWindowModel.SetModDisplay(await ApiHandler.GetMods());
        }

        private async Task GenerateModManifestAsync()
        {
            using FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();

            if (result.ToString().Trim() == "OK")
            {
                await ManifestGenerator.GenerateModManifest(modsDirectory: dialog.SelectedPath.Replace(oldValue: "\\", newValue: "/"));
            }
        }

        private void SetSwgDirectory()
        {
            MainWindowModel.SetSwgDirectory();
        }

        private async Task GetModDataAsync(int id)
        {
            InstallRequestResponse response = await ApiHandler.InstallMod(id);

            ConfigFile config = ConfigFile.GetConfig()!;

            if (!string.IsNullOrEmpty(config.SwgDirectory) && 
                !string.IsNullOrEmpty(response.DownloadUrl) && 
                !string.IsNullOrEmpty(response.Archive))
            {
                await MainWindowModel.InstallMod(response.DownloadUrl, response.Archive);
            }
            else
            {
                MessageBox.Show(text: "No SWG directory set! Please set your SWG location in Config -> Set SWG Directory and try again.",
                    caption: "No SWG Directory Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void DownloadProgressUpdated(long bytesReceived, long totalBytesToReceive, int progressPercentage)
        {
            ProgressBarPercentage = progressPercentage;
        }
    }
}
