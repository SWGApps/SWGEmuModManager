using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.Input;
using SWGEmuModManager.Views;

namespace SWGEmuModManager.ViewModels;

internal class ConflictDialogWindowViewModel : ConflictDialogWindowViewModelProperties
{
    public IRelayCommand ContinueButton { get; set; }
    public IRelayCommand CancelButton { get; set; }
    public static Action? ClickedContinueButton { get; set; }
    public static Action? ClickedCancelButton { get; set; }
    private ConflictDialogWindow ConflictDialogWindow { get; set; }

    public ConflictDialogWindowViewModel(List<string> conflictList, ConflictDialogWindow conflictDialogWindow)
    {
        ContinueButton = new RelayCommand(Continue);
        CancelButton = new RelayCommand(Cancel);

        ConflictList = conflictList;
        ConflictDialogWindow = conflictDialogWindow;
    }

    private void Continue()
    {
        ClickedContinueButton?.Invoke();
        ConflictDialogWindow.Close();
    }

    private void Cancel()
    {
        ClickedCancelButton?.Invoke();
        ConflictDialogWindow.Close();
    }
}
