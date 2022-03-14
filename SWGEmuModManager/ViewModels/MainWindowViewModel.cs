using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Toolkit.Mvvm.Input;
using SWGEmuModManager.Models;
using SWGEmuModManager.Util;
using SWGEmuModManager.Views;

namespace SWGEmuModManager.ViewModels;

internal class MainWindowViewModel : MainWindowViewModelProperties
{
    public IAsyncRelayCommand GenerateModManifestMenuItem { get; }
    public IRelayCommand SetSwgDirectoryMenuItem { get; set; }
    public IAsyncRelayCommand DownloadModButton { get; }
    public IRelayCommand CloseButton { get; set; }
    public IAsyncRelayCommand FilterNameButton { get; set; } 
    public bool ConflictContinue { get; set; }

    public MainWindowViewModel()
    {
        Task.Run(InitializeAsync);

        GenerateModManifestMenuItem = new AsyncRelayCommand(GenerateModManifestAsync);
        SetSwgDirectoryMenuItem = new RelayCommand(SetSwgDirectory);
        DownloadModButton = new AsyncRelayCommand<int>(GetModDataAsync);
        CloseButton = new RelayCommand(() => Environment.Exit(0));
        FilterNameButton = new AsyncRelayCommand(FilterByName);

        MainWindowModel.OnDownloadProgressUpdated += DownloadProgressUpdated;
        ZipArchiveExtension.OnInstallStarted += InstallStarted;
        ZipArchiveExtension.OnInstallProgressUpdated += InstallProgressUpdated;
        MainWindowModel.OnUninstallDone += UninstallDone;
        ConflictDialogWindowViewModel.ClickedContinueButton += ClickedContinueButton;
        ConflictDialogWindowViewModel.ClickedCancelButton += ClickedCancelButton;

        FilterWatermark = "Sort by name";
    }

    private async Task InitializeAsync()
    {
        ProgressBarVisibility = Visibility.Collapsed;
        await RefreshModDisplay();
        InstallButtonText = "Install";
    }

    private async Task GenerateModManifestAsync()
    {
        using FolderBrowserDialog dialog = new();
        DialogResult result = dialog.ShowDialog();

        if (result.ToString().Trim() == "OK")
        {
            await ManifestGenerator.GenerateModManifestAsync(
                modsDirectory: dialog.SelectedPath.Replace(oldValue: "\\", newValue: "/"));
        }
    }

    private void SetSwgDirectory()
    {
        MainWindowModel.SetSwgDirectory();
    }

