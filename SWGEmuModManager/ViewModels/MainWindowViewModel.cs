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
            GenerateModManifestMenuItem = new AsyncRelayCommand(GenerateModManifest);
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
