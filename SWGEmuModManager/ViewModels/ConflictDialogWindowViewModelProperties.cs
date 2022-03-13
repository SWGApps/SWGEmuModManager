using System.Collections.Generic;
using System.Configuration;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace SWGEmuModManager.ViewModels;

internal class ConflictDialogWindowViewModelProperties : ObservableObject
{
    private List<string>? _conflictList;

    public List<string>? ConflictList
    {
        get => _conflictList;
        set => SetProperty(ref _conflictList, value);
    }
}
