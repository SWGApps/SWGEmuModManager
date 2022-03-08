using System.Collections.Generic;
using System.Windows;

namespace SWGEmuModManager.ViewModels
{
    internal class MainWindowViewModelProperties : MainWindowViewModelResponses
    {
        private List<ModsDisplay>? _modList;
        private int? _progressBarPercentage;
        private Visibility? _progessBarVisibility;
        private string? _progressBarStatusLabel;
        private string? _installButtonText;

        public List<ModsDisplay>? ModList
        {
            get => _modList;
            set => SetProperty(ref _modList, value);
        }

        public int? ProgressBarPercentage
        {
            get => _progressBarPercentage;
            set => SetProperty(ref _progressBarPercentage, value);
        }

        public Visibility? ProgressBarVisibility
        {
            get => _progessBarVisibility;
            set => SetProperty(ref _progessBarVisibility, value);
        }

        public string? ProgressBarStatusLabel
        {
            get => _progressBarStatusLabel;
            set => SetProperty(ref _progressBarStatusLabel, value);
        }

        public string? InstallButtonText
        {
            get => _installButtonText;
            set => SetProperty(ref _installButtonText, value);
        }
    }
}