    private async Task GetModDataAsync(int id)
    {
        ConfigFile config = ConfigFile.GetConfig()!;

        // Uninstall mod
        if (MainWindowModel.ModIsInstalled(id))
        {
            Response<UninstallRequestResponse> uninstallResponse = await ApiHandler.UninstallModAsync(id);

            ProgressBarVisibility = Visibility.Visible;

            /*List<int> allowedConflicts = MainWindowModel.CheckConflictList(uninstallResponse.ConflictList);

            allowedConflicts.ForEach(conflict =>
            {
                Trace.WriteLine(conflict);
            });*/

            if (!string.IsNullOrEmpty(config.SwgDirectory))
            {
                if (uninstallResponse.Succeeded)
                {
                    if (uninstallResponse.Data!.FileList!.Count > 0)
                    {
                        ProgressBarStatusLabel = "Uninstalling " + ModList!
                            .Where(x => x.Id == id)
                            .Select(x => x.Name)
                            .FirstOrDefault() + "...";

                        await MainWindowModel.UninstallMod(id, uninstallResponse.Data.FileList);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(text: "Uninstall request response status: " +
                        $"{uninstallResponse.Succeeded} - Reason: {uninstallResponse.Errors![0]}", 
                        caption: "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    App.log.Error(message: $"Uninstall request response status: {uninstallResponse.Succeeded}" +
                        $" - Reason: {uninstallResponse.Errors[0]}");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(text: "No SWG directory set! " +
                    "Please set your SWG location in Config -> Set SWG Directory and try again.", 
                    caption: "No SWG Directory Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }

        // Install Mod
        Response<InstallRequestResponse> installResponse = await ApiHandler.InstallModAsync(id);

        // Set continue before getting conflicts, it will be set during conflict check
        // if conflicts are found
        ConflictContinue = true;

        List<string> conflictNames = MainWindowModel.GetConflictNames(installResponse.Data!.ConflictList!, ModList!, id);

        if (conflictNames.Count > 0)
        {
            new ConflictDialogWindow(conflictNames).ShowDialog();
        }

        if (!ConflictContinue) return;

        ProgressBarVisibility = Visibility.Visible;

        if (!string.IsNullOrEmpty(config.SwgDirectory) && 
                !string.IsNullOrEmpty(installResponse.Data.DownloadUrl) && 
                !string.IsNullOrEmpty(installResponse.Data.Archive))
        {
            if (installResponse.Succeeded)
            {
                ProgressBarStatusLabel = "Downloading " + ModList!
                    .Where(x => x.Id == id)
                    .Select(x => x.Name)
                    .FirstOrDefault() + "...";

                await MainWindowModel.DownloadModAsync(id, installResponse.Data.DownloadUrl, installResponse.Data.Archive);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(text: "Install request response status: " +
                    $"{installResponse.Succeeded} - Reason: {installResponse.Errors![0]}",
                    caption: "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                App.log.Error(message: "Install request response status: " +
                    $"{installResponse.Succeeded} - Reason: {installResponse.Errors[0]}");
            }
        }
        else
        {
            System.Windows.Forms.MessageBox.Show(text: "No SWG directory set! " +
                "Please set your SWG location in Config -> Set SWG Directory and try again.",
                caption: "No SWG Directory Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        await InstallDone(id);
    }

    private void DownloadProgressUpdated(long bytesReceived, long totalBytesToReceive, int progressPercentage)
    {
        ProgressBarPercentage = progressPercentage;
    }

    private void InstallStarted()
    {
        ProgressBarStatusLabel = ProgressBarStatusLabel!.Replace(oldValue: "Downloading", newValue: "Installing");
    }

    private void InstallProgressUpdated(int currentFile, int totalFiles)
    {
        ProgressBarPercentage = (currentFile / totalFiles) * 1000;
    }

    private async Task InstallDone(int id)
    {
        System.Threading.Thread.Sleep(millisecondsTimeout: 1000);
        ProgressBarVisibility = Visibility.Collapsed;
        Response<object> response = await ApiHandler.AddDownloadAsync(id);

        if (response.Succeeded) await RefreshModDisplay();
        // todo - else notify of failure
    }

    private async void UninstallDone()
    {
        ProgressBarVisibility = Visibility.Collapsed;

        await RefreshModDisplay();
    }

    private async Task RefreshModDisplay()
    {
        ModList = await MainWindowModel.SetModDisplay(
            await ApiHandler.GetModsAsync(1, 10, SortType, SortOrder!, FilterValue!));
    }

    private void ClickedContinueButton()
    {
        ConflictContinue = true;
    }

    private void ClickedCancelButton()
    {
        ConflictContinue = false;
    }

    internal static void WatermarkIntercept(MainWindowViewModelProperties vmp)
    {
        if (vmp.FilterValue is not null)
        {
            vmp.FilterWatermark = (vmp.FilterValue!.Length < 1) ? "Filter" : string.Empty;
        }
    }

    internal static void UpdateFilterMargin(MainWindowViewModelProperties vmp)
    {
        vmp.FilterMargin = vmp.ProgressBarVisibility switch
        {
            Visibility.Visible => new Thickness(left: 0, top: 0, right: 8, bottom: 13),
            Visibility.Collapsed => new Thickness(left: 0, top: 7, right: 8, bottom: -5),
            _ => new Thickness(left: 0, top: 7, right: 8, bottom: -5),
        };
    }

    internal static async void Sort(MainWindowViewModelProperties vmp)
    {
        vmp.ModList = await MainWindowModel.SetModDisplay(
            await ApiHandler.GetModsAsync(1, 10, vmp.SortType, vmp.SortOrder!, vmp.FilterValue!));
    }

    private async Task FilterByName()
    {
        ModList = await MainWindowModel.SetModDisplay(
            await ApiHandler.GetModsAsync(1, 10, SortType, SortOrder!, FilterValue!));
    }
}
