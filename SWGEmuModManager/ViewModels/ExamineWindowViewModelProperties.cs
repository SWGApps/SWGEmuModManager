using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace SWGEmuModManager.ViewModels;

internal class ExamineWindowViewModelProperties : ObservableObject
{
    private MainWindowViewModelResponses.ModsDisplay? _modData;

    public MainWindowViewModelResponses.ModsDisplay? ModData
    {
        get => _modData;
        set => SetProperty(ref _modData, value);
    }
}
